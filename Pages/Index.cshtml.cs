using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webrazorapp.models;

namespace webrazorapp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    private readonly AppDbContext myBlogContext;

    public IndexModel(ILogger<IndexModel> logger, AppDbContext _myBlogContext)
    {
        _logger = logger;
        myBlogContext = _myBlogContext;
    }

    public void OnGet()
    {
        var posts = (from a in myBlogContext.articles
                    orderby a.Created descending
                    select a).ToList();
        ViewData["posts"] = posts;
    }
}
