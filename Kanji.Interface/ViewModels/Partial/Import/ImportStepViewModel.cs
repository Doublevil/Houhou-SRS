using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.ViewModels
{
    abstract class ImportStepViewModel : ViewModel
    {
        #region Properties

        /// <summary>
        /// Gets the parent import mode.
        /// </summary>
        public ImportModeViewModel ParentMode { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the step should be skipped when going back from an ulterior step.
        /// </summary>
        public bool SkipOnPrevious { get; set; }

        #endregion

        #region Constructors

        public ImportStepViewModel(ImportModeViewModel parentMode)
        {
            ParentMode = parentMode;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes logic when entering this import step.
        /// </summary>
        public virtual void OnEnterStep()
        {

        }

        /// <summary>
        /// Executes logic when going to the next import step.
        /// Returns <value>true</value> to inform the parent mode that it's okay to go to the next step,
        /// or <value>false</value> to prevent the parent mode to go forward to the next step.
        /// </summary>
        public virtual bool OnNextStep()
        {
            return true;
        }

        /// <summary>
        /// Executes logic when going back to the previous step.
        /// </summary>
        public virtual void OnPreviousStep()
        {

        }

        #endregion
    }
}
