using System.Text;
using System.Text.Json;

namespace PnpFriendlyNameEditor;

internal static class FriendlyNameBackupService
{
    private static readonly UTF8Encoding UTF8WithoutBOM = new(false);

    public static void Append(DeviceEntry OldDevice, string NewFriendlyName)
    {
        var DirectoryPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PnpFriendlyNameEditor");

        Directory.CreateDirectory(DirectoryPath);

        var FilePath = Path.Combine(DirectoryPath, "friendly-name-backup.jsonl");
        var Record = new
        {
            Time = DateTimeOffset.Now,
            OldDevice.InstanceId,
            OldDevice.DisplayName,
            OldDevice.FriendlyName,
            OldDevice.Description,
            NewFriendlyName
        };

        File.AppendAllText(
            FilePath,
            JsonSerializer.Serialize(Record) + Environment.NewLine,
            UTF8WithoutBOM);
    }
}
