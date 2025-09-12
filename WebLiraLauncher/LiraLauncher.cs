using LiraSapr;
using System.Diagnostics;
using System.Text;

namespace WebLiraLauncher;

public class LiraLauncher
{
    const string LIRA_PROCESS_NAME = "LiraSapr";
    public static event Action? LaunchCompleted;
        
    public static bool IsLiraWorking()
    {
        Process[] allLiraProcesses = Process.GetProcessesByName(LIRA_PROCESS_NAME);

        if (allLiraProcesses.Length > 0)
        {
            Process liraProcess = allLiraProcesses.First();
            StringBuilder stringBuilder = new("ЛИРА САПР уже запущена");

            if (liraProcess.MainWindowHandle == IntPtr.Zero)
            {
                stringBuilder.Append(" в фоновом режиме. Просто дважды щелкните левой клавишей мыши по ярлыку ЛИРЫ САПР");
            }

            MessageBox.Show(stringBuilder.ToString());
            return true;
        }

        return false;
    }

    public static void Launch()
    {
        LiraApplication app = null!;

        try
        {
            app = new();

            //Проверка наличия доступа к Лире.
            //Создание нового документа не может быть выполнено,
            //если нет свободной лицензии. В таком случае
            //выбрасывается исключение
            LiraDocument doc = app.CreateNewDocument();
        }
        catch (System.Runtime.InteropServices.COMException)
        {
            MessageBox.Show("Произошла ошибка при запуске Лиры. " +
                "Попробуйте перезапустить приложение");
            CloseLira();
            LaunchCompleted?.Invoke();
            return;
        }

        //Закрытие созданного документа, который
        //нужен только для проверки наличия свободной лицензии
        app.ActiveDocument.Close();

        Task closingLira = new(() =>
        {
            //Время ожидания ответа от пользователя после того,
            //как появилось оповещение о наличии свободной лицензии
            Thread.Sleep(TimeSpan.FromMinutes(2.0));

            CloseLira();
            MessageBox.Show("Время ожидания вышло - Лира была закрыта.");
        });
        closingLira.Start();

        MessageBox.Show("Доступна свободная лицензия");
        LaunchCompleted?.Invoke();
    }

    private static void CloseLira()
    {
        Process[] liraProcesses = Process.GetProcessesByName(LIRA_PROCESS_NAME);
        foreach (Process process in liraProcesses)
        {
            process.Kill();
        }
    }
}