namespace PnpFriendlyNameEditor;

partial class Form1
{
    private System.ComponentModel.IContainer? components = null;

    private TableLayoutPanel rootLayout;
    private FlowLayoutPanel topPanel;
    private Button btnRefresh;
    private CheckBox chkShowNonPresent;
    private Button btnSave;
    private Button btnCopyInstanceId;
    private Button btnExportCustom;
    private Label lblCheckedCount;
    private SplitContainer splitMain;
    private TreeView treeDevices;

    private Panel rightPanel;
    private Label lblTitle;
    private TableLayoutPanel detailsLayout;
    private Label lblHint;

    // 每个字段标签显式声明为设计器控件。这样在 WinForms Designer 中可以直接
    // 看到、选中并调整右侧表格的各个单元格，而不是由运行时代码临时创建。
    private Label lblDisplayName;
    private Label lblFriendlyName;
    private Label lblDescription;
    private Label lblInstanceId;
    private Label lblClass;
    private Label lblManufacturer;
    private Label lblService;
    private Label lblEnumerator;
    private Label lblDriver;

    private TextBox txtDisplayName;
    private TextBox txtFriendlyName;
    private TextBox txtDescription;
    private TextBox txtInstanceId;
    private TextBox txtClass;
    private TextBox txtManufacturer;
    private TextBox txtService;
    private TextBox txtEnumerator;
    private TextBox txtDriver;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
        rootLayout = new TableLayoutPanel();
        topPanel = new FlowLayoutPanel();
        btnRefresh = new Button();
        chkShowNonPresent = new CheckBox();
        btnSave = new Button();
        btnCopyInstanceId = new Button();
        btnExportCustom = new Button();
        lblCheckedCount = new Label();
        splitMain = new SplitContainer();
        treeDevices = new TreeView();
        rightPanel = new Panel();
        detailsLayout = new TableLayoutPanel();
        lblDisplayName = new Label();
        txtDisplayName = new TextBox();
        lblFriendlyName = new Label();
        txtFriendlyName = new TextBox();
        lblDescription = new Label();
        txtDescription = new TextBox();
        lblInstanceId = new Label();
        txtInstanceId = new TextBox();
        lblClass = new Label();
        txtClass = new TextBox();
        lblManufacturer = new Label();
        txtManufacturer = new TextBox();
        lblService = new Label();
        txtService = new TextBox();
        lblEnumerator = new Label();
        txtEnumerator = new TextBox();
        lblDriver = new Label();
        txtDriver = new TextBox();
        lblHint = new Label();
        lblTitle = new Label();
        label1 = new Label();
        label2 = new Label();
        label3 = new Label();
        linkLabel1 = new LinkLabel();
        rootLayout.SuspendLayout();
        topPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
        splitMain.Panel1.SuspendLayout();
        splitMain.Panel2.SuspendLayout();
        splitMain.SuspendLayout();
        rightPanel.SuspendLayout();
        detailsLayout.SuspendLayout();
        SuspendLayout();
        // 
        // rootLayout
        // 
        rootLayout.ColumnCount = 1;
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootLayout.Controls.Add(topPanel, 0, 0);
        rootLayout.Controls.Add(splitMain, 0, 1);
        rootLayout.Dock = DockStyle.Fill;
        rootLayout.Location = new Point(0, 0);
        rootLayout.Name = "rootLayout";
        rootLayout.RowCount = 2;
        rootLayout.RowStyles.Add(new RowStyle());
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rootLayout.Size = new Size(1200, 760);
        rootLayout.TabIndex = 0;
        // 
        // topPanel
        // 
        topPanel.AutoSize = true;
        topPanel.Controls.Add(btnRefresh);
        topPanel.Controls.Add(chkShowNonPresent);
        topPanel.Controls.Add(btnSave);
        topPanel.Controls.Add(btnCopyInstanceId);
        topPanel.Controls.Add(btnExportCustom);
        topPanel.Controls.Add(lblCheckedCount);
        topPanel.Dock = DockStyle.Fill;
        topPanel.Location = new Point(0, 0);
        topPanel.Margin = new Padding(0);
        topPanel.Name = "topPanel";
        topPanel.Padding = new Padding(10, 8, 10, 8);
        topPanel.Size = new Size(1200, 49);
        topPanel.TabIndex = 0;
        topPanel.WrapContents = false;
        // 
        // btnRefresh
        // 
        btnRefresh.AutoSize = true;
        btnRefresh.Location = new Point(13, 11);
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Size = new Size(75, 27);
        btnRefresh.TabIndex = 0;
        btnRefresh.Text = "刷新";
        btnRefresh.UseVisualStyleBackColor = true;
        btnRefresh.Click += btnRefresh_Click;
        // 
        // chkShowNonPresent
        // 
        chkShowNonPresent.Location = new Point(94, 11);
        chkShowNonPresent.Name = "chkShowNonPresent";
        chkShowNonPresent.Size = new Size(135, 27);
        chkShowNonPresent.TabIndex = 1;
        chkShowNonPresent.Text = "显示非当前连接设备";
        chkShowNonPresent.UseVisualStyleBackColor = true;
        chkShowNonPresent.CheckedChanged += chkShowNonPresent_CheckedChanged;
        // 
        // btnSave
        // 
        btnSave.AutoSize = true;
        btnSave.Enabled = false;
        btnSave.Location = new Point(235, 11);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(132, 27);
        btnSave.TabIndex = 2;
        btnSave.Text = "保存 FriendlyName";
        btnSave.UseVisualStyleBackColor = true;
        btnSave.Click += btnSave_Click;
        // 
        // btnCopyInstanceId
        // 
        btnCopyInstanceId.AutoSize = true;
        btnCopyInstanceId.Enabled = false;
        btnCopyInstanceId.Location = new Point(373, 11);
        btnCopyInstanceId.Name = "btnCopyInstanceId";
        btnCopyInstanceId.Size = new Size(105, 27);
        btnCopyInstanceId.TabIndex = 3;
        btnCopyInstanceId.Text = "复制实例路径";
        btnCopyInstanceId.UseVisualStyleBackColor = true;
        btnCopyInstanceId.Click += btnCopyInstanceId_Click;
        // 
        // btnExportCustom
        // 
        btnExportCustom.AutoSize = true;
        btnExportCustom.Enabled = false;
        btnExportCustom.Location = new Point(484, 11);
        btnExportCustom.Name = "btnExportCustom";
        btnExportCustom.Size = new Size(132, 27);
        btnExportCustom.TabIndex = 4;
        btnExportCustom.Text = "导出自定义信息";
        btnExportCustom.UseVisualStyleBackColor = true;
        btnExportCustom.Click += btnExportCustom_Click;
        // 
        // lblCheckedCount
        // 
        lblCheckedCount.AutoSize = true;
        lblCheckedCount.Location = new Point(632, 16);
        lblCheckedCount.Margin = new Padding(13, 8, 3, 0);
        lblCheckedCount.Name = "lblCheckedCount";
        lblCheckedCount.Size = new Size(58, 17);
        lblCheckedCount.TabIndex = 5;
        lblCheckedCount.Text = "已选择: 0";
        // 
        // splitMain
        // 
        splitMain.Dock = DockStyle.Fill;
        splitMain.Location = new Point(3, 52);
        splitMain.Name = "splitMain";
        // 
        // splitMain.Panel1
        // 
        splitMain.Panel1.Controls.Add(treeDevices);
        splitMain.Panel1MinSize = 80;
        // 
        // splitMain.Panel2
        // 
        splitMain.Panel2.Controls.Add(rightPanel);
        splitMain.Panel2MinSize = 120;
        splitMain.Size = new Size(1194, 705);
        splitMain.SplitterDistance = 206;
        splitMain.TabIndex = 1;
        // 
        // treeDevices
        // 
        treeDevices.CheckBoxes = true;
        treeDevices.Dock = DockStyle.Fill;
        treeDevices.HideSelection = false;
        treeDevices.Location = new Point(0, 0);
        treeDevices.Name = "treeDevices";
        treeDevices.Size = new Size(206, 705);
        treeDevices.TabIndex = 0;
        treeDevices.AfterCheck += treeDevices_AfterCheck;
        treeDevices.AfterSelect += treeDevices_AfterSelect;
        // 
        // rightPanel
        // 
        rightPanel.Controls.Add(detailsLayout);
        rightPanel.Controls.Add(lblHint);
        rightPanel.Controls.Add(lblTitle);
        rightPanel.Dock = DockStyle.Fill;
        rightPanel.Location = new Point(0, 0);
        rightPanel.Name = "rightPanel";
        rightPanel.Padding = new Padding(20, 16, 20, 16);
        rightPanel.Size = new Size(984, 705);
        rightPanel.TabIndex = 0;
        // 
        // detailsLayout
        // 
        detailsLayout.ColumnCount = 2;
        detailsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
        detailsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
        detailsLayout.Controls.Add(lblDisplayName, 0, 0);
        detailsLayout.Controls.Add(txtDisplayName, 1, 0);
        detailsLayout.Controls.Add(lblFriendlyName, 0, 1);
        detailsLayout.Controls.Add(txtFriendlyName, 1, 1);
        detailsLayout.Controls.Add(lblDescription, 0, 2);
        detailsLayout.Controls.Add(txtDescription, 1, 2);
        detailsLayout.Controls.Add(lblInstanceId, 0, 3);
        detailsLayout.Controls.Add(txtInstanceId, 1, 3);
        detailsLayout.Controls.Add(lblClass, 0, 4);
        detailsLayout.Controls.Add(txtClass, 1, 4);
        detailsLayout.Controls.Add(lblManufacturer, 0, 5);
        detailsLayout.Controls.Add(txtManufacturer, 1, 5);
        detailsLayout.Controls.Add(lblService, 0, 6);
        detailsLayout.Controls.Add(txtService, 1, 6);
        detailsLayout.Controls.Add(lblEnumerator, 0, 7);
        detailsLayout.Controls.Add(txtEnumerator, 1, 7);
        detailsLayout.Controls.Add(lblDriver, 0, 8);
        detailsLayout.Controls.Add(txtDriver, 1, 8);
        detailsLayout.Controls.Add(label2, 1, 9);
        detailsLayout.Controls.Add(label1, 0, 9);
        detailsLayout.Controls.Add(label3, 0, 10);
        detailsLayout.Controls.Add(linkLabel1, 1, 10);
        detailsLayout.Dock = DockStyle.Fill;
        detailsLayout.Location = new Point(20, 60);
        detailsLayout.Name = "detailsLayout";
        detailsLayout.Padding = new Padding(0, 10, 0, 10);
        detailsLayout.RowCount = 11;
        detailsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        detailsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));
        detailsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        detailsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 96F));
        detailsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        detailsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        detailsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        detailsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        detailsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        detailsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        detailsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        detailsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        detailsLayout.Size = new Size(944, 521);
        detailsLayout.TabIndex = 1;
        // 
        // lblDisplayName
        // 
        lblDisplayName.AutoEllipsis = true;
        lblDisplayName.Dock = DockStyle.Fill;
        lblDisplayName.Location = new Point(3, 10);
        lblDisplayName.Margin = new Padding(3, 0, 3, 9);
        lblDisplayName.Name = "lblDisplayName";
        lblDisplayName.Padding = new Padding(8, 0, 12, 0);
        lblDisplayName.Size = new Size(277, 29);
        lblDisplayName.TabIndex = 0;
        lblDisplayName.Text = "当前显示名";
        lblDisplayName.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // txtDisplayName
        // 
        txtDisplayName.BorderStyle = BorderStyle.FixedSingle;
        txtDisplayName.Dock = DockStyle.Fill;
        txtDisplayName.Location = new Point(289, 15);
        txtDisplayName.Margin = new Padding(6, 5, 6, 5);
        txtDisplayName.Name = "txtDisplayName";
        txtDisplayName.ReadOnly = true;
        txtDisplayName.Size = new Size(649, 23);
        txtDisplayName.TabIndex = 1;
        // 
        // lblFriendlyName
        // 
        lblFriendlyName.AutoEllipsis = true;
        lblFriendlyName.Dock = DockStyle.Fill;
        lblFriendlyName.Location = new Point(3, 48);
        lblFriendlyName.Margin = new Padding(3, 0, 3, 9);
        lblFriendlyName.Name = "lblFriendlyName";
        lblFriendlyName.Padding = new Padding(8, 0, 12, 0);
        lblFriendlyName.Size = new Size(277, 37);
        lblFriendlyName.TabIndex = 2;
        lblFriendlyName.Text = "FriendlyName";
        lblFriendlyName.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // txtFriendlyName
        // 
        txtFriendlyName.BorderStyle = BorderStyle.FixedSingle;
        txtFriendlyName.Dock = DockStyle.Fill;
        txtFriendlyName.Location = new Point(289, 53);
        txtFriendlyName.Margin = new Padding(6, 5, 6, 5);
        txtFriendlyName.Name = "txtFriendlyName";
        txtFriendlyName.Size = new Size(649, 23);
        txtFriendlyName.TabIndex = 3;
        // 
        // lblDescription
        // 
        lblDescription.AutoEllipsis = true;
        lblDescription.Dock = DockStyle.Fill;
        lblDescription.Location = new Point(3, 94);
        lblDescription.Margin = new Padding(3, 0, 3, 10);
        lblDescription.Name = "lblDescription";
        lblDescription.Padding = new Padding(8, 0, 12, 0);
        lblDescription.Size = new Size(277, 32);
        lblDescription.TabIndex = 4;
        lblDescription.Text = "设备描述";
        lblDescription.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // txtDescription
        // 
        txtDescription.BorderStyle = BorderStyle.FixedSingle;
        txtDescription.Dock = DockStyle.Fill;
        txtDescription.Location = new Point(289, 99);
        txtDescription.Margin = new Padding(6, 5, 6, 5);
        txtDescription.Name = "txtDescription";
        txtDescription.ReadOnly = true;
        txtDescription.Size = new Size(649, 23);
        txtDescription.TabIndex = 5;
        // 
        // lblInstanceId
        // 
        lblInstanceId.AutoEllipsis = true;
        lblInstanceId.Dock = DockStyle.Fill;
        lblInstanceId.Location = new Point(3, 136);
        lblInstanceId.Name = "lblInstanceId";
        lblInstanceId.Padding = new Padding(8, 0, 12, 0);
        lblInstanceId.Size = new Size(277, 96);
        lblInstanceId.TabIndex = 6;
        lblInstanceId.Text = "实例路径";
        lblInstanceId.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // txtInstanceId
        // 
        txtInstanceId.BorderStyle = BorderStyle.FixedSingle;
        txtInstanceId.Dock = DockStyle.Fill;
        txtInstanceId.Location = new Point(289, 141);
        txtInstanceId.Margin = new Padding(6, 5, 6, 5);
        txtInstanceId.Multiline = true;
        txtInstanceId.Name = "txtInstanceId";
        txtInstanceId.ReadOnly = true;
        txtInstanceId.ScrollBars = ScrollBars.Vertical;
        txtInstanceId.Size = new Size(649, 86);
        txtInstanceId.TabIndex = 7;
        // 
        // lblClass
        // 
        lblClass.AutoEllipsis = true;
        lblClass.Dock = DockStyle.Fill;
        lblClass.Location = new Point(3, 232);
        lblClass.Margin = new Padding(3, 0, 3, 9);
        lblClass.Name = "lblClass";
        lblClass.Padding = new Padding(8, 0, 12, 0);
        lblClass.Size = new Size(277, 33);
        lblClass.TabIndex = 8;
        lblClass.Text = "类别";
        lblClass.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // txtClass
        // 
        txtClass.BorderStyle = BorderStyle.FixedSingle;
        txtClass.Dock = DockStyle.Fill;
        txtClass.Location = new Point(289, 237);
        txtClass.Margin = new Padding(6, 5, 6, 5);
        txtClass.Name = "txtClass";
        txtClass.ReadOnly = true;
        txtClass.Size = new Size(649, 23);
        txtClass.TabIndex = 9;
        // 
        // lblManufacturer
        // 
        lblManufacturer.AutoEllipsis = true;
        lblManufacturer.Dock = DockStyle.Fill;
        lblManufacturer.Location = new Point(3, 274);
        lblManufacturer.Margin = new Padding(3, 0, 3, 9);
        lblManufacturer.Name = "lblManufacturer";
        lblManufacturer.Padding = new Padding(8, 0, 12, 0);
        lblManufacturer.Size = new Size(277, 33);
        lblManufacturer.TabIndex = 10;
        lblManufacturer.Text = "厂商";
        lblManufacturer.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // txtManufacturer
        // 
        txtManufacturer.BorderStyle = BorderStyle.FixedSingle;
        txtManufacturer.Dock = DockStyle.Fill;
        txtManufacturer.Location = new Point(289, 279);
        txtManufacturer.Margin = new Padding(6, 5, 6, 5);
        txtManufacturer.Name = "txtManufacturer";
        txtManufacturer.ReadOnly = true;
        txtManufacturer.Size = new Size(649, 23);
        txtManufacturer.TabIndex = 11;
        // 
        // lblService
        // 
        lblService.AutoEllipsis = true;
        lblService.Dock = DockStyle.Fill;
        lblService.Location = new Point(3, 316);
        lblService.Margin = new Padding(3, 0, 3, 9);
        lblService.Name = "lblService";
        lblService.Padding = new Padding(8, 0, 12, 0);
        lblService.Size = new Size(277, 33);
        lblService.TabIndex = 12;
        lblService.Text = "服务";
        lblService.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // txtService
        // 
        txtService.BorderStyle = BorderStyle.FixedSingle;
        txtService.Dock = DockStyle.Fill;
        txtService.Location = new Point(289, 321);
        txtService.Margin = new Padding(6, 5, 6, 5);
        txtService.Name = "txtService";
        txtService.ReadOnly = true;
        txtService.Size = new Size(649, 23);
        txtService.TabIndex = 13;
        // 
        // lblEnumerator
        // 
        lblEnumerator.AutoEllipsis = true;
        lblEnumerator.Dock = DockStyle.Fill;
        lblEnumerator.Location = new Point(3, 358);
        lblEnumerator.Margin = new Padding(3, 0, 3, 9);
        lblEnumerator.Name = "lblEnumerator";
        lblEnumerator.Padding = new Padding(8, 0, 12, 0);
        lblEnumerator.Size = new Size(277, 33);
        lblEnumerator.TabIndex = 14;
        lblEnumerator.Text = "枚举器";
        lblEnumerator.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // txtEnumerator
        // 
        txtEnumerator.BorderStyle = BorderStyle.FixedSingle;
        txtEnumerator.Dock = DockStyle.Fill;
        txtEnumerator.Location = new Point(289, 363);
        txtEnumerator.Margin = new Padding(6, 5, 6, 5);
        txtEnumerator.Name = "txtEnumerator";
        txtEnumerator.ReadOnly = true;
        txtEnumerator.Size = new Size(649, 23);
        txtEnumerator.TabIndex = 15;
        // 
        // lblDriver
        // 
        lblDriver.AutoEllipsis = true;
        lblDriver.Dock = DockStyle.Fill;
        lblDriver.Location = new Point(3, 400);
        lblDriver.Margin = new Padding(3, 0, 3, 9);
        lblDriver.Name = "lblDriver";
        lblDriver.Padding = new Padding(8, 0, 12, 0);
        lblDriver.Size = new Size(277, 33);
        lblDriver.TabIndex = 16;
        lblDriver.Text = "驱动键";
        lblDriver.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // txtDriver
        // 
        txtDriver.BorderStyle = BorderStyle.FixedSingle;
        txtDriver.Dock = DockStyle.Fill;
        txtDriver.Location = new Point(289, 405);
        txtDriver.Margin = new Padding(6, 5, 6, 5);
        txtDriver.Name = "txtDriver";
        txtDriver.ReadOnly = true;
        txtDriver.Size = new Size(649, 23);
        txtDriver.TabIndex = 17;
        // 
        // lblHint
        // 
        lblHint.Dock = DockStyle.Bottom;
        lblHint.ForeColor = Color.DimGray;
        lblHint.Location = new Point(20, 581);
        lblHint.Name = "lblHint";
        lblHint.Padding = new Padding(0, 12, 0, 0);
        lblHint.Size = new Size(944, 108);
        lblHint.TabIndex = 2;
        lblHint.Text = "说明：本工具只尝试修改 SPDRP_FRIENDLYNAME。不要用它来改 HardwareID、CompatibleIDs、Service、ClassGUID、UpperFilters、LowerFilters 等关键项。\r\n保存后如果设备管理器没有立刻显示变化，可以刷新、重新插拔设备，或者重启。部分设备的显示名可能会被驱动/INF/重新枚举覆盖。";
        // 
        // lblTitle
        // 
        lblTitle.Dock = DockStyle.Top;
        lblTitle.Font = new Font("Microsoft YaHei UI", 14F, FontStyle.Bold);
        lblTitle.Location = new Point(20, 16);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(944, 44);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "设备属性";
        lblTitle.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // label1
        // 
        label1.Location = new Point(3, 442);
        label1.Name = "label1";
        label1.Size = new Size(277, 20);
        label1.TabIndex = 18;
        label1.Text = "作者: Rainbow SPY";
        label1.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // label2
        // 
        label2.Dock = DockStyle.Fill;
        label2.Location = new Point(286, 442);
        label2.Name = "label2";
        label2.Size = new Size(655, 20);
        label2.TabIndex = 19;
        label2.Text = "关注塔菲谢谢喵~";
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Location = new Point(3, 462);
        label3.Name = "label3";
        label3.Size = new Size(63, 17);
        label3.TabIndex = 20;
        label3.Text = "项目地址: ";
        // 
        // linkLabel1
        // 
        linkLabel1.AutoSize = true;
        linkLabel1.Location = new Point(286, 462);
        linkLabel1.Name = "linkLabel1";
        linkLabel1.Size = new Size(201, 17);
        linkLabel1.TabIndex = 21;
        linkLabel1.TabStop = true;
        linkLabel1.Text = "https://github.com/Rainbow-SPY/";
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1200, 760);
        Controls.Add(rootLayout);
        Icon = (Icon)resources.GetObject("$this.Icon");
        MinimumSize = new Size(900, 600);
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "PnP FriendlyName Editor";
        rootLayout.ResumeLayout(false);
        rootLayout.PerformLayout();
        topPanel.ResumeLayout(false);
        topPanel.PerformLayout();
        splitMain.Panel1.ResumeLayout(false);
        splitMain.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
        splitMain.ResumeLayout(false);
        rightPanel.ResumeLayout(false);
        detailsLayout.ResumeLayout(false);
        detailsLayout.PerformLayout();
        ResumeLayout(false);
    }

    private Label label1;
    private Label label2;
    private Label label3;
    private LinkLabel linkLabel1;
}
