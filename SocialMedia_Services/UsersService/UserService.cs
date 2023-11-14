using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SocialMedia_Common.FriendRequests;
using SocialMedia_Common.UserView;
using SocialMedia_Data.Entity;
using SocialMedia_DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia_Services.UsersService
{
    public class UserService : IUserService
    {
        public readonly IGenericRepository<User> _userRepository;
        public readonly IGenericRepository<FriendsRequest> _friendsRepository;
        private readonly IMapper _mapper;

        public UserService(IGenericRepository<User> userRepository, IMapper mapper, IGenericRepository<FriendsRequest> friendsRepository)
        {
            _userRepository = userRepository;
            _friendsRepository = friendsRepository;
            _mapper = mapper;
        }
        public async Task<UserViewModel> GetUserById(string id)
        {
            var user = await _userRepository.GetById(id);
            if(user == null)
            {
                return null;
            }
            return _mapper.Map<UserViewModel>(user);
        }

        public async Task<List<FriendRequestViewModel>> GetAllRequestUsers(string id, int pageNumber, int pageSize)
        {
            //var user = await _userRepository.GetAll()
            //            .Where(x => x.Id == id)
            //            .Include(x => x.ReceivedRequests)
            //            .ThenInclude(x => x.Requester).ToListAsync();
            var user = await _friendsRepository.GetAll()
                            .Where(x => x.RecipientId == id)
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();
            if(user.Count() == 0)
            {
                return null;
            }
            return _mapper.Map<List<FriendRequestViewModel>>(user);
        }

        public async Task<FriendRequestViewModel> SendFriendRequest(Guid senderId, Guid receiverId)
        {
            try
            {
                var user = await _userRepository.GetById(receiverId.ToString());
                var senderUser = await _userRepository.GetById(senderId.ToString());
                if (user == null)
                {
                    return null;
                }
                //var existingRequest = await _friendsRepository
                //        .GetAll()
                //        .Where(x => (x.RequesterId == senderId.ToString() && x.RecipientId == receiverId.ToString())
                //                 || (x.RequesterId == receiverId.ToString() && x.RecipientId == senderId.ToString()))
                //        .FirstOrDefaultAsync();

                //if (existingRequest != null)
                //{
                //    return _mapper.Map<FriendRequestViewModel>(existingRequest);
                //}
                var request = new FriendsRequest
                {
                    FriendsId = Guid.NewGuid().ToString(),
                    IsFriendRequestAccepted = false,
                    RecipientId = user.Id,
                    RequestDate = DateTime.Now,
                    RequesterId = senderUser.Id,
                };
                await _friendsRepository.Insert(request);
                await _friendsRepository.Save();

                //senderUser.SentRequests?.Add(request);
                //user.ReceivedRequests?.Add(request);

                //_userRepository.Update(user);
                //_userRepository.Update(senderUser);
                //await _userRepository.Save();

                return _mapper.Map<FriendRequestViewModel>(request);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<List<UserViewModel>> GetSearchUser(string searchString)
        {
            searchString = searchString.Trim().ToLower();
            var user = await _userRepository.GetAll()
                .Where(x => (x.FirstName + " " + x.LastName).ToLower().Contains(searchString.ToLower()))
                .Skip(0)
                .Take(10)
                .ToListAsync();

            if(user.Count == 0)
            {
                return null;
            }
            return _mapper.Map<List<UserViewModel>>(user);
        }

        public async Task<UserViewModel> EditUserDetails(EditUserDetailsViewModel editUserDetails, string userId)
        {
            var user = await _userRepository.GetById(userId);
            user.FirstName = editUserDetails.FirstName;
            user.LastName = editUserDetails.LastName;
            user.PhoneNumber = editUserDetails.PhoneNumber;
            user.Avatar = editUserDetails.Avatar;
            user.DateOfBirth = editUserDetails.DateOfBirth;
            user.Gender = editUserDetails.Gender;
            user.City = editUserDetails.City;
            user.Country = editUserDetails.Country;
            _userRepository.Update(user);
            await _userRepository.Save();
            return _mapper.Map<UserViewModel>(user);
        }

    }
}
