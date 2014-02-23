using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.ViewModels
{
    class SettingRadicalSetViewModel : SettingUserResourceViewModel
    {
        #region Methods

        #region Override

        protected override string GetInitialSetName()
        {
            return Properties.Settings.Default.RadicalSetName;
        }

        protected override bool CanChangeSet(Models.UserResourceSetInfo setInfo)
        {
            return true;
        }

        public override bool IsSettingChanged()
        {
            return Properties.Settings.Default.RadicalSetName != SelectedSetName;
        }

        protected override void DoSaveSetting()
        {
            Properties.Settings.Default.RadicalSetName = SelectedSetName;
        }

        #endregion

        #endregion
    }
}
