using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EBookReader.Data;
using EBookReader.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Diagnostics;
using EBookReader.ViewModels.BookViewModel;
using Microsoft.AspNetCore.Identity;
//using System.Web;
namespace EBookReader.Controllers
{
    [AllowAnonymous]
    public class BookController : Controller
    {
        private EBookReaderDbContext context_;
        private const string sessionId_ = "SessionId";

        private UserManager<User> _userManager;
        private readonly IHostingEnvironment hostingEnvironment_;
        private string webRootPath_ = null;
        private string filePath_ = null;

        public BookController(UserManager<User> usermanager, EBookReaderDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _userManager = usermanager;
            hostingEnvironment_ = hostingEnvironment;
            webRootPath_ = hostingEnvironment.WebRootPath;
            filePath_ = Path.Combine(webRootPath_, "FileStorage");

            context_ = context;
        }


        // GET: Home
        public IActionResult Index()
        {
            var listGenres = context_.Genres.OrderBy(g=>g.Name).ToList<Genre>();
            foreach (var genre in listGenres)
            {
                var bks = context_.Books.Where(l => l.Genre == genre);

                genre.Books = bks.OrderBy(l => l.Title).Select(l => l).ToList<Book>();

            }
            return View(listGenres);
        }


        //public IActionResult DeleteGenre(int? id)
        //{
        //    if (id == null)
        //    {
        //        return StatusCode(StatusCodes.Status400BadRequest);
        //    }
        //    try
        //    {
        //        var genre = context_.Genres.Find(id);
        //        if (genre != null)
        //        {
        //            context_.Remove(genre);
        //            context_.SaveChanges();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        // nothing for now
        //    }
        //    return RedirectToAction("Index");
        //}

        //----< shows form for creating a lecture >------------------

        [HttpGet]
        public IActionResult AddBook()
        {
            Book bk = new Book();
            bk.Genre = new Genre();
            return View(bk);
        }

        //----< posts back new courses details >---------------------

        [HttpPost]
        public IActionResult AddBook(int? id, Book bk)
        {
            var targetGenre = context_.Genres.ToList<Genre>().Find(x => x.Name == bk.Genre.Name);
            if (targetGenre == null)
            {
                targetGenre = new Genre { Name = (GenreNames)bk.Genre.Name };
                context_.Genres.Add(targetGenre);
                context_.SaveChanges();
            }
            bk.GenreId = targetGenre.GenreId;

            if (targetGenre.Books == null)  // doesn't have any lectures yet
            {
                List<Book> books = new List<Book>();
                targetGenre.Books = books;
            }
            targetGenre.Books.Add(bk);
            context_.SaveChanges();

            return RedirectToAction("Upload", new { bookId = bk.BookId });
        }

        //----< show list of lectures, ordered by Title >------------

        public IActionResult Books()
        {
            // fluent API
            var bks = context_.Books.Include(l => l.Genre);
            var orderedBks = bks.OrderBy(l => l.Title)
              .OrderBy(l => l.Genre)
              .Select(l => l);
            return View(orderedBks);

            // Linq
            //var lects = context_.Lectures.Include(l => l.Course);
            //var orderedLects = from l in lects
            //                   orderby l.Title
            //                   orderby l.Course
            //                   select l;
            //return View(orderedLects);

            // doesn't return Lecture's course nor order by title
            //return View(context_.Lectures.ToList<Lecture>());
        }

