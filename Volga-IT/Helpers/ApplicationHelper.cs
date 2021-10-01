// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Reflection;

namespace Volga_IT.Helpers
{
    public static class ApplicationHelper
    {
        static ApplicationHelper()
        {
            Directory = GetDirectoryInternal();
        }

        public static String? Directory { get; }

        private static String? GetDirectoryInternal()
        {
            try
            {
                String? location = Assembly.GetEntryAssembly()?.Location;
                if (String.IsNullOrWhiteSpace(location))
                {
                    return null;
                }

                String? directory = PathHelper.GetDirectoryName(location);

                if (String.IsNullOrEmpty(directory) || !PathHelper.IsExistAsFolder(directory))
                {
                    return null;
                }

                return directory;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}