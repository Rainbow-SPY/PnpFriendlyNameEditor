namespace PnpFriendlyNameEditor;

internal sealed class FriendlyNameApplyError
{
    public required DeviceEntry Device { get; init; }
    public required string DesiredFriendlyName { get; init; }
    public required Exception Exception { get; init; }
}

internal sealed class FriendlyNameApplyResult
{
    public int AttemptedCount { get; init; }
    public int SuccessCount { get; init; }
    public List<FriendlyNameApplyError> Errors { get; init; } = [];
    public int FailedCount => Errors.Count;
    public bool Succeeded => FailedCount == 0;
}

internal static class PnpBatchService
{
    public static ImportPreparation PrepareImport(
        IReadOnlyList<ImportRule> Rules,
        IProgress<ImportProgressInfo>? Progress = null)
    {
        Progress?.Report(new ImportProgressInfo { Percent = 8, Status = "枚举全部 PnP 设备..." });
        var Devices = PnpDeviceService.EnumerateDevices(false);
        Progress?.Report(new ImportProgressInfo
        {
            Percent = 15,
            Status = $"已枚举 {Devices.Count} 个设备，开始匹配..."
        });

        var Result = ImportMatcher.Prepare(Rules, Devices, Progress);
        Progress?.Report(new ImportProgressInfo { Percent = 100, Status = "匹配完成" });
        return Result;
    }

    public static ImportPreparation PrepareImport(
        string ConfigPath,
        IProgress<ImportProgressInfo>? Progress = null)
        => PrepareImport(ImportFileParser.LoadRules(ConfigPath), Progress);

    public static FriendlyNameApplyResult ApplyImportedNames(
        IReadOnlyList<ImportMatchResult> Matches,
        IProgress<ImportProgressInfo>? Progress = null)
    {
        var Errors = new List<FriendlyNameApplyError>();
        var SuccessCount = 0;

        for (var Index = 0; Index < Matches.Count; Index++)
        {
            var Match = Matches[Index];
            Progress?.Report(new ImportProgressInfo
            {
                Percent = Matches.Count == 0 ? 100 : (int)(100.0 * Index / Matches.Count),
                Status = $"应用 {Index + 1}/{Matches.Count}: {Match.Rule.DesiredFriendlyName}"
            });

            try
            {
                ApplyFriendlyName(Match.Device, Match.Rule.DesiredFriendlyName);
                SuccessCount++;
            }
            catch (Exception Ex)
            {
                Errors.Add(new FriendlyNameApplyError
                {
                    Device = Match.Device,
                    DesiredFriendlyName = Match.Rule.DesiredFriendlyName,
                    Exception = Ex
                });
            }
        }

        Progress?.Report(new ImportProgressInfo { Percent = 100, Status = "写入完成" });
        return new FriendlyNameApplyResult
        {
            AttemptedCount = Matches.Count,
            SuccessCount = SuccessCount,
            Errors = Errors
        };
    }

    public static void ApplyFriendlyName(DeviceEntry Device, string FriendlyName)
    {
        FriendlyNameBackupService.Append(Device, FriendlyName);
        PnpDeviceService.SetFriendlyName(Device.InstanceId, FriendlyName);
    }

    public static DeviceEntry FindDevice(string InstanceID)
        => PnpDeviceService.EnumerateDevices(false)
               .FirstOrDefault(Device => string.Equals(
                   Device.InstanceId,
                   InstanceID,
                   StringComparison.OrdinalIgnoreCase))
           ?? throw new KeyNotFoundException($"未找到 PnP 设备：{InstanceID}");
}
