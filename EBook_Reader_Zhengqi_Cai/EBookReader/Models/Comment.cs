using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EBookReader.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; }
        public string CFI { get; set; }

        public int BookId { get; set; }
        public string PublisherId { get; set; }

    }
}
