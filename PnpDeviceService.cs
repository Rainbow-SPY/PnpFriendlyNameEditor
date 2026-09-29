using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace PnpFriendlyNameEditor;

public sealed class DeviceEntry
{
    public string InstanceId { get; init; } = "";
    public string DisplayName { get; init; } = "";
    public string FriendlyName { get; init; } = "";
    public string Description { get; init; } = "";
    public string ClassName { get; init; } = "";
    public string ClassDisplayName { get; init; } = "";
    public string ClassGuid { get; init; } = "";
    public string Manufacturer { get; init; } = "";
    public string Service { get; init; } = "";
    public string Enumerator { get; init; } = "";
    public string Driver { get; init; } = "";
    public string LocationInformation { get; init; } = "";
    public string PhysicalDeviceObjectName { get; init; } = "";
    public int? LogicalProcessorNumber { get; init; }
    public string ProcessorTopology { get; init; } = "";

    public string[] HardwareIds { get; init; } = [];
    public string[] CompatibleIds { get; init; } = [];
    public string[] LocationPaths { get; init; } = [];
}

public sealed class CustomDeviceExport
{
    public string Schema { get; init; } = "";
    public DateTimeOffset ExportedAt { get; init; }
    public string MachineName { get; init; } = "";
    public string UserName { get; init; } = "";
    public string Note { get; init; } = "";
    public List<DeviceCustomInfo> Devices { get; init; } = [];
}

public sealed class DeviceCustomInfo
{
    public string InstanceId { get; init; } = "";
    public string CustomFriendlyName { get; init; } = "";
    public string DisplayNameAtExport { get; init; } = "";
    public string DeviceDescription { get; init; } = "";
    public string ClassName { get; init; } = "";
    public string ClassDisplayName { get; init; } = "";
    public string ClassGuid { get; init; } = "";
    public string Manufacturer { get; init; } = "";
    public string Service { get; init; } = "";
    public string Enumerator { get; init; } = "";
    public string Driver { get; init; } = "";
    public string LocationInformation { get; init; } = "";
    public string PhysicalDeviceObjectName { get; init; } = "";
    public int? LogicalProcessorNumber { get; init; }
    public string ProcessorTopology { get; init; } = "";
    public string[] HardwareIds { get; init; } = [];
    public string[] CompatibleIds { get; init; } = [];
    public string[] LocationPaths { get; init; } = [];

    public static DeviceCustomInfo FromDeviceEntry(DeviceEntry d) => new()
    {
        InstanceId = d.InstanceId,
        CustomFriendlyName = d.FriendlyName,
        DisplayNameAtExport = d.DisplayName,
        DeviceDescription = d.Description,
        ClassName = d.ClassName,
        ClassDisplayName = d.ClassDisplayName,
        ClassGuid = d.ClassGuid,
        Manufacturer = d.Manufacturer,
        Service = d.Service,
        Enumerator = d.Enumerator,
        Driver = d.Driver,
        LocationInformation = d.LocationInformation,
        PhysicalDeviceObjectName = d.PhysicalDeviceObjectName,
        LogicalProcessorNumber = d.LogicalProcessorNumber,
        ProcessorTopology = d.ProcessorTopology,
        HardwareIds = d.HardwareIds,
        CompatibleIds = d.CompatibleIds,
        LocationPaths = d.LocationPaths
    };
}

internal static class PnpDeviceService
{
    private const uint DIGCF_PRESENT = 0x00000002;
    private const uint DIGCF_ALLCLASSES = 0x00000004;

    private const uint SPDRP_DEVICEDESC = 0x00000000;
    private const uint SPDRP_HARDWAREID = 0x00000001;
    private const uint SPDRP_COMPATIBLEIDS = 0x00000002;
    private const uint SPDRP_SERVICE = 0x00000004;
    private const uint SPDRP_CLASS = 0x00000007;
    private const uint SPDRP_CLASSGUID = 0x00000008;
    private const uint SPDRP_DRIVER = 0x00000009;
    private const uint SPDRP_MFG = 0x0000000B;
    private const uint SPDRP_FRIENDLYNAME = 0x0000000C;
    private const uint SPDRP_LOCATION_INFORMATION = 0x0000000D;
    private const uint SPDRP_PHYSICAL_DEVICE_OBJECT_NAME = 0x0000000E;
    private const uint SPDRP_ENUMERATOR_NAME = 0x00000016;
    private const uint SPDRP_LOCATION_PATHS = 0x00000023;

