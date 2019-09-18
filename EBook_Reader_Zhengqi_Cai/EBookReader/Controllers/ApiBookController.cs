///////////////////////////////////////////////////////////////
// FilesController.cs - Web Api for handling Files           //
//                                                           //
// Jim Fawcett, CSE686 - Internet Programming, Spring 2019   //
///////////////////////////////////////////////////////////////
/*
 * This package implements Controller for Files Web Api.
 * The web api application:
 * - uploads files to wwwroot/FileStore
 * - displays all files in FileStore
 * - downloads a file from FileStore
 * - [will] delete a file, given its index, from FileStore
 * 
 * Note that Web Api applications don't use action names in their urls.
 * Instead, they use GET, POST, PUT, and DELETE based on the type of
 * the HTTP Request Message.  Also, they don't return views.  They
 * return data.
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using EBookReader.Models;
using EBookReader.Data;
using Microsoft.AspNetCore.Identity;
using EBookReader.ViewModels.BookViewModel;
using System.Diagnostics;

namespace EBookReader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiBookController : ControllerBase
    {
        private UserManager<User> _UserManager;
        private EBookReaderDbContext context_;
        private readonly IHostingEnvironment hostingEnvironment_;
        private string webRootPath = null;
        private string filePath = null;

        public ApiBookController(UserManager<User> usermanager, EBookReaderDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _UserManager = usermanager;
            context_ = context;
            hostingEnvironment_ = hostingEnvironment;
            webRootPath = hostingEnvironment_.WebRootPath;
            filePath = Path.Combine(webRootPath, "FileStorage");
        }
        ////----< show files in wwwroot/FileStorage >----------------
        //
        // Not quite functional attempt to make GetFiles asynchronous
        //
        //private List<string> GetFilesHelper(string filePath)
        //{
        //  List<string> files = null;
        //  try
        //  {
        //    files = Directory.GetFiles(filePath).ToList<string>();
        //    for (int i = 0; i < files.Count; ++i)
        //      files[i] = Path.GetFileName(files[i]);
        //  }
        //  catch
        //  {
        //    files = new List<string>();
        //    files.Add("404 - Not Found");
        //  }
        //  return files;
        //}

        // GET: api/<controller>
        //[HttpGet]
        //public async Task<List<string>> Get()
        //{
        //  List<string> files = await GetFilesHelper(filePath);
        //  return files;
        //}
        //----< show files in wwwroot/FileStorage >----------------
        //
        // The Core Framework will serialize the return value into
        // a JSon string in the Response message body.

        // GET: api/<controller>
        [HttpGet]
        public ActionResult<IEnumerable<ViewBook>> Get()
        {
            var books = context_.Books.ToList<Book>();
            List<ViewBook> viewBooks = new List<ViewBook>();
            foreach (var book in books)
            {
                ViewBook vb = new ViewBook();
                vb.BookId = book.BookId;
                vb.ISBN = book.ISBN;
                vb.Title = book.Title;
                vb.Author = book.Author;

                vb.isPublic = book.isPublic;
                vb.UploaderName = book.UploaderName;
                vb.PathUrl = Path.GetFileName(book.PathUrl);
                vb.GenreName = context_.Genres.Find(book.GenreId).Name;
                viewBooks.Add(vb);
            }
            viewBooks= viewBooks.OrderBy(vb => vb.GenreName).ToList<ViewBook>();
            return viewBooks;
        }
        //----< download single file in wwwroot\FileStorage >------

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Download(int id)
        {
            Book book;
            try
            {
                book = context_.Books.Find(id);

            }
            catch
            {
                return NotFound();
            }
            var memory = new MemoryStream();
            string file = book.PathUrl;
            using (var stream = new FileStream(file, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(file), Path.GetFileName(file));
        }
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
      {
                {".epub","application/epub+zip" },
        {".cs", "application/C#" },
        {".txt", "text/plain"},
        {".pdf", "application/pdf"},
        {".doc", "application/vnd.ms-word"},
        {".docx", "application/vnd.ms-word"},
        {".xls", "application/vnd.ms-excel"},
        {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
        {".png", "image/png"},
        {".jpg", "image/jpeg"},
        {".jpeg", "image/jpeg"},
        {".gif", "image/gif"},
        {".csv", "text/csv"}
      };
        }
        //----< upload file >--------------------------------------

        // POST api/<controller>
        [HttpPost]
        public async Task<IActionResult> Create()
        {
            var request = HttpContext.Request;
            var dict = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
            Book book = new Book();
            foreach (var key in dict.Keys)
            {
                var property = typeof(Book).GetProperty(key);
                if (property != null)
                {
                    var t = property.PropertyType;
                    var s = Convert.ChangeType(dict[key], t);
                    property.SetValue(book, s);
                }
            }
            //uploader
            if (String.IsNullOrEmpty(dict["UploaderName"]))
            {
                book.UploaderName = _UserManager.GetUsersInRoleAsync("Admin").Result.First<User>().UserName;

            }
            else
            {
                var user = _UserManager.FindByNameAsync(dict["UploaderName"]).Result;
                if (user == null)
                {
                    return BadRequest();
                }
                else
                {
                    book.UploaderName = user.UserName;
                }
            }
            if (request.Form.Files.Count == 1)
            {
                var file = request.Form.Files[0];
                var path = Path.Combine(filePath, file.FileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                book.PathUrl = createEpub(path);
                System.IO.File.Delete(path);
            }
            else
            {
                return BadRequest();
            }
            //handle genre
            GenreNames genreName;
            Enum.TryParse<GenreNames>(dict["GenreName"], out genreName);
            var targetGenre = context_.Genres.ToList<Genre>().Find(g => g.Name == genreName);
            if (targetGenre == null)
            {
                targetGenre = new Genre { Name = genreName };
                context_.Genres.Add(targetGenre);
                context_.SaveChanges();
            }
            book.GenreId = targetGenre.GenreId;
            if (targetGenre.Books == null)  // doesn't have any lectures yet
            {
                List<Book> books = new List<Book>();
                targetGenre.Books = books;
            }
            targetGenre.Books.Add(book);
            context_.SaveChanges();

            return Ok();
        }

        // POST api/<controller>
        // This is the usual technique for uploading files, but I never got more than
        // one file.  Something wrong with the configuration of my IFormFile model?

        //[HttpPost]
        //public async Task<IActionResult> Post(IList<IFormFile> files)
        //{
        //    var dummy = HttpContext.Request;  // statement for debugging
        //    foreach (var file in files)
        //    {
        //        if (file.Length > 0)
        //        {
        //            var path = Path.Combine(filePath, file.FileName);
        //            using (var fileStream = new FileStream(path, FileMode.Create))
        //            {
        //                await file.CopyToAsync(fileStream);
        //            }
        //        }
        //    }
        //    return Ok();
        //}

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id)
        {
            Book book = context_.Books.Find(id);
            if (book == null)
                return NotFound();
            string originPAthUrl = book.PathUrl;
            var request = HttpContext.Request;
            var dict = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
            foreach (var key in dict.Keys)
            {
                var property = typeof(Book).GetProperty(key);
                if (property != null)
                {
                    if (property.Name == "BookId") continue;
                    var t = property.PropertyType;
                    var s = Convert.ChangeType(dict[key], t);
                    property.SetValue(book, s);
                }
            }


            if (String.IsNullOrEmpty(dict["UploaderName"]))
            {
                book.UploaderName = _UserManager.GetUsersInRoleAsync("Admin").Result.First<User>().UserName;

            }
            else
            {
                var user = _UserManager.FindByNameAsync(dict["UploaderName"]).Result;
                if (user == null)
                {
                    return BadRequest();
                }
                else
                {
                    book.UploaderName = user.UserName;
                }
            }
            if (request.Form.Files.Count == 1)
            {
                var file = request.Form.Files[0];
                var path = Path.Combine(filePath, file.FileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                book.PathUrl = createEpub(path);
                System.IO.File.Delete(path);
            }
            else
            {
                book.PathUrl = originPAthUrl;
            }


            var originGenre = context_.Genres.Find(book.GenreId);

            GenreNames genreName;
            Enum.TryParse<GenreNames>(dict["GenreName"], out genreName);
            var targetGenre = context_.Genres.ToList<Genre>().Find(g => g.Name == genreName);
            if (targetGenre == null)
            {
                targetGenre = new Genre { Name = genreName };
                context_.Genres.Add(targetGenre);
                context_.SaveChanges();
            }
            book.GenreId = targetGenre.GenreId;
            if (targetGenre.Books == null)  // doesn't have any lectures yet
            {
                List<Book> books = new List<Book>();
                targetGenre.Books = books;
            }
            targetGenre.Books.Add(book);
            context_.SaveChanges();

            try
            {
                var bks = context_.Books.Where(l => l.Genre == originGenre).ToList<Book>();
                if (bks.Count == 0)
                {
                    context_.Remove(originGenre);
                    context_.SaveChanges();
                }
            }
            catch (Exception)
            {
                // do nothing for now
            }

            return Ok();
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var book = context_.Books.Find(id);
                if (book != null)
                {
                    if (!string.IsNullOrEmpty(book.PathUrl))
                    {
                        System.IO.File.Delete(book.PathUrl);

                    }
                    context_.Remove(book);
                    context_.SaveChanges();

                    var genre = context_.Genres.Find(book.GenreId);
                    var bks = context_.Books.Where(l => l.Genre == genre).ToList<Book>();
                    if (bks.Count == 0)
                    {
                        context_.Remove(genre);
                        context_.SaveChanges();
                    }

                }
            }
            catch (Exception)
            {
                // nothing for now
            }
            return Ok();
        }

        //[HttpPost]
        //public async Task<ActionResult> upload(IFormFile file)
        //{


        //    var path = Path.Combine(filePath, file.FileName);
        //    //going to do a epub conversion here
        //    using (var fileStream = new FileStream(path, FileMode.Create))
        //    {
        //        await file.CopyToAsync(fileStream);
        //    }
        //    return BadRequest();
        //}

        private string createEpub(string path)
        {
            string output = path + ".epub";
            string arg = "/c" + "\"C://Program Files (x86)//Calibre2//ebook-convert.exe\" " + path + " " + output;
            var processInfo = new ProcessStartInfo("cmd.exe", arg);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            var process = Process.Start(processInfo);
            process.WaitForExit();
            return output;
        }
    }


}
