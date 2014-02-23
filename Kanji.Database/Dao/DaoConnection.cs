using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kanji.Common.Helpers;
using Kanji.Database.Helpers;

namespace Kanji.Database.Dao
{
    class DaoConnection : IDisposable
    {
        #region Constants

        // Timeout for the connection waiting method.
        private static readonly int AsyncOpenTimeout = 5000;

        #endregion

        #region Static

        /// <summary>
        /// Creates and opens a new connection.
        /// </summary>
        /// <param name="endpoint">Connection endpoint.</param>
        /// <returns>Newly created connection in an open state.</returns>
        public static DaoConnection Open(DaoConnectionEnum endpoint)
        {
            DaoConnection connection = new DaoConnection(endpoint);
            connection.Open();
            return connection;
        }

        #endregion

        #region Fields

        /// <summary>
        /// Underlying connection object.
        /// </summary>
        private SQLiteConnection _connection;

        /// <summary>
        /// Lock preventing a database operation and the connection disposal
        /// to occur at the same time.
        /// </summary>
        private object _connectionLock = new object();

        /// <summary>
        /// ManualResetEvent used to prevent the connection from querying while
        /// the connection is opening asynchronously.
        /// The event is set once the connection is open, and will be
        /// waited by query and dispose operations.
        /// </summary>
        private ManualResetEvent _connectionOpenEvent;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new database connection set in a closed state.
        /// </summary>
        public DaoConnection(DaoConnectionEnum endpoint)
        {
            _connectionOpenEvent = new ManualResetEvent(false);

            //string connectionName = null;
            switch (endpoint)
            {
                case DaoConnectionEnum.KanjiDatabase:
                    _connection = new SQLiteConnection(
                        ConfigurationManager.ConnectionStrings[
                        ConnectionStringHelper.KanjiDatabaseConnectionName].ConnectionString);
                    break;
                case DaoConnectionEnum.SrsDatabase:
                    _connection = new SQLiteConnection(
                        ConnectionStringHelper.SrsDatabaseConnectionString);
                    break;
                default:
                    throw new ArgumentException(
                        string.Format("Unknown connection: \"{0}\".", endpoint));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Opens the connection sychronously.
        /// </summary>
        public void Open()
        {
            _connection.Open();
            _connectionOpenEvent.Set();
        }

        /// <summary>
        /// Opens the connection asynchronously.
        /// </summary>
        /// <remarks>A query operation on the connection will automatically wait for the
        /// connection to be in an open state before running.</remarks>
        public void OpenAsync()
        {
            new Task(() =>
            {
                try
                {
                    // Try to open the connection.
                    _connection.Open();
                }
                finally
                {
                    // After it is done, set the event.
                    _connectionOpenEvent.Set();
                }
            }).Start();
        }

        /// <summary>
        /// Waits for the connection open event to make sure an operation is not
        /// executed while the connection is asynchronously opening.
        /// </summary>
        private void WaitOpen()
        {
            if (!_connectionOpenEvent.WaitOne(AsyncOpenTimeout))
            {
                throw new TimeoutException(
                    "The opening event took too long to acquire.");
            }
        }

        #region Commands

        /// <summary>
        /// Executes an SQL query and returns the resulting rows.
        /// </summary>
        /// <param name="sqlQuery">Query to execute.</param>
        /// <param name="parameters">Parameters of the query.</param>
        /// <returns>Collections of named values representing the rows returned by the query.</returns>
        public IEnumerable<NameValueCollection> Query(string sqlQuery, params DaoParameter[] parameters)
        {
            // Make sure the connection is open before starting the operation.
            WaitOpen();

            // Create the command.
            using (SQLiteCommand cmd = new SQLiteCommand(sqlQuery, _connection))
            {
                // Add the parameters.
                foreach (DaoParameter param in parameters)
                {
                    cmd.Parameters.Add(new SQLiteParameter(param.Name, param.Value));
                }

                SQLiteDataReader reader = null;
                try
                {
                    bool isOkay = true;
                    // Lock and execute the query (and fetch the first result).
                    lock (_connectionLock)
                    {
                        if (_connection == null)
                        {
                            isOkay = false;
                        }
                        else
                        {
                            reader = cmd.ExecuteReader();
                            isOkay = reader.Read();
                        }
                    }

                    while (isOkay)
                    {
                        NameValueCollection nvc = null;
                        // Lock and get the result.
                        lock (_connectionLock)
                        {
                            if (_connection == null || reader.IsClosed) break;
                            nvc = reader.GetValues();
                        }

                        // Yield return the result if found.
                        if (nvc != null)
                        {
                            yield return nvc;
                        }

                        // Lock and get the next result, and loop.
                        lock (_connectionLock)
                        {
                            if (_connection == null || reader.IsClosed) break;
                            // Continue iterating only if there is more to read.
                            isOkay = reader.Read();
                        }
                    }
                }
                finally
                {
                    // When it's over or the enumerator is disposed:
                    // Lock, close, dispose the reader.
                    lock (_connectionLock)
                    {
                        if (reader != null)
                        {
                            if (!reader.IsClosed && _connection != null)
                            {
                                reader.Close();
                            }

                            reader.Dispose();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Executes an SQL query over a scalar object (ex: a row count).
        /// </summary>
        /// <param name="sqlQuery">SQL query to execute.</param>
        /// <param name="parameters">Parameters of the query.</param>
        /// <returns>Scalar object returned by the database query.</returns>
        public object QueryScalar(string sqlQuery, params DaoParameter[] parameters)
        {
            // Make sure the connection is open before starting the operation.
            WaitOpen();

            object result = null;
            using (SQLiteCommand cmd = new SQLiteCommand(sqlQuery, _connection))
            {
                foreach (DaoParameter param in parameters)
                {
                    cmd.Parameters.Add(new SQLiteParameter(param.Name, param.Value));
                }

                try
                {
                    result = cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    LogHelper.GetLogger(this.GetType().Name).ErrorFormat(
                        "An error occured while executing the following command:{0}{1}{0}{2}",
                        Environment.NewLine, sqlQuery, ex);
                    result = null;
                }
            }

            return result;
        }

        /// <summary>
        /// Executes the given non-query request on the database.
        /// </summary>
        /// <param name="sqlRequest">SQL request to execute.</param>
        /// <param name="parameters">Parameters of the query.</param>
        /// <returns>Number of rows affected.</returns>
        public int ExecuteNonQuery(string sqlRequest, params DaoParameter[] parameters)
        {
            // Make sure the connection is open before starting the operation.
            WaitOpen();

            int rows = 0;
            using (SQLiteCommand cmd = new SQLiteCommand(sqlRequest, _connection))
            {
                foreach (DaoParameter param in parameters)
                {
                    cmd.Parameters.Add(new SQLiteParameter(param.Name, param.Value));
                }

                try
                {
                    rows = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    LogHelper.GetLogger(this.GetType().Name).ErrorFormat(
                        "An error occured while executing the following command:{0}{1}{0}{2}",
                        Environment.NewLine, sqlRequest, ex);
                }
            }

            return rows;
        }

        /// <summary>
        /// Retrieves and returns the ID of the latest inserted row for the
        /// active connection.
        /// </summary>
        /// <returns>ID of the latest inserted row for the active connection.</returns>
        public long GetLastInsertId()
        {
            WaitOpen();
            return _connection.LastInsertRowId;
        }

        #endregion

        /// <summary>
        /// Closes and disposes the connection.
        /// </summary>
        public void Dispose()
        {
            // Lock to prevent the disposal from occuring
            // during a pending database query.
            lock (_connectionLock)
            {
                if (_connection != null)
                {
                    WaitOpen();
                    _connection.Close();
                    _connection.Dispose();
                    _connection = null;
                }
            }
        }

        #endregion
    }
}
