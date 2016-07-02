using GalaSoft.MvvmLight.Command;
using Kanji.Database.Entities;
using Kanji.Database.Models;

namespace Kanji.Interface.ViewModels
{
    class JlptLevelFilterViewModel : FilterClauseViewModel
    {
        #region Constants

        /// <summary>
        /// The value that <see cref="JlptLevel"/> has if it should be ignored.
        /// </summary>
        private const int filterIgnoreLevel = 6;

        #endregion

        #region Properties
		
        public int JlptLevel { get; set; }
		
        #endregion
		
        #region Constructors

        public JlptLevelFilterViewModel()
        {
            JlptLevel = filterIgnoreLevel;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Clears the filter. Should not raise a FilterChanged event.
        /// </summary>
        public override void ClearFilter()
        {
            JlptLevel = filterIgnoreLevel;
        }

        /// <summary>
        /// Gets the matching filter clause.
        /// </summary>
        public override FilterClause GetFilterClause()
        {
            return new SrsEntryFilterJlptLevelClause()
            {
                Value = JlptLevel == filterIgnoreLevel ? (int?)null : JlptLevel
            };
        }
        
        #endregion
    }
}