using EBookReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EBookReader.ViewModels.BookViewModel
{
    public class ReaderViewModel
    {
        public int BookId { set; get; }
        public string BookTitle { set; get; }
        public string RelativeFilePath { set; get; }
        public ICollection<Comment> Comments { set; get; }
    }
}
