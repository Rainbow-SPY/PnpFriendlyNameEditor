using System.Drawing;

namespace PnpFriendlyNameEditor;

internal sealed class ImportDiffForm : Form
{
    private readonly IReadOnlyList<ImportMatchResult> _matches;
    private readonly TreeView _tree = new();
    private readonly DataGridView _leftGrid = CreateGrid();
    private readonly DataGridView _rightGrid = CreateGrid();
    private readonly CheckBox _hideSame = new();
    private readonly Label _status = new();
    private readonly Button _okButton = new();
    private readonly Button _cancelButton = new();
    private bool _updatingChecks;
    private bool _syncingScroll;
    private ImportMatchResult? _selectedMatch;

    public ImportDiffForm(IReadOnlyList<ImportMatchResult> matches)
    {
        _matches = matches;
        Text = "导入差异审阅";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(1100, 680);
        Size = new Size(1450, 860);
        ShowIcon = false;

        BuildUi();
        PopulateTree();
    }

    public IReadOnlyList<ImportMatchResult> GetSelectedMatches()
    {
        var result = new List<ImportMatchResult>();
        foreach (TreeNode group in _tree.Nodes)
        foreach (TreeNode child in group.Nodes)
            if (child.Checked && child.Tag is ImportMatchResult match)
                result.Add(match);
        return result;
    }

