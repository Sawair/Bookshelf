using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Bookshelf.Data.Model
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsEbook { get; set; }
        public bool Available { get; set; }
        public List<BookTag> Tags { get; set; }
        public IdentityUser Borrower { get; set; }
    }
}
