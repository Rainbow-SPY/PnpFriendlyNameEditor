using System.Text.Json;
using System.Text.RegularExpressions;

namespace PnpFriendlyNameEditor;

internal enum PnpEnumeratorType
{
    Unknown,
    USB,
    HID,
    PCI,
    ACPI,
    SCSI,
    USBSTOR,
    STORAGE,
    SWD,
    ROOT,
    BTHENUM,
    BTH,
    BTHLE,
    BTHHFENUM,
    HDAUDIO,
    UEFI,
    DISPLAY,
    HTREE,
    ACPI_HAL,
    BUTTONCONVERTER,
    SW,
    SD,
    SDSTOR,
    PCMCIA,
    ISAPNP,
    SERENUM,
    FTDIBUS,
    LPTENUM,
    PRINTENUM,
    DOT4,
    DOT4PRT,
    VMBUS,
    UMB,
    RDPBUS,
    SENSOR,
    I2C,
    SPI,
    GPIO,
    CustomGuid
}

internal enum PnpClassType
{
    Unknown,
    HIDClass,
    System,
    USB,
    Volume,
    WPD,
    Keyboard,
    DiskDrive,
    Net,
    Processor,
    SoftwareDevice,
    USBDevice,
    Bluetooth,
    MEDIA,
    Mouse,
    AudioEndpoint,
    CDROM,
    VolumeSnapshot,
    BluetoothVirtual,
    SCSIAdapter,
    SoftwareComponent,
    Firmware,
    Monitor,
    Display,
    Ports,
    Computer,
    AudioProcessingObject,
    XnaComposite,
    SecurityDevices,
    Camera,
    AndroidUsbDeviceClass,
    HDC,
    Battery,
    Biometric,
    Image,
    Printer,
    PrintQueue,
    Modem,
    SmartCardReader,
    Sensor,
    Memory,
    NetClient,
    NetService,
    NetTrans
}

internal enum ImportMatchKind
{
    Exact,
    Fuzzy
}

internal sealed class DeviceIdentity
{
    public PnpEnumeratorType EnumeratorType { get; init; }
    public string RawEnumerator { get; init; } = "";
    public string ModelSegment { get; init; } = "";
    public string InstanceTail { get; init; } = "";
    public int? LogicalProcessorNumber { get; init; }
    public Dictionary<string, string> Fields { get; } = new(StringComparer.OrdinalIgnoreCase);

    public string? Get(string key) => Fields.TryGetValue(key, out var value) ? value : null;
}

internal sealed class ImportRule
{
    public int SourceIndex { get; init; }
    public string InstanceId { get; init; } = "";
    public string DesiredFriendlyName { get; init; } = "";
    public string ClassName { get; init; } = "";
    public string ClassDisplayName { get; init; } = "";
    public PnpClassType ClassType { get; init; }
    public string Manufacturer { get; init; } = "";
    public string DeviceDescription { get; init; } = "";
    public string Service { get; init; } = "";
    public string[] HardwareIds { get; init; } = [];
    public int? LogicalProcessorNumber { get; init; }
    public DeviceIdentity Identity { get; init; } = new();
    public Dictionary<string, string> SourceProperties { get; init; } = new(StringComparer.OrdinalIgnoreCase);
}

internal sealed class ImportMatchResult
{
    public required ImportRule Rule { get; init; }
    public required DeviceEntry Device { get; init; }
    public required ImportMatchKind Kind { get; init; }
    public required Dictionary<string, string> SourceProperties { get; init; }
    public required Dictionary<string, string> CurrentProperties { get; init; }
}

internal sealed class UnmatchedImportRule
{
    public required ImportRule Rule { get; init; }
    public int CandidateCount { get; init; }
    public string Reason { get; init; } = "";
}

internal sealed class ImportPreparation
{
    public List<ImportRule> Rules { get; init; } = [];
    public List<ImportMatchResult> Matches { get; init; } = [];
    public List<UnmatchedImportRule> Unmatched { get; init; } = [];

    public int ExactCount => Matches.Count(x => x.Kind == ImportMatchKind.Exact);
    public int FuzzyCount => Matches.Count(x => x.Kind == ImportMatchKind.Fuzzy);
    public int MatchedCount => Matches.Count;
    public int UnmatchedCount => Unmatched.Count;
    public int AmbiguousCount => Unmatched.Count(x => x.CandidateCount > 1);
}

internal sealed class ImportProgressInfo
{
    public int Percent { get; init; }
    public string Status { get; init; } = "";
}

