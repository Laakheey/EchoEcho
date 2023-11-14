using SocialMedia_Common.FriendRequests;
using SocialMedia_Common.UserView;
using SocialMedia_Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia_Services.UsersService
{
    public interface IUserService
    {
        Task<UserViewModel> GetUserById(string id);
        Task<List<FriendRequestViewModel>> GetAllRequestUsers(string id, int pageNumber, int pageSize);
        Task<FriendRequestViewModel> SendFriendRequest(Guid senderId, Guid receiverId);
        Task<List<UserViewModel>> GetSearchUser(string searchString);
        Task<UserViewModel> EditUserDetails(EditUserDetailsViewModel editUserDetails, string userId);
    }
}
