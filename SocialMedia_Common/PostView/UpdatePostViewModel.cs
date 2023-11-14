using SocialMedia_Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia_Common.PostView
{
    public class UpdatePostViewModel
    {
        public string PostId { get; set; }
        public string? Description { get; set; }
        public string? FileUrl { get; set; }
        public FileUrlType FileUrlType { get; set; }
    }
}
