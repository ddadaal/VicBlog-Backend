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
        private static int maxLength = 1;

        public PV(BlogContext context)
        {
            this.context = context;
        }

        public async void Add(string articleID, string ip)
        {
            ArticlePVModel pv = new ArticlePVModel()
            {
                ArticleID = articleID,
                IP = ip,
                ViewTime = DateTime.Now.ToUniversalTime()
            };
            list.Add(pv);
            if (list.Count > maxLength)
            {
                await WriteIn();
                list.Clear();
            }
        }

        private async Task WriteIn()
        {
            context.ArticlePVs.AddRange(list);
            await context.SaveChangesAsync();
        }
    }
}
