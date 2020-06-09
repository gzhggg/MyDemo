using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogSystem.Models;

namespace BlogSystem.DAL
{
    public class ArticleToCategoryService : BaseService<Models.ArticleToCategory>, IDAL.IArticleToCategoryService
    {
        public ArticleToCategoryService() : base(new BlogContext())
        {
        }
    }
}
