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

        /// <summary>
        /// Gets the view model for the ignore answer shortcut setting.
        /// </summary>
        public SettingIgnoreAnswerShortcutViewModel IgnoreAnswerShortcutVm { get; private set; }

        #endregion

        #region Methods

        #region Override

        protected override SettingControlViewModel[] InitializeSettingViewModels()
        {
            SrsLevelSetVm = new SettingSrsLevelSetViewModel();
            IgnoreAnswerShortcutVm = new SettingIgnoreAnswerShortcutViewModel();

            return new SettingControlViewModel[]
            {
                SrsLevelSetVm,
                IgnoreAnswerShortcutVm
            };
        }

        #endregion

        #endregion
    }
}
