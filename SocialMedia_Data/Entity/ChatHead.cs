using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia_Data.Entity
{
    public class ChatHead
    {
        [Key]
        public string ChatHeadId { get; set; }
        public string ReceiverFirstName { get; set; }
        public string ReceiverLastName { get; set; }
        public string LastMessage { get; set; }
        public bool IsMessageRead { get; set; }
    }
}
