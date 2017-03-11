using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using VicBlog.Data;

namespace VicBlog.Models
{
    public class TagLink
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string TagName { get; set; }
        public string ArticleID { get; set; }

        public static TagLink Create(string tagName, string articleID, BlogContext context)
        {
            
            var newLink = new TagLink() { TagName = tagName, ArticleID = articleID };
            context.TagLinks.Add(newLink);
            context.SaveChanges();
            return newLink;
        }
    }
}
