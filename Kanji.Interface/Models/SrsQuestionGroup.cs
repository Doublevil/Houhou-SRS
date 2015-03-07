using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Database.Entities;
using Kanji.Database.Helpers;
using Kanji.Interface.Utilities;

namespace Kanji.Interface.Models
{
    class SrsQuestionGroup : NotifyPropertyChanged
    {
        #region Fields

        private VocabAudio _audio;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the reference SRS entry that this question
        /// is about.
        /// </summary>
        public SrsEntry Reference { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if at least one of the
        /// questions was answered wrong.
        /// </summary>
        public bool IsWrong { get; set; }

        /// <summary>
        /// Gets or sets the questions of this group.
        /// </summary>
        public List<SrsQuestion> Questions { get; set; }

        /// <summary>
        /// Gets the string representation of the SRS Item.
        /// </summary>
        public string Item
        {
            get { return Reference.AssociatedKanji ?? Reference.AssociatedVocab; }
        }

        /// <summary>
        /// Gets a boolean value indicating if the underlying SRS entry
        /// refers to a kanji entity.
        /// </summary>
        public bool IsKanji
        {
            get { return !string.IsNullOrEmpty(Reference.AssociatedKanji); }
        }

        /// <summary>
        /// Gets a boolean value indicating if the underlying SRS entry
        /// refers to a vocab entity.
        /// </summary>
        public bool IsVocab
        {
            get { return !string.IsNullOrEmpty(Reference.AssociatedVocab); }
        }

        /// <summary>
        /// Gets or sets the audio component used to play the vocab when relevant.
        /// </summary>
        public VocabAudio Audio
        {
            get { return _audio; }
            set
            {
                if (_audio != value)
                {
                    _audio = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Builds a question group for the given SRS entry.
        /// </summary>
        /// <param name="reference">Target SRS entry.</param>
        public SrsQuestionGroup(SrsEntry reference)
        {
            Reference = reference;

            Questions = new List<SrsQuestion>();
            if (!string.IsNullOrWhiteSpace(reference.Meanings))
            {
                Questions.Add(new SrsQuestion()
                {
                    Question = SrsQuestionEnum.Meaning,
                    ParentGroup = this
                });
            }

            if (!string.IsNullOrWhiteSpace(reference.Readings))
            {
                Questions.Add(new SrsQuestion()
                {
                    Question = SrsQuestionEnum.Reading,
                    ParentGroup = this
                });
            }

            Audio = new VocabAudio(Reference);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the questions of this group that weren't answered yet.
        /// </summary>
        public IEnumerable<SrsQuestion> GetUnansweredQuestions()
        {
            return Questions.Where(q => !q.IsAnswered);
        }

        #endregion
    }
}
