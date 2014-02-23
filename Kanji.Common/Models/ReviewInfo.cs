using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kanji.Common.Models
{
    public class ReviewInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the date the review info query was issued.
        /// </summary>
        public DateTime QueryDate { get; set; }

        /// <summary>
        /// Gets or sets the number of available reviews.
        /// </summary>
        public long AvailableReviewsCount { get; set; }

        /// <summary>
        /// Gets or sets the first review date in chronological order.
        /// </summary>
        public DateTime? FirstReviewDate { get; set; }

        /// <summary>
        /// Gets or sets the total number of kanji items.
        /// </summary>
        public long KanjiItemsCount { get; set; }

        /// <summary>
        /// Gets or sets the total number of vocab items.
        /// </summary>
        public long VocabItemsCount { get; set; }

        /// <summary>
        /// Gets or sets the total number of successful reviews.
        /// </summary>
        public long TotalSuccessCount { get; set; }

        /// <summary>
        /// Gets or sets the total number of failed reviews.
        /// </summary>
        public long TotalFailureCount { get; set; }

        /// <summary>
        /// Gets or sets the remaining number of reviews for today (until midnight).
        /// </summary>
        public long TodayReviewsCount { get; set; }

        /// <summary>
        /// Gets or sets the dictionary storing the number of reviews per level.
        /// </summary>
        public Dictionary<short, long> ReviewsPerLevel { get; set; }

        #region Calculated

        /// <summary>
        /// Gets the total number of items.
        /// </summary>
        public long TotalItemsCount
        {
            get { return KanjiItemsCount + VocabItemsCount; }
        }

        /// <summary>
        /// Gets the total amount of reviews done.
        /// </summary>
        public double TotalReviewsCount
        {
            get { return TotalSuccessCount + TotalFailureCount; }
        }

        /// <summary>
        /// Gets the ratio of success per review.
        /// </summary>
        public double SuccessRate
        {
            get
            {
                if (TotalReviewsCount == 0)
                {
                    return 0;
                }

                return (double)TotalSuccessCount / (double)TotalReviewsCount;
            }
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Builds a review info object.
        /// </summary>
        public ReviewInfo()
        {
            QueryDate = DateTime.UtcNow;
            ReviewsPerLevel = new Dictionary<short, long>();
        }

        #endregion
    }
}
