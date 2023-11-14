using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia_Data.Entity
{
    public class Chat
    {
        [Key]
        public string ChatId { get; set; }
        public string SenderId { get; set; }
        public User SenderUser { get; set; }
        public string ReceiverId { get; set; }
        public User ReceiverUser { get; set; }
        public string Message { get; set; }
        public DateTime SentTime { get; set; }
        public bool IsMessageRead { get; set; }
        public ChatHead ChatHead { get; set; }
        public string ChatHeadId { get; set; }
    }
}
