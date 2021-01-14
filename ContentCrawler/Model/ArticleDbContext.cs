using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentCrawler.Model
{
    class ArticleDbContext : DbContext
    {
        public ArticleDbContext() : base("name=ArticleDbContext")
        {

        }
        public DbSet<Article> Articles { get; set; }
    }   
}
