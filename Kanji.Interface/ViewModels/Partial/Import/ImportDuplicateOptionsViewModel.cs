using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Interface.Models;

namespace Kanji.Interface.ViewModels
{
    class ImportDuplicateOptionsViewModel : ViewModel
    {
        #region Fields

        private ImportDuplicateNewItemAction _duplicateNewItemAction;
        private ImportDuplicateExistingItemAction _duplicateExistingItemAction;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the behavior for an imported item when a similar item already exists.
        /// </summary>
        public ImportDuplicateNewItemAction DuplicateNewItemAction
        {
            get { return _duplicateNewItemAction; }
            set
            {
                if (_duplicateNewItemAction != value)
                {
                    _duplicateNewItemAction = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the behavior for an existing item when a similar item is to be imported.
        /// </summary>
        public ImportDuplicateExistingItemAction DuplicateExistingItemAction
        {
            get { return _duplicateExistingItemAction; }
            set
            {
                if (_duplicateExistingItemAction != value)
                {
                    _duplicateExistingItemAction = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Constructors

        public ImportDuplicateOptionsViewModel()
        {
            DuplicateNewItemAction = ImportDuplicateNewItemAction.Import;
            DuplicateExistingItemAction = ImportDuplicateExistingItemAction.Ignore;
        }

        #endregion

        #region Methods



        #endregion
    }
}
