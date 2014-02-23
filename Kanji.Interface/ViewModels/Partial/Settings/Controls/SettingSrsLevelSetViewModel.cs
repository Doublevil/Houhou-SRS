using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Kanji.Interface.Actors;
using Kanji.Interface.Models;

namespace Kanji.Interface.ViewModels
{
    class SettingSrsLevelSetViewModel : SettingUserResourceViewModel
    {
        #region Methods

        #region Override

        protected override string GetInitialSetName()
        {
            return Properties.Settings.Default.SrsLevelSetName;
        }

        protected override bool CanChangeSet(UserResourceSetInfo setInfo)
        {
            // Show validation messagebox.
            return MessageBox.Show(NavigationActor.Instance.ActiveWindow,
                string.Format("Please be aware that modifying the SRS level set may block "
                + "some existing SRS items in the case where the new level set has less "
                + "levels than the previous one.{0}Also, please note that the current "
                + "scheduled review dates will not be affected.", Environment.NewLine),
                "SRS level set change warning",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Warning,
                MessageBoxResult.Cancel) == MessageBoxResult.OK;
        }

        public override bool IsSettingChanged()
        {
            return Properties.Settings.Default.SrsLevelSetName != SelectedSetName;
        }

        protected override void DoSaveSetting()
        {
            Properties.Settings.Default.SrsLevelSetName = SelectedSetName;
        }

        #endregion

        #endregion
    }
}
