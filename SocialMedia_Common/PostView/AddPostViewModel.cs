using Microsoft.AspNetCore.Http;
using SocialMedia_Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.AspNetCore.;

namespace SocialMedia_Common.PostView
{
    public class AddPostViewModel
    {
        public Guid? PostId { get; set; }
        public string? Description { get; set; }
        //public string? ParentId { get; set; }
        //public string? ParentName { get; set; }
        public string? FileUrl { get; set; }
        public FileUrlType FileUrlType { get; set; }
    }
}
