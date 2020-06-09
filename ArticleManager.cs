using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogSystem.Dto;

namespace BlogSystem.BLL
{
    public class ArticleManager : IBLL.IArticleManager
    {
        public async Task BadCount(Guid articleId)
        {
            using (var articleSvc = new DAL.ArticleService())
            {
                var data = await articleSvc.GetOneByIdAsync(articleId);
                data.BadCount += 1;
                await articleSvc.EditAsync(data);
            }
        }

        public async Task CreateArticle(string title, string content, Guid[] categoryIds, Guid userId)
        {
            using (IDAL.IArticleService articleSvc = new DAL.ArticleService())
            {
                var article = new Models.Article()
                {
                    Title = title,
                    Content = content,
                    UserId = userId
                };
                await articleSvc.CreateAsync(article);

                Guid articleId = article.Id;
                using (var articleToCategroySvc = new DAL.ArticleToCategoryService())
                {
                    foreach (var categoryId in categoryIds)
                    {
                        await articleToCategroySvc.CreateAsync(new Models.ArticleToCategory()
                        {
                            ArticleId = articleId,
                            BlogCategoryId = categoryId
                        }, saved: false);
                    }
                    await articleToCategroySvc.Save();
                }
            }
        }

        public async Task CreateCategory(string name, Guid userId)
        {
            using (IDAL.IBlogCategory categorySvc = new DAL.BlogCategoryService())
            {
                await categorySvc.CreateAsync(new Models.BlogCategory()
                {
                    CategoryName = name,
                    UserId = userId
                });
            }
        }

        public async Task EditArticle(Guid articleId, string title, string content, Guid[] categoryIds)
        {
            using (var articleSvc = new DAL.ArticleService())
            {
                var article = await articleSvc.GetOneByIdAsync(articleId);
                article.Title = title;
                article.Content = content;
                await articleSvc.EditAsync(article);
            }
            using (var articleToCategorySvc = new DAL.ArticleToCategoryService())
            {
                foreach (var item in articleToCategorySvc.GetAllAsync().Where(m=>m.ArticleId==articleId))
                {
                    await articleToCategorySvc.RemoveAsync(item, false);
                }
                foreach (var item in categoryIds)
                {
                    await articleToCategorySvc.CreateAsync(new Models.ArticleToCategory() { ArticleId = articleId, BlogCategoryId = item }, false);
                }
                await articleToCategorySvc.Save();
            }
        }

        public async Task EditCategory(Guid categoryId, string newcategoryName)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ExistsArticle(Guid articleId)
        {
            using (var articleSvc = new DAL.ArticleService())
            {
                return await articleSvc.GetAllAsync().AnyAsync(m => m.Id == articleId);
            }
        }

        public async Task<List<ArticleDto>> GetAllArticlesByCategoryId(Guid categoryId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ArticleDto>> GetAllArticlesByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ArticleDto>> GetAllArticlesByUserId(Guid userId, int pageIndex, int pageSize)
        {
            using (var articleSvc = new DAL.ArticleService())
            {
                var list = await articleSvc.GetAllByPageOrderAsync(pageSize, pageIndex, false).Include(m => m.User).Where(m => m.UserId == userId).Select(m => new ArticleDto()
                {
                    Id = m.Id,
                    Title = m.Title,
                    Content = m.Content,
                    CreateTime = m.CreateTime,
                    GoodCount = m.GoodCount,
                    BadCount = m.BadCount,
                    Email = m.User.Email,
                    ImagePath = m.User.ImagePath
                }).ToListAsync();

                using (var articleToCategroySvc = new DAL.ArticleToCategoryService())
                {
                    foreach (var item in list)
                    {
                        var articleToCategroyData = articleToCategroySvc.GetAllAsync().Include(m => m.BlogCategory).Where(m => m.ArticleId == item.Id);
                        item.CategoryIds = articleToCategroyData.Select(m => m.BlogCategoryId).ToArray();
                        item.CategoryNames = articleToCategroyData.Select(m => m.BlogCategory.CategoryName).ToArray();
                    }
                    return list;
                }
            }
        }

        public async Task<List<BlogCategoryDto>> GetAllCategories(Guid userId)
        {
            using (IDAL.IBlogCategory categorySvc = new DAL.BlogCategoryService())
            {
                return await categorySvc.GetAllAsync().Where(m => m.UserId == userId).Select(m => new BlogCategoryDto()
                {
                    Id = m.Id,
                    CategoryName = m.CategoryName
                }).ToListAsync();
            }
        }

        public async Task<int> GetDataCount(Guid userId)
        {
            using (var articleSvc = new DAL.ArticleService())
            {
                return await articleSvc.GetAllAsync().CountAsync(m => m.UserId == userId);
            }
        }

        public async Task<ArticleDto> GetOneArticleById(Guid articleId)
        {
            using (var articleSvc = new DAL.ArticleService())
            {
                var data = await articleSvc.GetAllAsync().Include(m => m.User).Where(m => m.Id == articleId).Select(m => new Dto.ArticleDto()
                {
                    Id = m.Id,
                    Title = m.Title,
                    Content = m.Content,
                    CreateTime = m.CreateTime,
                    GoodCount = m.GoodCount,
                    BadCount = m.BadCount,
                    Email = m.User.Email,
                    ImagePath = m.User.ImagePath
                }).FirstAsync();

                using (var articleToCategroySvc = new DAL.ArticleToCategoryService())
                {
                    var articleToCategroyData = articleToCategroySvc.GetAllAsync().Include(m => m.BlogCategory).Where(m => m.ArticleId == data.Id);
                    data.CategoryIds = articleToCategroyData.Select(m => m.BlogCategoryId).ToArray();
                    data.CategoryNames = articleToCategroyData.Select(m => m.BlogCategory.CategoryName).ToArray();

                    return data;
                }
            }
        }

        public async Task GoodCount(Guid articleId)
        {
            using (var articleSvc = new DAL.ArticleService())
            {
                var data = await articleSvc.GetOneByIdAsync(articleId);
                data.GoodCount += 1;
                await articleSvc.EditAsync(data);
            }
        }

        public async Task RemoveArticle(Guid articleId)
        {
            using (var articleSvc = new DAL.ArticleService())
            {
                await articleSvc.RemoveAsync(articleId);
            }

        }

        public async Task RemoveCategory(string name)
        {
            throw new NotImplementedException();
        }
    }
}
