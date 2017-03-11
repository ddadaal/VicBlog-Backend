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

    }
}