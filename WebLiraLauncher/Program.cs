namespace WebLiraLauncher;

public class Program
{
    public static void Main()
    {
        if (!DotNetChecker.IsDotNet8Installed())
        {
            MessageBox.Show("На компьютере не установлен .NET 8 SDK. " +
                "Попросите системных администраторов установить его.");
        }
        else
        {
            LisenceProvider lisenceProvider = new();
            bool isThereFreeLicense;

            try
            {
                //Проверяем наличие свободной лицензии
                isThereFreeLicense = lisenceProvider.IsThereFreeLicense();
            }
            catch (AggregateException)
            {
                MessageBox.Show("Сервер, предоставляющий лицензии, не отвечает. " +
                    "Попробуйте запустить приложение позже");
                return;
            }

            if (isThereFreeLicense)
            {
                LiraLauncher.Launch();
            }
            else
            {
                //Console.WriteLine("Постановка пользователя в очередь на получение лицензии" +
                //    Environment.NewLine + "Ожидание своей очереди");

                FreeLicenseWaiter lisenceWaiter = new();
                lisenceWaiter.Wait();
            }
        }
    }
}