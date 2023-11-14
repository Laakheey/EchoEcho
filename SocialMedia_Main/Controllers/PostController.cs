using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using SocialMedia_Common.PostView;
using SocialMedia_Data.Entity;
using SocialMedia_Services.Posts;
using SocialMedia_Services.UsersService;

namespace SocialMedia_Main.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostController : BaseController
    {
        public readonly IUserService _userService;
        public readonly IPostService _postService;
        public readonly UserManager<User> _userManager;

        public PostController(IUserService userService, IPostService postService, UserManager<User> userManager)
        {
            _userManager = userManager;
            _userService = userService;
            _postService = postService;
        }

        [HttpGet]
        [Route("getAllPost")]
        public async Task<IActionResult> GetAllPost(Guid parentId, int pageNumber, int pageSize)
        {
            var posts = await _postService.GetAllPosts(parentId.ToString(), pageNumber, pageSize);
            if (posts == null)
            {
                return Ok(new { IsSuccess = true, Post = posts, ErrorMessage = $"You have no more posts" });
            }
            return Ok(new { IsSuccess = true, Post = posts, ErrorMessage = $"" });
        }

        [HttpGet]
        [Route("getPostById")]
        public async Task<IActionResult> GetPostById(string id)
        {
            var post = await _postService.GetPostById(id);
            if(post == null)
            {
                return Ok(new { IsSuccess = false, Post = post, ErrorMessage = $"Post does not exist for the postId: {id}"});
            }
            return Ok(new { IsSuccess = true, Post = post, ErrorMessage = "" });
        }

        [HttpPost]
        [Route("addNewPost")]
        public async Task<IActionResult> AddNewPost(AddPostViewModel postViewModel)
        {
            var userId = await GetUserIdAsync(this._userManager);
            var posts = await _postService.AddPost(postViewModel, userId);
            return Ok(new { IsSuccess = true, Post = posts, ErrorMessage = "" });
        }

        [HttpPost]
        [Route("updatePost")]
        public async Task<IActionResult> UpdatePost(UpdatePostViewModel updatePostViewModel)
        {
            var userId = await GetUserIdAsync(this._userManager);
            var posts = await _postService.UpdatePost(updatePostViewModel, userId);
            var nullObject = null as object;
            if (posts == null)
            {
                return Ok(new { IsSuccess = false, Post = posts, ErrorMessage = $"You are unauthorized to update this post" });
            }
            else if (posts.PostId == "null")
            {
                return Ok(new { IsSuccess = false, Post = nullObject, ErrorMessage = $"Post does not exist from postId: {updatePostViewModel.PostId}" });
            }
            return Ok(new { IsSuccess = true, Post = posts, ErrorMessage = "" });
        }

        [HttpGet]
        [Route("getUsersAllPost")]
        public async Task<IActionResult> GetUsersAllPost(Guid parentId, int pageNumber, int? numberOfPosts)
        {
            if(numberOfPosts == null || numberOfPosts <= 0)
            {
                numberOfPosts = 10;
            }
            var posts = await _postService.GetUserAllPosts(parentId.ToString(), pageNumber, (int)numberOfPosts);
            if (posts == null)
            {
                return Ok(new { IsSuccess = true, Post = posts, ErrorMessage = $"You have no more posts" });
            }
            return Ok(new { IsSuccess = true, Post = posts, ErrorMessage = $"" });
        }

        [HttpDelete]
        [Route("deletePostById")]
        public async Task<IActionResult> DeletePostById(Guid postId)
        {
            var userId = await GetUserIdAsync(this._userManager);
            var post = await _postService.DeletePostById(postId.ToString(), userId);
            if(post == null)
            {
                return Ok(new { IsSuccess = false, Post = post, ErrorMessage = $"You are not authorized to delete this post" });
            }
            return Ok(new { IsSuccess = true, Post = post, ErrorMessage = $"" });
        }

        [HttpPost]
        [Route("likeUnlikePost")]
        public async Task<IActionResult> LikeUnlikePost(Guid postId, Guid userId)
        {
            var post = await _postService.LikeUnlikePost(postId.ToString(), userId.ToString());
            return Ok(new { IsSuccess = true, Post = post, ErrorMessage = $"" });
        }
    }
}
