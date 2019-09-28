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

        public async Task SaveBookViewModel(BookViewModel bookViewModel)
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
                        .Include(t => t.BookTags)
                        .ThenInclude(bt => bt.Book)
                        .Where(t => tags.Contains(t.Name))
                        .Where(t => t.BookTags.All(bt => bt.Book.Id != bookViewModel.Id))
                        .Select(t => new BookTag { Tag = t })
                        .ToListAsync();
                    
                    if (bookViewModel.Id == 0)
                    {
                        _context.Book.Add(new Book
                        {
                            Title = bookViewModel.Title,
                            IsEbook = bookViewModel.IsEbook,
                            Available = bookViewModel.Available,
                            Tags = bookTags
                        });
                    }
                    else
                    {
                        var book = _context.Book.Include(b => b.Tags).ThenInclude(t => t.Tag).First(b => b.Id == bookViewModel.Id);
                        var enumerable = book.Tags.Where(t => !tags.Contains(t.Tag.Name));
                        _context.BookTag.RemoveRange(enumerable);

                        if (!string.IsNullOrEmpty(bookViewModel.Borrower))
                        {
                            var user = _context.Users.FirstOrDefault(u => u.NormalizedUserName == bookViewModel.Borrower.ToLower());
                            book.Borrower = user;
                        }
                        else
                        {
                            book.Borrower = null;
                        }

                        book.Title = bookViewModel.Title;
                        book.IsEbook = bookViewModel.IsEbook;
                        book.Available = bookViewModel.Available;


                        book.Tags.AddRange(bookTags);
                    }

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