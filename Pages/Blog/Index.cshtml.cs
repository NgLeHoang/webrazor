using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webrazorapp.models;

namespace webrazorapp.Pages_Blog
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly webrazorapp.models.AppDbContext _context;

        public IndexModel(webrazorapp.models.AppDbContext context)
        {
            _context = context;
        }

        public IList<Article> Article { get;set; } = default!;

        public const int ITEMS_PER_PAGE = 10;

        [BindProperty(SupportsGet = true, Name = "p")]
        public int CurrentPage {get; set;}
        public int CountPages {get; set;}
        public async Task OnGetAsync(string SearchString)
        {
            if (_context.articles != null)
            {
                // Article = await _context.articles.ToListAsync();
                int totalArticle = await _context.articles.CountAsync();

                CountPages = (int)Math.Ceiling((double)totalArticle / ITEMS_PER_PAGE);

                if (CurrentPage < 1)
                    CurrentPage = 1;
                if (CurrentPage > CountPages) 
                    CurrentPage = CountPages;

                var query = (from a in _context.articles
                            orderby a.Created descending
                            select a)
                            .Skip((CurrentPage - 1) * ITEMS_PER_PAGE)
                            .Take(ITEMS_PER_PAGE);
                if (!string.IsNullOrEmpty(SearchString)) 
                {
                    Article = query.Where(a => a.Title.Contains(SearchString)).ToList();
                }
                else
                {
                    Article = await query.ToListAsync();
                }
            }
        }
    }
}
