using System;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using VicBlog.Data;
using System.ComponentModel.DataAnnotations;

namespace VicBlog.Models
{
    public partial class ArticleBrief
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ID { get; set; }

        public string Username { get; set; }

        public DateTime SubmitTime { get; set; }

        public DateTime LastEditedTime { get; set; }

        public string Title { get; set; }

        public string Category { get; set; }

        [NotMapped]
        public string[] Tags { get; set; }

        [NotMapped]
        public double Rate { get; set; }

        [NotMapped]
        public int PV { get; set; }

        public ArticleBrief LoadTheRest(BlogContext context)
        {
            Tags = context.TagLinks.Where(x => x.ArticleID == ID).Select(x => x.TagName).ToArray();
            Rate = context.Rates.Where(x => x.ArticleID == ID).Select(x => x.Score).Average();
            PV = context.ArticlePVs.Where(x => x.ArticleID == ID).Count();
            return this;
        }

        public ArticleBrief LoadRate(BlogContext context)
        {
            Rate = context.Rates.Where(x => x.ArticleID == ID).Select(x => x.Score).Average();
            return this;
        }
        public ArticleBrief LoadTags(BlogContext context)
        {
            Tags = context.TagLinks.Where(x => x.ArticleID == ID).Select(x => x.TagName).ToArray();
            return this;
        }
        public ArticleBrief LoadPV(BlogContext context)
        {
            PV = context.ArticlePVs.Where(x => x.ArticleID == ID).Count();
            return this;
        }

    }
}
