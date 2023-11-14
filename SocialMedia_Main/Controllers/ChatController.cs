using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PusherServer;
using SocialMedia_Common.ChatView;
using SocialMedia_Data.Entity;
using SocialMedia_DataAccess.Repository;
using SocialMedia_Services.ChatService;
using SocialMedia_Services.UsersService;
using System.Net;

namespace SocialMedia_Main.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : BaseController
    {
        public readonly IChatServices _chatServices;
        public readonly UserManager<User> _userManager;
        public readonly IUserService _userService;
        public readonly IGenericRepository<Chat> _chatRepository;
        private readonly IGenericRepository<ChatHead> _chatHeadRepository;
        public ChatController(IChatServices chatServices, UserManager<User> userManager,IGenericRepository<Chat> chatRepository, IUserService userService, IGenericRepository<ChatHead> chatHeadRepository)
        {
            _chatServices = chatServices;
            _userManager = userManager;
            _userService = userService;
            _chatRepository = chatRepository;
            _chatHeadRepository = chatHeadRepository;
        }
        [HttpPost]
        [Route("sendMessage")]
        public async Task<IActionResult> SendMessage(AddMessageViewModel messageViewModel)
        {
            var loggedInUser = await GetUserIdAsync(this._userManager);
            messageViewModel.SenderId = Guid.Parse(loggedInUser);
            var isMessageStored = await _chatServices.SendMessageAsync(messageViewModel);
            if (isMessageStored == null)
            {
                return Ok(new { IsSuccess = false, Data = "", ErrorMessage = "Message was not sent" });
            }
            //var channelName = $"private-{messageViewModel.ChatHeadID}";
            var channelName = "chat";
            var options = new PusherOptions
            {
                Cluster = "ap2",
                Encrypted = true
            };

            var pusher = new Pusher(
              "1687856",
              "014bb49542364098f9cb",
              "ec8dbf1e9ae6321642d7",
              options);

            await pusher.TriggerAsync(
              channelName,
              "sendMessage",
              new 
              {
                  Data = isMessageStored,
              });
            return Ok(new { IsSuccess = true, Data = isMessageStored, ErrorMessage = "" });
        }

        [HttpGet]
        [Route("getUserMessage")]
        public async Task<IActionResult> GetUserMessage(Guid receiverId, int pageNumber, int? pageSize)
        {
            var loggedInUser = await GetUserIdAsync(this._userManager);
            var isUserExist = await _userService.GetUserById(receiverId.ToString());
            var isNull = null as object;
            if(isUserExist == null)
            {
                return Ok(new { IsSuccess = false, Data = isNull, ErrorMessage = "The receiverId doesnot exist" });
            }
            if(pageSize == null || pageSize <= 0)
            {
                pageSize = 10;
            }
            var userMessage = await _chatServices.GetUserMessageAsync(receiverId, Guid.Parse(loggedInUser), pageNumber, pageSize);
            return Ok(new { IsSuccess = true, Data = userMessage, ErrorMessage = "" });
        }

        [HttpGet]
        [Route("getUserChatHeads")]
        public async Task<IActionResult> GetUserChatHeads(Guid userId, int pageNumber, int? pageSize, string? searchString)
        {
            var loggedInUser = await GetUserIdAsync(this._userManager);
            if(pageSize == null || pageSize <= 0)
            {
                pageSize = 10;
            }
            if (searchString == null)
                searchString = string.Empty;
            var userChatHeads = await _chatServices.GetUserChatHeads(Guid.Parse(loggedInUser), pageNumber, pageSize, searchString);
            return Ok(new { IsSuccess = true, Data = userChatHeads, ErrorMessage = "" });
        }

        [HttpGet]
        [Route("getUserChatsById")]
        public async Task<IActionResult> GetUserChatsById(Guid chatHeadId)
        {
            var userChatHeads = await _chatServices.GetUserChatsById(chatHeadId);
            return Ok(new { IsSuccess = true, Data = userChatHeads, ErrorMessage = "" });
        }

        [HttpGet]
        [Route("getUserChatHeadById")]
        public async Task<IActionResult> GetUserChatHeadById(Guid chatHeadId, Guid senderId)
        {
            //var userId = await GetUserIdAsync(this._userManager);
            var userChatHeads = await _chatServices.GetUserChatHeadById(chatHeadId, senderId.ToString());
            return Ok(new { IsSuccess = true, Data = userChatHeads, ErrorMessage = "" });
        }


        [HttpPost]
        [Route("pusher/auth")]
        public async Task<IActionResult> PusherAuthenticate([FromForm]PusherAuthentication request)
        {
            var loggedInUser = await GetUserIdAsync(this._userManager);
            string channel_name = request.Channel_Name;
            string socket_id = request.Socket_Id;
            var options = new PusherOptions
            {
                Cluster = "ap2",
                Encrypted = true
            };

            var pusher = new Pusher("1687856", "014bb49542364098f9cb", "ec8dbf1e9ae6321642d7", options);
            var isAllowed = await IsUserAllowedToAccessChannel(channel_name, loggedInUser);

            if (isAllowed)
            {
                var auth = pusher.Authenticate(channel_name, socket_id);
                return new JsonResult(auth);
            }
            return new UnauthorizedResult();
        }

        private async Task<bool> IsUserAllowedToAccessChannel(string channelName, string userId)
        {
            var segments = channelName.Split('-');
            var chatHeadId = string.Join("-", segments.Skip(1));
            if(chatHeadId == "chat")
            {
                return true;
            }
            var chatHead = await _chatRepository.GetAll().Where(x => x.ChatHeadId == chatHeadId).FirstOrDefaultAsync();
            if (chatHead != null && (chatHead.SenderId == userId || chatHead.ReceiverId == userId))
            {
                return true;
            }
            return false;
        }

        [HttpGet]
        [Route("isChatHeadExist")]
        public async Task<IActionResult> IsChatHeadExist(Guid currentSenderId, Guid currentReceiverId)
        {
            var currentSenderUser = await _userService.GetUserById(currentSenderId.ToString());
            if(currentSenderUser == null)
            {
                return Ok(new { IsSuccess = false, Data = currentSenderUser, ErrorMessage = $"Sender doesnot exist from id: {currentSenderId}" });
            }
            var currentReceiverUser = await _userService.GetUserById(currentReceiverId.ToString());
            if (currentReceiverUser == null)
            {
                return Ok(new { IsSuccess = false, Data = currentReceiverUser, ErrorMessage = $"Receiver doesnot exist from id: {currentSenderId}" });
            }
            var user = await _chatServices.IsChatHeadExist(currentSenderId.ToString(), currentReceiverId.ToString());
            return Ok(new { IsSuccess = true, Data = user, ErrorMessage = $"" });
        }


    }
}
