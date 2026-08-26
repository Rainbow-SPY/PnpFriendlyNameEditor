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

    private System.Windows.Forms.TextBox txtDisplayName;
    private System.Windows.Forms.TextBox txtFriendlyName;
    private System.Windows.Forms.TextBox txtDescription;
    private TextBox txtInstanceId;
    private System.Windows.Forms.TextBox txtClass;
    private System.Windows.Forms.TextBox txtManufacturer;
    private System.Windows.Forms.TextBox txtService;
    private System.Windows.Forms.TextBox txtEnumerator;
    private System.Windows.Forms.TextBox txtDriver;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
        rootLayout = new System.Windows.Forms.TableLayoutPanel();
        topPanel = new System.Windows.Forms.FlowLayoutPanel();
        btnRefresh = new System.Windows.Forms.Button();
        chkShowNonPresent = new System.Windows.Forms.CheckBox();
        btnSave = new System.Windows.Forms.Button();
        btnCopyInstanceId = new System.Windows.Forms.Button();
        btnExportCustom = new System.Windows.Forms.Button();
        lblCheckedCount = new System.Windows.Forms.Label();
        splitMain = new System.Windows.Forms.SplitContainer();
        treeDevices = new System.Windows.Forms.TreeView();
        rightPanel = new System.Windows.Forms.Panel();
        detailsLayout = new System.Windows.Forms.TableLayoutPanel();
        lblDisplayName = new System.Windows.Forms.Label();
        txtDisplayName = new System.Windows.Forms.TextBox();
        lblFriendlyName = new System.Windows.Forms.Label();
        txtFriendlyName = new System.Windows.Forms.TextBox();
        lblDescription = new System.Windows.Forms.Label();
        txtDescription = new System.Windows.Forms.TextBox();
        lblInstanceId = new System.Windows.Forms.Label();
        txtInstanceId = new System.Windows.Forms.TextBox();
        lblClass = new System.Windows.Forms.Label();
        txtClass = new System.Windows.Forms.TextBox();
        lblManufacturer = new System.Windows.Forms.Label();
        txtManufacturer = new System.Windows.Forms.TextBox();
        lblService = new System.Windows.Forms.Label();
        txtService = new System.Windows.Forms.TextBox();
        lblEnumerator = new System.Windows.Forms.Label();
        txtEnumerator = new System.Windows.Forms.TextBox();
        lblDriver = new System.Windows.Forms.Label();
        txtDriver = new System.Windows.Forms.TextBox();
        label2 = new System.Windows.Forms.Label();
        label1 = new System.Windows.Forms.Label();
        label3 = new System.Windows.Forms.Label();
        linkLabel1 = new System.Windows.Forms.LinkLabel();
        lblHint = new System.Windows.Forms.Label();
        lblTitle = new System.Windows.Forms.Label();
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
        rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        rootLayout.Controls.Add(topPanel, 0, 0);
        rootLayout.Controls.Add(splitMain, 0, 1);
        rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
        rootLayout.Location = new System.Drawing.Point(0, 0);
        rootLayout.Name = "rootLayout";
        rootLayout.RowCount = 2;
        rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
        rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        rootLayout.Size = new System.Drawing.Size(1200, 760);
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
        topPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        topPanel.Location = new System.Drawing.Point(0, 0);
        topPanel.Margin = new System.Windows.Forms.Padding(0);
        topPanel.Name = "topPanel";
        topPanel.Padding = new System.Windows.Forms.Padding(10, 8, 10, 8);
        topPanel.Size = new System.Drawing.Size(1200, 49);
        topPanel.TabIndex = 0;
        topPanel.WrapContents = false;
        // 
        // btnRefresh
        // 
        btnRefresh.AutoSize = true;
        btnRefresh.Location = new System.Drawing.Point(13, 11);
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Size = new System.Drawing.Size(75, 27);
        btnRefresh.TabIndex = 0;
        btnRefresh.Text = "刷新";
        btnRefresh.UseVisualStyleBackColor = true;
        btnRefresh.Click += btnRefresh_Click;
        // 
        // chkShowNonPresent
        // 
        chkShowNonPresent.Location = new System.Drawing.Point(94, 11);
        chkShowNonPresent.Name = "chkShowNonPresent";
        chkShowNonPresent.Size = new System.Drawing.Size(135, 27);
        chkShowNonPresent.TabIndex = 1;
        chkShowNonPresent.Text = "显示非当前连接设备";
        chkShowNonPresent.UseVisualStyleBackColor = true;
        chkShowNonPresent.CheckedChanged += chkShowNonPresent_CheckedChanged;
        // 
        // btnSave
        // 
        btnSave.AutoSize = true;
        btnSave.Enabled = false;
        btnSave.Location = new System.Drawing.Point(235, 11);
        btnSave.Name = "btnSave";
        btnSave.Size = new System.Drawing.Size(132, 27);
        btnSave.TabIndex = 2;
        btnSave.Text = "保存 FriendlyName";
        btnSave.UseVisualStyleBackColor = true;
        btnSave.Click += btnSave_Click;
        // 
        // btnCopyInstanceId
        // 
        btnCopyInstanceId.AutoSize = true;
        btnCopyInstanceId.Enabled = false;
        btnCopyInstanceId.Location = new System.Drawing.Point(373, 11);
        btnCopyInstanceId.Name = "btnCopyInstanceId";
        btnCopyInstanceId.Size = new System.Drawing.Size(105, 27);
        btnCopyInstanceId.TabIndex = 3;
        btnCopyInstanceId.Text = "复制实例路径";
        btnCopyInstanceId.UseVisualStyleBackColor = true;
        btnCopyInstanceId.Click += btnCopyInstanceId_Click;
        // 
        // btnExportCustom
        // 
        btnExportCustom.AutoSize = true;
        btnExportCustom.Enabled = false;
        btnExportCustom.Location = new System.Drawing.Point(484, 11);
        btnExportCustom.Name = "btnExportCustom";
        btnExportCustom.Size = new System.Drawing.Size(132, 27);
        btnExportCustom.TabIndex = 4;
        btnExportCustom.Text = "导出自定义信息";
        btnExportCustom.UseVisualStyleBackColor = true;
        btnExportCustom.Click += btnExportCustom_Click;
        // 
        // lblCheckedCount
        // 
        lblCheckedCount.AutoSize = true;
        lblCheckedCount.Location = new System.Drawing.Point(632, 16);
        lblCheckedCount.Margin = new System.Windows.Forms.Padding(13, 8, 3, 0);
        lblCheckedCount.Name = "lblCheckedCount";
        lblCheckedCount.Size = new System.Drawing.Size(58, 17);
        lblCheckedCount.TabIndex = 5;
        lblCheckedCount.Text = "已选择: 0";
        // 
        // splitMain
        // 
        splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
        splitMain.Location = new System.Drawing.Point(3, 52);
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
        splitMain.Size = new System.Drawing.Size(1194, 705);
        splitMain.SplitterDistance = 206;
        splitMain.TabIndex = 1;
        // 
        // treeDevices
        // 
        treeDevices.CheckBoxes = true;
        treeDevices.Dock = System.Windows.Forms.DockStyle.Fill;
        treeDevices.HideSelection = false;
        treeDevices.Location = new System.Drawing.Point(0, 0);
        treeDevices.Name = "treeDevices";
        treeDevices.Size = new System.Drawing.Size(206, 705);
        treeDevices.TabIndex = 0;
        treeDevices.AfterCheck += treeDevices_AfterCheck;
        treeDevices.AfterSelect += treeDevices_AfterSelect;
        // 
        // rightPanel
        // 
        rightPanel.Controls.Add(detailsLayout);
        rightPanel.Controls.Add(lblHint);
        rightPanel.Controls.Add(lblTitle);
        rightPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        rightPanel.Location = new System.Drawing.Point(0, 0);
        rightPanel.Name = "rightPanel";
        rightPanel.Padding = new System.Windows.Forms.Padding(20, 16, 20, 16);
        rightPanel.Size = new System.Drawing.Size(984, 705);
        rightPanel.TabIndex = 0;
        // 
        // detailsLayout
        // 
        detailsLayout.ColumnCount = 2;
        detailsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
        detailsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
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
        detailsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
        detailsLayout.Location = new System.Drawing.Point(20, 60);
        detailsLayout.Name = "detailsLayout";
        detailsLayout.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);
        detailsLayout.RowCount = 11;
        detailsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
        detailsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
        detailsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
        detailsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 96F));
        detailsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
        detailsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
        detailsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
        detailsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
        detailsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
        detailsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
        detailsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
        detailsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
        detailsLayout.Size = new System.Drawing.Size(944, 521);
        detailsLayout.TabIndex = 1;
        // 
        // lblDisplayName
        // 
        lblDisplayName.AutoEllipsis = true;
        lblDisplayName.Dock = System.Windows.Forms.DockStyle.Fill;
        lblDisplayName.Location = new System.Drawing.Point(3, 10);
        lblDisplayName.Margin = new System.Windows.Forms.Padding(3, 0, 3, 9);
        lblDisplayName.Name = "lblDisplayName";
        lblDisplayName.Padding = new System.Windows.Forms.Padding(8, 0, 12, 0);
        lblDisplayName.Size = new System.Drawing.Size(277, 29);
        lblDisplayName.TabIndex = 0;
        lblDisplayName.Text = "当前显示名";
        lblDisplayName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        // 
        // txtDisplayName
        // 
        txtDisplayName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        txtDisplayName.Dock = System.Windows.Forms.DockStyle.Fill;
        txtDisplayName.Location = new System.Drawing.Point(289, 15);
        txtDisplayName.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
        txtDisplayName.Name = "txtDisplayName";
        txtDisplayName.ReadOnly = true;
        txtDisplayName.Size = new System.Drawing.Size(649, 23);
        txtDisplayName.TabIndex = 1;
        // 
        // lblFriendlyName
        // 
        lblFriendlyName.AutoEllipsis = true;
        lblFriendlyName.Dock = System.Windows.Forms.DockStyle.Fill;
        lblFriendlyName.Location = new System.Drawing.Point(3, 48);
        lblFriendlyName.Margin = new System.Windows.Forms.Padding(3, 0, 3, 9);
        lblFriendlyName.Name = "lblFriendlyName";
        lblFriendlyName.Padding = new System.Windows.Forms.Padding(8, 0, 12, 0);
        lblFriendlyName.Size = new System.Drawing.Size(277, 37);
        lblFriendlyName.TabIndex = 2;
        lblFriendlyName.Text = "FriendlyName";
        lblFriendlyName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        // 
        // txtFriendlyName
        // 
        txtFriendlyName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        txtFriendlyName.Dock = System.Windows.Forms.DockStyle.Fill;
        txtFriendlyName.Location = new System.Drawing.Point(289, 53);
        txtFriendlyName.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
        txtFriendlyName.Name = "txtFriendlyName";
        txtFriendlyName.Size = new System.Drawing.Size(649, 23);
        txtFriendlyName.TabIndex = 3;
        // 
        // lblDescription
        // 
        lblDescription.AutoEllipsis = true;
        lblDescription.Dock = System.Windows.Forms.DockStyle.Fill;
        lblDescription.Location = new System.Drawing.Point(3, 94);
        lblDescription.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
        lblDescription.Name = "lblDescription";
        lblDescription.Padding = new System.Windows.Forms.Padding(8, 0, 12, 0);
        lblDescription.Size = new System.Drawing.Size(277, 32);
        lblDescription.TabIndex = 4;
        lblDescription.Text = "设备描述";
        lblDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        // 
        // txtDescription
        // 
        txtDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        txtDescription.Dock = System.Windows.Forms.DockStyle.Fill;
        txtDescription.Location = new System.Drawing.Point(289, 99);
        txtDescription.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
        txtDescription.Name = "txtDescription";
        txtDescription.ReadOnly = true;
        txtDescription.Size = new System.Drawing.Size(649, 23);
        txtDescription.TabIndex = 5;
        // 
        // lblInstanceId
        // 
        lblInstanceId.AutoEllipsis = true;
        lblInstanceId.Dock = System.Windows.Forms.DockStyle.Fill;
        lblInstanceId.Location = new System.Drawing.Point(3, 136);
        lblInstanceId.Name = "lblInstanceId";
        lblInstanceId.Padding = new System.Windows.Forms.Padding(8, 0, 12, 0);
        lblInstanceId.Size = new System.Drawing.Size(277, 96);
        lblInstanceId.TabIndex = 6;
        lblInstanceId.Text = "实例路径";
        lblInstanceId.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        // 
        // txtInstanceId
        // 
        txtInstanceId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        txtInstanceId.Dock = System.Windows.Forms.DockStyle.Fill;
        txtInstanceId.Location = new System.Drawing.Point(289, 141);
        txtInstanceId.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
        txtInstanceId.Multiline = true;
        txtInstanceId.Name = "txtInstanceId";
        txtInstanceId.ReadOnly = true;
        txtInstanceId.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        txtInstanceId.Size = new System.Drawing.Size(649, 86);
        txtInstanceId.TabIndex = 7;
        // 
        // lblClass
        // 
        lblClass.AutoEllipsis = true;
        lblClass.Dock = System.Windows.Forms.DockStyle.Fill;
        lblClass.Location = new System.Drawing.Point(3, 232);
        lblClass.Margin = new System.Windows.Forms.Padding(3, 0, 3, 9);
        lblClass.Name = "lblClass";
        lblClass.Padding = new System.Windows.Forms.Padding(8, 0, 12, 0);
        lblClass.Size = new System.Drawing.Size(277, 33);
        lblClass.TabIndex = 8;
        lblClass.Text = "类别";
        lblClass.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        // 
        // txtClass
        // 
        txtClass.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        txtClass.Dock = System.Windows.Forms.DockStyle.Fill;
        txtClass.Location = new System.Drawing.Point(289, 237);
        txtClass.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
        txtClass.Name = "txtClass";
        txtClass.ReadOnly = true;
        txtClass.Size = new System.Drawing.Size(649, 23);
        txtClass.TabIndex = 9;
        // 
        // lblManufacturer
        // 
        lblManufacturer.AutoEllipsis = true;
        lblManufacturer.Dock = System.Windows.Forms.DockStyle.Fill;
        lblManufacturer.Location = new System.Drawing.Point(3, 274);
        lblManufacturer.Margin = new System.Windows.Forms.Padding(3, 0, 3, 9);
        lblManufacturer.Name = "lblManufacturer";
        lblManufacturer.Padding = new System.Windows.Forms.Padding(8, 0, 12, 0);
        lblManufacturer.Size = new System.Drawing.Size(277, 33);
        lblManufacturer.TabIndex = 10;
        lblManufacturer.Text = "厂商";
        lblManufacturer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        // 
        // txtManufacturer
        // 
        txtManufacturer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        txtManufacturer.Dock = System.Windows.Forms.DockStyle.Fill;
        txtManufacturer.Location = new System.Drawing.Point(289, 279);
        txtManufacturer.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
        txtManufacturer.Name = "txtManufacturer";
        txtManufacturer.ReadOnly = true;
        txtManufacturer.Size = new System.Drawing.Size(649, 23);
        txtManufacturer.TabIndex = 11;
        // 
        // lblService
        // 
        lblService.AutoEllipsis = true;
        lblService.Dock = System.Windows.Forms.DockStyle.Fill;
        lblService.Location = new System.Drawing.Point(3, 316);
        lblService.Margin = new System.Windows.Forms.Padding(3, 0, 3, 9);
        lblService.Name = "lblService";
        lblService.Padding = new System.Windows.Forms.Padding(8, 0, 12, 0);
        lblService.Size = new System.Drawing.Size(277, 33);
        lblService.TabIndex = 12;
        lblService.Text = "服务";
        lblService.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        // 
        // txtService
        // 
        txtService.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        txtService.Dock = System.Windows.Forms.DockStyle.Fill;
        txtService.Location = new System.Drawing.Point(289, 321);
        txtService.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
        txtService.Name = "txtService";
        txtService.ReadOnly = true;
        txtService.Size = new System.Drawing.Size(649, 23);
        txtService.TabIndex = 13;
        // 
        // lblEnumerator
        // 
        lblEnumerator.AutoEllipsis = true;
        lblEnumerator.Dock = System.Windows.Forms.DockStyle.Fill;
        lblEnumerator.Location = new System.Drawing.Point(3, 358);
        lblEnumerator.Margin = new System.Windows.Forms.Padding(3, 0, 3, 9);
        lblEnumerator.Name = "lblEnumerator";
        lblEnumerator.Padding = new System.Windows.Forms.Padding(8, 0, 12, 0);
        lblEnumerator.Size = new System.Drawing.Size(277, 33);
        lblEnumerator.TabIndex = 14;
        lblEnumerator.Text = "枚举器";
        lblEnumerator.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        // 
        // txtEnumerator
        // 
        txtEnumerator.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        txtEnumerator.Dock = System.Windows.Forms.DockStyle.Fill;
        txtEnumerator.Location = new System.Drawing.Point(289, 363);
        txtEnumerator.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
        txtEnumerator.Name = "txtEnumerator";
        txtEnumerator.ReadOnly = true;
        txtEnumerator.Size = new System.Drawing.Size(649, 23);
        txtEnumerator.TabIndex = 15;
        // 
        // lblDriver
        // 
        lblDriver.AutoEllipsis = true;
        lblDriver.Dock = System.Windows.Forms.DockStyle.Fill;
        lblDriver.Location = new System.Drawing.Point(3, 400);
        lblDriver.Margin = new System.Windows.Forms.Padding(3, 0, 3, 9);
        lblDriver.Name = "lblDriver";
        lblDriver.Padding = new System.Windows.Forms.Padding(8, 0, 12, 0);
        lblDriver.Size = new System.Drawing.Size(277, 33);
        lblDriver.TabIndex = 16;
        lblDriver.Text = "驱动键";
        lblDriver.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        // 
        // txtDriver
        // 
        txtDriver.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        txtDriver.Dock = System.Windows.Forms.DockStyle.Fill;
        txtDriver.Location = new System.Drawing.Point(289, 405);
        txtDriver.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
        txtDriver.Name = "txtDriver";
        txtDriver.ReadOnly = true;
        txtDriver.Size = new System.Drawing.Size(649, 23);
        txtDriver.TabIndex = 17;
        // 
        // label2
        // 
        label2.Dock = System.Windows.Forms.DockStyle.Fill;
        label2.Location = new System.Drawing.Point(286, 442);
        label2.Name = "label2";
        label2.Size = new System.Drawing.Size(655, 20);
        label2.TabIndex = 19;
        label2.Text = "关注塔菲谢谢喵~";
        // 
        // label1
        // 
        label1.Location = new System.Drawing.Point(3, 442);
        label1.Name = "label1";
        label1.Size = new System.Drawing.Size(277, 20);
        label1.TabIndex = 18;
        label1.Text = "作者: Rainbow SPY";
        label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        // 
        // label3
        // 
        label3.Location = new System.Drawing.Point(3, 462);
        label3.Name = "label3";
        label3.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
        label3.Size = new System.Drawing.Size(277, 22);
        label3.TabIndex = 20;
        label3.Text = "项目地址: ";
        label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        // 
        // linkLabel1
        // 
        linkLabel1.ActiveLinkColor = System.Drawing.SystemColors.Highlight;
        linkLabel1.AutoSize = true;
        linkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
        linkLabel1.LinkColor = System.Drawing.SystemColors.Highlight;
        linkLabel1.Location = new System.Drawing.Point(286, 462);
        linkLabel1.Name = "linkLabel1";
        linkLabel1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
        linkLabel1.Size = new System.Drawing.Size(338, 22);
        linkLabel1.TabIndex = 21;
        linkLabel1.TabStop = true;
        linkLabel1.Text = "https://github.com/Rainbow-SPY/PnpFriendlyNameEditor";
        linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        linkLabel1.VisitedLinkColor = System.Drawing.SystemColors.Highlight;
        linkLabel1.LinkClicked += linkLabel1_LinkClicked;
        linkLabel1.MouseDoubleClick += linkLabel1_MouseDoubleClick;
        // 
        // lblHint
        // 
        lblHint.Dock = System.Windows.Forms.DockStyle.Bottom;
        lblHint.ForeColor = System.Drawing.Color.DimGray;
        lblHint.Location = new System.Drawing.Point(20, 581);
        lblHint.Name = "lblHint";
        lblHint.Padding = new System.Windows.Forms.Padding(0, 12, 0, 0);
        lblHint.Size = new System.Drawing.Size(944, 108);
        lblHint.TabIndex = 2;
        lblHint.Text = ("说明：本工具只尝试修改 SPDRP_FRIENDLYNAME。不要用它来改 HardwareID、CompatibleIDs、Service、ClassGUID、" + "UpperFilters、LowerFilters 等关键项。\r\n保存后如果设备管理器没有立刻显示变化，可以刷新、重新插拔设备，或者重启。部分设备的显示名可能会" + "被驱动/INF/重新枚举覆盖。");
        // 
        // lblTitle
        // 
        lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
        lblTitle.Font = new System.Drawing.Font("Microsoft YaHei UI", 14F, System.Drawing.FontStyle.Bold);
        lblTitle.Location = new System.Drawing.Point(20, 16);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new System.Drawing.Size(944, 44);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "设备属性";
        lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        // 
        // Form1
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(1200, 760);
        Controls.Add(rootLayout);
        Icon = ((System.Drawing.Icon)resources.GetObject("$this.Icon"));
        MinimumSize = new System.Drawing.Size(900, 600);
        StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
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
