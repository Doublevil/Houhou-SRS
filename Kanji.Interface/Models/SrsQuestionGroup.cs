using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Database.Entities;
using Kanji.Database.Helpers;

namespace Kanji.Interface.Models
{
    class SrsQuestionGroup
    {
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
        /// Gets or sets a reference to the meaning question.
        /// </summary>
        public SrsQuestion MeaningQuestion { get; set; }

        /// <summary>
        /// Gets or sets a reference to the reading question.
        /// </summary>
        public SrsQuestion ReadingQuestion { get; set; }

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

        #endregion

        #region Constructors

        /// <summary>
        /// Builds a question group for the given SRS entry.
        /// </summary>
        /// <param name="reference">Target SRS entry.</param>
        public SrsQuestionGroup(SrsEntry reference)
        {
            Reference = reference;
            MeaningQuestion = new SrsQuestion()
            {
                Question = SrsQuestionEnum.Meaning,
                ParentGroup = this
            };

            ReadingQuestion = new SrsQuestion()
            {
                Question = SrsQuestionEnum.Reading,
                ParentGroup = this
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the questions of this group that weren't answered yet.
        /// </summary>
        public IEnumerable<SrsQuestion> GetUnansweredQuestions()
        {
            if (!MeaningQuestion.IsAnswered)
            {
                yield return MeaningQuestion;
            }

            if (!ReadingQuestion.IsAnswered)
            {
                yield return ReadingQuestion;
            }
        }

        #endregion
    }
}
