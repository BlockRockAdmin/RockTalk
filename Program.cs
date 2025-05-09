using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Windows.Forms;

namespace RockTalk
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Configura DI
            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());
            services.AddSingleton<SupabaseService>();
            services.AddSingleton<BaseForm>();
            var serviceProvider = services.BuildServiceProvider();

            // Avvia la form
            using (var scope = serviceProvider.CreateScope())
            {
                var form = scope.ServiceProvider.GetRequiredService<BaseForm>();
                Application.Run(form);
            }
        }
    }
}
