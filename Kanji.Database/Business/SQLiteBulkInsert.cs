/*
Some credits to JP: http://procbits.com/2009/09/08/sqlite-bulk-insert
Thanks man. But damn, your code was messy.
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Reflection;
using System.Text;
using Kanji.Database.Dao;
using Kanji.Database.Entities;
using Kanji.Database.Helpers;

namespace Kanji.Database.Business
{
    public class SQLiteBulkInsert<T> : IDisposable where T : Entity, new()
    {
        #region Constants

        /// <summary>
        /// String prepended to the name of SQL parameters in the generated command.
        /// </summary>
        private static readonly string ParameterPrefix = ":";

        /// <summary>
        /// String appended in the middle of the SQL command, i.e.
        /// after naming parameters but before their values.
        /// </summary>
        private static readonly string SqlCommandMiddle = ") VALUES (";

        /// <summary>
        /// String appended in the end of the SQL command.
        /// </summary>
        private static readonly string SqlCommandEnd = ")";

        #endregion

        #region Fields

        private SQLiteConnection _sqliteConnection;
        private SQLiteCommand _sqliteCommand;
        private SQLiteTransaction _currentTransaction;

        private Dictionary<string, SQLiteParameter> _parameters = new Dictionary<string, SQLiteParameter>();
        private uint _entryCounter = 0;
        private string _sqlCommandStart;

        private string _tableName;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the maximal count of items that can be inserted before the transaction is committed.
        /// </summary>
        public int CommitMax { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Builds a new bulk insert object.
        /// </summary>
        /// <param name="commitMax">Maximal amount of items that can be inserted before the
        /// transaction is committed.</param>
        public SQLiteBulkInsert(int commitMax)
        {
            CommitMax = commitMax;
            T reference = new T();

            _tableName = reference.GetTableName();
            foreach (KeyValuePair<string, DbType> kvp in reference.GetMapping())
            {
                AddParameter(kvp.Key, kvp.Value);
            }
            _sqliteConnection = new SQLiteConnection(ConnectionStringHelper.GetConnectionString(
                reference.GetEndpoint()));
            _sqliteConnection.Open();

            StringBuilder query = new StringBuilder(255);
            query.Append("INSERT INTO ["); query.Append(_tableName); query.Append("] (");
            _sqlCommandStart = query.ToString();

            CreateCommand();
        }

        #endregion

        #region Methods

        #region External manipulation

        /// <summary>
        /// Commits the transaction.
        /// Should be called at the end of the bulk insert operation to make sure everything is committed.
        /// </summary>
        public void Flush()
        {
            try
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Could not commit transaction. See InnerException for more details", ex);
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                }

                _currentTransaction = null;
                _entryCounter = 0;
            }
        }

        /// <summary>
        /// Inserts the given entity to the database.
        /// Commits the transaction if the number of uncommitted operations exceeds the CommitMax value.
        /// </summary>
        /// <param name="entity">Entity to insert.</param>
        /// <returns>ID of the inserted entity.</returns>
        public long Insert(T entity)
        {
            object[] paramValues = entity.GetValues();
            long lastInsertId = 0;

            if (paramValues.Length != _parameters.Count)
            {
                throw new Exception(
                    "The values array count must be equal to the count of the number of parameters.");
            }

            _entryCounter++;
            int i = 0;
            foreach (SQLiteParameter par in _parameters.Values)
            {
                par.Value = paramValues[i];
                i++;
            }

            // Start a transaction if none were.
            if (_currentTransaction == null)
            {
                _currentTransaction = _sqliteConnection.BeginTransaction();
            }

            // Execute the query and retrieve the last insert row ID.
            _sqliteCommand.ExecuteNonQuery();
            lastInsertId = _sqliteConnection.LastInsertRowId;

            if (_entryCounter >= CommitMax)
            {
                // The number of waiting insert operations exceeds the max amount.
                // Let's commit the transaction!

                Flush();
            }

            return lastInsertId;
        }

        #endregion

        #region Internal manipulation

        /// <summary>
        /// Initializes the command field.
        /// </summary>
        protected void CreateCommand()
        {
            _sqliteCommand = _sqliteConnection.CreateCommand();
            foreach (SQLiteParameter par in _parameters.Values)
            {
                _sqliteCommand.Parameters.Add(par);
            }
            _sqliteCommand.CommandText = GetSqlCommand();
        }

        /// <summary>
        /// Generates and returns the SQL command string.
        /// </summary>
        /// <returns>SQL command string.</returns>
        protected string GetSqlCommand()
        {
            if (_parameters.Count < 1)
            {
                return _sqlCommandStart.Trim(new char[] { ' ', '(' }) + " DEFAULT VALUES";
            }

            StringBuilder sb = new StringBuilder(255);
            sb.Append(_sqlCommandStart);

            foreach (string param in _parameters.Keys)
            {
                sb.Append('[');
                sb.Append(param);
                sb.Append(']');
                sb.Append(", ");
            }
            sb.Remove(sb.Length - 2, 2);

            sb.Append(SqlCommandMiddle);

            foreach (string param in _parameters.Keys)
            {
                sb.Append(ParameterPrefix);
                sb.Append(param);
                sb.Append(", ");
            }
            sb.Remove(sb.Length - 2, 2);

            sb.Append(SqlCommandEnd);

            return sb.ToString();
        }

        /// <summary>
        /// Adds a field parameter of the given name and type.
        /// </summary>
        /// <param name="name">Name of the field to add.</param>
        /// <param name="dbType">Database type of the field.</param>
        protected void AddParameter(string name, DbType dbType)
        {
            SQLiteParameter param = new SQLiteParameter(ParameterPrefix + name, dbType);
            _parameters.Add(name, param);
        }

        #endregion

        /// <summary>
        /// Disposes the connection.
        /// </summary>
        public void Dispose()
        {
            if (_currentTransaction != null)
            {
                Flush();
            }

            if (_sqliteCommand != null)
            {
                _sqliteCommand.Dispose();
                _sqliteCommand = null;
            }

            if (_sqliteConnection != null)
            {
                if (_sqliteConnection.State != ConnectionState.Closed)
                {
                    _sqliteConnection.Close();
                }

                _sqliteConnection.Dispose();
                _sqliteConnection = null;
            }
        }

        #endregion
    }
}
