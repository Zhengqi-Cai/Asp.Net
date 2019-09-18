using EBookReader.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EBookReader.Data
{
    public class EBookReaderDbContext : IdentityDbContext<User>
    {
        public EBookReaderDbContext(DbContextOptions<EBookReaderDbContext> options) : base(options) { }


        public DbSet<Book> Books { get; set; }
        public DbSet<Genre> Genres { get; set; }
        //public DbSet<BookUserProfile> BookUsers { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<Book>()
            //.HasOptional<Genre>(s => s.Genre)
            //.WithMany()
            //.WillCascadeOnDelete(true);
        }
    }
}
