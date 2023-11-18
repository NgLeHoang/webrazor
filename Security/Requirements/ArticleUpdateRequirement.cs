using Microsoft.AspNetCore.Authorization;

namespace MyApp.Security.Requirements
{
    public class ArticleUpdateRequirement : IAuthorizationRequirement
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }

        public ArticleUpdateRequirement(int year = 2023, int month = 11, int day = 18)
        {
            Year = year;
            Month = month;
            Day = day;
        }
    }
}