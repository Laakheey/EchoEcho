using SocialMedia_Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia_Common.ChatView
{
    public class ChatHeadViewModel
    {
        public string ReceiverId { get; set; }
        public string LastMessage { get; set; }
        public bool IsMessageRead { get; set; }
        public string ReceiverFirstName { get; set; }
        public string ReceiverLastName { get; set; }
        public string ChatHeadId { get; set; }
        public User User { get; set; }
    }
}
