// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using Microsoft.EntityFrameworkCore;

namespace Volga_IT.Models
{
    public class HtmlContext : DbContext
    {
        public DbSet<HtmlFileModel> Files { get; set; } = null!;
        public DbSet<HtmlWordsModel> Words { get; set; } = null!;

        public HtmlContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<HtmlFileModel>(entity =>
            {
                entity.HasIndex(e => e.Hash).IsUnique();
            });
        }
    }
}