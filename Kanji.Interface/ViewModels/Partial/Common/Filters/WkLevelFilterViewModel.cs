using Kanji.Database.Models;

namespace Kanji.Interface.ViewModels
{
    class WkLevelFilterViewModel : FilterClauseViewModel
    {
        #region Constants
        
        /// <summary>
        /// The value that <see cref="WkLevel"/> has if it should be ignored.
        /// </summary>
        private const int filterIgnoreLevel = 0;

        #endregion

        #region Properties
		
        public int WkLevel { get; set; }
		
        #endregion
		
        #region Constructors

        public WkLevelFilterViewModel()
        {
            WkLevel = filterIgnoreLevel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clears the filter. Should not raise a FilterChanged event.
        /// </summary>
        public override void ClearFilter()
        {
            WkLevel = filterIgnoreLevel;
        }

        /// <summary>
        /// Gets the matching filter clause.
        /// </summary>
        public override FilterClause GetFilterClause()
        {
            return new SrsEntryFilterWkLevelClause()
            {
                Value = WkLevel == filterIgnoreLevel ? (int?)null : WkLevel
            };
        }
        
        #endregion
    }
}