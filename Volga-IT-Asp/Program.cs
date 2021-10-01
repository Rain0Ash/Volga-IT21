using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Volga_IT
{
    public static class Program
    {
        // Это бонус. Выполняет все пункты задания кроме тестов. Возможно выполняет даже задание из финала :) Тестировать можно через Postman или прямо из SwaggerUI.
        // Поддерживает Create Read Delete операции.
        public static void Main(String[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(String[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseKestrel();
                    builder.UseStartup<Startup>();
                });
        }
    }
}