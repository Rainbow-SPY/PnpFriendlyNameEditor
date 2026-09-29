using System.Diagnostics;
using System.Security.Principal;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using static Rox.Runtimes.LogLibraries;

namespace PnpFriendlyNameEditor;

public partial class Form1 : Form
{
    private DeviceEntry? _selected;
    private bool _updatingChecks;
    private Button? _btnImportCustom;
    private ProgressBar? _importProgress;
    private Label? _importStatus;
    private Dictionary<Control, bool>? _importEnabledState;

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        EnsureImportUi();
    }

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
            Clipboard.SetText(txtInstanceId.Text);
    }


    private void EnsureImportUi()
    {
        if (_btnImportCustom is not null)
            return;

        _btnImportCustom = new Button
        {
            AutoSize = true,
            Text = "导入 / 匹配配置",
            Name = "btnImportCustom",
            Margin = new Padding(6, 3, 3, 3)
        };
        _btnImportCustom.Click += async (_, _) => await ImportCustomInfoAsync();

        _importProgress = new ProgressBar
        {
            Width = 135,
            Height = 22,
            Minimum = 0,
            Maximum = 100,
            Value = 0,
            Style = ProgressBarStyle.Continuous,
            Margin = new Padding(10, 4, 3, 3),
            Visible = false
        };

        _importStatus = new Label
        {
            AutoSize = true,
            Text = "",
            Margin = new Padding(8, 7, 3, 0),
            Visible = false
        };

        topPanel.Controls.Add(_btnImportCustom);
        topPanel.Controls.Add(_importProgress);
        topPanel.Controls.Add(_importStatus);

        var importIndex = Math.Min(5, topPanel.Controls.Count - 1);
        topPanel.Controls.SetChildIndex(_btnImportCustom, importIndex);
        topPanel.Controls.SetChildIndex(_importProgress, Math.Min(importIndex + 2, topPanel.Controls.Count - 1));
        topPanel.Controls.SetChildIndex(_importStatus, Math.Min(importIndex + 3, topPanel.Controls.Count - 1));
    }

    private async Task ImportCustomInfoAsync()
    {
        using var dialog = new OpenFileDialog();
        dialog.Title = "导入 FriendlyName 配置 / 备份";
        dialog.Filter = "JSON 文件 (*.json)|*.json|所有文件 (*.*)|*.*";
        dialog.CheckFileExists = true;
        dialog.Multiselect = false;

        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;

        List<ImportRule> rules;
        try
        {
            rules = ImportFileParser.LoadRules(dialog.FileName);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"无法读取配置文件：\r\n\r\n{ex.Message}", "导入失败",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (rules.Count == 0)
        {
            MessageBox.Show(this, "配置文件中的 Devices 为空，没有可导入的设备。", "没有设备",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var start = MessageBox.Show(
            this,
            $"正在准备导入 {rules.Count} 个设备。\r\n\r\n单击“确定”后开始枚举并匹配本机全部 PnP 设备（包括非当前连接设备）。",
            "准备导入",
            MessageBoxButtons.OKCancel,
            MessageBoxIcon.Information);
        if (start != DialogResult.OK)
            return;

        ImportPreparation preparation;
        IProgress<ImportProgressInfo> progress = new Progress<ImportProgressInfo>(UpdateImportProgress);
        SetImportBusy(true, "正在枚举设备...", 5);
        try
        {
            preparation = await Task.Run(() => PnpBatchService.PrepareImport(rules, progress));
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.ToString(), "匹配失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        finally
        {
            SetImportBusy(false);
        }

        var summary =
            $"匹配完成。\r\n\r\n" +
            $"完全匹配：{preparation.ExactCount}\r\n" +
            $"模糊匹配：{preparation.FuzzyCount}\r\n" +
            $"未匹配：{preparation.UnmatchedCount}" +
            (preparation.AmbiguousCount > 0 ? $"（其中 {preparation.AmbiguousCount} 项存在多个候选）" : "") +
            $"\r\n\r\n已匹配到 {preparation.MatchedCount} 个设备。是否继续进入差异审阅？";

        var continueResult = MessageBox.Show(this, summary, "匹配结果",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (continueResult != DialogResult.Yes)
            return;

        if (preparation.MatchedCount == 0)
        {
            MessageBox.Show(this, "没有任何设备可进入差异审阅。", "没有匹配",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        IReadOnlyList<ImportMatchResult> selected;
        try
        {
            using var review = new ImportDiffForm(preparation.Matches);
            if (review.ShowDialog(this) != DialogResult.OK)
                return;

            selected = review.GetSelectedMatches();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this,
                $"无法打开导入差异审阅窗口。\r\n\r\n{ex.Message}",
                "导入审阅失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (selected.Count == 0)
        {
            MessageBox.Show(this, "没有勾选任何设备，不会写入 FriendlyName。", "未应用",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var finalConfirm = MessageBox.Show(
            this,
            $"即将覆盖 {selected.Count} 个设备的 FriendlyName。\r\n\r\n继续？",
            "确认应用",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);
        if (finalConfirm != DialogResult.Yes)
            return;

        await ApplyImportedNamesAsync(selected);
    }

    private async Task ApplyImportedNamesAsync(IReadOnlyList<ImportMatchResult> matches)
    {
        IProgress<ImportProgressInfo> progress = new Progress<ImportProgressInfo>(UpdateImportProgress);
        FriendlyNameApplyResult result;
        SetImportBusy(true, "正在写入 FriendlyName...");
        try
        {
            result = await Task.Run(() => PnpBatchService.ApplyImportedNames(matches, progress));
        }
        finally
        {
            SetImportBusy(false);
        }

        if (result.Succeeded)
            MessageBox.Show(this,
                $"已成功应用 {result.SuccessCount} 个 FriendlyName。\r\n\r\n如果设备管理器没有立即刷新，可重新扫描硬件或重新插拔设备。",
                "导入完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
        else
        {
            var errors = result.Errors
                .Select(error => $"{error.Device.InstanceId}\r\n{error.Exception.Message}")
                .ToArray();
            var preview = string.Join("\r\n\r\n", errors.Take(8));
            if (errors.Length > 8)
                preview += $"\r\n\r\n……另有 {errors.Length - 8} 项错误未显示。";
            MessageBox.Show(this,
                $"已尝试应用 {result.AttemptedCount} 个设备，其中 {result.FailedCount} 个失败。\r\n\r\n{preview}",
                "导入部分完成", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        LoadDevices(_selected?.InstanceId);
    }

    private void SetImportBusy(bool busy, string status = "", int percent = 0)
    {
        if (_importProgress is null || _importStatus is null)
            return;

        if (busy)
        {
            _importEnabledState = new Dictionary<Control, bool>();
            foreach (Control control in topPanel.Controls)
            {
                if (ReferenceEquals(control, _importProgress) || ReferenceEquals(control, _importStatus))
                    continue;
                _importEnabledState[control] = control.Enabled;
                control.Enabled = false;
            }

            _importEnabledState[splitMain] = splitMain.Enabled;
            splitMain.Enabled = false;

            _importProgress.Visible = true;
            _importStatus.Visible = true;
            _importProgress.Value = Math.Clamp(percent, 0, 100);
            _importStatus.Text = status;
            Cursor = Cursors.WaitCursor;
        }
        else
        {
            if (_importEnabledState is not null)
                foreach (var pair in _importEnabledState.Where(pair => !pair.Key.IsDisposed))
                    pair.Key.Enabled = pair.Value;

            _importEnabledState = null;
            _importProgress.Visible = false;
            _importStatus.Visible = false;
            _importStatus.Text = "";
            Cursor = Cursors.Default;
        }
    }

    private void UpdateImportProgress(ImportProgressInfo info)
    {
        if (_importProgress is null || _importStatus is null)
            return;
        _importProgress.Value = Math.Clamp(info.Percent, 0, 100);
        _importStatus.Text = info.Status;
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
                        nodeToSelect = node;
                }

                // 关键：分类节点只添加一次
                treeDevices.Nodes.Add(classNode);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.ToString(), "枚举设备失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            WriteLog.Error("1" + ex + ex.Message);
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
            PnpBatchService.ApplyFriendlyName(_selected, newName);

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
            Note =
                "用于备份设备 FriendlyName 等自定义显示信息。后续导入时应优先用 InstanceId 匹配，失败后再尝试 HardwareIds / CompatibleIds / ClassGuid / Service / Enumerator 等弱匹配。",
            Devices = [.. devices.Select(DeviceCustomInfo.FromDeviceEntry)]
        };

        using var dialog = new SaveFileDialog();
        dialog.Title = "导出自定义设备信息";
        dialog.Filter = "JSON 文件 (*.json)|*.json|所有文件 (*.*)|*.*";
        dialog.FileName = $"device-custom-info-{DateTime.Now:yyyyMMdd-HHmmss}.json";
        dialog.AddExtension = true;
        dialog.DefaultExt = "json";
        dialog.OverwritePrompt = true;

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

    private static bool IsAdministrator()
    {
        using var identity = WindowsIdentity.GetCurrent();
        return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
    }

    private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) =>
        Process.Start("explorer.exe", linkLabel1.Text).Dispose();

    private void linkLabel1_MouseDoubleClick(object sender, MouseEventArgs e) =>
        linkLabel1_LinkClicked(sender, new LinkLabelLinkClickedEventArgs(linkLabel1.Links[0]));
}