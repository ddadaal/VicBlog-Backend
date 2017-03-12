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


    public class PV
    {
        private BlogContext context = null;
        private List<ArticlePVModel> list = new List<ArticlePVModel>();
        private static int maxLength = 3;

        public PV(BlogContext context)
        {
            this.context = context;
        }

        public void Add(string articleID, string ip)
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

        public int GetPV(string articleID)
        {
            return context.ArticlePVs.Where(x => articleID == x.ArticleID).Count();
        }

        public void DeleteAll(string articleID, bool autoSave = false)
        {
            context.ArticlePVs.RemoveRange(context.ArticlePVs.Where(x => articleID == x.ArticleID));
            if (autoSave)
                context.SaveChanges();
        }


        private void WriteIn()
        {
            context.ArticlePVs.AddRange(list);
            context.SaveChanges();
            list.Clear();
        }
    }
}
