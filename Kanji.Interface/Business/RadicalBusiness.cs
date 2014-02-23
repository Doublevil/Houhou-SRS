using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Database.Dao;
using Kanji.Database.Entities;
using Kanji.Database.Models;
using Kanji.Interface.Models;

namespace Kanji.Interface.Business
{
    class RadicalBusiness
    {
        #region Fields

        private RadicalDao _radicalDao;

        #endregion

        #region Constructors

        public RadicalBusiness()
        {
            _radicalDao = new RadicalDao();
        }

        #endregion

        #region Methods

        public RadicalEntity[] GetAvailableRadicals(KanjiFilter filter)
        {
            string anyReadingFilter = filter.MainFilterMode == KanjiFilterModeEnum.AnyReading ?
                filter.MainFilter : string.Empty;
            string kunYomiFilter = filter.MainFilterMode == KanjiFilterModeEnum.KunYomi ?
                filter.MainFilter : string.Empty;
            string onYomiFilter = filter.MainFilterMode == KanjiFilterModeEnum.OnYomi ?
                filter.MainFilter : string.Empty;
            string nanoriFilter = filter.MainFilterMode == KanjiFilterModeEnum.Nanori ?
                filter.MainFilter : string.Empty;
            string meaningFilter = filter.MainFilterMode == KanjiFilterModeEnum.Meaning ?
                filter.MainFilter : string.Empty;
            RadicalGroup[] radicalGroups = filter.Radicals.SelectMany(r => r.Reference.RadicalGroups).ToArray();

            return _radicalDao.GetAvailableRadicals(radicalGroups, filter.TextFilter, meaningFilter,
                anyReadingFilter, onYomiFilter, kunYomiFilter, nanoriFilter)
                .ToArray();
        }

        #endregion
    }
}
