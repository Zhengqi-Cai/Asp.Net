using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EBookReader.Models;
namespace EBookReader.BookViewModel
{
    public class BookUserProfile
    {
        public int BookUserId { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

    }
}
