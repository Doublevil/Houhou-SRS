using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.ViewModels
{
    class KanjiSettingsViewModel : SettingsPageViewModel
    {
        #region Properties

        /// <summary>
        /// Gets the radical set view model.
        /// </summary>
        public SettingRadicalSetViewModel RadicalSetVm { get; private set; }

        /// <summary>
        /// Gets the kanji per page view model.
        /// </summary>
        public SettingKanjiPerPageViewModel KanjiPerPageVm { get; private set; }

        /// <summary>
        /// Gets the kanji info view model.
        /// </summary>
        public SettingKanjiInfoViewModel KanjiInfoVm { get; private set; }

        #endregion

        #region Methods

        #region Override

        protected override SettingControlViewModel[] InitializeSettingViewModels()
        {
            RadicalSetVm = new SettingRadicalSetViewModel();
            KanjiPerPageVm = new SettingKanjiPerPageViewModel();
            KanjiInfoVm = new SettingKanjiInfoViewModel();

            return new SettingControlViewModel[]
            {
                RadicalSetVm,
                KanjiPerPageVm,
                KanjiInfoVm
            };
        }

        #endregion

        #endregion
    }
}
