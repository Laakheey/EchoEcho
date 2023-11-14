using Microsoft.AspNetCore.Http;
using SocialMedia_Common.PostView;
using SocialMedia_Data.Entity;
using SocialMedia_DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia_Services.Posts
{
    public interface IPostService
    {
        Task<PostViewModel> GetPostById (string id);
        Task<PostViewModel> AddPost (AddPostViewModel post, string userId);
        Task<PostViewModel> UpdatePost (UpdatePostViewModel post, string userId);
        Task<DeletePostViewModel> DeletePost (DeletePostViewModel deletePostViewModel);
        Task<List<PostViewModel>> GetAllPosts (string createdById, int pageNumber, int? numberOfPosts);
        Task<List<PostViewModel>> GetUserAllPosts(string createdById, int pageNumber, int numberOfPosts);
        Task<PostViewModel> DeletePostById(string postId, string userId);
        Task<NewPostViewModel> LikeUnlikePost(string postId, string userId);
    }
}
