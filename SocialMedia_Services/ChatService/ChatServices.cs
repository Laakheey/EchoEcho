using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SocialMedia_Common.ChatView;
using SocialMedia_Data.Entity;
using SocialMedia_DataAccess.Repository;
using System;

namespace SocialMedia_Services.ChatService
{
    public class ChatServices : IChatServices
    {
        public readonly IGenericRepository<Chat> _chatRepository;
        public readonly IGenericRepository<ChatHead> _chatHeadRepository;
        public readonly IGenericRepository<User> _userRepository;
        public readonly IMapper _chatMapper;
        public ChatServices(IGenericRepository<Chat> chatRepository, IGenericRepository<ChatHead> chatHeadRepository, IMapper chatMapper, IGenericRepository<User> userRepository)
        {
            _chatRepository = chatRepository;
            _chatMapper = chatMapper;
            _userRepository = userRepository;
            _chatHeadRepository = chatHeadRepository;
        }

        public async Task<List<MessageViewModel>> GetUserMessageAsync(Guid receiverId, Guid senderId, int pageNumber, int? pageSize)
        {
            var chats = await _chatRepository
                .GetAll()
                .Where(x => (x.ReceiverId == receiverId.ToString() && x.SenderId == senderId.ToString()) 
                || (x.ReceiverId == senderId.ToString() && x.SenderId == receiverId.ToString()))
                .Include(x => x.ChatHead)
                .Include(x => x.SenderUser)
                .Include(x => x.ReceiverUser)
                .OrderByDescending(x => x.SentTime)
                .Skip((pageNumber - 1) * (int)pageSize)
                .Take((int)pageSize)
                .ToListAsync();

            return _chatMapper.Map<List<MessageViewModel>>(chats);
        }

        public async Task<MessageViewModel> SendMessageAsync(AddMessageViewModel message)
        {
            var isUserExist = await _userRepository.GetById(message.ReceiverId.ToString());
            var senderDetails = await _userRepository.GetById(message.SenderId.ToString());
            if (isUserExist == null)
            {
                return null;
            }
            var chat = new Chat();
            var isChatHeadUpdate = false;

            if (message.ChatHeadID == new Guid())
            {
                var isChatHeadExist = await _chatRepository
                        .GetAll()
                        .Where(x => (x.SenderId == message.SenderId.ToString() && x.ReceiverId == message.ReceiverId.ToString())
                         || (x.SenderId == message.ReceiverId.ToString() && x.ReceiverId == message.SenderId.ToString()))
                        .FirstOrDefaultAsync();

                if (isChatHeadExist != null)
                {

                    return null;
                }
            }
            else
            {
                chat.ChatHeadId = message.ChatHeadID.ToString();
                isChatHeadUpdate = true;
            }

            chat.ChatId = Guid.NewGuid().ToString();
            chat.SenderId = message.SenderId.ToString();
            chat.ReceiverId = message.ReceiverId.ToString();
            chat.Message = message.Message;
            chat.SentTime = DateTime.Now;
            chat.IsMessageRead = false;

            chat.SenderUser = senderDetails;
            chat.ReceiverUser = isUserExist;

            var chatHead = await ChatHeadsAddOrUpdate(chat, isChatHeadUpdate);

            chat.ChatHead = chatHead;
            chat.ChatHeadId = chatHead.ChatHeadId.ToString();

            await _chatRepository.Insert(chat);
            await _chatRepository.Save();

            return _chatMapper.Map<MessageViewModel>(chat);
        }

