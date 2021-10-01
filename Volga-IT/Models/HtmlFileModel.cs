using System;
using System.ComponentModel.DataAnnotations;

namespace Volga_IT.Models
{
    public record HtmlFileModel
    {
        [Key]
        public Int32 Id { get; set; }

        public String? Name { get; set; }
        
        [Required]
        public Int64 Hash { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public HtmlFileModel()
        {
        }
        
        public HtmlFileModel(String? name, Int64 hash)
        {
            Name = name;
            Hash = hash;
        }
    }
}