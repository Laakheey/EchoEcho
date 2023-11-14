using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia_Data.Entity
{
    public class FriendsRequest
    {
        [Key]
        public string FriendsId { get; set; }

        //[ForeignKey("Requester")]
        public string RequesterId { get; set; }
        //public User Requester { get; set; }
        //[ForeignKey("Recipient")]
        public string RecipientId { get; set; }
        //public User Recipient { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public bool IsFriendRequestAccepted { get; set; }
        public bool IsFriendRequestSent { get; set; }
    }

}
