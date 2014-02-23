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

        #endregion

        #region Methods

        #region Override

        protected override SettingControlViewModel[] InitializeSettingViewModels()
        {
            RadicalSetVm = new SettingRadicalSetViewModel();

            return new SettingControlViewModel[]
            {
                RadicalSetVm
            };
        }

        #endregion

        #endregion
    }
}
