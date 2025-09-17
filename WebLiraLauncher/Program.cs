namespace WebLiraLauncher;

public class Program
{
    public static void Main()
    {
        string mutexName = "865AA237-C0EC-4C6B-960C-EB8CFD4C99FA";
        Mutex mutex = new(true, mutexName, out bool createdNew);

        //Если уже есть запущенный экземпляр приложения, то закрываем текущий
        if (!createdNew)
            return;

        Console.SetWindowSize(50, 10);

        if (LiraLauncher.IsLiraWorking())
        {
            return;
        }

        if (!DotNetChecker.IsDotNet8Installed())
        {
            MessageBox.Show("На компьютере не установлен .NET 8 SDK. " +
                "Попросите системных администраторов установить его.");
            return;
        }

        LisenceProvider lisenceProvider = new();
        bool isThereFreeLicense;

        try
        {
            //Проверяем наличие свободной лицензии
            isThereFreeLicense = lisenceProvider.IsThereFreeLicense();
        }
        catch (InvalidOperationException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            MessageBox.Show(ex.Message);
            Console.ReadKey();
            return;
        }

        //HACK: Debug code
        //isThereFreeLicense = false;

        if (isThereFreeLicense)
        {
            LiraLauncher.Launch();
        }
        else
        {
            //Ожидаем появления свободной лицензии, затем запускаем Лиру
            FreeLicenseWaiter lisenceWaiter = new();
            lisenceWaiter.LaunchLira();
        }

        mutex.ReleaseMutex();
    }
}