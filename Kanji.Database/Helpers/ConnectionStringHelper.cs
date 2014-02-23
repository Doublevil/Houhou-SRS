using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Kanji.Common.Helpers;
using Kanji.Database.Dao;

namespace Kanji.Database.Helpers
{
    public static class ConnectionStringHelper
    {
        public static readonly string KanjiDatabaseConnectionName = "KanjiDatabase";
        //public static readonly string SrsDatabaseConnectionName = "SrsDatabase";
        public static string SrsDatabaseConnectionString;

        static ConnectionStringHelper()
        {
            // Alter the connection string to the user SRS content database.
            // I couldn't find better ways to do that. Connection strings wouldn't work because of dynamic parts.
            Kanji.Database.Helpers.ConnectionStringHelper.SrsDatabaseConnectionString =
                string.Format("data source={0};UTF8Encoding=True;Journal Mode=WAL",
                ConfigurationHelper.UserContentSrsDatabaseFilePath);
        }

        /// <summary>
        /// Gets the connection string associated with the given endpoint.
        /// </summary>
        /// <param name="endpoint">Target endpoint.</param>
        /// <returns>Connection string associated with the given endpoint.</returns>
        internal static string GetConnectionString(DaoConnectionEnum endpoint)
        {
            switch (endpoint)
            {
                case DaoConnectionEnum.KanjiDatabase:
                    return ConfigurationManager.ConnectionStrings[
                        ConnectionStringHelper.KanjiDatabaseConnectionName].ConnectionString;
                case DaoConnectionEnum.SrsDatabase:
                    return ConnectionStringHelper.SrsDatabaseConnectionString;
                default:
                    throw new ArgumentException(
                        string.Format("Unknown connection: \"{0}\".", endpoint));
            }
        }
    }
}
