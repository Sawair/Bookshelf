using System;
using System.Linq;
using System.Threading.Tasks;
using Bookshelf.Data;
using Bookshelf.Data.Model;
using Bookshelf.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

        public async Task AddBookViewModel(BookViewModel bookViewModel)
        {
            var tags = bookViewModel.Tags.Select(t => t.ToLower().Trim()).Distinct().ToList();
            var newTags = tags.Where(t => !_context.Tag.Any(tag => tag.Name == t)).ToList();

            await using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var tag in newTags)
                    {
                        _context.Tag.Add(new Tag() { Name = tag });
                    }

                    await _context.SaveChangesAsync();
                    
                    var bookTags = await _context
                        .Tag
                        .Where(t => tags.Contains(t.Name))
                        .Select(t => new BookTag { Tag = t })
                        .ToListAsync();

                    _context.Book.Add(new Book
                    {
                        Title = bookViewModel.Title,
                        IsEbook = bookViewModel.IsEbook,
                        Available = bookViewModel.Available,
                        Tags = bookTags
                    });

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }



        }
    }
}