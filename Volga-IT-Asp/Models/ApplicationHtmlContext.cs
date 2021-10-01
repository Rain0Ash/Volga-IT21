// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System.IO;
using Microsoft.EntityFrameworkCore;
using Volga_IT.Helpers;

namespace Volga_IT.Models
{
    public class ApplicationHtmlContext : HtmlContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            DirectoryInfo directory = Directory.CreateDirectory(Path.Join(ApplicationHelper.Directory, "Database"));
            builder.UseSqlite($"Filename={Path.Join(directory.FullName, "files.db")}");
        }
    }
}