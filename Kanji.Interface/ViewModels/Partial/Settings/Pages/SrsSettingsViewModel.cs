using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.ViewModels
{
    class SrsSettingsViewModel : SettingsPageViewModel
    {
        #region Properties

        /// <summary>
        /// Gets the SRS level set view model.
        /// </summary>
        public SettingSrsLevelSetViewModel SrsLevelSetVm { get; private set; }

        #endregion

        #region Methods

        #region Override

        protected override SettingControlViewModel[] InitializeSettingViewModels()
        {
            SrsLevelSetVm = new SettingSrsLevelSetViewModel();

            return new SettingControlViewModel[]
            {
                SrsLevelSetVm
            };
        }

        #endregion

        #endregion
    }
}
