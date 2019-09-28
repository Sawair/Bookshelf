using System.Linq;
using System.Threading.Tasks;
using Bookshelf.Data;
using Bookshelf.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace Bookshelf.Service
{
    public class BookService
    {
        private readonly ApplicationDbContext _context;

        public BookService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<BookViewModel[]> GetBookViewModelsAsync()
        {
            return _context
                .Book
                .Include(b => b.Borrower)
                .Include(b => b.Tags)
                .ThenInclude(bt => bt.Tag)
                .Select(b => new BookViewModel()
                {
                    Id = b.Id,
                    Title = b.Title,
                    Available = b.Available,
                    IsEbook = b.IsEbook,
                    Borrower = b.Borrower.UserName,
                    Tags = b.Tags.Select(t => t.Tag.Name).ToList()
                })
                .ToArrayAsync();
        }
    }
}