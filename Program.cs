namespace PnpFriendlyNameEditor;

internal static class Program
{
    [STAThread]
    private static int Main(string[] Args)
    {
        if (Args.Length > 0)
            return CliRunner.Run(Args);

        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
        return 0;
    }
}
