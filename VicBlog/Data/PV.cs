using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VicBlog.Data
{
    
    public class ArticlePVModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string ArticleID { get; set; }
        public string IP { get; set; }
        public DateTime ViewTime { get; set; }
    }


    public static class PV
    {

        public static void Add(string articleID, string ip, BlogContext context)
        {
            ArticlePVModel pv = new ArticlePVModel()
            {
                ArticleID = articleID,
                IP = ip,
                ViewTime = DateTime.Now.ToUniversalTime()
            };


            context.ArticlePVs.Add(pv);
            context.SaveChanges();



            //if (list.Count > maxLength)
            //{
            //    WriteIn();
            //}
        }

        public static int GetPV(string articleID, BlogContext context)
        {
            return context.ArticlePVs.Where(x => articleID == x.ArticleID).Count();
        }

        public static void DeleteAll(BlogContext context, string articleID, bool autoSave = false)
        {
            context.ArticlePVs.RemoveRange(context.ArticlePVs.Where(x => articleID == x.ArticleID));
            if (autoSave)
                context.SaveChanges();
        }

    }
}
