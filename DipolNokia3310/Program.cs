using System;
using Avalonia;
using System.Threading.Tasks;

namespace DipolNokia3310
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                // Устанавливаем глобальный обработчик исключений
                AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
                {
                    Console.WriteLine($"Необработанное исключение: {e.ExceptionObject}");
                };

                // Устанавливаем обработчик исключений для задач
                TaskScheduler.UnobservedTaskException += (sender, e) =>
                {
                    Console.WriteLine($"Необработанное исключение в Task: {e.Exception}");
                    e.SetObserved(); // Предотвращаем обрушение приложения
                };

                // Запускаем приложение
                BuildAvaloniaApp()
                    .StartWithClassicDesktopLifetime(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критическая ошибка при запуске: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner StackTrace: {ex.InnerException.StackTrace}");
                }
                Console.ReadLine(); // Чтобы увидеть сообщение об ошибке
            }
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();
    }
}