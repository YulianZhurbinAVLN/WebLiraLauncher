namespace WebLiraLauncher;

public class Program
{
    public static void Main()
    {
        Console.SetWindowSize(50, 10);

        if (LiraLauncher.IsLiraWorking())
        {
            return;
        }

        if (!DotNetChecker.IsDotNet8Installed())
        {
            MessageBox.Show("�� ���������� �� ���������� .NET 8 SDK. " +
                "��������� ��������� ��������������� ���������� ���.");
            return;
        }

        LisenceProvider lisenceProvider = new();
        bool isThereFreeLicense;

        try
        {
            //��������� ������� ��������� ��������
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
            //������� ��������� ��������� ��������, ����� ��������� ����
            FreeLicenseWaiter lisenceWaiter = new();
            lisenceWaiter.LaunchLira();
        }

    }
}