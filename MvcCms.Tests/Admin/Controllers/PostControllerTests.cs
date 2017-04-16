using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcCms.Areas.Admin.Controllers;
using MvcCms.Data;
using MvcCms.Models;
using Telerik.JustMock;

namespace MvcCms.Tests.Admin.Controllers
{
    [TestClass]
    public class PostControllerTests
    {
        private const string _postId = "test-post";

        [TestMethod]
        public void Edit_GetRequestSendsPostToView()
        {
            var repo = Mock.Create<IPostRepository>();
            Mock.Arrange(() => repo.Get(_postId))
                .Returns(new Post { Id = _postId });
            var controller = new PostController(repo);

            var result = controller.Edit(_postId) as ViewResult;
            var model = result.Model as Post;

            Assert.AreEqual(_postId, model.Id);
        }

        [TestMethod]
        public void Edit_GetRequestNotFoundResult()
        {
            var repo = Mock.Create<IPostRepository>();
            Mock.Arrange(() => repo.Get(_postId))
                .Returns(null as Post);
            var controller = new PostController(repo);

            ActionResult result = controller.Edit(_postId);

            Assert.IsTrue(result is HttpNotFoundResult);
        }

        [TestMethod]
        public void Edit_PostRequestNotFoundResult()
        {
            var repo = Mock.Create<IPostRepository>();
            Mock.Arrange(() => repo.Get(_postId))
                .Returns(null as Post);
            var controller = new PostController(repo);

            ActionResult result = controller.Edit(_postId, new Post());

            Assert.IsTrue(result is HttpNotFoundResult);
        }

        [TestMethod]
        public void Edit_PostRequestSendsPostToView()
        {
            var repo = Mock.Create<IPostRepository>();
            Mock.Arrange(() => repo.Get(_postId))
                .Returns(new Post { Id = _postId });
            var controller = new PostController(repo);

            controller.ViewData.ModelState.AddModelError("key", "error message");

            var result = controller.Edit(_postId, new Post { Id = "test-post-2" }) as ViewResult;
            var model = result.Model as Post;

            Assert.AreEqual("test-post-2", model.Id);
        }

        [TestMethod]
        public void Edit_PostRequestCallsEditAndRedirects()
        {
            var repo = Mock.Create<IPostRepository>();
            Mock.Arrange(() => repo.Edit(Arg.IsAny<string>(), Arg.IsAny<Post>()))
                .MustBeCalled();
            var controller = new PostController(repo);

            ActionResult result = controller.Edit("foo", new Post { Id = "test-post-2" });

            Mock.Assert(repo);
            Assert.IsTrue(result is RedirectToRouteResult);
        }

    }
}
