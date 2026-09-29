using System.ComponentModel;
using System.Text;
using System.Text.Json;

namespace PnpFriendlyNameEditor;

internal enum CliExitCode
{
    Success = 0,
    InvalidArguments = 1,
    InvalidConfig = 2,
    IncompleteMatch = 3,
    PartialFailure = 4,
    RuntimeFailure = 5
}

internal static class CliRunner
{
    public static int Run(IReadOnlyList<string> Args)
    {
        if (Args.Count == 0)
            return (int)CliExitCode.InvalidArguments;

        try
        {
            return Args[0].Trim().ToLowerInvariant() switch
            {
                "help" or "--help" or "-h" or "/?" => PrintHelp(),
                "import" => RunImport(Args.Skip(1).ToArray()),
                "list" => RunList(Args.Skip(1).ToArray()),
                "set" => RunSet(Args.Skip(1).ToArray()),
                _ => InvalidCommand(Args[0])
            };
        }
        catch (InvalidDataException Ex)
        {
            Console.Error.WriteLine($"配置错误：{Ex.Message}");
            return (int)CliExitCode.InvalidConfig;
        }
        catch (JsonException Ex)
        {
            Console.Error.WriteLine($"JSON 错误：{Ex.Message}");
            return (int)CliExitCode.InvalidConfig;
        }
        catch (FileNotFoundException Ex)
        {
            Console.Error.WriteLine($"文件不存在：{Ex.FileName ?? Ex.Message}");
            return (int)CliExitCode.InvalidConfig;
        }
        catch (DirectoryNotFoundException Ex)
        {
            Console.Error.WriteLine($"路径不存在：{Ex.Message}");
            return (int)CliExitCode.InvalidConfig;
        }
        catch (KeyNotFoundException Ex)
        {
            Console.Error.WriteLine(Ex.Message);
            return (int)CliExitCode.IncompleteMatch;
        }
        catch (Win32Exception Ex)
        {
            Console.Error.WriteLine($"Win32 错误 {Ex.NativeErrorCode}: {Ex.Message}");
            return (int)CliExitCode.RuntimeFailure;
        }
        catch (UnauthorizedAccessException Ex)
        {
            Console.Error.WriteLine($"权限不足：{Ex.Message}");
            return (int)CliExitCode.RuntimeFailure;
        }
        catch (IOException Ex)
        {
            Console.Error.WriteLine($"IO 错误：{Ex.Message}");
            return (int)CliExitCode.RuntimeFailure;
        }
        catch (Exception Ex)
        {
            Console.Error.WriteLine(Ex);
            return (int)CliExitCode.RuntimeFailure;
        }
    }

