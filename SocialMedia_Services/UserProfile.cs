using AutoMapper;
using SocialMedia_Common.FriendRequests;
using SocialMedia_Common.UserView;
using SocialMedia_Data.Entity;

namespace SocialMedia_Services
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserViewModel>();
            CreateMap<FriendsRequest, FriendRequestViewModel>();
        }
    }
}