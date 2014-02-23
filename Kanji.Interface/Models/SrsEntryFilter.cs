using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Database.Entities;
using Kanji.Database.Models;

namespace Kanji.Interface.Models
{
    class SrsEntryFilter : Filter<SrsEntry>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the collection of filter clauses.
        /// </summary>
        public FilterClause[] FilterClauses { get; set; }

        #endregion

        #region Constructors

        public SrsEntryFilter()
        {
            FilterClauses = new FilterClause[] { };
        }

        #endregion

        #region Methods

        public override bool IsEmpty()
        {
            return !FilterClauses.Any();
        }

        public override Filter<SrsEntry> Clone()
        {
            return new SrsEntryFilter()
            {
                ForceFilter = this.ForceFilter,
                FilterClauses = (FilterClause[])this.FilterClauses.Clone()
            };
        }

        #endregion
    }
}
