using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EBookReader.Models
{
  public class Book
  {
    public int BookId { get; set; }
    public string ISBN { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public bool isPublic { get; set; }
    public string UploaderName{ set; get; }
    public string PathUrl { get; set; }


    public int GenreId { get; set; }
    public virtual Genre Genre{ get; set; }
    public virtual ICollection<Comment> Comments { get; set; }
    }

}
