// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Volga_IT.Extractor;
using Volga_IT.Extractor.Interfaces;
using Volga_IT.Models;

namespace Volga_IT.Database
{
    public class DatabaseHtmlExtractHandler : HtmlExtractHandler
    {
        public HtmlContext HtmlContext { get; }
        
        public DatabaseHtmlExtractHandler(HtmlContext context)
        {
            HtmlContext = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public virtual Boolean DatabaseContainsHash(Int64 hash)
        {
            return HtmlContext.Words.Any(item => item.File.Hash == hash);
        }

        public virtual IEnumerable<WordCounterRecord>? Extract(Int64 hash)
        {
            Boolean contains = DatabaseContainsHash(hash);

            if (!contains)
            {
                return null;
            }
            
            IQueryable<WordCounterRecord> query = HtmlContext.Words
                .Where(item => item.File.Hash == hash)
                .Select(item => new WordCounterRecord(item.Word, item.Count));
            
            query.Load();

            return query;
        }
        
        public override IEnumerable<WordCounterRecord> Extract(Stream stream, IHtmlTextExtractor extractor)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (extractor is null)
            {
                throw new ArgumentNullException(nameof(extractor));
            }

            Int64 hash = ComputeHash(stream);
            return Extract(hash, extractor);
        }
        
        public override IEnumerable<WordCounterRecord> Extract(Int64 hash, IHtmlTextExtractor extractor)
        {
            if (extractor is null)
            {
                throw new ArgumentNullException(nameof(extractor));
            }

            return Extract(hash) ?? ExtractFromHtml(extractor);
        }

        public virtual IEnumerable<WordCounterRecord> ExtractAndSaveToDatabase(Stream stream, IHtmlTextExtractor extractor)
        {
            return ExtractAndSaveToDatabase(stream, extractor, out _);
        }

        public virtual IEnumerable<WordCounterRecord> ExtractAndSaveToDatabase(Stream stream, IHtmlTextExtractor extractor, out Int64 hash)
        {
            return ExtractAndSaveToDatabase((stream as FileStream)?.Name, stream, extractor, out hash);
        }

        public virtual IEnumerable<WordCounterRecord> ExtractAndSaveToDatabase(String? name, Stream stream, IHtmlTextExtractor extractor)
        {
            return ExtractAndSaveToDatabase(name, stream, extractor, out _);
        }

        public virtual IEnumerable<WordCounterRecord> ExtractAndSaveToDatabase(String? name, Stream stream, IHtmlTextExtractor extractor, out Int64 hash)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (extractor is null)
            {
                throw new ArgumentNullException(nameof(extractor));
            }

            hash = ComputeHash(stream);
            Boolean contains = DatabaseContainsHash(hash);

            if (contains)
            {
                return Extract(hash, extractor);
            }

            using IDbContextTransaction transaction = HtmlContext.Database.BeginTransaction();

            HtmlFileModel file = new HtmlFileModel(name, hash);
            HtmlContext.Files.Add(file);

            IEnumerable<WordCounterRecord> source = ExtractFromHtml(extractor);
            source = source.ToList();

            HtmlContext.Words.AddRange(
                source.Select(item => new HtmlWordsModel(file, item.Word, item.Count))
            );

            transaction.Commit();
            HtmlContext.SaveChanges();
            
            return source;
        }

        public virtual Boolean DeleteFromDatabase(Int64 hash)
        {
            try
            {
                using IDbContextTransaction transaction = HtmlContext.Database.BeginTransaction();

                HtmlFileModel? file = HtmlContext.Files.FirstOrDefault(file => file.Hash == hash);

                if (file is null)
                {
                    return false;
                }
                
                HtmlContext.Words.RemoveRange(HtmlContext.Words.Where(word => word.File == file));
                HtmlContext.Files.Remove(file);

                transaction.Commit();
                HtmlContext.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}