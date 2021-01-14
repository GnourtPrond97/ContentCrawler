using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentCrawler.Model
{
    class Article
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }
        public string Images { get; set; }
        public string Content { get; set; }

        public Article()
        {
        }
    }
}
