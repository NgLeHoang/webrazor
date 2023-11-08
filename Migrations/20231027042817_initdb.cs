using System;
using Bogus;
using Microsoft.EntityFrameworkCore.Migrations;
using webrazorapp.models;

#nullable disable

namespace webrazorapp.Migrations
{
    /// <inheritdoc />
    public partial class initdb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "articles",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Content = table.Column<string>(type: "ntext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_articles", x => x.ID);
                });

                // Fake Data: Bogus

                Randomizer.Seed = new Random(8675309);

                var fakeArticle = new Faker<Article>();
                fakeArticle.RuleFor(a => a.Title, f => f.Lorem.Sentence(5, 5));
                fakeArticle.RuleFor(a => a.Created, f => f.Date.Between(new DateTime(2023, 1, 1), new DateTime(2023, 11, 7)));
                fakeArticle.RuleFor(a => a.Content, f => f.Lorem.Paragraphs(1, 4));

                Article article = fakeArticle.Generate();

                for (int i = 0; i < 150; i++) {
                    migrationBuilder.InsertData(
                    table: "articles",
                    columns: new[] {"Title", "Created", "Content"},
                    values: new object[] {
                        article.Title,
                        article.Created,
                        article.Content
                    }
                );
                }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "articles");
        }
    }
}
