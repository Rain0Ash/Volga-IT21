// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using Microsoft.EntityFrameworkCore;
using Volga_IT.Environment;

namespace Volga_IT.Helpers
{
    public static class ArgumentHandlerHelper
    {
        public static T? GetDatabaseModel<T>(this ArgumentHandler handler) where T : DbContext, new()
        {
            return handler.Contains(ArgumentHandler.UseDatabaseArgument) ? new T() : null;
        }
    }
}