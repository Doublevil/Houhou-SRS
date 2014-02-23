using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Kanji.Database.EntityBuilders
{
    abstract class EntityBuilder<T>
    {
        /// <summary>
        /// Builds an entity from the given row, using the given prefix as
        /// a field key prefix.
        /// </summary>
        public abstract T BuildEntity(NameValueCollection row, string prefix);

        /// <summary>
        /// Builds the string key that will allow a field to be retrieved
        /// from a NameValueCollection.
        /// </summary>
        /// <param name="prefix">Key prefix.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns>Key built from the prefix and field name.</returns>
        protected string GetField(string prefix, string fieldName)
        {
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                return prefix + "." + fieldName;
            }
            else return fieldName;
        }
    }
}