        public async Task<List<ChatHeadViewModel>> GetUserChatHeads(Guid userId, int pageNumber, int? pageSize, string? searchString)
        {
            try
            {
                var chats = await _chatRepository
                                .GetAll()
                                .Where(x => x.SenderId == userId.ToString() || x.ReceiverId == userId.ToString())
                                .Include(x => x.SenderUser)
                                .Include(x => x.ReceiverUser)
                                .Include(x => x.ChatHead)
                                .ToListAsync();

                var userChat = chats
                    .GroupBy(x => x.SenderId == userId.ToString() ? new Tuple<string, string>(x.SenderId, x.ReceiverId) 
                            : new Tuple<string, string>(x.ReceiverId, x.SenderId))
                    .Select(g => g.OrderByDescending(x => x.SentTime).FirstOrDefault())
                    .OrderByDescending(x => x.SentTime).ToList();

                //if (!string.IsNullOrEmpty(searchString))
                //{
                //    userChat = userChat
                //        .Where(x => x.ChatHead.ReceiverFirstName.ToLower().Contains(searchString.ToLower())
                //        || x.ChatHead.ReceiverLastName.ToLower().Contains(searchString.ToLower())).ToList();
                //}
                //if (!string.IsNullOrEmpty(searchString))
                //{
                //    foreach (var item in userChat)
                //    {
                //        if(item.SenderId != userId.ToString())
                //        {
                //            userChat = userChat
                //                .Where(x => (x.SenderUser.FirstName + " " + x.SenderUser.LastName)
                //                .ToLower()
                //                .Contains(searchString.ToLower()))
                //                .ToList();
                //        }
                //        else
                //        {
                //            userChat = userChat
                //               .Where(x => (x.ReceiverUser.FirstName + " " + x.ReceiverUser.LastName)
                //               .ToLower()
                //               .Contains(searchString.ToLower()))
                //               .ToList();
                //        }
                //    }
                //}
                if (!string.IsNullOrEmpty(searchString))
                {
                    userChat = userChat
                        .Where(chat =>
                            (chat.SenderId != userId.ToString() &&
                                (chat.SenderUser.FirstName + " " + chat.SenderUser.LastName)
                                .ToLower()
                                .Contains(searchString.ToLower())) ||
                            (chat.ReceiverId != userId.ToString() &&
                                (chat.ReceiverUser.FirstName + " " + chat.ReceiverUser.LastName)
                                .ToLower()
                                .Contains(searchString.ToLower())))
                        .ToList();
                }



                userChat = userChat
                           .Skip((pageNumber - 1) * pageSize.Value)
                           .Take(pageSize.Value).ToList();

                var userChatHeads = new List<ChatHeadViewModel>();


                foreach (var chat in userChat)
                {
                    var viewModel = new ChatHeadViewModel
                    {
                        IsMessageRead = false,
                        LastMessage = chat.ChatHead.LastMessage,
                        ReceiverId = chat.SenderId == userId.ToString() ? chat.ReceiverId : chat.SenderId,
                        ChatHeadId = chat.ChatHeadId
                    };

                    if (chat.SenderUser.Id == userId.ToString())
                    {
                        var user = await _userRepository.GetById(chat.ReceiverUser.Id);
                        viewModel.User = user;
                        viewModel.ReceiverFirstName = chat.ReceiverUser.FirstName;
                        viewModel.ReceiverLastName = chat.ReceiverUser.LastName;
                    }
                    else
                    {
                        var user = await _userRepository.GetById(chat.SenderUser.Id);
                        viewModel.User = user;
                        viewModel.ReceiverFirstName = chat.SenderUser.FirstName;
                        viewModel.ReceiverLastName = chat.SenderUser.LastName;
                    }

                    userChatHeads.Add(viewModel);
                }



                return userChatHeads;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<ChatHead> ChatHeadsAddOrUpdate(Chat chat, bool isChatHeadUpdate)
        {
            var user = await _userRepository.GetById(chat.ReceiverId);
            if (!isChatHeadUpdate)
            {
                var chatHead = new ChatHead()
                {
                    ChatHeadId = Guid.NewGuid().ToString(),
                    ReceiverFirstName = user.FirstName,
                    ReceiverLastName = user.LastName,
                    LastMessage = chat.Message,
                    IsMessageRead = chat.IsMessageRead
                };
                await _chatHeadRepository.Insert(chatHead);
                await _chatHeadRepository.Save();
                return (chatHead);
            }
            else
            {
                var chatHead = await _chatHeadRepository.GetById(chat.ChatHeadId.ToString());
                chatHead.IsMessageRead = chat.IsMessageRead;
                chatHead.LastMessage = chat.Message;
                _chatHeadRepository.Update(chatHead);
                await _chatHeadRepository.Save();
                return chatHead;
            }
        }

        public async Task<List<MessageViewModel>> GetUserChatsById(Guid chatHeadId)
        {
            try
            {
                var chatsByChatHeadId = await _chatRepository.GetAll()
                .Where(x => x.ChatHeadId == chatHeadId.ToString())
                .Skip(0)
                .Take(10)
                .OrderByDescending(x => x.SentTime)
                .ToListAsync();
                return _chatMapper.Map<List<MessageViewModel>>(chatsByChatHeadId);
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<ChatHeadViewModel> GetUserChatHeadById(Guid chatHeadId)
        {
            try
            {
                var chatHeadByChatHeadId = await _chatHeadRepository.GetById(chatHeadId.ToString());
                var chats = await _chatRepository.GetAll()
                                    .Where(x => x.ChatHeadId == chatHeadId.ToString())
                                    .FirstOrDefaultAsync();
                return new ChatHeadViewModel()
                {
                    ChatHeadId = chatHeadByChatHeadId.ChatHeadId,
                    IsMessageRead = chatHeadByChatHeadId.IsMessageRead,
                    LastMessage = chatHeadByChatHeadId.LastMessage,
                    ReceiverId = chats.ReceiverId,
                    ReceiverFirstName = chatHeadByChatHeadId.ReceiverFirstName,
                    ReceiverLastName = chatHeadByChatHeadId.ReceiverLastName,
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<ChatHeadViewModel> GetUserChatHeadById(Guid chatHeadId, string userId)
        {
            try
            {
                var chatHeadByChatHeadId = await _chatHeadRepository.GetById(chatHeadId.ToString());
                var chats = await _chatRepository.GetAll()
                                    .Where(x => x.ChatHeadId == chatHeadId.ToString())
                                    .FirstOrDefaultAsync();
                var user = await _userRepository.GetById(userId.ToString());
                //if(chats.SenderId == userId)
                //{
                //    user = await _userRepository.GetById(chats.ReceiverId.ToString());
                //}
                //else
                //{
                //    user = await _userRepository.GetById(chats.SenderId.ToString());
                //}
                var chatHead = new ChatHeadViewModel()
                {
                    ChatHeadId = chatHeadByChatHeadId.ChatHeadId,
                    IsMessageRead = chatHeadByChatHeadId.IsMessageRead,
                    LastMessage = chatHeadByChatHeadId.LastMessage,
                    ReceiverId = chats.ReceiverId,
                    ReceiverFirstName = chatHeadByChatHeadId.ReceiverFirstName,
                    ReceiverLastName = chatHeadByChatHeadId.ReceiverLastName,
                    User = user
                };
                return chatHead;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<ChatHeadViewModel> IsChatHeadExist(string currentSenderId, string currentReceiverId)
        {
            var user = await _chatRepository
                                .GetAll()
                                .Where(x => (x.SenderId == currentSenderId && x.ReceiverId == currentReceiverId) 
                                    || (x.SenderId == currentReceiverId && x.ReceiverId == currentSenderId))
                                .FirstOrDefaultAsync();
            if(user == null)
            {
                var receiverUser = await _userRepository.GetById(currentReceiverId);
                return new ChatHeadViewModel()
                {
                    ChatHeadId = new Guid().ToString(),
                    IsMessageRead = false,
                    ReceiverFirstName = receiverUser.FirstName,
                    LastMessage = "",
                    ReceiverId = currentReceiverId,
                    ReceiverLastName = receiverUser.LastName
                };
            }
            var chatHead = await _chatHeadRepository.GetById(user.ChatHeadId);
            return new ChatHeadViewModel()
            {
                ChatHeadId = chatHead.ChatHeadId,
                IsMessageRead = chatHead.IsMessageRead,
                LastMessage = chatHead.LastMessage,
                ReceiverFirstName = chatHead.ReceiverFirstName,
                ReceiverId = currentReceiverId,
                ReceiverLastName = chatHead.ReceiverLastName
            };

        }


    }
}