internal static class ImportFileParser
{
    public static List<ImportRule> LoadRules(string path)
    {
        using var stream = File.OpenRead(path);
        using var document = JsonDocument.Parse(stream, new JsonDocumentOptions
        {
            AllowTrailingCommas = true,
            CommentHandling = JsonCommentHandling.Skip
        });

        var root = document.RootElement;
        if (root.ValueKind != JsonValueKind.Object)
            throw new InvalidDataException("JSON 根节点必须是对象。");

        if (!TryGetPropertyIgnoreCase(root, "Devices", out var devices) || devices.ValueKind != JsonValueKind.Array)
            throw new InvalidDataException("配置中缺少 Devices 数组。");

        var result = new List<ImportRule>();
        var index = 0;
        foreach (var item in devices.EnumerateArray())
        {
            index++;
            if (item.ValueKind != JsonValueKind.Object)
                continue;

            var instanceId = GetString(item, "InstanceId");
            var friendlyName = GetString(item, "CustomFriendlyName");
            if (friendlyName is null)
                throw new InvalidDataException($"Devices[{index - 1}] 缺少 CustomFriendlyName。");

            var explicitType = FirstNonEmpty(GetString(item, "ClassType"), GetString(item, "Type"));
            var className = GetString(item, "ClassName") ?? "";
            var classDisplayName = GetString(item, "ClassDisplayName") ?? "";
            if (string.IsNullOrWhiteSpace(className) && !string.IsNullOrWhiteSpace(explicitType))
            {
                if (Enum.TryParse<PnpClassType>(explicitType, true, out var parsedType) && parsedType != PnpClassType.Unknown)
                    className = explicitType!;
                else if (string.IsNullOrWhiteSpace(classDisplayName))
                    classDisplayName = explicitType!;
            }

            var hardwareIds = GetStringArray(item, "HardwareIds");
            var logicalProcessorNumber = GetNullableInt(item, "LogicalProcessorNumber");
            var identity = DeviceIdentityParser.Parse(instanceId ?? "", hardwareIds, logicalProcessorNumber);

            if (TryGetPropertyIgnoreCase(item, "Identity", out var identityObject) && identityObject.ValueKind == JsonValueKind.Object)
                identity = DeviceIdentityParser.MergeExplicitIdentity(identity, identityObject);

            if (string.IsNullOrWhiteSpace(instanceId) && identity.Fields.Count == 0 && string.IsNullOrWhiteSpace(identity.ModelSegment))
                throw new InvalidDataException($"Devices[{index - 1}] 既没有 InstanceId，也没有可用的 Identity 字段。");

            var props = FlattenProperties(item);
            if (!props.ContainsKey("ClassName") && !string.IsNullOrWhiteSpace(className))
                props["ClassName"] = className;
            if (!props.ContainsKey("ClassDisplayName") && !string.IsNullOrWhiteSpace(classDisplayName))
                props["ClassDisplayName"] = classDisplayName;

            AppendParsedIdentityProperties(props, identity);

            result.Add(new ImportRule
            {
                SourceIndex = index - 1,
                InstanceId = instanceId ?? "",
                DesiredFriendlyName = friendlyName,
                ClassName = className,
                ClassDisplayName = classDisplayName,
                ClassType = ParseClassType(className),
                Manufacturer = GetString(item, "Manufacturer") ?? "",
                DeviceDescription = FirstNonEmpty(GetString(item, "DeviceDescription"), GetString(item, "Description")) ?? "",
                Service = GetString(item, "Service") ?? "",
                HardwareIds = hardwareIds,
                LogicalProcessorNumber = logicalProcessorNumber,
                Identity = identity,
                SourceProperties = props
            });
        }

        return result;
    }

    private static PnpClassType ParseClassType(string className)
        => Enum.TryParse<PnpClassType>(className, true, out var parsed) ? parsed : PnpClassType.Unknown;

