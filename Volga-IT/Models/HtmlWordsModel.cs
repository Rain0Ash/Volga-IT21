using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Volga_IT.Models
{
    public record HtmlWordsModel
    {
        [Key]
        public Int32 Id { get; set; }
        
        [Required]
        public Int32 FileId { get; set; }
        
        [ForeignKey(nameof(FileId))]
        public HtmlFileModel File { get; set; } = null!;

        [Required]
        public String Word { get; set; } = null!;

        public Int64 Count { get; set; }

        public HtmlWordsModel()
        {
        }
        
        public HtmlWordsModel(HtmlFileModel file, String word, Int64 count)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (String.IsNullOrEmpty(word))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(word));
            }

            FileId = file.Id;
            File = file;
            Word = word;
            Count = count;
        }
    }
}