    private const int ERROR_NO_MORE_ITEMS = 259;
    private const int ERROR_INSUFFICIENT_BUFFER = 122;
    private const int RelationProcessorCore = 0;
    private static readonly IntPtr INVALID_HANDLE_VALUE = new(-1);

    [StructLayout(LayoutKind.Sequential)]
    private struct SP_DEVINFO_DATA
    {
        public int cbSize;
        public Guid ClassGuid;
        public uint DevInst;
        public IntPtr Reserved;
    }

    [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr SetupDiGetClassDevs(
        IntPtr ClassGuid,
        string? Enumerator,
        IntPtr hwndParent,
        uint Flags);

    [DllImport("setupapi.dll", SetLastError = true)]
    private static extern bool SetupDiEnumDeviceInfo(
        IntPtr DeviceInfoSet,
        uint MemberIndex,
        ref SP_DEVINFO_DATA DeviceInfoData);

    [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool SetupDiGetDeviceInstanceId(
        IntPtr DeviceInfoSet,
        ref SP_DEVINFO_DATA DeviceInfoData,
        StringBuilder DeviceInstanceId,
        uint DeviceInstanceIdSize,
        out uint RequiredSize);

    [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool SetupDiOpenDeviceInfo(
        IntPtr DeviceInfoSet,
        string DeviceInstanceId,
        IntPtr hwndParent,
        uint OpenFlags,
        ref SP_DEVINFO_DATA DeviceInfoData);

    [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool SetupDiGetDeviceRegistryProperty(
        IntPtr DeviceInfoSet,
        ref SP_DEVINFO_DATA DeviceInfoData,
        uint Property,
        out uint PropertyRegDataType,
        byte[]? PropertyBuffer,
        uint PropertyBufferSize,
        out uint RequiredSize);

    [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool SetupDiSetDeviceRegistryProperty(
        IntPtr DeviceInfoSet,
        ref SP_DEVINFO_DATA DeviceInfoData,
        uint Property,
        byte[] PropertyBuffer,
        uint PropertyBufferSize);

    [DllImport("setupapi.dll", SetLastError = true)]
    private static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetLogicalProcessorInformationEx(
        int RelationshipType,
        IntPtr Buffer,
        ref int ReturnedLength);

    [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool SetupDiClassNameFromGuid(
        ref Guid ClassGuid,
        StringBuilder ClassName,
        int ClassNameSize,
        out int RequiredSize);

    [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool SetupDiGetClassDescription(
        ref Guid ClassGuid,
        StringBuilder ClassDescription,
        int ClassDescriptionSize,
        out int RequiredSize);

    public static List<DeviceEntry> EnumerateDevices(bool presentOnly)
    {
        // 只读系统拓扑，不修改 CPU/PnP 配置。实例尾号 0..B 会用十六进制解析。
        var processorTopology = ReadLogicalProcessorTopology();
        var flags = DIGCF_ALLCLASSES;
        if (presentOnly)
            flags |= DIGCF_PRESENT;

        var h = SetupDiGetClassDevs(IntPtr.Zero, null, IntPtr.Zero, flags);
        if (h == INVALID_HANDLE_VALUE)
            ThrowLastWin32();

        try
        {
            var result = new List<DeviceEntry>();

            for (uint i = 0; ; i++)
            {
                var data = NewDevInfoData();

                if (!SetupDiEnumDeviceInfo(h, i, ref data))
                {
                    var err = Marshal.GetLastWin32Error();
                    if (err == ERROR_NO_MORE_ITEMS)
                        break;

                    throw new Win32Exception(err);
                }

                var instanceId = GetInstanceId(h, ref data);

                var friendly = GetStringProperty(h, ref data, SPDRP_FRIENDLYNAME);
                var desc = GetStringProperty(h, ref data, SPDRP_DEVICEDESC);
                var className = GetStringProperty(h, ref data, SPDRP_CLASS);
                var classGuid = GetStringProperty(h, ref data, SPDRP_CLASSGUID);
                var manufacturer = GetStringProperty(h, ref data, SPDRP_MFG);
                var service = GetStringProperty(h, ref data, SPDRP_SERVICE);
                var enumerator = GetStringProperty(h, ref data, SPDRP_ENUMERATOR_NAME);
                var driver = GetStringProperty(h, ref data, SPDRP_DRIVER);
                var locationInfo = GetStringProperty(h, ref data, SPDRP_LOCATION_INFORMATION);
                var physicalDeviceObjectName = GetStringProperty(h, ref data, SPDRP_PHYSICAL_DEVICE_OBJECT_NAME);

                var hardwareIds = GetMultiStringProperty(h, ref data, SPDRP_HARDWAREID);
                var compatibleIds = GetMultiStringProperty(h, ref data, SPDRP_COMPATIBLEIDS);
                var locationPaths = GetMultiStringProperty(h, ref data, SPDRP_LOCATION_PATHS);

                var classApiName = GetClassApiName(data.ClassGuid);
                var classDesc = GetClassApiDescription(data.ClassGuid);

                if (string.IsNullOrWhiteSpace(className))
                    className = classApiName;

                if (string.IsNullOrWhiteSpace(classGuid))
                    classGuid = data.ClassGuid.ToString("B").ToUpperInvariant();

                if (string.IsNullOrWhiteSpace(classDesc))
                    classDesc = className;

                var display = !string.IsNullOrWhiteSpace(friendly)
                    ? friendly
                    : !string.IsNullOrWhiteSpace(desc)
                        ? desc
                        : instanceId;

                var logicalProcessorNumber = TryGetAcpiLogicalProcessorNumber(instanceId, out var number)
                    ? number
                    : (int?)null;

                var topologyText = "";
                if (logicalProcessorNumber is int logicalProcessor &&
                    processorTopology.TryGetValue(logicalProcessor, out var topology))
                {
                    topologyText = topology.ToDisplayText();
                    display = $"{display} — {topologyText}";
                }

                result.Add(new DeviceEntry
                {
                    InstanceId = instanceId,
                    DisplayName = display,
                    FriendlyName = friendly,
                    Description = desc,
                    ClassName = className,
                    ClassDisplayName = classDesc,
                    ClassGuid = classGuid,
                    Manufacturer = manufacturer,
                    Service = service,
                    Enumerator = enumerator,
                    Driver = driver,
                    LocationInformation = locationInfo,
                    PhysicalDeviceObjectName = physicalDeviceObjectName,
                    LogicalProcessorNumber = logicalProcessorNumber,
                    ProcessorTopology = topologyText,
                    HardwareIds = hardwareIds,
                    CompatibleIds = compatibleIds,
                    LocationPaths = locationPaths
                });
            }

            return result;
        }
        finally
        {
            SetupDiDestroyDeviceInfoList(h);
        }
    }

    public static void SetFriendlyName(string instanceId, string friendlyName)
    {
        var h = SetupDiGetClassDevs(IntPtr.Zero, null, IntPtr.Zero, DIGCF_ALLCLASSES);
        if (h == INVALID_HANDLE_VALUE)
            ThrowLastWin32();

        try
        {
            var data = NewDevInfoData();

            if (!SetupDiOpenDeviceInfo(h, instanceId, IntPtr.Zero, 0, ref data))
                ThrowLastWin32();

            var bytes = Encoding.Unicode.GetBytes(friendlyName + "\0");

            if (!SetupDiSetDeviceRegistryProperty(
                    h,
                    ref data,
                    SPDRP_FRIENDLYNAME,
                    bytes,
                    (uint)bytes.Length))
            {
                ThrowLastWin32();
            }
        }
        finally
        {
            SetupDiDestroyDeviceInfoList(h);
        }
    }

    private sealed record LogicalProcessorTopology(
        int LogicalProcessorNumber,
        int PhysicalCoreNumber,
        int SmtThreadNumber,
        int LogicalProcessorsOnCore,
        byte EfficiencyClass,
        bool IsHeterogeneous,
        bool IsHighestEfficiencyClass,
        bool IsLowestEfficiencyClass)
    {
        public string ToDisplayText()
        {
            var coreType = IsHeterogeneous
                ? IsHighestEfficiencyClass
                    ? "性能核心"
                    : IsLowestEfficiencyClass
                        ? "能效核心"
                        : $"效率等级 {EfficiencyClass} 核心"
                : "物理核心";

            return LogicalProcessorsOnCore > 1
                ? $"逻辑处理器 {LogicalProcessorNumber}（{coreType} {PhysicalCoreNumber}，SMT 线程 {SmtThreadNumber}/{LogicalProcessorsOnCore}）"
                : $"逻辑处理器 {LogicalProcessorNumber}（{coreType} {PhysicalCoreNumber}）";
        }
    }

    private sealed record PhysicalCoreTopology(
        int PhysicalCoreNumber,
        byte EfficiencyClass,
        List<int> LogicalProcessorNumbers);

    private static Dictionary<int, LogicalProcessorTopology> ReadLogicalProcessorTopology()
    {
        var length = 0;
        if (GetLogicalProcessorInformationEx(RelationProcessorCore, IntPtr.Zero, ref length) ||
            Marshal.GetLastWin32Error() != ERROR_INSUFFICIENT_BUFFER ||
            length <= 0)
        {
            return [];
        }

        var buffer = Marshal.AllocHGlobal(length);
        try
        {
            if (!GetLogicalProcessorInformationEx(RelationProcessorCore, buffer, ref length))
                return [];

            var cores = new List<PhysicalCoreTopology>();
            var offset = 0;
            var physicalCoreNumber = 0;

            while (offset < length)
            {
                var entry = IntPtr.Add(buffer, offset);
                var relationship = Marshal.ReadInt32(entry, 0);
                var entrySize = Marshal.ReadInt32(entry, sizeof(int));

                if (entrySize <= 0 || offset + entrySize > length)
                    break;

                if (relationship == RelationProcessorCore)
                {
                    // SYSTEM_LOGICAL_PROCESSOR_INFORMATION_EX 的 union 从 offset 8 开始；
                    // PROCESSOR_RELATIONSHIP 的 GroupMask 从该位置起第 24 字节开始。
                    var efficiencyClass = Marshal.ReadByte(entry, 9);
                    var groupCount = (ushort)Marshal.ReadInt16(entry, 30);
                    var logicalNumbers = new List<int>();
                    var groupAffinityOffset = 32;
                    var groupAffinitySize = IntPtr.Size == 8 ? 16 : 12;

                    for (var i = 0; i < groupCount; i++)
                    {
                        var affinity = IntPtr.Add(entry, groupAffinityOffset + i * groupAffinitySize);
                        var mask = IntPtr.Size == 8
                            ? unchecked((ulong)Marshal.ReadInt64(affinity))
                            : unchecked((uint)Marshal.ReadInt32(affinity));
                        var group = (ushort)Marshal.ReadInt16(affinity, IntPtr.Size);

                        for (var bit = 0; bit < IntPtr.Size * 8; bit++)
                        {
                            if ((mask & (1UL << bit)) != 0)
                                logicalNumbers.Add(group * 64 + bit);
                        }
                    }

                    if (logicalNumbers.Count > 0)
                    {
                        cores.Add(new PhysicalCoreTopology(
                            physicalCoreNumber++,
                            efficiencyClass,
                            logicalNumbers));
                    }
                }

                offset += entrySize;
            }

            var distinctEfficiencyClasses = cores.Select(c => c.EfficiencyClass).Distinct().ToArray();
            var isHeterogeneous = distinctEfficiencyClasses.Length > 1;
            var highestEfficiencyClass = distinctEfficiencyClasses.DefaultIfEmpty().Max();
            var lowestEfficiencyClass = distinctEfficiencyClasses.DefaultIfEmpty().Min();
            var result = new Dictionary<int, LogicalProcessorTopology>();

            foreach (var core in cores)
            {
                for (var i = 0; i < core.LogicalProcessorNumbers.Count; i++)
                {
                    var logicalNumber = core.LogicalProcessorNumbers[i];
                    result[logicalNumber] = new LogicalProcessorTopology(
                        logicalNumber,
                        core.PhysicalCoreNumber,
                        i + 1,
                        core.LogicalProcessorNumbers.Count,
                        core.EfficiencyClass,
                        isHeterogeneous,
                        core.EfficiencyClass == highestEfficiencyClass,
                        core.EfficiencyClass == lowestEfficiencyClass);
                }
            }

            return result;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    private static bool TryGetAcpiLogicalProcessorNumber(string instanceId, out int logicalProcessorNumber)
    {
        logicalProcessorNumber = -1;
        const string prefix = "ACPI\\GENUINEINTEL_";

        if (!instanceId.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            return false;

        var separator = instanceId.LastIndexOf('\\');
        if (separator < 0 || separator == instanceId.Length - 1)
            return false;

        // ACPI 处理器实例号采用十六进制：A=10，B=11。
        return int.TryParse(
            instanceId[(separator + 1)..],
            NumberStyles.AllowHexSpecifier,
            CultureInfo.InvariantCulture,
            out logicalProcessorNumber);
    }

    private static SP_DEVINFO_DATA NewDevInfoData() => new()
    {
        cbSize = Marshal.SizeOf<SP_DEVINFO_DATA>()
    };

    private static string GetInstanceId(IntPtr h, ref SP_DEVINFO_DATA data)
    {
        var sb = new StringBuilder(4096);

        if (!SetupDiGetDeviceInstanceId(h, ref data, sb, (uint)sb.Capacity, out _))
            ThrowLastWin32();

        return sb.ToString();
    }

    private static string GetStringProperty(IntPtr h, ref SP_DEVINFO_DATA data, uint property)
    {
        var strings = GetMultiStringProperty(h, ref data, property);
        return strings.Length == 0 ? "" : strings[0];
    }

    private static string[] GetMultiStringProperty(IntPtr h, ref SP_DEVINFO_DATA data, uint property)
    {
        if (!SetupDiGetDeviceRegistryProperty(h, ref data, property, out _, null, 0, out var required))
            if (required == 0)
                return [];

        if (required == 0)
            return [];

        var buffer = new byte[required];

        if (!SetupDiGetDeviceRegistryProperty(h, ref data, property, out _, buffer, (uint)buffer.Length, out _))
            return [];

        return DecodeRegStringOrMultiString(buffer);
    }

    private static string[] DecodeRegStringOrMultiString(byte[] buffer)
    {
        if (string.IsNullOrWhiteSpace(Encoding.Unicode.GetString(buffer).TrimEnd('\0')))
            return [];

        return [.. Encoding.Unicode.GetString(buffer).TrimEnd('\0')
            .Split('\0', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(x => !string.IsNullOrWhiteSpace(x))];
    }

    private static string GetClassApiName(Guid guid)
    {
        var sb = new StringBuilder(256);

        return SetupDiClassNameFromGuid(ref guid, sb, sb.Capacity, out _)
            ? sb.ToString()
            : "";
    }

    private static string GetClassApiDescription(Guid guid)
    {
        var sb = new StringBuilder(512);

        return SetupDiGetClassDescription(ref guid, sb, sb.Capacity, out _)
            ? sb.ToString()
            : "";
    }

    private static void ThrowLastWin32() => throw new Win32Exception(Marshal.GetLastWin32Error());
}
