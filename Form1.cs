using System.Diagnostics;
using System.Security.Principal;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace PnpFriendlyNameEditor;

public partial class Form1 : Form
{
    private DeviceEntry? _selected;
    private bool _updatingChecks;

    public Form1()
    {
        InitializeComponent();

        var adminText = IsAdministrator() ? "管理员" : "非管理员：保存可能失败";
        Text = $"PnP FriendlyName Editor - {adminText}";

        Load += Form1_Load;
        Shown += (_, _) => ApplySplitRatio();
        splitMain.SizeChanged += (_, _) => ApplySplitRatio();
    }

    private void Form1_Load(object? sender, EventArgs e) => LoadDevices();

    private void btnRefresh_Click(object? sender, EventArgs e) => LoadDevices(_selected?.InstanceId);

    private void chkShowNonPresent_CheckedChanged(object? sender, EventArgs e) => LoadDevices(_selected?.InstanceId);

    private void btnCopyInstanceId_Click(object? sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(txtInstanceId.Text))
        {
            Clipboard.SetText(txtInstanceId.Text);
        }
    }

    private void btnSave_Click(object? sender, EventArgs e) => SaveFriendlyName();

    private void btnExportCustom_Click(object? sender, EventArgs e) => ExportCheckedCustomInfo();

    private void treeDevices_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        _selected = e.Node?.Tag as DeviceEntry;
        ShowDevice(_selected);
    }

    private void treeDevices_AfterCheck(object? sender, TreeViewEventArgs e)
    {
        if (_updatingChecks || e.Node == null)
            return;

        try
        {
            _updatingChecks = true;

            // 分类节点勾选/取消时，批量影响子设备。
            if (e.Node.Tag is null)
                foreach (TreeNode child in e.Node.Nodes)
                    child.Checked = e.Node.Checked;
        }
        finally
        {
            _updatingChecks = false;
        }

        UpdateExportButtonState();
    }

    private void ApplySplitRatio()
    {
        if (!splitMain.IsHandleCreated)
            return;

        var width = splitMain.ClientSize.Width;
        if (width <= 0)
            return;

        const int desiredPanel1Min = 280;
        const int desiredPanel2Min = 520;

        if (width > desiredPanel1Min + desiredPanel2Min + splitMain.SplitterWidth + 20)
        {
            splitMain.Panel1MinSize = desiredPanel1Min;
            splitMain.Panel2MinSize = desiredPanel2Min;
        }
        else
        {
            splitMain.Panel1MinSize = 80;
            splitMain.Panel2MinSize = 120;
        }

        var min = splitMain.Panel1MinSize;
        var max = width - splitMain.SplitterWidth - splitMain.Panel2MinSize;

        if (max <= min)
            return;

        var desired = (int)(width * 0.30);
        var safeDistance = Math.Clamp(desired, min, max);

        if (splitMain.SplitterDistance != safeDistance)
            splitMain.SplitterDistance = safeDistance;
    }

    private void LoadDevices(string? reselectInstanceId = null)
    {
        var checkedIds = GetCheckedDevices()
            .Select(d => d.InstanceId)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        Cursor = Cursors.WaitCursor;

        try
        {
            _updatingChecks = true;
            treeDevices.BeginUpdate();
            treeDevices.Nodes.Clear();

            var presentOnly = !chkShowNonPresent.Checked;
            var devices = PnpDeviceService.EnumerateDevices(presentOnly);

            var groups = devices
                .OrderBy(d => d.ClassDisplayName)
                // ACPI 处理器实例尾号是十六进制逻辑处理器号（0..B），不能按显示文本排序，
                // 否则 10、11 会排在 2 前面。
                .ThenBy(d => d.LogicalProcessorNumber ?? int.MaxValue)
                .ThenBy(d => d.DisplayName)
                .GroupBy(d => string.IsNullOrWhiteSpace(d.ClassDisplayName) ? "未分类" : d.ClassDisplayName);

            TreeNode? nodeToSelect = null;

            foreach (var group in groups)
            {
                var classNode = new TreeNode($"{group.Key} ({group.Count()})")
                {
                    Tag = null
                };

                foreach (var device in group)
                {
                    var node = new TreeNode(device.DisplayName)
                    {
                        Tag = device,
                        Checked = checkedIds.Contains(device.InstanceId)
                    };

                    classNode.Nodes.Add(node);

                    if (!string.IsNullOrWhiteSpace(reselectInstanceId) &&
                        string.Equals(device.InstanceId, reselectInstanceId, StringComparison.OrdinalIgnoreCase))
                    {
                        nodeToSelect = node;
                    }

                    treeDevices.Nodes.Add(classNode);
                }

                if (nodeToSelect != null)
                {
                    nodeToSelect.EnsureVisible();
                    treeDevices.SelectedNode = nodeToSelect;
                }
                else
                    ShowDevice(null);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.ToString(), "枚举设备失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            treeDevices.EndUpdate();
            _updatingChecks = false;
            Cursor = Cursors.Default;
            UpdateExportButtonState();
        }
    }

    private void ShowDevice(DeviceEntry? d)
    {
        _selected = d;

        txtDisplayName.Text = d?.DisplayName ?? "";
        txtFriendlyName.Text = d?.FriendlyName ?? "";
        txtDescription.Text = d?.Description ?? "";
        txtInstanceId.Text = d?.InstanceId ?? "";
        txtClass.Text = d == null ? "" : $"{d.ClassDisplayName} / {d.ClassName} / {d.ClassGuid}";
        txtManufacturer.Text = d?.Manufacturer ?? "";
        txtService.Text = d?.Service ?? "";
        txtEnumerator.Text = d?.Enumerator ?? "";
        txtDriver.Text = d?.Driver ?? "";

        btnSave.Enabled = d != null;
        btnCopyInstanceId.Enabled = d != null;
    }

    private void SaveFriendlyName()
    {
        if (_selected == null)
            return;

        var newName = txtFriendlyName.Text.Trim();

        if (string.IsNullOrWhiteSpace(newName))
        {
            MessageBox.Show(this, "FriendlyName 不建议留空。你可以改成一个你喜欢的名称。", "未保存",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var confirm = MessageBox.Show(
            this,
            $"只会尝试修改此设备的 FriendlyName：\r\n\r\n{_selected.InstanceId}\r\n\r\n新名称：{newName}\r\n\r\n继续？",
            "确认修改",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes)
            return;

        try
        {
            AppendFriendlyNameBackup(_selected, newName);
            PnpDeviceService.SetFriendlyName(_selected.InstanceId, newName);

            MessageBox.Show(
                this,
                "已写入 FriendlyName。若设备管理器未立即刷新，重新扫描硬件、重新插拔设备或重启后再看。",
                "保存成功",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            LoadDevices(_selected.InstanceId);
        }
        catch (System.ComponentModel.Win32Exception ex)
        {
            MessageBox.Show(
                this,
                $"保存失败。\r\n\r\nWin32 错误 {ex.NativeErrorCode}: {ex.Message}\r\n\r\n建议：用管理员身份运行；如果仍失败，该设备项可能被系统权限保护或被驱动覆盖。",
                "保存失败",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.ToString(), "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ExportCheckedCustomInfo()
    {
        var devices = GetCheckedDevices().ToList();

        // 按你的“选中多个框之后 Enable”字面逻辑：至少 2 个设备才允许导出。
        // 如果你后面想单选也能导出，把这里和 UpdateExportButtonState() 里的 >= 2 改成 >= 1。
        if (devices.Count < 2)
        {
            MessageBox.Show(this, "至少勾选 2 个设备后再导出。", "未导出",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var export = new CustomDeviceExport
        {
            Schema = "PnpFriendlyNameEditor.CustomDeviceInfo.v1",
            ExportedAt = DateTimeOffset.Now,
            MachineName = Environment.MachineName,
            UserName = Environment.UserName,
            Note = "用于备份设备 FriendlyName 等自定义显示信息。后续导入时应优先用 InstanceId 匹配，失败后再尝试 HardwareIds / CompatibleIds / ClassGuid / Service / Enumerator 等弱匹配。",
            Devices = devices.Select(DeviceCustomInfo.FromDeviceEntry).ToList()
        };

        using var dialog = new SaveFileDialog
        {
            Title = "导出自定义设备信息",
            Filter = "JSON 文件 (*.json)|*.json|所有文件 (*.*)|*.*",
            FileName = $"device-custom-info-{DateTime.Now:yyyyMMdd-HHmmss}.json",
            AddExtension = true,
            DefaultExt = "json",
            OverwritePrompt = true
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        File.WriteAllText(dialog.FileName, JsonSerializer.Serialize(export, options), new UTF8Encoding(false));

        MessageBox.Show(
            this,
            $"已导出 {devices.Count} 个设备的自定义信息：\r\n\r\n{dialog.FileName}",
            "导出完成",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private IEnumerable<DeviceEntry> GetCheckedDevices()
    {
        foreach (TreeNode root in treeDevices.Nodes)
            foreach (TreeNode child in root.Nodes)
                if (child.Checked && child.Tag is DeviceEntry device)
                    yield return device;
    }

    private void UpdateExportButtonState()
    {
        var count = GetCheckedDevices().Count();
        lblCheckedCount.Text = $"已选择: {count}";

        // 按“多个框”处理：2 个及以上才 Enable。
        btnExportCustom.Enabled = count >= 2;
    }

    private static void AppendFriendlyNameBackup(DeviceEntry oldDevice, string newName)
    {
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PnpFriendlyNameEditor");

        Directory.CreateDirectory(dir);

        var file = Path.Combine(dir, "friendly-name-backup.jsonl");

        var record = new
        {
            Time = DateTimeOffset.Now,
            oldDevice.InstanceId,
            oldDevice.DisplayName,
            oldDevice.FriendlyName,
            oldDevice.Description,
            NewFriendlyName = newName
        };

        File.AppendAllText(file, JsonSerializer.Serialize(record) + Environment.NewLine, Encoding.UTF8);
    }

    private static bool IsAdministrator()
    {
        using var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => Process.Start("explorer.exe", linkLabel1.Text).Dispose();

    private void linkLabel1_MouseDoubleClick(object sender, MouseEventArgs e) => linkLabel1_LinkClicked(sender, new LinkLabelLinkClickedEventArgs(linkLabel1.Links[0]));
}
