using SocialMedia_Common.ChatView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia_Services.ChatService
{
    public interface IChatServices
    {
        Task<MessageViewModel> SendMessageAsync (AddMessageViewModel message);
        Task<List<MessageViewModel>> GetUserMessageAsync(Guid receiverId, Guid senderId, int pageNumber, int? pageSize);
        Task<List<ChatHeadViewModel>> GetUserChatHeads(Guid userId, int pageNumber, int? pageSize, string? searchString);
        Task<ChatHeadViewModel> GetUserChatHeadById(Guid chatHeadId);
        Task<ChatHeadViewModel> GetUserChatHeadById(Guid chatHeadId, string userId);
        Task<List<MessageViewModel>> GetUserChatsById(Guid chatHeadId);
        Task<ChatHeadViewModel> IsChatHeadExist(string currentSenderId, string currentReceiverId);
    }
}