    private void BuildUi()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(10)
        };
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        Controls.Add(root);

        _status.AutoSize = true;
        _status.Padding = new Padding(4, 2, 4, 8);
        _status.Text = "左侧：配置/导出记录；右侧：当前 PnP 设备。红色=不同，绿色=相同。";
        root.Controls.Add(_status, 0, 0);

        var main = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Vertical,
            FixedPanel = FixedPanel.Panel1
        };
        root.Controls.Add(main, 0, 1);

        // 不设置 Panel1MinSize / Panel2MinSize。WinForms 在初始化和 DPI/布局变化期间
        // 会立即用临时 Width 校验这些属性，容易与 SplitterDistance 互相触发越界。
        // 只在窗口真正显示后延迟设置 SplitterDistance；后续缩放同样只调整 Distance。
        Shown += (_, _) => BeginInvoke(new Action(() => TryApplySplitterDistance(main)));
        main.SizeChanged += (_, _) =>
        {
            if (!Visible || !IsHandleCreated || main.IsDisposed)
                return;

            BeginInvoke(new Action(() => TryApplySplitterDistance(main, preserveCurrentDistance: true)));
        };

        _tree.Dock = DockStyle.Fill;
        _tree.CheckBoxes = true;
        _tree.HideSelection = false;
        _tree.AfterSelect += (_, e) =>
        {
            if (e.Node?.Tag is ImportMatchResult match)
            {
                _selectedMatch = match;
                RenderMatch(match);
            }
        };
        _tree.AfterCheck += TreeAfterCheck;
        main.Panel1.Controls.Add(_tree);

        var comparison = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2
        };
        comparison.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        comparison.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        comparison.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        comparison.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        main.Panel2.Controls.Add(comparison);

        var leftTitle = new Label { Text = "配置 / 导出记录", Dock = DockStyle.Fill, AutoSize = true, Padding = new Padding(4, 4, 4, 8) };
        var rightTitle = new Label { Text = "当前设备", Dock = DockStyle.Fill, AutoSize = true, Padding = new Padding(4, 4, 4, 8) };
        comparison.Controls.Add(leftTitle, 0, 0);
        comparison.Controls.Add(rightTitle, 1, 0);
        comparison.Controls.Add(_leftGrid, 0, 1);
        comparison.Controls.Add(_rightGrid, 1, 1);

        _leftGrid.Scroll += (_, e) => SyncScroll(_leftGrid, _rightGrid, e.ScrollOrientation);
        _rightGrid.Scroll += (_, e) => SyncScroll(_rightGrid, _leftGrid, e.ScrollOrientation);

        var bottom = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(0, 8, 0, 0)
        };
        root.Controls.Add(bottom, 0, 2);

        _hideSame.Text = "隐藏相同的属性";
        _hideSame.Checked = true;
        _hideSame.AutoSize = true;
        _hideSame.Margin = new Padding(3, 8, 20, 3);
        _hideSame.CheckedChanged += (_, _) =>
        {
            if (_selectedMatch is not null)
                RenderMatch(_selectedMatch);
        };
        bottom.Controls.Add(_hideSame);

        var spacer = new Label { AutoSize = false, Width = 30, Height = 1 };
        bottom.Controls.Add(spacer);

        _okButton.Text = "应用已勾选设备";
        _okButton.AutoSize = true;
        _okButton.DialogResult = DialogResult.OK;
        bottom.Controls.Add(_okButton);

        _cancelButton.Text = "取消";
        _cancelButton.AutoSize = true;
        _cancelButton.DialogResult = DialogResult.Cancel;
        bottom.Controls.Add(_cancelButton);

        AcceptButton = _okButton;
        CancelButton = _cancelButton;
    }


    private static void TryApplySplitterDistance(SplitContainer split, bool preserveCurrentDistance = false)
    {
        if (split.IsDisposed || split.Orientation != Orientation.Vertical || split.Width <= 0)
            return;

        // 只读取 WinForms 当前已有的最小尺寸，不再主动修改它们。
        // 再留出 SplitterWidth + 1 像素余量，确保落在 setter 接受的区间内部。
        var min = Math.Max(0, split.Panel1MinSize);
        var max = split.Width - Math.Max(0, split.Panel2MinSize) - split.SplitterWidth - 1;
        if (max < min)
            return;

        var preferred = preserveCurrentDistance && split.SplitterDistance >= min && split.SplitterDistance <= max
            ? split.SplitterDistance
            : Math.Min(330, Math.Max(min, (int)Math.Round(split.Width * 0.28)));

        var target = Math.Clamp(preferred, min, max);
        if (split.SplitterDistance == target)
            return;

        try
        {
            split.SplitterDistance = target;
        }
        catch (InvalidOperationException)
        {
            // 极端 DPI/布局重入时 Width 可能在检查期间再次变化。
            // 分隔位置只是视觉效果，保持 WinForms 当前默认值即可，不应让导入流程崩溃。
        }
    }

    private void PopulateTree()
    {
        _updatingChecks = true;
        try
        {
            _tree.BeginUpdate();
            _tree.Nodes.Clear();

            AddGroup("完全匹配", _matches.Where(x => x.Kind == ImportMatchKind.Exact).ToList());
            AddGroup("模糊匹配", _matches.Where(x => x.Kind == ImportMatchKind.Fuzzy).ToList());

            _tree.ExpandAll();
            var first = _tree.Nodes.Cast<TreeNode>().SelectMany(x => x.Nodes.Cast<TreeNode>()).FirstOrDefault();
            if (first is not null)
                _tree.SelectedNode = first;
        }
        finally
        {
            _tree.EndUpdate();
            _updatingChecks = false;
        }
    }

    private void AddGroup(string title, IReadOnlyList<ImportMatchResult> matches)
    {
        var group = new TreeNode($"{title} ({matches.Count})") { Checked = matches.Count > 0 };
        foreach (var match in matches)
        {
            var current = string.IsNullOrWhiteSpace(match.Device.DisplayName) ? match.Device.InstanceId : match.Device.DisplayName;
            var desired = string.IsNullOrWhiteSpace(match.Rule.DesiredFriendlyName) ? "<空 FriendlyName>" : match.Rule.DesiredFriendlyName;
            group.Nodes.Add(new TreeNode($"{desired}  ←  {current}")
            {
                Tag = match,
                Checked = true
            });
        }
        _tree.Nodes.Add(group);
    }

    private void TreeAfterCheck(object? sender, TreeViewEventArgs e)
    {
        if (_updatingChecks || e.Node is null)
            return;
        try
        {
            _updatingChecks = true;
            if (e.Node.Tag is null)
            {
                foreach (TreeNode child in e.Node.Nodes)
                    child.Checked = e.Node.Checked;
            }
            else if (e.Node.Parent is TreeNode parent)
            {
                parent.Checked = parent.Nodes.Cast<TreeNode>().Any(x => x.Checked);
            }
        }
        finally
        {
            _updatingChecks = false;
        }
    }

    private void RenderMatch(ImportMatchResult match)
    {
        var keys = BuildOrderedKeys(match.SourceProperties.Keys.Union(match.CurrentProperties.Keys, StringComparer.OrdinalIgnoreCase));
        _leftGrid.Rows.Clear();
        _rightGrid.Rows.Clear();

        foreach (var key in keys)
        {
            var left = match.SourceProperties.TryGetValue(key, out var leftValue) ? leftValue ?? "" : "";
            var right = match.CurrentProperties.TryGetValue(key, out var rightValue) ? rightValue ?? "" : "";
            var same = ValuesEqual(left, right);
            if (_hideSame.Checked && same)
                continue;

            var leftIndex = _leftGrid.Rows.Add(left);
            var rightIndex = _rightGrid.Rows.Add(right);
            _leftGrid.Rows[leftIndex].HeaderCell.Value = key;
            _rightGrid.Rows[rightIndex].HeaderCell.Value = key;

            var color = same ? Color.Honeydew : Color.MistyRose;
            _leftGrid.Rows[leftIndex].DefaultCellStyle.BackColor = color;
            _rightGrid.Rows[rightIndex].DefaultCellStyle.BackColor = color;
        }

        if (_leftGrid.Rows.Count == 0)
        {
            var l = _leftGrid.Rows.Add("所有可比较属性均相同");
            var r = _rightGrid.Rows.Add("所有可比较属性均相同");
            _leftGrid.Rows[l].HeaderCell.Value = "结果";
            _rightGrid.Rows[r].HeaderCell.Value = "结果";
            _leftGrid.Rows[l].DefaultCellStyle.BackColor = Color.Honeydew;
            _rightGrid.Rows[r].DefaultCellStyle.BackColor = Color.Honeydew;
        }

        _status.Text = $"匹配方式：{(match.Kind == ImportMatchKind.Exact ? "完全匹配" : "模糊匹配")}    " +
                       $"目标 FriendlyName：{match.Rule.DesiredFriendlyName}    当前实例：{match.Device.InstanceId}";
    }

    private static IEnumerable<string> BuildOrderedKeys(IEnumerable<string> keys)
    {
        var preferred = new[]
        {
            "[解析] EnumeratorType", "[解析] Bus", "[解析] VID", "[解析] PID", "[解析] VEN", "[解析] DEV",
            "[解析] MI", "[解析] COL", "[解析] SUBSYS", "[解析] FUNC", "[解析] PROD", "[解析] REV",
            "[解析] KIND", "[解析] PROFILE", "[解析] ModelSegment", "[解析] LogicalProcessorNumber", "[解析] InstanceTail",
            "InstanceId", "CustomFriendlyName", "DisplayNameAtExport", "DeviceDescription", "ClassName", "ClassDisplayName",
            "ClassGuid", "Manufacturer", "Service", "Enumerator", "Driver", "LocationInformation",
            "PhysicalDeviceObjectName", "LogicalProcessorNumber", "ProcessorTopology", "HardwareIds", "CompatibleIds", "LocationPaths"
        };

        var set = new HashSet<string>(keys, StringComparer.OrdinalIgnoreCase);
        foreach (var key in preferred)
            if (set.Remove(key))
                yield return key;
        foreach (var key in set.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
            yield return key;
    }

    private static bool ValuesEqual(string a, string b)
    {
        static string Normalize(string s) => (s ?? "")
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Trim();
        return string.Equals(Normalize(a), Normalize(b), StringComparison.OrdinalIgnoreCase);
    }

    private void SyncScroll(DataGridView source, DataGridView target, ScrollOrientation orientation)
    {
        if (_syncingScroll || orientation != ScrollOrientation.VerticalScroll)
            return;
        try
        {
            _syncingScroll = true;
            var index = source.FirstDisplayedScrollingRowIndex;
            if (index >= 0 && index < target.Rows.Count)
                target.FirstDisplayedScrollingRowIndex = index;
        }
        catch
        {
            // 仅用于视觉同步，滚动边界异常不影响导入。
        }
        finally
        {
            _syncingScroll = false;
        }
    }

    private static DataGridView CreateGrid()
    {
        var grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = true,
            RowHeadersVisible = true,
            RowHeadersWidth = 190,
            AutoGenerateColumns = false,
            SelectionMode = DataGridViewSelectionMode.CellSelect,
            MultiSelect = false,
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
            BackgroundColor = SystemColors.Window
        };
        grid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "值",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            SortMode = DataGridViewColumnSortMode.NotSortable
        });
        return grid;
    }
}
