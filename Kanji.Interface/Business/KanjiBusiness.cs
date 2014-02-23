using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Database.Dao;
using Kanji.Database.Entities;
using Kanji.Interface.Models;

namespace Kanji.Interface.Business
{
    class KanjiBusiness
    {
        #region Fields

        private KanjiDao _kanjiDao;

        #endregion

        #region Properties



        #endregion

        #region Constructors

        public KanjiBusiness()
        {
            _kanjiDao = new KanjiDao();
        }

        #endregion

        #region Methods

        //public KanjiFilter GetKanjiByRadicals(params RadicalEntity[] testedRadicals)
        //{
        //    if (testedRadicals.Any())
        //    {
        //        // Select kanji where the intersection of tested radicals ID list and the kanji radicals ID list contain the same number of elements.
        //        // i.e. all tested radicals are contained in the kanji radicals.
        //        IEnumerable<KanjiEntity> kanjiSet = testedRadicals
        //            .SelectMany(r => r.Kanji)
        //            .Where(k => k.Radicals.Intersect(testedRadicals).Count() == testedRadicals.Count())
        //            .Distinct()
        //            .OrderByDescending(k => k.MostUsedRank != null)
        //            .ThenBy(k => k.MostUsedRank);

        //        //var kanjiSet = new KanjiDao().GetKanjiPerRadicals(testedRadicals)
        //        //    .OrderByDescending(k => k.MostUsedRank != null)
        //        //    .ThenBy(k => k.MostUsedRank);

        //        // Sort kanji
        //        //kanjiSet.Sort(SortKanji);

        //        // Determine which radicals are still eligible
        //        IEnumerable<RadicalEntity> radicalSet = kanjiSet.SelectMany(k => k.Radicals).Distinct();

        //        return new KanjiFilter() { Results = kanjiSet.ToArray(), FilteredRadicals = radicalSet.ToArray() };
        //    }
        //    else
        //    {
        //        return new KanjiFilter() { Results = new KanjiEntity[0], FilteredRadicals = _container.RadicalSet.ToArray() };
        //    }
        //}

        //public IEnumerable<KanjiEntity> GetFilteredKanji(KanjiFilter filter)
        //{
        //    // Take the base set.
        //    IEnumerable<KanjiEntity> kanjiSet = _kanjiDao.GetFilteredKanji(.KanjiSet;

        //    // Filter on radicals.
        //    if (filter.Radicals.Any())
        //    {
        //        long[] ids = filter.Radicals.Select(r => r.Reference.DbRadical.ID).ToArray();
        //        kanjiSet = kanjiSet
        //            .Where(k => k.Radicals
        //                .Select(r => r.ID)
        //                .Intersect(ids)
        //                .Count() == ids.Count());
        //    }

        //    // Apply the text filter.
        //    if (!string.IsNullOrWhiteSpace(filter.TextFilter))
        //    {
        //        kanjiSet = kanjiSet.Where(k => filter.TextFilter.Contains(k.Character));
        //    }

        //    // Apply the main filter.
        //    if (!string.IsNullOrWhiteSpace(filter.MainFilter))
        //    {
        //        switch (filter.MainFilterMode)
        //        {
        //            case KanjiFilterModeEnum.Meaning:
        //                kanjiSet = kanjiSet
        //                    .Where(k => k.Meanings
        //                        .Any(m => (m.Language == "en"
        //                            || string.IsNullOrEmpty(m.Language))
        //                            && m.Meaning.Contains(filter.MainFilter)));
        //                break;
        //            case KanjiFilterModeEnum.KunYomi:
        //                kanjiSet = kanjiSet
        //                    .Where(k => k.KunYomi != null && k.KunYomi
        //                        .Split(MultiValueFieldHelper.ValueSeparator)
        //                        .Any(r => r.Contains(filter.MainFilter)));
        //                break;
        //            case KanjiFilterModeEnum.OnYomi:
        //                kanjiSet = kanjiSet
        //                    .Where(k => k.OnYomi != null && k.OnYomi
        //                        .Split(MultiValueFieldHelper.ValueSeparator)
        //                        .Any(r => r.Contains(filter.MainFilter)));
        //                break;
        //            case KanjiFilterModeEnum.Nanori:
        //                kanjiSet = kanjiSet
        //                    .Where(k => k.Nanori != null && k.Nanori
        //                        .Split(MultiValueFieldHelper.ValueSeparator)
        //                        .Any(r => r.Contains(filter.MainFilter)));
        //                break;
        //            case KanjiFilterModeEnum.AnyReading:
        //                kanjiSet = kanjiSet
        //                    .Where(k =>
        //                        (k.KunYomi ?? string.Empty
        //                        + MultiValueFieldHelper.ValueSeparator
        //                        + k.OnYomi ?? string.Empty
        //                        + MultiValueFieldHelper.ValueSeparator
        //                        + k.Nanori ?? string.Empty)
        //                        .Split(new char[] { MultiValueFieldHelper.ValueSeparator }, StringSplitOptions.RemoveEmptyEntries)
        //                        .Any(r => r.Contains(filter.MainFilter)));
        //                break;
        //        }
        //    }

        //    // Order by common-ness
        //    kanjiSet = kanjiSet.OrderByDescending(k => k.MostUsedRank != null)
        //        .ThenBy(k => k.MostUsedRank);

        //    // The set is ready to be exploited.
        //    return kanjiSet;
        //}

        //private int SortKanji(KanjiEntity a, KanjiEntity b)
        //{
        //    int aRank = a.MostUsedRank ?? 2501, bRank = b.MostUsedRank ?? 2501;
        //    if (aRank > bRank)
        //    {
        //        return 1;
        //    }
        //    else if (aRank < bRank)
        //    {
        //        return -1;
        //    }
        //    else
        //    {
        //        return 0;
        //    }
        //}

        #endregion
    }
}
