using SocialMedia_Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia_Common.PostView
{
    public class NewPostViewModel
    {
        public Post Post { get; set; }
        public int TotalPostLike { get; set; }
    }
}
