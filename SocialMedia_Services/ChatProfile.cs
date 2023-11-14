using AutoMapper;
using SocialMedia_Common.ChatView;
using SocialMedia_Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia_Services
{
    public class ChatProfile : Profile
    {
        public ChatProfile() 
        {
            CreateMap<Chat, MessageViewModel>();
            CreateMap<ChatHead, ChatHeadViewModel>();
        }
    }
}
