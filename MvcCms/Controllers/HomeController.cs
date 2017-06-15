using MvcCms.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MvcCms.Controllers
{
    [RoutePrefix("")]
    public class HomeController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly int _pageSize = 2;


        public HomeController(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }
        public HomeController() : this(new PostRepository())
        { }


        // GET: Home
        // root/
        [Route("")]
        public async Task<ActionResult> Index()
        {
            var posts = await _postRepository.GetPageAsync(1, _pageSize);

            ViewBag.PreviousPage = 0;
            ViewBag.NextPage = Decimal.Divide(_postRepository.CountPublished, _pageSize) > 1
                ? 2
                : -1;

            return View(posts);
        }

        [Route("page/{page:int}")]
        public async Task<ActionResult> Page(int page = 1)
        {
            if (page < 2)
            {
                return RedirectToAction("index");
            }

            var posts = await _postRepository.GetPageAsync(page, _pageSize);

            ViewBag.PreviousPage = page - 1;
            ViewBag.NextPage = Decimal.Divide(_postRepository.CountPublished, _pageSize) > page
                ? page + 1
                : -1;

            return View("index", posts);
        }

        //root/posts/post-id
        [Route("posts/{postId}")]
        public async Task<ActionResult> Post(string postId)
        {
            var post = _postRepository.Get(postId);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);

        }

        //root/tags/tag-id
        [Route("tags/{tagId}")]
        public async Task<ActionResult> Tag(string tagId)
        {
            var posts = await _postRepository.GetPostsByTag(tagId);
            if (!posts.Any())
            {
                return HttpNotFound();
            }
            ViewBag.Tag = tagId;
            return View(posts);
        }
    }
}