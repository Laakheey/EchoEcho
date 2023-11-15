using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
    public class PostService : IPostService
    {
        public readonly IGenericRepository<Post> _postRepository;
        public readonly IMapper _mapper;
        public readonly IGenericRepository<User> _userRepository;
        public readonly UserManager<User> _userManager;
        public readonly IGenericRepository<Like> _likeRepository;

        public PostService(IGenericRepository<Post> postRepository, IGenericRepository<Like> likeRepository, IMapper mapper, IGenericRepository<User> userRepository, UserManager<User> userManager)
        {
            _postRepository = postRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _userManager = userManager;
            _likeRepository = likeRepository;
        }


        public async Task<PostViewModel> AddPost(AddPostViewModel post, string userId)
        {
            var user = await _userRepository.GetById(userId);
            var posts = new Post
            {
                PostId = Guid.NewGuid().ToString(),
                Description = post.Description,
                ParentName = $"{user.FirstName} {user.LastName}",
                ParentId = userId,
                Created = DateTime.Now,
                User = user,
                FileUrl = post.FileUrl,
                FileUrlType = post.FileUrlType
            };
            await _postRepository.Insert(posts);
            await _postRepository.Save();

            return _mapper.Map<PostViewModel>(posts);
        }


        public async Task<DeletePostViewModel> DeletePost(DeletePostViewModel deletePostViewModel)
        {
            var post = await _postRepository.GetAll()
                .Where(x => x.PostId == deletePostViewModel.PostId.ToString() && x.ParentId == deletePostViewModel.ParentId.ToString())
                .FirstOrDefaultAsync();
            if(post == null)
            {
                return null;
            }
            await _postRepository.Delete(post.PostId);
            await _postRepository.Save();
            return deletePostViewModel;
        }

        public async Task<List<PostViewModel>> GetAllPosts(string createdById, int pageNumber, int? numberOfPosts)
        {
            try
            {
                if (numberOfPosts == null || numberOfPosts == 0)
                {
                    numberOfPosts = 10;
                }

                var posts = await _postRepository.GetAll()
                    .OrderByDescending(x => x.Created)
                    .Include(x => x.User)
                    .Skip((pageNumber - 1) * (int)numberOfPosts)
                    .Take((int)numberOfPosts)
                    .ToListAsync();

                if (posts.Count <= 0)
                {
                    return null;
                }

                var currentPostIds = posts.Select(p => p.PostId).ToList();

                var likedPostIds = new HashSet<string>(
                                        await _likeRepository.GetAll()
                                            .Where(l => l.UserId == createdById && currentPostIds.Contains(l.PostId))
                                            .Select(l => l.PostId)
                                            .ToListAsync());

                var totalLikesByPost = await _likeRepository.GetAll()
                                        .Where(l => currentPostIds.Contains(l.PostId))
                                        .GroupBy(l => l.PostId)
                                        .Select(g => new { PostId = g.Key, Total = g.Count() })
                                        .ToListAsync();

                var postViewModels = _mapper.Map<List<PostViewModel>>(posts);
                foreach (var postViewModel in postViewModels)
                {
                    postViewModel.IsLikedByCurrentUser = likedPostIds.Contains(postViewModel.PostId);
                    postViewModel.TotalLikes = totalLikesByPost.FirstOrDefault(t => t.PostId == postViewModel.PostId)?.Total ?? 0;
                }

                return postViewModels;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<PostViewModel> GetPostById(string id)
        {
            var posts = await _postRepository.GetById(id);
            var parentUser = await _userRepository.GetById(posts.ParentId);
            posts.User = parentUser;

            return _mapper.Map<PostViewModel>(posts);
            //throw new NotImplementedException();
        }

        public async Task<PostViewModel> UpdatePost(UpdatePostViewModel post, string userId)
        {
            var isPost = await _postRepository.GetById(post.PostId);
            if(isPost.ParentId != userId)
            {
                return null;
            }
            var parentUser = await _userRepository.GetById(userId);
            if (isPost == null)
            {
                return new PostViewModel
                {
                    PostId = "null",
                };
            }
            isPost.Description = post.Description;
            isPost.Updated = DateTime.Now;
            isPost.User = parentUser;
            isPost.FileUrl = post.FileUrl;
            isPost.FileUrlType = post.FileUrlType;
            _postRepository.Update(isPost);
            await _postRepository.Save();
            return _mapper.Map<PostViewModel>(isPost);
        }

        public async Task<List<PostViewModel>> GetUserAllPosts(string createdById, int pageNumber, int numberOfPosts)
        {
            var posts = await _postRepository.GetAll()
                .Where(x => x.ParentId == createdById).Include(x => x.User)
                .OrderByDescending(x => x.Created)
                .Skip((pageNumber - 1) * (int)numberOfPosts)
                .Take((int)numberOfPosts)
                .ToListAsync();
            if(posts.Count <= 0)
            {
                return null;
            }

            var currentPostIds = posts.Select(p => p.PostId).ToList();

            var likedPostIds = new HashSet<string>(
                                    await _likeRepository.GetAll()
                                        .Where(l => l.UserId == createdById && currentPostIds.Contains(l.PostId))
                                        .Select(l => l.PostId)
                                        .ToListAsync());

            var totalLikesByPost = await _likeRepository.GetAll()
                                    .Where(l => currentPostIds.Contains(l.PostId))
                                    .GroupBy(l => l.PostId)
                                    .Select(g => new { PostId = g.Key, Total = g.Count() })
                                    .ToListAsync();
            var postViewModels = _mapper.Map<List<PostViewModel>>(posts);
            foreach (var postViewModel in postViewModels)
            {
                postViewModel.IsLikedByCurrentUser = likedPostIds.Contains(postViewModel.PostId);
                postViewModel.TotalLikes = totalLikesByPost.FirstOrDefault(t => t.PostId == postViewModel.PostId)?.Total ?? 0;
            }

            return postViewModels;
        }

        public async Task<PostViewModel> DeletePostById(string postId, string userId)
        {
            var post = await _postRepository
                            .GetAll()
                            .Where(x => x.PostId == postId && x.ParentId == userId)
                            .FirstOrDefaultAsync();
            if(post == null)
            {
                return null;
            }
            await _postRepository.Delete(postId);
            await _postRepository.Save();
            return _mapper.Map<PostViewModel>(post);
        }

        public async Task<PostViewModel> UpdatePostById(string postId, string userId)
        {
            var post = await _postRepository
                            .GetAll()
                            .Where(x => x.PostId == postId && x.ParentId == userId)
                            .FirstOrDefaultAsync();
            if(post == null)
            {
                return null;
            }
            //await _postRepository.Update(post);
            return null;
        }

        public async Task<NewPostViewModel> LikeUnlikePost(string postId, string userId)
        {
            var postLike = await _likeRepository
                                .GetAll()
                                .Where(x => x.PostId == postId && x.UserId == userId)
                                .FirstOrDefaultAsync();
            var post = await _postRepository.GetById(postId);
            if (postLike == null)
            {
                var isLiked = new Like()
                {
                    LikeId = Guid.NewGuid().ToString(),
                    PostId = postId,
                    UserId = userId,
                };
                await _likeRepository.Insert(isLiked);
            }
            else
            {
                await _likeRepository.Delete(postLike.LikeId);
            }
            await _likeRepository.Save();
            var totalLikesForPost = await _likeRepository.GetAll().CountAsync(x => x.PostId == postId);

            return new NewPostViewModel()
            {
                Post = post,
                TotalPostLike = totalLikesForPost
            };


            return null;
        }


    }
}
