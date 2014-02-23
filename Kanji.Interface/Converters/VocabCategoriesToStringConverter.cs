using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using Kanji.Database.Dao;
using Kanji.Database.Entities;
using Kanji.Interface.Internationalization;

namespace Kanji.Interface.Converters
{
    class VocabCategoriesToStringConverter : IValueConverter
    {
        #region Static

        private class VocabCategoryInfo
        {
            public string Label { get; set; }
            public Color Color { get; set; }
            public VocabCategoryInfo(string label, Color color) { Label = label; Color = color; }
        }

        private static readonly Dictionary<long, VocabCategoryInfo> CategoryDictionary;

        static VocabCategoriesToStringConverter()
        {
            CategoryDictionary = new Dictionary<long, VocabCategoryInfo>();

            VocabDao vocabDao = new VocabDao();
            foreach (VocabCategory category in vocabDao.GetAllCategories())
            {
                VocabCategoryInfo info = GetInfo(category);
                if (info != null)
                {
                    CategoryDictionary.Add(category.ID, info);
                }
            }
        }

        private static VocabCategoryInfo GetInfo(VocabCategory c)
        {
            switch (c.ShortName)
            {
                case "MA":
	                return new VocabCategoryInfo(R.VocabCat_MartialArts, Colors.Black);
                case "X":
	                return new VocabCategoryInfo(R.VocabCat_X, Colors.Black);
                case "abbr":
	                return new VocabCategoryInfo(R.VocabCat_Abbreviation, Colors.Black);
                case "adj-i":
	                return new VocabCategoryInfo(R.VocabCat_IAdjective, Colors.Black);
                case "adj-na":
	                return new VocabCategoryInfo(R.VocabCat_NaAdjective, Colors.Black);
                case "adj-no":
	                return new VocabCategoryInfo(R.VocabCat_NoAdjective, Colors.Black);
                case "adj-pn":
	                return new VocabCategoryInfo(R.VocabCat_PrenominalAdjective, Colors.Black);
                case "adj-t":
	                return new VocabCategoryInfo(R.VocabCat_TaruAdjective, Colors.Black);
                case "adj-f":
	                return new VocabCategoryInfo(R.VocabCat_PrenominalNounVerb, Colors.Black);
                case "adj":
	                return new VocabCategoryInfo(R.VocabCat_Adjective, Colors.Black);
                case "adv":
	                return new VocabCategoryInfo(R.VocabCat_Adverb, Colors.Black);
                case "adv-to":
	                return new VocabCategoryInfo(R.VocabCat_ToAdverb, Colors.Black);
                case "arch":
	                return new VocabCategoryInfo(R.VocabCat_Archaism, Colors.Black);
                case "ateji":
	                return new VocabCategoryInfo(R.VocabCat_Ateji, Colors.Black);
                case "aux":
	                return new VocabCategoryInfo(R.VocabCat_Auxiliary, Colors.Black);
                case "aux-v":
	                return new VocabCategoryInfo(R.VocabCat_AuxiliaryVerb, Colors.Black);
                case "aux-adj":
	                return new VocabCategoryInfo(R.VocabCat_AuxiliaryAdjective, Colors.Black);
                case "Buddh":
	                return new VocabCategoryInfo(R.VocabCat_BuddhistTerm, Colors.Black);
                case "chem":
	                return new VocabCategoryInfo(R.VocabCat_Chemistry, Colors.Black);
                case "chn":
	                return new VocabCategoryInfo(R.VocabCat_Children, Colors.Black);
                case "col":
	                return new VocabCategoryInfo(R.VocabCat_Colloquialism, Colors.Black);
                case "comp":
	                return new VocabCategoryInfo(R.VocabCat_ComputerTerminology, Colors.Black);
                case "conj":
	                return new VocabCategoryInfo(R.VocabCat_Conjunction, Colors.Black);
                case "ctr":
	                return new VocabCategoryInfo(R.VocabCat_Counter, Colors.Black);
                case "derog":
	                return new VocabCategoryInfo(R.VocabCat_Derogatory, Colors.Black);
                case "eK":
	                return new VocabCategoryInfo(R.VocabCat_ExclusivelyKanji, Colors.Black);
                case "ek":
	                return new VocabCategoryInfo(R.VocabCat_ExclusivelyKana, Colors.Black);
                case "exp":
	                return new VocabCategoryInfo(R.VocabCat_Expression, Colors.Black);
                case "fam":
	                return new VocabCategoryInfo(R.VocabCat_Familiar, Colors.Black);
                case "fem":
	                return new VocabCategoryInfo(R.VocabCat_Female, Colors.Black);
                case "food":
	                return new VocabCategoryInfo(R.VocabCat_Food, Colors.Black);
                case "geom":
	                return new VocabCategoryInfo(R.VocabCat_Geometry, Colors.Black);
                case "gikun":
	                return new VocabCategoryInfo(R.VocabCat_Gikun, Colors.Black);
                case "hon":
	                return new VocabCategoryInfo(R.VocabCat_Honorific, Colors.Black);
                case "hum":
	                return new VocabCategoryInfo(R.VocabCat_Humble, Colors.Black);
                case "iK":
	                return new VocabCategoryInfo(R.VocabCat_IrregularKanjiUsage, Colors.Black);
                case "id":
	                return new VocabCategoryInfo(R.VocabCat_Idiomatic, Colors.Black);
                case "ik":
	                return new VocabCategoryInfo(R.VocabCat_IrregularKanaUsage, Colors.Black);
                case "int":
	                return new VocabCategoryInfo(R.VocabCat_Interjection, Colors.Black);
                case "io":
	                return new VocabCategoryInfo(R.VocabCat_IrregularOkuriganaUsage, Colors.Black);
                case "iv":
	                return new VocabCategoryInfo(R.VocabCat_IrregularVerb, Colors.Black);
                case "ling":
	                return new VocabCategoryInfo(R.VocabCat_Linguistics, Colors.Black);
                case "m-sl":
	                return new VocabCategoryInfo(R.VocabCat_MangaSlang, Colors.Black);
                case "male":
	                return new VocabCategoryInfo(R.VocabCat_Male, Colors.Black);
                case "male-sl":
	                return new VocabCategoryInfo(R.VocabCat_MaleSlang, Colors.Black);
                case "math":
	                return new VocabCategoryInfo(R.VocabCat_Mathematics, Colors.Black);
                case "mil":
	                return new VocabCategoryInfo(R.VocabCat_Military, Colors.Black);
                case "n":
	                return new VocabCategoryInfo(R.VocabCat_Noun, Colors.Black);
                case "n-adv":
	                return new VocabCategoryInfo(R.VocabCat_AdverbialNoun, Colors.Black);
                case "n-suf":
	                return new VocabCategoryInfo(R.VocabCat_SuffixNoun, Colors.Black);
                case "n-pref":
	                return new VocabCategoryInfo(R.VocabCat_PrefixNoun, Colors.Black);
                case "n-t":
	                return new VocabCategoryInfo(R.VocabCat_TemporalNoun, Colors.Black);
                case "num":
	                return new VocabCategoryInfo(R.VocabCat_Numeric, Colors.Black);
                case "oK":
	                return new VocabCategoryInfo(R.VocabCat_OutdatedKanji, Colors.Black);
                case "obs":
	                return new VocabCategoryInfo(R.VocabCat_Obsolete, Colors.Black);
                case "obsc":
	                return new VocabCategoryInfo(R.VocabCat_Obscure, Colors.Black);
                case "ok":
	                return new VocabCategoryInfo(R.VocabCat_OutdatedKana, Colors.Black);
                case "oik":
	                return new VocabCategoryInfo(R.VocabCat_OldOrIrregularKana, Colors.Black);
                case "on-mim":
	                return new VocabCategoryInfo(R.VocabCat_Onomatopoeic, Colors.Black);
                case "pn":
	                return new VocabCategoryInfo(R.VocabCat_Pronoun, Colors.Black);
                case "poet":
	                return new VocabCategoryInfo(R.VocabCat_Poetical, Colors.Black);
                case "pol":
	                return new VocabCategoryInfo(R.VocabCat_Polite, Colors.Black);
                case "pref":
	                return new VocabCategoryInfo(R.VocabCat_Prefix, Colors.Black);
                case "proverb":
	                return new VocabCategoryInfo(R.VocabCat_Proverb, Colors.Black);
                case "prt":
	                return new VocabCategoryInfo(R.VocabCat_Particle, Colors.Black);
                case "physics":
	                return new VocabCategoryInfo(R.VocabCat_Physics, Colors.Black);
                case "rare":
	                return new VocabCategoryInfo(R.VocabCat_Rare, Colors.Black);
                case "sens":
	                return new VocabCategoryInfo(R.VocabCat_Sensitive, Colors.Black);
                case "sl":
	                return new VocabCategoryInfo(R.VocabCat_Slang, Colors.Black);
                case "suf":
	                return new VocabCategoryInfo(R.VocabCat_Suffix, Colors.Black);
                case "uK":
	                return new VocabCategoryInfo(R.VocabCat_UsuallyKanji, Colors.Black);
                case "uk":
	                return new VocabCategoryInfo(R.VocabCat_UsuallyKana, Colors.Black);
                case "v1":
	                return new VocabCategoryInfo(R.VocabCat_IchidanVerb, Colors.Black);
                case "v2a-s":
	                return new VocabCategoryInfo(R.VocabCat_NidanVerbU, Colors.Black);
                case "v4h":
	                return new VocabCategoryInfo(R.VocabCat_YodanVerbFu, Colors.Black);
                case "v4r":
	                return new VocabCategoryInfo(R.VocabCat_YodanVerbRu, Colors.Black);
                case "v5":
	                return new VocabCategoryInfo(R.VocabCat_GodanVerb, Colors.Black);
                case "v5aru":
	                return new VocabCategoryInfo(R.VocabCat_GodanVerbAru, Colors.Black);
                case "v5b":
	                return new VocabCategoryInfo(R.VocabCat_GodanVerbBu, Colors.Black);
                case "v5g":
	                return new VocabCategoryInfo(R.VocabCat_GodanVerbGu, Colors.Black);
                case "v5k":
	                return new VocabCategoryInfo(R.VocabCat_GodanVerbKu, Colors.Black);
                case "v5k-s":
	                return new VocabCategoryInfo(R.VocabCat_GodanVerbIku, Colors.Black);
                case "v5m":
	                return new VocabCategoryInfo(R.VocabCat_GodanVerbMu, Colors.Black);
                case "v5n":
	                return new VocabCategoryInfo(R.VocabCat_GodanVerbNu, Colors.Black);
                case "v5r":
	                return new VocabCategoryInfo(R.VocabCat_GodanVerbRu, Colors.Black);
                case "v5r-i":
	                return new VocabCategoryInfo(R.VocabCat_GodanVerbRuIrregular, Colors.Black);
                case "v5s":
	                return new VocabCategoryInfo(R.VocabCat_GodanVerbSu, Colors.Black);
                case "v5t":
	                return new VocabCategoryInfo(R.VocabCat_GodanVerbTsu, Colors.Black);
                case "v5u":
	                return new VocabCategoryInfo(R.VocabCat_GodanVerbU, Colors.Black);
                case "v5u-s":
	                return new VocabCategoryInfo(R.VocabCat_GodanVerbUIrregular, Colors.Black);
                case "v5uru":
	                return new VocabCategoryInfo(R.VocabCat_GodanVerbUru, Colors.Black);
                case "vz":
	                return new VocabCategoryInfo(R.VocabCat_IchidanVerbZuru, Colors.Black);
                case "vi":
	                return new VocabCategoryInfo(R.VocabCat_IntransitiveVerb, Colors.Black);
                case "vk":
	                return new VocabCategoryInfo(R.VocabCat_KuruVerb, Colors.Black);
                case "vn":
	                return new VocabCategoryInfo(R.VocabCat_NuVerb, Colors.Black);
                case "vr":
	                return new VocabCategoryInfo(R.VocabCat_RuVerb, Colors.Black);
                case "vs":
	                return new VocabCategoryInfo(R.VocabCat_SuruVerbNoun, Colors.Black);
                case "vs-c":
	                return new VocabCategoryInfo(R.VocabCat_SuVerb, Colors.Black);
                case "vs-s":
	                return new VocabCategoryInfo(R.VocabCat_SuruVerb, Colors.Black);
                case "vs-i":
	                return new VocabCategoryInfo(R.VocabCat_SuruVerbIrregular, Colors.Black);
                case "kyb":
	                return new VocabCategoryInfo(R.VocabCat_KyotoBen, Colors.Black);
                case "osb":
	                return new VocabCategoryInfo(R.VocabCat_OsakaBen, Colors.Black);
                case "ksb":
	                return new VocabCategoryInfo(R.VocabCat_KansaiBen, Colors.Black);
                case "ktb":
	                return new VocabCategoryInfo(R.VocabCat_KantouBen, Colors.Black);
                case "tsb":
	                return new VocabCategoryInfo(R.VocabCat_TosaBen, Colors.Black);
                case "thb":
	                return new VocabCategoryInfo(R.VocabCat_TouhokuBen, Colors.Black);
                case "tsug":
	                return new VocabCategoryInfo(R.VocabCat_TsugaruBen, Colors.Black);
                case "kyu":
	                return new VocabCategoryInfo(R.VocabCat_KyuushuuBen, Colors.Black);
                case "rkb":
	                return new VocabCategoryInfo(R.VocabCat_RyuukyuuBen, Colors.Black);
                case "nab":
	                return new VocabCategoryInfo(R.VocabCat_NaganoBen, Colors.Black);
                case "hob":
	                return new VocabCategoryInfo(R.VocabCat_HokkaidoBen, Colors.Black);
                case "vt":
	                return new VocabCategoryInfo(R.VocabCat_TransitiveVerb, Colors.Black);
                case "vulg":
	                return new VocabCategoryInfo(R.VocabCat_Vulgar, Colors.Black);
                case "adj-kari":
	                return new VocabCategoryInfo(R.VocabCat_KariAdjective, Colors.Black);
                case "adj-ku":
	                return new VocabCategoryInfo(R.VocabCat_KuAdjective, Colors.Black);
                case "adj-shiku":
	                return new VocabCategoryInfo(R.VocabCat_ShikuAdjective, Colors.Black);
                case "adj-nari":
	                return new VocabCategoryInfo(R.VocabCat_NariAdjective, Colors.Black);
                case "n-pr":
	                return new VocabCategoryInfo(R.VocabCat_ProperNoun, Colors.Black);
                case "v-unspec":
	                return new VocabCategoryInfo(R.VocabCat_Verb, Colors.Black);
                case "v4k":
	                return new VocabCategoryInfo(R.VocabCat_ArchaicVerb, Colors.Black);
                case "v4g":
                    return new VocabCategoryInfo(R.VocabCat_ArchaicVerb, Colors.Black);
                case "v4s":
                    return new VocabCategoryInfo(R.VocabCat_ArchaicVerb, Colors.Black);
                case "v4t":
                    return new VocabCategoryInfo(R.VocabCat_ArchaicVerb, Colors.Black);
                case "v4n":
                    return new VocabCategoryInfo(R.VocabCat_ArchaicVerb, Colors.Black);
                case "v4b":
                    return new VocabCategoryInfo(R.VocabCat_ArchaicVerb, Colors.Black);
                case "v4m":
                    return new VocabCategoryInfo(R.VocabCat_ArchaicVerb, Colors.Black);
                case "v2k-k":
                    return new VocabCategoryInfo(R.VocabCat_ArchaicVerb, Colors.Black);
                case "v2g-k":
                    return new VocabCategoryInfo(R.VocabCat_ArchaicVerb, Colors.Black);
                case "v2t-k":
                    return new VocabCategoryInfo(R.VocabCat_ArchaicVerb, Colors.Black);
                case "v2d-k":
                    return new VocabCategoryInfo(R.VocabCat_ArchaicVerb, Colors.Black);
                case "v2h-k":
                    return new VocabCategoryInfo(R.VocabCat_ArchaicVerb, Colors.Black);
                case "v2b-k":
                    return new VocabCategoryInfo(R.VocabCat_ArchaicVerb, Colors.Black);
                case "v2m-k":
                    return new VocabCategoryInfo(R.VocabCat_ArchaicVerb, Colors.Black);
                case "v2y-k":
                    return new VocabCategoryInfo(R.VocabCat_ArchaicVerb, Colors.Black);
                case "v2r-k":
                    return new VocabCategoryInfo(R.VocabCat_ArchaicVerb, Colors.Black);
                case "v2k-s":
                    return new VocabCategoryInfo(R.VocabCat_ArchaicVerb, Colors.Black);
                case "v2g-s":
                    return new VocabCategoryInfo(R.VocabCat_ArchaicVerb, Colors.Black);
                case "v2s-s":
                    return new VocabCategoryInfo(R.VocabCat_ArchaicVerb, Colors.Black);
                case "v2z-s":
                    return new VocabCategoryInfo(R.VocabCat_ArchaicVerb, Colors.Black);
                case "v2t-s":
                    return new VocabCategoryInfo(R.VocabCat_ArchaicVerb, Colors.Black);
                case "v2d-s":
                    return new VocabCategoryInfo(R.VocabCat_ArchaicVerb, Colors.Black);
                case "v2n-s":
                    return new VocabCategoryInfo(R.VocabCat_ArchaicVerb, Colors.Black);
                case "v2h-s":
                    return new VocabCategoryInfo(R.VocabCat_ArchaicVerb, Colors.Black);
                case "v2b-s":
                    return new VocabCategoryInfo(R.VocabCat_ArchaicVerb, Colors.Black);
                case "v2m-s":
                    return new VocabCategoryInfo(R.VocabCat_ArchaicVerb, Colors.Black);
                case "v2y-s":
                    return new VocabCategoryInfo(R.VocabCat_ArchaicVerb, Colors.Black);
                case "v2r-s":
                    return new VocabCategoryInfo(R.VocabCat_ArchaicVerb, Colors.Black);
                case "v2w-s":
	                return new VocabCategoryInfo(R.VocabCat_ArchaicVerb, Colors.Black);
                case "archit":
	                return new VocabCategoryInfo(R.VocabCat_Architecture, Colors.Black);
                case "anat":
	                return new VocabCategoryInfo(R.VocabCat_Anatomy, Colors.Black);
                case "astron":
	                return new VocabCategoryInfo(R.VocabCat_Astronomy, Colors.Black);
                case "baseb":
	                return new VocabCategoryInfo(R.VocabCat_Baseball, Colors.Black);
                case "biol":
	                return new VocabCategoryInfo(R.VocabCat_Biology, Colors.Black);
                case "bot":
	                return new VocabCategoryInfo(R.VocabCat_Botany, Colors.Black);
                case "bus":
	                return new VocabCategoryInfo(R.VocabCat_Business, Colors.Black);
                case "econ":
	                return new VocabCategoryInfo(R.VocabCat_Economy, Colors.Black);
                case "engr":
	                return new VocabCategoryInfo(R.VocabCat_Engineering, Colors.Black);
                case "finc":
	                return new VocabCategoryInfo(R.VocabCat_Finance, Colors.Black);
                case "geol":
	                return new VocabCategoryInfo(R.VocabCat_Geology, Colors.Black);
                case "law":
	                return new VocabCategoryInfo(R.VocabCat_Law, Colors.Black);
                case "med":
	                return new VocabCategoryInfo(R.VocabCat_Medicine, Colors.Black);
                case "music":
	                return new VocabCategoryInfo(R.VocabCat_Music, Colors.Black);
                case "Shinto":
	                return new VocabCategoryInfo(R.VocabCat_Shinto, Colors.Black);
                case "sports":
	                return new VocabCategoryInfo(R.VocabCat_Sports, Colors.Black);
                case "sumo":
	                return new VocabCategoryInfo(R.VocabCat_Sumo, Colors.Black);
                case "zool":
	                return new VocabCategoryInfo(R.VocabCat_Zoology, Colors.Black);
                case "joc":
	                return new VocabCategoryInfo(R.VocabCat_Jocular, Colors.Black);
                default:
                    return null;
            }
        }

        #endregion

        #region Conversion
        
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is IEnumerable<VocabCategory>)
            {
                IEnumerable<VocabCategory> categories = (IEnumerable<VocabCategory>)value;
                StringBuilder fullString = new StringBuilder();
                foreach (VocabCategory category in categories)
                {
                    if (CategoryDictionary.ContainsKey(category.ID))
                    {
                        fullString.Append(CategoryDictionary[category.ID].Label).Append(" ; ");
                    }
                }

                return fullString.ToString().Trim(new char[] { ' ', ';' });
            }
            else if (value is VocabCategory)
            {
                VocabCategory category = (VocabCategory)value;

                if (CategoryDictionary.ContainsKey(category.ID))
                {
                    return CategoryDictionary[category.ID].Label;
                }
            }
            else if (value != null)
            {
                throw new ArgumentException("This converter takes one or more VocabCategory entities.");
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
