using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client
{
    public class ViewBook
    {

        public int BookId { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }

        public bool isPublic { get; set; }
        public string UploaderName { set; get; }
        public string PathUrl { get; set; }

        public GenreNames GenreName { get; set; }

    }

}
