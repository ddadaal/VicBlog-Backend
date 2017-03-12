using System.Runtime.Serialization;

namespace VicBlog.Models
{
    public enum ArticleBriefListOrder
    {
        NotSpecified,
        SubmitEarliestToLatest,
        SubmitLatestToEarliest,
        LastEditedEarliestToLatest,
        LastEditedLatestToEarlist,
        RankHighestToLowest,
        RankLowestToHighest
    }

    [DataContract]
    public class ArticleFilter
    {
        [DataMember(Name = "tags")]
        public string[] Tags { get; set; }

        [DataMember(Name = "categories")]
        public string[] Categories { get; set; }

        [DataMember(Name = "titleText")]
        public string TitleText { get; set; }
        [DataMember(Name = "order")]
        public ArticleBriefListOrder Order { get; set; }
        [DataMember(Name = "createdTimeEnabled")]
        public bool CreatedTimeEnabled { get; set; }
        [DataMember(Name = "createdTimeRange")]
        public long[] CreatedTimeRange { get; set; }
        [DataMember(Name = "editedTimeEnabled")]
        public bool EditedTimeEnabled { get; set; }
        [DataMember(Name = "editedTimeRange")]
        public long[] EditedTimeRange { get; set; }

    }
}