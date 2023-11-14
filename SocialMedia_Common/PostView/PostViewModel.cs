using SocialMedia_Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia_Common.PostView
{
    public class PostViewModel
    {
        public string PostId { get; set; }
        public string? Description { get; set; }
        public string ParentId { get; set; }
        public string ParentName { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime? Updated { get; set; }
        public User User { get; set; }
        public FileUrlType FileUrlType { get; set; }
        public string FileUrl { get; set; }
        //public Like Likes { get; set; }
        public int TotalLikes { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
    }
}
