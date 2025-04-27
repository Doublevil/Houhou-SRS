using System;

namespace Kanji.Interface.Models
{
    /// <summary>
    /// Represents the paging behavior for item lists.
    /// </summary>
    [Serializable]
    public enum ItemListPagingMode
    {
        /// <summary>
        /// Items on the next page are added to the list, keeping previously fetched items.
        /// </summary>
        Additive = 0,

        /// <summary>
        /// Items on the next page replace previously fetched items.
        /// </summary>
        Substitutive = 1
    }
}
