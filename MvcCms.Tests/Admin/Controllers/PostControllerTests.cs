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
            var postRepo = Mock.Create<IPostRepository>();
            var userRepo = Mock.Create<IUserRepository>();

            Mock.Arrange(() => postRepo.Get(_postId))
                .Returns(new Post { Id = _postId });
            var controller = new PostController(postRepo, userRepo);

            var result = controller.Edit(_postId).Result as ViewResult;
            var model = result.Model as Post;

            Assert.AreEqual(_postId, model.Id);
        }

        [TestMethod]
        public void Edit_GetRequestNotFoundResult()
        {
            var repo = Mock.Create<IPostRepository>();
            var userRepo = Mock.Create<IUserRepository>();
            Mock.Arrange(() => repo.Get(_postId))
                .Returns(null as Post);
            var controller = new PostController(repo, userRepo);

            ActionResult result = controller.Edit(_postId).Result;

            Assert.IsTrue(result is HttpNotFoundResult);
        }

        [TestMethod]
        public void Edit_PostRequestNotFoundResult()
        {
            var repo = Mock.Create<IPostRepository>();
            var userRepo = Mock.Create<IUserRepository>();
            Mock.Arrange(() => repo.Get(_postId))
                .Returns(null as Post);
            var controller = new PostController(repo, userRepo);

            ActionResult result = controller.Edit(_postId, new Post()).Result;

            Assert.IsTrue(result is HttpNotFoundResult);
        }

        [TestMethod]
        public void Edit_PostRequestSendsPostToView()
        {
            var repo = Mock.Create<IPostRepository>();
            var userRepo = Mock.Create<IUserRepository>();
            Mock.Arrange(() => repo.Get(_postId))
                .Returns(new Post { Id = _postId });
            var controller = new PostController(repo, userRepo);

            controller.ViewData.ModelState.AddModelError("key", "error message");

            var result = controller.Edit(_postId, new Post { Id = "test-post-2" }).Result as ViewResult;
            var model = result.Model as Post;

            Assert.AreEqual("test-post-2", model.Id);
        }

        [TestMethod]
        public void Edit_PostRequestCallsEditAndRedirects()
        {
            var repo = Mock.Create<IPostRepository>();
            var userRepo = Mock.Create<IUserRepository>();
            Mock.Arrange(() => repo.Edit(Arg.IsAny<string>(), Arg.IsAny<Post>()))
                .MustBeCalled();
            var controller = new PostController(repo, userRepo);

            ActionResult result = controller.Edit("foo", new Post { Id = "test-post-2" }).Result;

            Mock.Assert(repo);
            Assert.IsTrue(result is RedirectToRouteResult);
        }

    }
}
