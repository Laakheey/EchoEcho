using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia_Common.FriendRequests
{
    public class FriendRequestViewModel
    {
        public Guid FriendsId { get; set; }
        public Guid RequesterId { get; set; }
        public string RequesterName { get; set; }
        public DateTime RequestDate { get; set; }
        public bool? IsAccepted { get; set; }
    }
}
