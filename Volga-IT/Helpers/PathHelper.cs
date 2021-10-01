// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Volga_IT.Helpers
{
    public static class PathHelper
    {
        public static readonly Char[] Separators = {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar};
        
        public static Boolean IsPathContainsEndSeparator(String path)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            path = path.Trim();
            return path.Length > 0 && Separators.Any(chr => path.EndsWith(chr.ToString()));
        }
        
        public static String? GetFullPath(String path)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            try
            {
                return Path.GetFullPath(String.IsNullOrEmpty(path) ? "." : path);
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        public static Boolean IsNetworkPath(String path)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            return !String.IsNullOrEmpty(path) && path.StartsWith("\\");
        }
        
        public static Boolean IsValidNetworkPath(String path)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            try
            {
                return !String.IsNullOrEmpty(path) && IsNetworkPath(path) && new Uri(path).IsUnc &&
                       Regex.IsMatch(path, @"^\\{2}[\w-]+(\\{1}(([\w-][\w-\s]*[\w-]+[$$]?)|([\w-][$$]?$)))+");
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public static Boolean IsValidPath(String path, Boolean network = true)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            return !String.IsNullOrEmpty(path) && !String.IsNullOrEmpty(GetFullPath(path)) && (!IsNetworkPath(path) || network && IsValidNetworkPath(path));
        }
        
        public static Boolean IsValidFilePath(String path, Boolean network = true)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            return IsValidPath(path, network) && !IsPathContainsEndSeparator(path);
        }
        
        public static Boolean IsExistAsFile(String path)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            return IsValidFilePath(path) && File.Exists(path);
        }
        
        public static Boolean IsExistAsFolder(String path)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            return IsValidFolderPath(path) && Directory.Exists(path);
        }
        
        public static Boolean IsValidFolderPath(String path, Boolean network = true)
        {
            return IsValidPath(path, network);
        }
        
        public static String? GetDirectoryName(String? path)
        {
            return Path.GetDirectoryName(path);
        }
        
        [return: NotNullIfNotNull("path")]
        public static String? GetFileNameWithoutExtension(String? path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }
    }
}