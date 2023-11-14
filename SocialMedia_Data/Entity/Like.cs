using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia_Data.Entity
{
    public class Like
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string LikeId {  get; set; }
        public string PostId { get; set; }
        public string UserId { get; set; }
        public int TotalLikes { get; set; }
        public bool IsLiked { get; set; }
    }
}