    private static int RunImport(IReadOnlyList<string> Args)
    {
        if (Args.Count == 0 || Args.Any(Arg => Arg is "--help" or "-h" or "/?"))
            return PrintImportHelp(Args.Count == 0 ? CliExitCode.InvalidArguments : CliExitCode.Success);

        var ConfigPaths = Args
            .Where(Arg => !Arg.StartsWith("--", StringComparison.Ordinal))
            .ToArray();
        if (ConfigPaths.Length == 0)
            return PrintImportHelp(CliExitCode.InvalidArguments, "缺少 JSON 配置文件路径。");
        if (ConfigPaths.Length > 1)
            return PrintImportHelp(
                CliExitCode.InvalidArguments,
                $"只能指定一个 JSON 配置文件，额外参数：{string.Join(", ", ConfigPaths.Skip(1))}");

        var ConfigPath = ConfigPaths[0];
        var Apply = Args.Any(Arg => string.Equals(Arg, "--apply", StringComparison.OrdinalIgnoreCase));
        var ExactOnly = Args.Any(Arg => string.Equals(Arg, "--exact-only", StringComparison.OrdinalIgnoreCase));
        var UnknownOptions = Args
            .Where(Arg => Arg.StartsWith("--", StringComparison.Ordinal))
            .Where(Arg => !string.Equals(Arg, "--apply", StringComparison.OrdinalIgnoreCase))
            .Where(Arg => !string.Equals(Arg, "--exact-only", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        if (UnknownOptions.Length > 0)
            return PrintImportHelp(
                CliExitCode.InvalidArguments,
                $"未知参数：{string.Join(", ", UnknownOptions)}");

        var Rules = ImportFileParser.LoadRules(ConfigPath);
        if (Rules.Count == 0)
        {
            Console.Error.WriteLine("配置文件中的 Devices 为空，没有可处理的设备。");
            return (int)CliExitCode.InvalidConfig;
        }

        Console.WriteLine($"配置：{Path.GetFullPath(ConfigPath)}");
        Console.WriteLine($"规则：{Rules.Count}");
        Console.WriteLine("正在枚举并匹配全部 PnP 设备...");

        var Preparation = PnpBatchService.PrepareImport(Rules);
        var Matches = ExactOnly
            ? Preparation.Matches.Where(Match => Match.Kind == ImportMatchKind.Exact).ToArray()
            : Preparation.Matches.ToArray();
        var SkippedFuzzyCount = ExactOnly ? Preparation.FuzzyCount : 0;

        PrintImportSummary(Preparation, SkippedFuzzyCount);
        PrintMatches(Matches);
        PrintUnmatched(Preparation.Unmatched);

        if (!Apply)
        {
            Console.WriteLine();
            Console.WriteLine("预览完成，未写入任何 FriendlyName。需要实际应用时追加 --apply。");
            return Preparation.UnmatchedCount > 0 || SkippedFuzzyCount > 0
                ? (int)CliExitCode.IncompleteMatch
                : (int)CliExitCode.Success;
        }

        if (Matches.Length == 0)
        {
            Console.Error.WriteLine("没有可应用的匹配设备。未执行写入。");
            return Preparation.UnmatchedCount > 0 || SkippedFuzzyCount > 0
                ? (int)CliExitCode.IncompleteMatch
                : (int)CliExitCode.Success;
        }

        Console.WriteLine();
        Console.WriteLine($"开始写入 {Matches.Length} 个 FriendlyName...");
        var Result = PnpBatchService.ApplyImportedNames(Matches);

        Console.WriteLine($"写入完成：成功 {Result.SuccessCount}，失败 {Result.FailedCount}。");
        foreach (var Error in Result.Errors)
        {
            Console.Error.WriteLine();
            Console.Error.WriteLine($"[失败] {Error.Device.InstanceId}");
            Console.Error.WriteLine($"目标 FriendlyName: {Error.DesiredFriendlyName}");
            Console.Error.WriteLine(Error.Exception.Message);
        }

        if (!Result.Succeeded)
            return (int)CliExitCode.PartialFailure;

        return Preparation.UnmatchedCount > 0 || SkippedFuzzyCount > 0
            ? (int)CliExitCode.IncompleteMatch
            : (int)CliExitCode.Success;
    }

    private static int RunList(IReadOnlyList<string> Args)
    {
        if (Args.Any(Arg => Arg is "--help" or "-h" or "/?"))
            return PrintListHelp();

        var ShowAll = Args.Any(Arg => string.Equals(Arg, "--all", StringComparison.OrdinalIgnoreCase));
        var HasOutput = Args.Any(Arg => string.Equals(Arg, "--output", StringComparison.OrdinalIgnoreCase));
        if (HasOutput && !TryGetOptionValue(Args, "--output", out _))
            return PrintListHelp(CliExitCode.InvalidArguments, "--output 必须指定输出文件路径。");

        _ = TryGetOptionValue(Args, "--output", out var OutputPath);

        var KnownArguments = new HashSet<int>();
        for (var Index = 0; Index < Args.Count; Index++)
            if (string.Equals(Args[Index], "--all", StringComparison.OrdinalIgnoreCase))
                KnownArguments.Add(Index);
        MarkOptionAndValue(Args, "--output", KnownArguments);

        var UnknownArguments = Args.Where((_, Index) => !KnownArguments.Contains(Index)).ToArray();
        if (UnknownArguments.Length > 0)
            return PrintListHelp(
                CliExitCode.InvalidArguments,
                $"未知参数：{string.Join(", ", UnknownArguments)}");

        var Devices = PnpDeviceService.EnumerateDevices(!ShowAll)
            .OrderBy(Device => Device.ClassDisplayName)
            .ThenBy(Device => Device.LogicalProcessorNumber ?? int.MaxValue)
            .ThenBy(Device => Device.DisplayName)
            .ToArray();

        if (!string.IsNullOrWhiteSpace(OutputPath))
        {
            var FullOutputPath = Path.GetFullPath(OutputPath);
            WriteDeviceList(FullOutputPath, Devices);
            Console.WriteLine($"已以 UTF-8 导出 {Devices.Length} 个设备：{FullOutputPath}");
            return (int)CliExitCode.Success;
        }

        Console.WriteLine(BuildDeviceListHeader());
        foreach (var Device in Devices)
            Console.WriteLine(BuildDeviceListLine(Device));

        Console.WriteLine();
        Console.WriteLine($"共 {Devices.Length} 个设备。{(ShowAll ? "包含非当前连接设备。" : "仅当前连接设备。")}");
        return (int)CliExitCode.Success;
    }

    private static void WriteDeviceList(string OutputPath, IReadOnlyList<DeviceEntry> Devices)
    {
        var DirectoryPath = Path.GetDirectoryName(OutputPath);
        if (!string.IsNullOrWhiteSpace(DirectoryPath))
            Directory.CreateDirectory(DirectoryPath);

        using StreamWriter Writer = new(OutputPath, false, new UTF8Encoding(false))
        {
            NewLine = "\r\n"
        };

        Writer.WriteLine(BuildDeviceListHeader());
        foreach (var Device in Devices)
            Writer.WriteLine(BuildDeviceListLine(Device));
    }

    private static string BuildDeviceListHeader()
        => "实例路径\t当前 FriendlyName\t设备描述（原名称）";

    private static string BuildDeviceListLine(DeviceEntry Device)
        => $"{Device.InstanceId}\t{Device.FriendlyName}\t{Device.Description}";

    private static int RunSet(IReadOnlyList<string> Args)
    {
        if (Args.Any(Arg => Arg is "--help" or "-h" or "/?"))
            return PrintSetHelp();

        if (!TryGetOptionValue(Args, "--instance-id", out var InstanceID) ||
            !TryGetOptionValue(Args, "--name", out var FriendlyName))
            return PrintSetHelp(CliExitCode.InvalidArguments, "必须同时提供 --instance-id 和 --name。");

        if (string.IsNullOrWhiteSpace(InstanceID))
            return PrintSetHelp(CliExitCode.InvalidArguments, "--instance-id 不能为空。");
        if (string.IsNullOrWhiteSpace(FriendlyName))
            return PrintSetHelp(CliExitCode.InvalidArguments, "--name 不能为空。");

        InstanceID = InstanceID.Trim();
        FriendlyName = FriendlyName.Trim();

        var KnownArguments = new HashSet<int>();
        MarkOptionAndValue(Args, "--instance-id", KnownArguments);
        MarkOptionAndValue(Args, "--name", KnownArguments);
        var UnknownArguments = Args.Where((_, Index) => !KnownArguments.Contains(Index)).ToArray();
        if (UnknownArguments.Length > 0)
            return PrintSetHelp(
                CliExitCode.InvalidArguments,
                $"未知参数：{string.Join(", ", UnknownArguments)}");

        var Device = PnpBatchService.FindDevice(InstanceID);
        Console.WriteLine($"设备：{Device.InstanceId}");
        Console.WriteLine($"设备描述（原名称）：{Device.Description}");
        Console.WriteLine($"当前 FriendlyName：{Device.FriendlyName}");
        Console.WriteLine($"目标 FriendlyName：{FriendlyName}");

        PnpBatchService.ApplyFriendlyName(Device, FriendlyName);
        Console.WriteLine("写入成功。原值已追加到 friendly-name-backup.jsonl。");
        return (int)CliExitCode.Success;
    }

    private static void PrintImportSummary(ImportPreparation Preparation, int SkippedFuzzyCount)
    {
        Console.WriteLine();
        Console.WriteLine("匹配结果：");
        Console.WriteLine($"  完全匹配：{Preparation.ExactCount}");
        Console.WriteLine($"  模糊匹配：{Preparation.FuzzyCount}");
        Console.WriteLine($"  未匹配：{Preparation.UnmatchedCount}");
        Console.WriteLine($"  多候选：{Preparation.AmbiguousCount}");
        if (SkippedFuzzyCount > 0)
            Console.WriteLine($"  --exact-only 跳过模糊匹配：{SkippedFuzzyCount}");
    }

    private static void PrintMatches(IReadOnlyList<ImportMatchResult> Matches)
    {
        if (Matches.Count == 0)
            return;

        Console.WriteLine();
        Console.WriteLine("将处理的匹配：");
        foreach (var Match in Matches)
        {
            var Kind = Match.Kind == ImportMatchKind.Exact ? "EXACT" : "FUZZY";
            Console.WriteLine(
                $"  [{Kind}] {Match.Device.InstanceId}\r\n" +
                $"          {Match.Device.FriendlyName} -> {Match.Rule.DesiredFriendlyName}");
        }
    }

    private static void PrintUnmatched(IReadOnlyList<UnmatchedImportRule> Unmatched)
    {
        if (Unmatched.Count == 0)
            return;

        Console.WriteLine();
        Console.WriteLine("未匹配：");
        foreach (var Item in Unmatched)
        {
            var Source = string.IsNullOrWhiteSpace(Item.Rule.InstanceId)
                ? $"Devices[{Item.Rule.SourceIndex}]"
                : Item.Rule.InstanceId;
            Console.WriteLine(
                $"  {Source}\r\n" +
                $"    FriendlyName: {Item.Rule.DesiredFriendlyName}\r\n" +
                $"    {Item.Reason}");
        }
    }

    private static bool TryGetOptionValue(IReadOnlyList<string> Args, string Option, out string Value)
    {
        for (var Index = 0; Index < Args.Count; Index++)
        {
            if (!string.Equals(Args[Index], Option, StringComparison.OrdinalIgnoreCase))
                continue;
            if (Index + 1 >= Args.Count || Args[Index + 1].StartsWith("--", StringComparison.Ordinal))
                break;

            Value = Args[Index + 1];
            return true;
        }

        Value = "";
        return false;
    }

    private static void MarkOptionAndValue(IReadOnlyList<string> Args, string Option, ISet<int> KnownArguments)
    {
        for (var Index = 0; Index < Args.Count; Index++)
        {
            if (!string.Equals(Args[Index], Option, StringComparison.OrdinalIgnoreCase))
                continue;

            KnownArguments.Add(Index);
            if (Index + 1 < Args.Count)
                KnownArguments.Add(Index + 1);
            return;
        }
    }

    private static int InvalidCommand(string Command)
    {
        Console.Error.WriteLine($"未知命令：{Command}");
        Console.Error.WriteLine();
        return PrintHelp(CliExitCode.InvalidArguments);
    }

    private static int PrintHelp(CliExitCode ExitCode = CliExitCode.Success)
    {
        Console.WriteLine("PnP FriendlyName Editor 命令行接口");
        Console.WriteLine();
        Console.WriteLine("用法：");
        Console.WriteLine("  PnpFriendlyNameEditor.exe import <配置.json> [--apply] [--exact-only]");
        Console.WriteLine("  PnpFriendlyNameEditor.exe list [--all] [--output <文件>]");
        Console.WriteLine("  PnpFriendlyNameEditor.exe set --instance-id <InstanceID> --name <FriendlyName>");
        Console.WriteLine("  PnpFriendlyNameEditor.exe --help");
        Console.WriteLine();
        Console.WriteLine("无参数启动原 WinForms UI。import 默认只预览，必须追加 --apply 才会写入。");
        Console.WriteLine();
        Console.WriteLine("退出码：0=成功，1=参数错误，2=配置错误，3=匹配不完整，4=部分写入失败，5=运行时错误。");
        return (int)ExitCode;
    }

    private static int PrintImportHelp(CliExitCode ExitCode = CliExitCode.Success, string? Error = null)
    {
        if (!string.IsNullOrWhiteSpace(Error))
        {
            Console.Error.WriteLine(Error);
            Console.Error.WriteLine();
        }

        Console.WriteLine("用法：PnpFriendlyNameEditor.exe import <配置.json> [--apply] [--exact-only]");
        Console.WriteLine("  --apply       实际写入；不指定时只做枚举、匹配和预览。");
        Console.WriteLine("  --exact-only  只应用 InstanceID 完全匹配，跳过模糊匹配。");
        return (int)ExitCode;
    }

    private static int PrintListHelp(CliExitCode ExitCode = CliExitCode.Success, string? Error = null)
    {
        if (!string.IsNullOrWhiteSpace(Error))
        {
            Console.Error.WriteLine(Error);
            Console.Error.WriteLine();
        }

        Console.WriteLine("用法：PnpFriendlyNameEditor.exe list [--all] [--output <文件>]");
        Console.WriteLine("  --all            包含非当前连接设备；默认仅列出当前连接设备。");
        Console.WriteLine("  --output <文件>  直接以 UTF-8（无 BOM）+ CRLF 写入文件。");
        Console.WriteLine();
        Console.WriteLine("输出列：实例路径 / 当前 FriendlyName / 设备描述（原名称）。");
        Console.WriteLine("CMD 也可直接重定向：PnpFriendlyNameEditor.exe list > connected-devices.txt");
        Console.WriteLine("或直接导出 UTF-8：PnpFriendlyNameEditor.exe list --output connected-devices.txt");
        return (int)ExitCode;
    }

    private static int PrintSetHelp(CliExitCode ExitCode = CliExitCode.Success, string? Error = null)
    {
        if (!string.IsNullOrWhiteSpace(Error))
        {
            Console.Error.WriteLine(Error);
            Console.Error.WriteLine();
        }

        Console.WriteLine("用法：PnpFriendlyNameEditor.exe set --instance-id <InstanceID> --name <FriendlyName>");
        return (int)ExitCode;
    }
}
