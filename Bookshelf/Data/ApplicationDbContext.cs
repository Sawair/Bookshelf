using System;
using System.Collections.Generic;
using System.Text;
using Bookshelf.Data.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bookshelf.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Book { get; set; }
        public DbSet<Tag> Tag { get; set; }
        public DbSet<BookTag> BookTag { get; set; }
    }
}