    private static Dictionary<string, string> FlattenProperties(JsonElement item)
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var property in item.EnumerateObject())
            Flatten(property.Name, property.Value, dict);
        return dict;
    }

    private static void Flatten(string name, JsonElement value, Dictionary<string, string> dict)
    {
        switch (value.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var p in value.EnumerateObject())
                    Flatten($"{name}.{p.Name}", p.Value, dict);
                break;
            case JsonValueKind.Array:
                var parts = value.EnumerateArray().Select(RenderSimpleValue).ToArray();
                dict[name] = string.Join(Environment.NewLine, parts);
                break;
            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
                dict[name] = "";
                break;
            default:
                dict[name] = RenderSimpleValue(value);
                break;
        }
    }

    private static string RenderSimpleValue(JsonElement value) => value.ValueKind switch
    {
        JsonValueKind.String => value.GetString() ?? "",
        JsonValueKind.True => "true",
        JsonValueKind.False => "false",
        JsonValueKind.Number => value.GetRawText(),
        JsonValueKind.Null => "",
        _ => value.GetRawText()
    };

    private static void AppendParsedIdentityProperties(Dictionary<string, string> dict, DeviceIdentity identity)
    {
        dict["[解析] EnumeratorType"] = identity.EnumeratorType.ToString();
        dict["[解析] Bus"] = identity.RawEnumerator;
        dict["[解析] ModelSegment"] = identity.ModelSegment;
        dict["[解析] InstanceTail"] = identity.InstanceTail;
        if (identity.LogicalProcessorNumber is int lp)
            dict["[解析] LogicalProcessorNumber"] = lp.ToString();
        foreach (var pair in identity.Fields.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
            dict[$"[解析] {pair.Key}"] = pair.Value;
    }

    private static string? GetString(JsonElement obj, string name)
    {
        if (!TryGetPropertyIgnoreCase(obj, name, out var value))
            return null;
        return value.ValueKind == JsonValueKind.String ? value.GetString() : value.ValueKind == JsonValueKind.Null ? null : value.ToString();
    }

    private static int? GetNullableInt(JsonElement obj, string name)
    {
        if (!TryGetPropertyIgnoreCase(obj, name, out var value) || value.ValueKind == JsonValueKind.Null)
            return null;
        if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var number))
            return number;
        return int.TryParse(value.ToString(), out number) ? number : null;
    }

    private static string[] GetStringArray(JsonElement obj, string name)
    {
        if (!TryGetPropertyIgnoreCase(obj, name, out var value) || value.ValueKind != JsonValueKind.Array)
            return [];
        return value.EnumerateArray()
            .Where(x => x.ValueKind == JsonValueKind.String)
            .Select(x => x.GetString() ?? "")
            .Where(x => x.Length > 0)
            .ToArray();
    }

    internal static bool TryGetPropertyIgnoreCase(JsonElement obj, string name, out JsonElement value)
    {
        foreach (var property in obj.EnumerateObject())
        {
            if (string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                value = property.Value;
                return true;
            }
        }
        value = default;
        return false;
    }

    private static string? FirstNonEmpty(params string?[] values)
        => values.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
}

internal static class DeviceIdentityParser
{
    private static readonly Regex StandardFieldRegex = new(
        @"(?:^|[&\\])(?<key>VID|PID|VEN|DEV|SUBSYS|REV|MI|FUNC|CC|PROD|CLASS|SUBCLASS|PROT)_(?<value>[^&\\]+)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex ColRegex = new(
        @"(?:^|&)COL_?(?<value>[0-9A-F]+)(?:&|\\|$)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex BluetoothVidPidRegex = new(
        @"_VID&(?<vid>[^_\\]+)_PID&(?<pid>[^\\]+)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex GuidPrefixRegex = new(
        @"^(?<guid>\{[0-9A-Fa-f-]{36}\})",
        RegexOptions.Compiled);

    public static DeviceIdentity Parse(DeviceEntry device)
        => Parse(device.InstanceId, device.HardwareIds, device.LogicalProcessorNumber);

    public static DeviceIdentity Parse(string instanceId, IEnumerable<string>? hardwareIds, int? logicalProcessorNumber)
    {
        var parts = (instanceId ?? "").Split('\\');
        var rawEnumerator = parts.Length > 0 ? parts[0].Trim() : "";
        var modelSegment = parts.Length > 1 ? parts[1].Trim() : "";
        var instanceTail = parts.Length > 2 ? string.Join("\\", parts.Skip(2)) : "";

        var identity = new DeviceIdentity
        {
            EnumeratorType = ParseEnumerator(rawEnumerator),
            RawEnumerator = rawEnumerator.ToUpperInvariant(),
            ModelSegment = modelSegment.ToUpperInvariant(),
            InstanceTail = instanceTail,
            LogicalProcessorNumber = logicalProcessorNumber
        };

        ExtractFields(identity, instanceId ?? "");
        if (hardwareIds is not null)
        {
            foreach (var hardwareId in hardwareIds)
                ExtractFields(identity, hardwareId ?? "");
        }

        var firstToken = modelSegment.Split('&', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(firstToken) && !firstToken.Contains('_'))
            identity.Fields.TryAdd("KIND", firstToken.ToUpperInvariant());

        var guid = GuidPrefixRegex.Match(modelSegment);
        if (guid.Success)
            identity.Fields.TryAdd("PROFILE", guid.Groups["guid"].Value.ToUpperInvariant());

        return identity;
    }