        // GET: Home/Details/5
        public IActionResult BookDetails(int? id)
        {
            if (id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            Book book = context_.Books.Find(id);

            if (book == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            Genre genre = context_.Genres.Find(book.GenreId);
            book.Genre = genre;
            return View(book);
        }

        //----< gets form to edit a specific lecture via id >---------

        [HttpGet]
        public IActionResult EditBook(int? id)
        {
            if (id == null)
            {
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest);
            }
            Book book = context_.Books.Find(id);

            if (book == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            book.Genre = context_.Genres.Find(book.GenreId);

            return View(book);
        }

        //----< posts back edited results for specific lecture >------

        [HttpPost]
        public IActionResult EditBook(int? id, Book bk)
        {
            if (id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            var book = context_.Books.Find(id);

            if (book != null)
            {
                var originGenre = context_.Genres.Find(book.GenreId);
                var targetGenre = context_.Genres.ToList<Genre>().Find(x => x.Name == bk.Genre.Name);
                if (targetGenre == null)
                {
                    targetGenre = new Genre { Name = (GenreNames)bk.Genre.Name };
                    context_.Genres.Add(targetGenre);
                    context_.SaveChanges();
                }
                if (targetGenre.Books == null)
                {
                    List<Book> books = new List<Book>();
                    targetGenre.Books = books;

                }
                targetGenre.Books.Add(book);
                context_.SaveChanges();

                book.GenreId = targetGenre.GenreId;
                book.ISBN = bk.ISBN;
                book.Title = bk.Title;
                book.Author = bk.Author;
                book.isPublic = bk.isPublic;

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
            }
            return RedirectToAction("Upload", new { bookId = book.BookId });
        }

        public IActionResult DeleteBook(int? id)
        {
            if (id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
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
            return RedirectToAction("Index");
        }

        public ActionResult about()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Upload(int bookId)
        {
            var book = context_.Books.Find(bookId);
            if (book == null)
                return StatusCode(StatusCodes.Status404NotFound);
            return View(book);
        }
        //[HttpPost]
        //public async Task<ActionResult> upload(IFormFile file)
        //{
        //    var path = Path.Combine(filePath_, file.FileName);
        //    //going to do a epub conversion here
        //    using (var fileStream = new FileStream(path, FileMode.Create))
        //    {
        //        await file.CopyToAsync(fileStream);
        //    }
        //    return BadRequest();
        //}

        [HttpPost]
        public async Task<ActionResult> Upload(int id, IFormFile file)
        {

            var book = context_.Books.Find(id);
            if (book == null)
                return StatusCode(StatusCodes.Status404NotFound);
            if (file == null || file.Length == 0)
                return StatusCode(StatusCodes.Status400BadRequest);
            if (!string.IsNullOrEmpty(book.PathUrl))
            {
                System.IO.File.Delete(book.PathUrl);
                //var bookComments = context_.Comments.Where(c => c.BookId == id).ToList<Comment>();
                //foreach(var comment in bookComments)
            }
            var path = Path.Combine(filePath_, file.FileName);
            //going to do a epub conversion here
            try
            {
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                book.PathUrl = createEpub(path);
                book.UploaderName = _userManager.GetUserName(HttpContext.User);
                System.IO.File.Delete(path);
                context_.SaveChanges();

            }
            catch (Exception)
            {

            }
            return RedirectToAction("Index");
        }



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

        [HttpGet]
        public IActionResult ReadOnline(int id)
        {
            var book = context_.Books.Find(id);
            if (book == null)
                return StatusCode(StatusCodes.Status404NotFound);
            ReaderViewModel rvm = new ReaderViewModel();
            rvm.BookId = book.BookId;
            rvm.BookTitle = book.Title;
            rvm.RelativeFilePath = "/Library/" + Path.GetFileName(book.PathUrl);
            var userid = _userManager.GetUserId(HttpContext.User);
            var comments = context_.Comments.Where(c => c.BookId == book.BookId && c.PublisherId == userid).OrderByDescending(c=>c.CreateTime).ToList();
            rvm.Comments = comments;
            return View(rvm);
        }


        [HttpPost]
        public IActionResult AddComment([FromBody]Comment comment)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            comment.PublisherId = user.Id;
            comment.CreateTime = DateTime.Now;
            if (user.Comments == null)
            {
                user.Comments = new List<Comment>();
            }
            var book = context_.Books.Find(comment.BookId);
            if (book.Comments == null)
            {
                book.Comments = new List<Comment>();
            }
            try
            {
                user.Comments.Add(comment);
                book.Comments.Add(comment);
                context_.SaveChanges();
            }
            catch
            {

            }

            return Json(new { commentId = comment.CommentId,createTime = comment.CreateTime.ToString(),cfi=comment.CFI });
        }

        [HttpPost]
        public IActionResult DeleteComment([FromBody]Comment passedcomment)
        {
            var id = Convert.ToInt32(passedcomment.CommentId);
            var comment = context_.Comments.Find(id);
            context_.Comments.Remove(comment);
            context_.SaveChanges();

            return Ok();
        }

    }
}