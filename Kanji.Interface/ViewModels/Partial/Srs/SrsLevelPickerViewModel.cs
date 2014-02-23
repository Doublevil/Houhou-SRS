using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Kanji.Interface.Business;
using Kanji.Interface.Models;

namespace Kanji.Interface.ViewModels
{
    class SrsLevelPickerViewModel : ViewModel
    {
        #region Fields

        private SrsLevelGroup[] _srsLevelGroups;

        private short _currentLevelValue;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the available SRS level groups.
        /// </summary>
        public SrsLevelGroup[] SrsLevelGroups
        {
            get { return _srsLevelGroups; }
            private set
            {
                if (_srsLevelGroups != value)
                {
                    _srsLevelGroups = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating if the SRS levels loading
        /// operation is finished.
        /// </summary>
        public bool IsSrsLevelLoadingDone
        {
            get { return _srsLevelGroups != null; }
        }

        /// <summary>
        /// Gets or sets the currently selected level value.
        /// </summary>
        public short CurrentLevelValue
        {
            get { return _currentLevelValue; }
            set
            {
                if (_currentLevelValue != value)
                {
                    _currentLevelValue = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Events

        public delegate void SrsLevelSelectedHandler(
            object sender, SrsLevelSelectedEventArgs e);
        /// <summary>
        /// Event triggered when an SRS level is selected.
        /// </summary>
        public event SrsLevelSelectedHandler SrsLevelSelected;

        #endregion

        #region Commands

        /// <summary>
        /// Gets or sets the command that sets the SRS level of the entry.
        /// </summary>
        public RelayCommand<SrsLevel> SelectSrsLevelCommand { get; set; }

        #endregion

        #region Constructors

        public SrsLevelPickerViewModel()
        {
            SelectSrsLevelCommand = new RelayCommand<SrsLevel>(OnSelectSrsLevel);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the ViewModel with the current value.
        /// Starts the level retrieving method.
        /// </summary>
        /// <param name="currentLevelValue">Initial value.</param>
        public void Initialize(short currentLevelValue)
        {
            CurrentLevelValue = currentLevelValue;
            SrsLevelStore.Instance.IssueWhenLoaded(OnSrsLevelsLoaded);
        }

        /// <summary>
        /// Sets the given value as the current selection.
        /// </summary>
        /// <param name="value">Value of the level to set as the
        /// current selection.</param>
        public void SelectLevel(short value)
        {
            SrsLevelStore.Instance.IssueWhenLoaded(() =>
                {
                    SelectLevel(SrsLevelStore.Instance.GetLevelByValue(value));
                });
        }

        /// <summary>
        /// Sets the given level as the current selection.
        /// </summary>
        /// <param name="level">Level to set as the current selection.</param>
        public void SelectLevel(SrsLevel level)
        {
            if (level != null)
            {
                CurrentLevelValue = level.Value;
            }

            if (SrsLevelSelected != null)
            {
                SrsLevelSelected(this, new SrsLevelSelectedEventArgs(level));
            }
        }

        #region Command callbacks

        /// <summary>
        /// Command callback.
        /// Called to react to a new SRS Level selection.
        /// </summary>
        /// <param name="selectedLevel">Newly selected SRS level.</param>
        private void OnSelectSrsLevel(SrsLevel selectedLevel)
        {
            SelectLevel(selectedLevel);
        }

        #endregion

        #region Event callbacks

        /// <summary>
        /// Action callback executed during the ViewModel initialization, or
        /// when SRS levels are loaded if they were not already.
        /// </summary>
        private void OnSrsLevelsLoaded()
        {
            SrsLevelGroups = SrsLevelStore.Instance.CurrentSet;
            OnSelectSrsLevel(
                SrsLevelStore.Instance.GetLevelByValue(CurrentLevelValue));
        }

        #endregion

        #endregion
    }
}
