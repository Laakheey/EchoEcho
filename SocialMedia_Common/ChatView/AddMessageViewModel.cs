using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia_Common.ChatView
{
    public class AddMessageViewModel
    {
        public Guid ChatId { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public Guid ChatHeadID { get; set; }
        public string Message { get; set; }
    }
}
