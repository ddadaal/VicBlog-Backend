using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VicBlogServer.Models.ListOrder
{
    public enum ListOrder
    {
        LastEditTimeLatestFirst,
        LastEditTimeEarliestFirst,
        SubmitTimeLatestFirst,
        SubmitTimeEarliestFirst,
        LikeMostFirst
    }

}