    public static DeviceIdentity MergeExplicitIdentity(DeviceIdentity parsed, JsonElement identityObject)
    {
        var rawBus = GetString(identityObject, "Bus") ?? GetString(identityObject, "Enumerator") ?? GetString(identityObject, "Type") ?? parsed.RawEnumerator;
        var merged = new DeviceIdentity
        {
            EnumeratorType = ParseEnumerator(rawBus),
            RawEnumerator = rawBus.ToUpperInvariant(),
            ModelSegment = GetString(identityObject, "Model")?.ToUpperInvariant() ?? parsed.ModelSegment,
            InstanceTail = parsed.InstanceTail,
            LogicalProcessorNumber = GetInt(identityObject, "LogicalProcessorNumber") ?? parsed.LogicalProcessorNumber
        };

        foreach (var pair in parsed.Fields)
            merged.Fields[pair.Key] = pair.Value;

        foreach (var property in identityObject.EnumerateObject())
        {
            var key = property.Name.ToUpperInvariant();
            if (key is "BUS" or "ENUMERATOR" or "TYPE" or "MODEL" or "LOGICALPROCESSORNUMBER")
                continue;
            if (property.Value.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
                continue;
            var value = property.Value.ValueKind == JsonValueKind.String ? property.Value.GetString() : property.Value.ToString();
            if (!string.IsNullOrWhiteSpace(value))
                merged.Fields[key] = value!.Trim().ToUpperInvariant();
        }

        return merged;
    }

    private static void ExtractFields(DeviceIdentity identity, string source)
    {
        foreach (Match match in StandardFieldRegex.Matches(source.ToUpperInvariant()))
        {
            var key = match.Groups["key"].Value.ToUpperInvariant();
            var value = match.Groups["value"].Value.Trim().ToUpperInvariant();
            if (value.Length > 0)
                identity.Fields.TryAdd(key, value);
        }

        var col = ColRegex.Match(source.ToUpperInvariant());
        if (col.Success)
            identity.Fields.TryAdd("COL", col.Groups["value"].Value.ToUpperInvariant());

        var bt = BluetoothVidPidRegex.Match(source.ToUpperInvariant());
        if (bt.Success)
        {
            identity.Fields.TryAdd("VID", bt.Groups["vid"].Value.ToUpperInvariant());
            identity.Fields.TryAdd("PID", bt.Groups["pid"].Value.ToUpperInvariant());
        }
    }

    public static PnpEnumeratorType ParseEnumerator(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return PnpEnumeratorType.Unknown;
        var normalized = raw.Trim().ToUpperInvariant();
        if (normalized.StartsWith('{') && normalized.EndsWith('}'))
            return PnpEnumeratorType.CustomGuid;
        return Enum.TryParse<PnpEnumeratorType>(normalized, true, out var parsed) ? parsed : PnpEnumeratorType.Unknown;
    }

    private static string? GetString(JsonElement obj, string name)
        => ImportFileParser.TryGetPropertyIgnoreCase(obj, name, out var value)
            ? value.ValueKind == JsonValueKind.String ? value.GetString() : value.ToString()
            : null;

    private static int? GetInt(JsonElement obj, string name)
    {
        if (!ImportFileParser.TryGetPropertyIgnoreCase(obj, name, out var value) || value.ValueKind == JsonValueKind.Null)
            return null;
        if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var i))
            return i;
        return int.TryParse(value.ToString(), out i) ? i : null;
    }
}

internal static class ImportMatcher
{
    private static readonly string[] OptionalDiscriminators = ["MI", "COL", "SUBSYS", "FUNC"];

