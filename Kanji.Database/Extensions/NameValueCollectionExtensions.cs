using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Kanji.Database.Extensions
{
    static class NameValueCollectionExtensions
    {
        /// <summary>
        /// Tries to read a string value at the given key.
        /// If the string read is empty, returns null.
        /// </summary>
        /// <param name="c">Collection.</param>
        /// <param name="key">Key to read in the collection.</param>
        /// <returns>Value read, or null if empty.</returns>
        public static string ReadString(this NameValueCollection c, string key)
        {
            string value = c.Get(key);
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            return value;
        }

        /// <summary>
        /// Tries to read a short value at the given key.
        /// Returns a null value if the field is not set, has a null value or
        /// is not an integer.
        /// </summary>
        /// <param name="c">Collection.</param>
        /// <param name="key">Key to read in the collection.</param>
        /// <returns>Value read as a nullable short.</returns>
        public static short? ReadShort(this NameValueCollection c, string key)
        {
            string value = c.Get(key);
            if (value != null)
            {
                short result = 0;
                if (short.TryParse(value, out result))
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Tries to read an integer value at the given key.
        /// Returns a null value if the field is not set, has a null value or
        /// is not an integer.
        /// </summary>
        /// <param name="c">Collection.</param>
        /// <param name="key">Key to read in the collection.</param>
        /// <returns>Value read as a nullable integer.</returns>
        public static int? ReadInt(this NameValueCollection c, string key)
        {
            string value = c.Get(key);
            if (value != null)
            {
                int result = 0;
                if (int.TryParse(value, out result))
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Tries to read a long value at the given key.
        /// Returns a null value if the field is not set, has a null value or
        /// is not an integer.
        /// </summary>
        /// <param name="c">Collection.</param>
        /// <param name="key">Key to read in the collection.</param>
        /// <returns>Value read as a nullable long.</returns>
        public static long? ReadLong(this NameValueCollection c, string key)
        {
            string value = c.Get(key);
            if (value != null)
            {
                long result = 0;
                if (long.TryParse(value, out result))
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Tries to read a boolean value at the given key.
        /// Returns a null value if the field is not set, has a null value or
        /// is not an integer.
        /// </summary>
        /// <param name="c">Collection.</param>
        /// <param name="key">Key to read in the collection.</param>
        /// <returns>Value read as a nullable boolean.</returns>
        public static bool? ReadBool(this NameValueCollection c, string key)
        {
            string value = c.Get(key);
            if (value != null)
            {
                bool result = false;
                if (bool.TryParse(value, out result))
                {
                    return result;
                }
                else if (value == "1")
                {
                    return true;
                }
                else if (value == "0")
                {
                    return false;
                }
            }

            return null;
        }

        /// <summary>
        /// Tries to read a DateTime value at the given key.
        /// Returns a null value if the field is not set, has a null value or
        /// is not a DateTime.
        /// </summary>
        /// <param name="c">Collection.</param>
        /// <param name="key">Key to read in the collection.</param>
        /// <returns>Value read as a nullable DateTime.</returns>
        public static DateTime? ReadDateTime(this NameValueCollection c, string key)
        {
            string value = c.Get(key);
            if (value != null)
            {
                long numericValue = 0;
                if (long.TryParse(value, out numericValue))
                {
                    // The DB should store it as UTC, but we need to work with it as local time.
                    return new DateTime(numericValue, DateTimeKind.Utc).ToLocalTime();
                }
            }

            return null;
        }

        public static byte[] ReadBinary(this NameValueCollection c, string key)
        {
            string value = c.Get(key);
            return Encoding.UTF8.GetBytes(value);
        }
    }
}
