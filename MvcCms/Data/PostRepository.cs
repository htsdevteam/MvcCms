using System;
using System.Collections.Generic;
using System.Linq;
using MvcCms.Models;
using System.Data.Entity;
using System.Threading.Tasks;

namespace MvcCms.Data
{
    public class PostRepository : IPostRepository
    {
        public int CountPublished
        {
            get
            {
                using (var db = new CmsContext())
                {
                    return db.Posts.Count(p => p.Published < DateTime.Now);
                }
            }
        }

        public Post Get(string id)
        {
            using (var db = new CmsContext())
            {
                return db.Posts.Include("Author").SingleOrDefault(p => p.Id == id);
            }
        }

        public void Create(Post model)
        {
            using (var db = new CmsContext())
            {
                var post = db.Posts.SingleOrDefault(p => p.Id == model.Id);
                if (post != null)
                {
                    throw new ArgumentException(string.Format(
                        "A post with the id of {0} already exists", model.Id));
                }

                db.Posts.Add(model);
                db.SaveChanges();
            }
        }

        public void Edit(string id, Post updatedItem)
        {
            using (var db = new CmsContext())
            {
                var post = db.Posts.SingleOrDefault(p => p.Id == id);
                if (post == null)
                {
                    throw new KeyNotFoundException(string.Format(
                        "A post with the id of {0} does not exist in the data store.", id));
                }

                post.Id = updatedItem.Id;
                post.Title = updatedItem.Title;
                post.Content = updatedItem.Content;
                post.Published = updatedItem.Published;
                post.Tags = updatedItem.Tags;

                db.SaveChanges();
            }
        }

        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            using (var db = new CmsContext())
            {
                return await db.Posts.Include("Author")
                    .OrderByDescending(p => p.Created)
                    .ToArrayAsync();
            }
        }

        public async Task<IEnumerable<Post>> GetPostsByAuthorAsync(string authorId)
        {
            using (var db = new CmsContext())
            {
                return await db.Posts.Include("Author")
                    .Include("Author")
                    .Where(p => p.AuthorId == authorId)
                    .OrderByDescending(p => p.Created)
                    .ToArrayAsync();

            }
        }

        public void Delete(string id)
        {
            using (var db = new CmsContext())
            {
                var post = db.Posts.SingleOrDefault(p => p.Id == id);
                if (post == null)
                {
                    throw new KeyNotFoundException(
                        string.Format("The post with the id of {0} does not exist.", id));
                }
                db.Posts.Remove(post);
                db.SaveChanges();
            }
        }

        public async Task<IEnumerable<Post>> GetPublishedPostsAsync()
        {
            using (var db = new CmsContext())
            {
                return await db.Posts.Where(p => p.Published < DateTime.Now)
                    .Include("Author")
                    .OrderByDescending(p => p.Published)
                    .ToArrayAsync();
            }
        }

        public async Task<IEnumerable<Post>> GetPostsByTag(string tagId)
        {
            using (var db = new CmsContext())
            {
                var posts = await db.Posts
                    .Include("Author")
                    .Where(p => p.CombinedTags.Contains(tagId))
                    .ToListAsync();
                posts = posts.Where(p => p.Tags
                        .Contains(tagId, StringComparer.CurrentCultureIgnoreCase))
                    .ToList();
                return posts;
            }
        }

        public async Task<IEnumerable<Post>> GetPageAsync(int pageNumber, int pageSize)
        {
            using (var db = new CmsContext())
            {
                int skip = (pageNumber - 1) * pageSize;

                return await db.Posts
                    .Where(p => p.Published < DateTime.Now)
                    .Include("Author")
                    .OrderByDescending(p => p.Published)
                    .Skip(skip)
                    .Take(pageSize)
                    .ToArrayAsync();
            }
        }
    }
}