    public static ImportPreparation Prepare(
        IReadOnlyList<ImportRule> rules,
        IReadOnlyList<DeviceEntry> devices,
        IProgress<ImportProgressInfo>? progress = null)
    {
        var prep = new ImportPreparation { Rules = rules.ToList() };
        var exactLookup = devices
            .Where(x => !string.IsNullOrWhiteSpace(x.InstanceId))
            .GroupBy(x => x.InstanceId, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.ToList(), StringComparer.OrdinalIgnoreCase);

        var identityCache = devices.ToDictionary(x => x, DeviceIdentityParser.Parse);
        var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var i = 0; i < rules.Count; i++)
        {
            var rule = rules[i];
            progress?.Report(new ImportProgressInfo
            {
                Percent = rules.Count == 0 ? 100 : 15 + (int)(75.0 * i / rules.Count),
                Status = $"匹配设备 {i + 1}/{rules.Count}"
            });

            DeviceEntry? exact = null;
            if (!string.IsNullOrWhiteSpace(rule.InstanceId) && exactLookup.TryGetValue(rule.InstanceId, out var exactCandidates))
            {
                exact = exactCandidates.FirstOrDefault(x => !used.Contains(x.InstanceId) && ClassMatches(rule, x));
            }

            if (exact is not null)
            {
                used.Add(exact.InstanceId);
                prep.Matches.Add(CreateMatch(rule, exact, ImportMatchKind.Exact));
                continue;
            }

            var fuzzy = devices
                .Where(x => !used.Contains(x.InstanceId))
                .Where(x => ClassMatches(rule, x))
                .Where(x => IdentityMatches(rule, rule.Identity, x, identityCache[x]))
                .ToList();

            if (fuzzy.Count > 1)
                fuzzy = NarrowByInformationalFields(rule, fuzzy);

            if (fuzzy.Count == 1)
            {
                var candidate = fuzzy[0];
                used.Add(candidate.InstanceId);
                prep.Matches.Add(CreateMatch(rule, candidate, ImportMatchKind.Fuzzy));
            }
            else
            {
                prep.Unmatched.Add(new UnmatchedImportRule
                {
                    Rule = rule,
                    CandidateCount = fuzzy.Count,
                    Reason = fuzzy.Count == 0 ? "未找到满足必要身份字段的设备" : $"存在 {fuzzy.Count} 个同等候选，未自动选择"
                });
            }
        }

