using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia_Data.Entity
{
    public class Post
    {
        [Key]
        public string PostId { get; set; }
        public string? Description { get; set; }
        public string ParentId { get; set; }
        public string ParentName { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime? Updated { get; set; }
        public string? FileUrl { get; set; }
        public User User { get; set; }
        public FileUrlType FileUrlType { get; set; }
    }
    public enum FileUrlType
    {
        Default,
        Image,
        Video
    }
}
