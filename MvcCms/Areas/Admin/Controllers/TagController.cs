using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MvcCms.Data;

namespace MvcCms.Areas.Admin.Controllers
{
    [RouteArea("admin")]
    [RoutePrefix("tag")]
    [Authorize(Roles = "admin, editor")]
    public class TagController : Controller
    {
        private readonly ITagRepository _repository;

        public TagController(ITagRepository repository)
        {
            _repository = repository;
        }

        // GET: Admin/Tag
        public ActionResult Index()
        {
            IEnumerable<string> tags = _repository.GetAll();
            return View(tags);
        }

        public ActionResult Edit(string tag)
        {
            try
            {
                var model = _repository.Get(tag);
                return View(model);
            }
            catch (KeyNotFoundException)
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string tag, string newTag)
        {
            if (string.IsNullOrWhiteSpace(newTag))
            {
                ModelState.AddModelError("key", "New tag value cannot be empty.");
                return View(tag);
            }

            IEnumerable<string> tags = _repository.GetAll();

            if (!tags.Contains(tag))
            {
                return HttpNotFound();
            }
            if (tags.Contains(newTag))
            {
                return RedirectToAction("index");
            }

            _repository.Edit(tag, newTag);

            return RedirectToAction("index");
        }

        public ActionResult Delete(string tag)
        {
            try
            {
                string model = _repository.Get(tag);
                return View(model);
            }
            catch (KeyNotFoundException)
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string tag, bool foo)
        {
            try
            {
                _repository.Delete(tag);
            }
            catch (KeyNotFoundException)
            {
                return HttpNotFound();
            }

            return RedirectToAction("index");
        }

    }
}