        progress?.Report(new ImportProgressInfo { Percent = 95, Status = "生成差异数据..." });
        return prep;
    }

    private static ImportMatchResult CreateMatch(ImportRule rule, DeviceEntry device, ImportMatchKind kind)
        => new()
        {
            Rule = rule,
            Device = device,
            Kind = kind,
            SourceProperties = rule.SourceProperties,
            CurrentProperties = ImportPropertyBuilder.FromDevice(device)
        };

    private static bool ClassMatches(ImportRule rule, DeviceEntry device)
    {
        if (!string.IsNullOrWhiteSpace(rule.ClassName) &&
            !string.Equals(rule.ClassName.Trim(), device.ClassName?.Trim(), StringComparison.OrdinalIgnoreCase))
            return false;
        if (!string.IsNullOrWhiteSpace(rule.ClassDisplayName) &&
            !string.Equals(rule.ClassDisplayName.Trim(), device.ClassDisplayName?.Trim(), StringComparison.OrdinalIgnoreCase))
            return false;
        return true;
    }

    private static bool IdentityMatches(ImportRule rule, DeviceIdentity source, DeviceEntry device, DeviceIdentity target)
    {
        if (!SameEnumerator(source, target))
            return false;

        if (source.LogicalProcessorNumber is int lp && device.LogicalProcessorNumber != lp)
            return false;

        var hasStrongIdentity = false;

        if (HasBoth(source, "VID", "PID"))
        {
            hasStrongIdentity = true;
            if (!SameField(source, target, "VID") || !SameField(source, target, "PID"))
                return false;
        }
        else if (HasBoth(source, "VEN", "DEV"))
        {
            hasStrongIdentity = true;
            if (!SameField(source, target, "VEN") || !SameField(source, target, "DEV"))
                return false;
        }
        else if (!string.IsNullOrWhiteSpace(source.Get("PROD")))
        {
            hasStrongIdentity = true;
            if (!SameField(source, target, "PROD"))
                return false;
            if (!string.IsNullOrWhiteSpace(source.Get("VEN")) && !SameField(source, target, "VEN"))
                return false;
        }
        else if (!string.IsNullOrWhiteSpace(source.Get("PROFILE")))
        {
            hasStrongIdentity = true;
            if (!SameField(source, target, "PROFILE"))
                return false;
        }
        else if (!string.IsNullOrWhiteSpace(source.ModelSegment))
        {
            hasStrongIdentity = true;
            if (!string.Equals(source.ModelSegment, target.ModelSegment, StringComparison.OrdinalIgnoreCase))
                return false;
        }

        if (!hasStrongIdentity)
            return false;

        if (!string.IsNullOrWhiteSpace(source.Get("KIND")) && !SameField(source, target, "KIND"))
            return false;

        foreach (var key in OptionalDiscriminators)
        {
            var sourceValue = source.Get(key);
            if (!string.IsNullOrWhiteSpace(sourceValue) && !SameField(source, target, key))
                return false;
        }

        return true;
    }

    private static bool SameEnumerator(DeviceIdentity a, DeviceIdentity b)
    {
        if (!string.Equals(a.RawEnumerator, b.RawEnumerator, StringComparison.OrdinalIgnoreCase))
            return false;
        if (a.EnumeratorType == PnpEnumeratorType.CustomGuid || b.EnumeratorType == PnpEnumeratorType.CustomGuid)
            return string.Equals(a.RawEnumerator, b.RawEnumerator, StringComparison.OrdinalIgnoreCase);
        return a.EnumeratorType == b.EnumeratorType || a.EnumeratorType == PnpEnumeratorType.Unknown || b.EnumeratorType == PnpEnumeratorType.Unknown;
    }

    private static bool HasBoth(DeviceIdentity identity, string a, string b)
        => !string.IsNullOrWhiteSpace(identity.Get(a)) && !string.IsNullOrWhiteSpace(identity.Get(b));

    private static bool SameField(DeviceIdentity a, DeviceIdentity b, string key)
        => string.Equals(a.Get(key), b.Get(key), StringComparison.OrdinalIgnoreCase);

    private static List<DeviceEntry> NarrowByInformationalFields(ImportRule rule, List<DeviceEntry> candidates)
    {
        var current = candidates;
        current = Narrow(current, rule.Manufacturer, x => x.Manufacturer);
        if (current.Count == 1) return current;
        current = Narrow(current, rule.DeviceDescription, x => x.Description);
        if (current.Count == 1) return current;
        current = Narrow(current, rule.Service, x => x.Service);
        return current;
    }

    private static List<DeviceEntry> Narrow(List<DeviceEntry> input, string expected, Func<DeviceEntry, string> selector)
    {
        if (input.Count <= 1 || string.IsNullOrWhiteSpace(expected))
            return input;
        var narrowed = input.Where(x => string.Equals(selector(x)?.Trim(), expected.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
        return narrowed.Count > 0 ? narrowed : input;
    }
}

internal static class ImportPropertyBuilder
{
    public static Dictionary<string, string> FromDevice(DeviceEntry d)
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["InstanceId"] = d.InstanceId,
            ["CustomFriendlyName"] = d.FriendlyName,
            ["DisplayNameAtExport"] = d.DisplayName,
            ["DeviceDescription"] = d.Description,
            ["ClassName"] = d.ClassName,
            ["ClassDisplayName"] = d.ClassDisplayName,
            ["ClassGuid"] = d.ClassGuid,
            ["Manufacturer"] = d.Manufacturer,
            ["Service"] = d.Service,
            ["Enumerator"] = d.Enumerator,
            ["Driver"] = d.Driver,
            ["LocationInformation"] = d.LocationInformation,
            ["PhysicalDeviceObjectName"] = d.PhysicalDeviceObjectName,
            ["LogicalProcessorNumber"] = d.LogicalProcessorNumber?.ToString() ?? "",
            ["ProcessorTopology"] = d.ProcessorTopology,
            ["HardwareIds"] = string.Join(Environment.NewLine, d.HardwareIds ?? []),
            ["CompatibleIds"] = string.Join(Environment.NewLine, d.CompatibleIds ?? []),
            ["LocationPaths"] = string.Join(Environment.NewLine, d.LocationPaths ?? [])
        };

        var identity = DeviceIdentityParser.Parse(d);
        dict["[解析] EnumeratorType"] = identity.EnumeratorType.ToString();
        dict["[解析] Bus"] = identity.RawEnumerator;
        dict["[解析] ModelSegment"] = identity.ModelSegment;
        dict["[解析] InstanceTail"] = identity.InstanceTail;
        if (identity.LogicalProcessorNumber is int lp)
            dict["[解析] LogicalProcessorNumber"] = lp.ToString();
        foreach (var pair in identity.Fields.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
            dict[$"[解析] {pair.Key}"] = pair.Value;
        return dict;
    }
}
