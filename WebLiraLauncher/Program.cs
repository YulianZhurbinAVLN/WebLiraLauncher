namespace WebLiraLauncher;

public class Program
{
    public static void Main()
    {
        if (!DotNetChecker.IsDotNet8Installed())
        {
            MessageBox.Show("�� ���������� �� ���������� .NET 8 SDK. " +
                "��������� ��������� ��������������� ���������� ���.");
        }
        else
        {
            LisenceProvider lisenceProvider = new();
            bool isThereFreeLicense;

            try
            {
                //��������� ������� ��������� ��������
                isThereFreeLicense = lisenceProvider.IsThereFreeLicense();
            }
            catch (AggregateException)
            {
                MessageBox.Show("������, ��������������� ��������, �� ��������. " +
                    "���������� ��������� ���������� �����");
                return;
            }

            if (isThereFreeLicense)
            {
                LiraLauncher.Launch();
            }
            else
            {
                //Console.WriteLine("���������� ������������ � ������� �� ��������� ��������" +
                //    Environment.NewLine + "�������� ����� �������");

                FreeLicenseWaiter lisenceWaiter = new();
                lisenceWaiter.Wait();
            }
        }
    }
}