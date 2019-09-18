using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EBookReader.Models
{
    public class Genre
    {
        public int GenreId { get; set; }
        public GenreNames Name { get; set; }

        public virtual ICollection<Book> Books { get; set; }
    }

  
}
