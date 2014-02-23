using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Database.Entities;
using Kanji.Database.Helpers;

namespace Kanji.Interface.Models
{
    class SrsQuestion
    {
        /// <summary>
        /// Gets or sets the question type.
        /// </summary>
        public SrsQuestionEnum Question { get; set; }

        /// <summary>
        /// Gets or sets the reference group.
        /// </summary>
        public SrsQuestionGroup ParentGroup { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if this question
        /// has been correctly answered.
        /// </summary>
        public bool IsAnswered { get; set; }

        /// <summary>
        /// Gets the answers accepted for this question.
        /// </summary>
        public string AcceptedAnswers
        {
            get
            {
                return MultiValueFieldHelper.Expand(
                    Question == SrsQuestionEnum.Meaning ?
                    ParentGroup.Reference.Meanings
                    : ParentGroup.Reference.Readings);
            }
        }
    }
}
