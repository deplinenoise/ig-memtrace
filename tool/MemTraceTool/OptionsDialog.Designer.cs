namespace MemTraceTool
{
  partial class OptionsDialog
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsDialog));
      this.m_OKButton = new System.Windows.Forms.Button();
      this.m_CancelButton = new System.Windows.Forms.Button();
      this.m_SuppressionPage = new System.Windows.Forms.TabPage();
      this.m_EnableSuppression = new System.Windows.Forms.CheckBox();
      this.m_SuppressedSymbolList = new BrightIdeasSoftware.ObjectListView();
      this.m_SymbolColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.m_RemoveSymbolButton = new System.Windows.Forms.Button();
      this.m_AddSymbolButton = new System.Windows.Forms.Button();
      this.m_Tabs = new System.Windows.Forms.TabControl();
      this.m_SymbolRemapping = new System.Windows.Forms.TabPage();
      this.m_RemoveMappingButton = new System.Windows.Forms.Button();
      this.m_AddMappingButton = new System.Windows.Forms.Button();
      this.m_SymbolMappingList = new BrightIdeasSoftware.FastObjectListView();
      this.m_MapPlatformColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.m_MapSourceColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.m_MapDestColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.m_SymbolPathsTab = new System.Windows.Forms.TabPage();
      this.m_RemovePathButton = new System.Windows.Forms.Button();
      this.m_AddPathButton = new System.Windows.Forms.Button();
      this.m_SymbolPathsList = new BrightIdeasSoftware.FastObjectListView();
      this.m_SymPathColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.m_NetworkTab = new System.Windows.Forms.TabPage();
      this.m_BindPort = new System.Windows.Forms.TextBox();
      this.m_BindAddress = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.m_FilesTab = new System.Windows.Forms.TabPage();
      this.m_BrowseButton = new System.Windows.Forms.Button();
      this.m_TraceDirectory = new System.Windows.Forms.TextBox();
      this.m_CaptureDirectoryLabel = new System.Windows.Forms.Label();
      this.m_SuppressionPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_SuppressedSymbolList)).BeginInit();
      this.m_Tabs.SuspendLayout();
      this.m_SymbolRemapping.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_SymbolMappingList)).BeginInit();
      this.m_SymbolPathsTab.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_SymbolPathsList)).BeginInit();
      this.m_NetworkTab.SuspendLayout();
      this.m_FilesTab.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_OKButton
      // 
      this.m_OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.m_OKButton.Location = new System.Drawing.Point(396, 391);
      this.m_OKButton.Name = "m_OKButton";
      this.m_OKButton.Size = new System.Drawing.Size(101, 25);
      this.m_OKButton.TabIndex = 1;
      this.m_OKButton.Text = "OK";
      this.m_OKButton.UseVisualStyleBackColor = true;
      // 
      // m_CancelButton
      // 
      this.m_CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.m_CancelButton.Location = new System.Drawing.Point(503, 391);
      this.m_CancelButton.Name = "m_CancelButton";
      this.m_CancelButton.Size = new System.Drawing.Size(101, 25);
      this.m_CancelButton.TabIndex = 2;
      this.m_CancelButton.Text = "Cancel";
      this.m_CancelButton.UseVisualStyleBackColor = true;
      // 
      // m_SuppressionPage
      // 
      this.m_SuppressionPage.Controls.Add(this.m_EnableSuppression);
      this.m_SuppressionPage.Controls.Add(this.m_SuppressedSymbolList);
      this.m_SuppressionPage.Controls.Add(this.m_RemoveSymbolButton);
      this.m_SuppressionPage.Controls.Add(this.m_AddSymbolButton);
      this.m_SuppressionPage.Location = new System.Drawing.Point(4, 22);
      this.m_SuppressionPage.Name = "m_SuppressionPage";
      this.m_SuppressionPage.Padding = new System.Windows.Forms.Padding(3);
      this.m_SuppressionPage.Size = new System.Drawing.Size(588, 347);
      this.m_SuppressionPage.TabIndex = 1;
      this.m_SuppressionPage.Text = "Symbol Suppressions";
      this.m_SuppressionPage.UseVisualStyleBackColor = true;
      // 
      // m_EnableSuppression
      // 
      this.m_EnableSuppression.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.m_EnableSuppression.AutoSize = true;
      this.m_EnableSuppression.Location = new System.Drawing.Point(9, 322);
      this.m_EnableSuppression.Name = "m_EnableSuppression";
      this.m_EnableSuppression.Size = new System.Drawing.Size(200, 17);
      this.m_EnableSuppression.TabIndex = 6;
      this.m_EnableSuppression.Text = "Enable Symbol Suppression (Default)";
      this.m_EnableSuppression.UseVisualStyleBackColor = true;
      this.m_EnableSuppression.CheckedChanged += new System.EventHandler(this.EnableSuppression_CheckedChanged);
      // 
      // m_SuppressedSymbolList
      // 
      this.m_SuppressedSymbolList.AllColumns.Add(this.m_SymbolColumn);
      this.m_SuppressedSymbolList.AlternateRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
      this.m_SuppressedSymbolList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.m_SuppressedSymbolList.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
      this.m_SuppressedSymbolList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.m_SymbolColumn});
      this.m_SuppressedSymbolList.FullRowSelect = true;
      this.m_SuppressedSymbolList.IsSearchOnSortColumn = false;
      this.m_SuppressedSymbolList.Location = new System.Drawing.Point(7, 7);
      this.m_SuppressedSymbolList.Name = "m_SuppressedSymbolList";
      this.m_SuppressedSymbolList.Size = new System.Drawing.Size(467, 310);
      this.m_SuppressedSymbolList.TabIndex = 4;
      this.m_SuppressedSymbolList.UseAlternatingBackColors = true;
      this.m_SuppressedSymbolList.UseCompatibleStateImageBehavior = false;
      this.m_SuppressedSymbolList.UseOverlays = false;
      this.m_SuppressedSymbolList.View = System.Windows.Forms.View.Details;
      this.m_SuppressedSymbolList.SelectedIndexChanged += new System.EventHandler(this.SuppressedSymbolList_SelectedIndexChanged);
      // 
      // m_SymbolColumn
      // 
      this.m_SymbolColumn.AspectName = "";
      this.m_SymbolColumn.CellPadding = null;
      this.m_SymbolColumn.FillsFreeSpace = true;
      this.m_SymbolColumn.Groupable = false;
      this.m_SymbolColumn.Searchable = false;
      this.m_SymbolColumn.Text = "Symbol Name";
      this.m_SymbolColumn.Width = 439;
      // 
      // m_RemoveSymbolButton
      // 
      this.m_RemoveSymbolButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.m_RemoveSymbolButton.Enabled = false;
      this.m_RemoveSymbolButton.Location = new System.Drawing.Point(480, 37);
      this.m_RemoveSymbolButton.Name = "m_RemoveSymbolButton";
      this.m_RemoveSymbolButton.Size = new System.Drawing.Size(102, 25);
      this.m_RemoveSymbolButton.TabIndex = 3;
      this.m_RemoveSymbolButton.Text = "Remove Symbol";
      this.m_RemoveSymbolButton.UseVisualStyleBackColor = true;
      this.m_RemoveSymbolButton.Click += new System.EventHandler(this.RemoveSymbolButton_Click);
      // 
      // m_AddSymbolButton
      // 
      this.m_AddSymbolButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.m_AddSymbolButton.Location = new System.Drawing.Point(480, 6);
      this.m_AddSymbolButton.Name = "m_AddSymbolButton";
      this.m_AddSymbolButton.Size = new System.Drawing.Size(102, 25);
      this.m_AddSymbolButton.TabIndex = 2;
      this.m_AddSymbolButton.Text = "Add Symbol";
      this.m_AddSymbolButton.UseVisualStyleBackColor = true;
      this.m_AddSymbolButton.Click += new System.EventHandler(this.AddSymbolButton_Click);
      // 
      // m_Tabs
      // 
      this.m_Tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.m_Tabs.Controls.Add(this.m_SuppressionPage);
      this.m_Tabs.Controls.Add(this.m_SymbolRemapping);
      this.m_Tabs.Controls.Add(this.m_SymbolPathsTab);
      this.m_Tabs.Controls.Add(this.m_NetworkTab);
      this.m_Tabs.Controls.Add(this.m_FilesTab);
      this.m_Tabs.Location = new System.Drawing.Point(12, 12);
      this.m_Tabs.Name = "m_Tabs";
      this.m_Tabs.SelectedIndex = 0;
      this.m_Tabs.Size = new System.Drawing.Size(596, 373);
      this.m_Tabs.TabIndex = 0;
      // 
      // m_SymbolRemapping
      // 
      this.m_SymbolRemapping.Controls.Add(this.m_RemoveMappingButton);
      this.m_SymbolRemapping.Controls.Add(this.m_AddMappingButton);
      this.m_SymbolRemapping.Controls.Add(this.m_SymbolMappingList);
      this.m_SymbolRemapping.Location = new System.Drawing.Point(4, 22);
      this.m_SymbolRemapping.Name = "m_SymbolRemapping";
      this.m_SymbolRemapping.Padding = new System.Windows.Forms.Padding(3);
      this.m_SymbolRemapping.Size = new System.Drawing.Size(588, 347);
      this.m_SymbolRemapping.TabIndex = 4;
      this.m_SymbolRemapping.Text = "Symbol Remapping";
      this.m_SymbolRemapping.UseVisualStyleBackColor = true;
      // 
      // m_RemoveMappingButton
      // 
      this.m_RemoveMappingButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.m_RemoveMappingButton.Enabled = false;
      this.m_RemoveMappingButton.Location = new System.Drawing.Point(480, 37);
      this.m_RemoveMappingButton.Name = "m_RemoveMappingButton";
      this.m_RemoveMappingButton.Size = new System.Drawing.Size(102, 25);
      this.m_RemoveMappingButton.TabIndex = 5;
      this.m_RemoveMappingButton.Text = "Remove Mapping";
      this.m_RemoveMappingButton.UseVisualStyleBackColor = true;
      this.m_RemoveMappingButton.Click += new System.EventHandler(this.OnRemoveMapping);
      // 
      // m_AddMappingButton
      // 
      this.m_AddMappingButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.m_AddMappingButton.Location = new System.Drawing.Point(480, 6);
      this.m_AddMappingButton.Name = "m_AddMappingButton";
      this.m_AddMappingButton.Size = new System.Drawing.Size(102, 25);
      this.m_AddMappingButton.TabIndex = 4;
      this.m_AddMappingButton.Text = "Add Mapping";
      this.m_AddMappingButton.UseVisualStyleBackColor = true;
      this.m_AddMappingButton.Click += new System.EventHandler(this.OnAddSymbolMapping);
      // 
      // m_SymbolMappingList
      // 
      this.m_SymbolMappingList.AllColumns.Add(this.m_MapPlatformColumn);
      this.m_SymbolMappingList.AllColumns.Add(this.m_MapSourceColumn);
      this.m_SymbolMappingList.AllColumns.Add(this.m_MapDestColumn);
      this.m_SymbolMappingList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.m_SymbolMappingList.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
      this.m_SymbolMappingList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.m_MapPlatformColumn,
            this.m_MapSourceColumn,
            this.m_MapDestColumn});
      this.m_SymbolMappingList.FullRowSelect = true;
      this.m_SymbolMappingList.Location = new System.Drawing.Point(7, 7);
      this.m_SymbolMappingList.Name = "m_SymbolMappingList";
      this.m_SymbolMappingList.ShowGroups = false;
      this.m_SymbolMappingList.Size = new System.Drawing.Size(467, 334);
      this.m_SymbolMappingList.TabIndex = 0;
      this.m_SymbolMappingList.UseCompatibleStateImageBehavior = false;
      this.m_SymbolMappingList.View = System.Windows.Forms.View.Details;
      this.m_SymbolMappingList.VirtualMode = true;
      this.m_SymbolMappingList.SelectionChanged += new System.EventHandler(this.OnRemapSelectionChanged);
      // 
      // m_MapPlatformColumn
      // 
      this.m_MapPlatformColumn.AspectName = "Platform";
      this.m_MapPlatformColumn.CellPadding = null;
      this.m_MapPlatformColumn.Text = "Platform";
      this.m_MapPlatformColumn.Width = 59;
      // 
      // m_MapSourceColumn
      // 
      this.m_MapSourceColumn.AspectName = "Path";
      this.m_MapSourceColumn.CellPadding = null;
      this.m_MapSourceColumn.Text = "Path";
      this.m_MapSourceColumn.Width = 136;
      // 
      // m_MapDestColumn
      // 
      this.m_MapDestColumn.AspectName = "ReplacementPath";
      this.m_MapDestColumn.CellPadding = null;
      this.m_MapDestColumn.Text = "Replacement Path";
      this.m_MapDestColumn.Width = 258;
      // 
      // m_SymbolPathsTab
      // 
      this.m_SymbolPathsTab.Controls.Add(this.m_RemovePathButton);
      this.m_SymbolPathsTab.Controls.Add(this.m_AddPathButton);
      this.m_SymbolPathsTab.Controls.Add(this.m_SymbolPathsList);
      this.m_SymbolPathsTab.Location = new System.Drawing.Point(4, 22);
      this.m_SymbolPathsTab.Name = "m_SymbolPathsTab";
      this.m_SymbolPathsTab.Padding = new System.Windows.Forms.Padding(3);
      this.m_SymbolPathsTab.Size = new System.Drawing.Size(588, 347);
      this.m_SymbolPathsTab.TabIndex = 5;
      this.m_SymbolPathsTab.Text = "Symbol Paths";
      this.m_SymbolPathsTab.UseVisualStyleBackColor = true;
      // 
      // m_RemovePathButton
      // 
      this.m_RemovePathButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.m_RemovePathButton.Enabled = false;
      this.m_RemovePathButton.Location = new System.Drawing.Point(480, 37);
      this.m_RemovePathButton.Name = "m_RemovePathButton";
      this.m_RemovePathButton.Size = new System.Drawing.Size(102, 25);
      this.m_RemovePathButton.TabIndex = 8;
      this.m_RemovePathButton.Text = "Remove Path";
      this.m_RemovePathButton.UseVisualStyleBackColor = true;
      this.m_RemovePathButton.Click += new System.EventHandler(this.OnRemoveSymbolPath);
      // 
      // m_AddPathButton
      // 
      this.m_AddPathButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.m_AddPathButton.Location = new System.Drawing.Point(480, 6);
      this.m_AddPathButton.Name = "m_AddPathButton";
      this.m_AddPathButton.Size = new System.Drawing.Size(102, 25);
      this.m_AddPathButton.TabIndex = 7;
      this.m_AddPathButton.Text = "Add Path";
      this.m_AddPathButton.UseVisualStyleBackColor = true;
      this.m_AddPathButton.Click += new System.EventHandler(this.OnAddSymbolPath);
      // 
      // m_SymbolPathsList
      // 
      this.m_SymbolPathsList.AllColumns.Add(this.m_SymPathColumn);
      this.m_SymbolPathsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.m_SymbolPathsList.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
      this.m_SymbolPathsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.m_SymPathColumn});
      this.m_SymbolPathsList.FullRowSelect = true;
      this.m_SymbolPathsList.Location = new System.Drawing.Point(7, 7);
      this.m_SymbolPathsList.Name = "m_SymbolPathsList";
      this.m_SymbolPathsList.ShowGroups = false;
      this.m_SymbolPathsList.Size = new System.Drawing.Size(467, 334);
      this.m_SymbolPathsList.TabIndex = 6;
      this.m_SymbolPathsList.UseCompatibleStateImageBehavior = false;
      this.m_SymbolPathsList.View = System.Windows.Forms.View.Details;
      this.m_SymbolPathsList.VirtualMode = true;
      this.m_SymbolPathsList.SelectedIndexChanged += new System.EventHandler(this.OnSymbolPathSelectionChanged);
      // 
      // m_SymPathColumn
      // 
      this.m_SymPathColumn.AspectName = "";
      this.m_SymPathColumn.CellPadding = null;
      this.m_SymPathColumn.Text = "Symbol Path";
      this.m_SymPathColumn.Width = 450;
      // 
      // m_NetworkTab
      // 
      this.m_NetworkTab.Controls.Add(this.m_BindPort);
      this.m_NetworkTab.Controls.Add(this.m_BindAddress);
      this.m_NetworkTab.Controls.Add(this.label3);
      this.m_NetworkTab.Controls.Add(this.label2);
      this.m_NetworkTab.Location = new System.Drawing.Point(4, 22);
      this.m_NetworkTab.Name = "m_NetworkTab";
      this.m_NetworkTab.Padding = new System.Windows.Forms.Padding(3);
      this.m_NetworkTab.Size = new System.Drawing.Size(588, 347);
      this.m_NetworkTab.TabIndex = 2;
      this.m_NetworkTab.Text = "Network";
      this.m_NetworkTab.UseVisualStyleBackColor = true;
      // 
      // m_BindPort
      // 
      this.m_BindPort.Location = new System.Drawing.Point(105, 43);
      this.m_BindPort.Name = "m_BindPort";
      this.m_BindPort.Size = new System.Drawing.Size(76, 20);
      this.m_BindPort.TabIndex = 3;
      this.m_BindPort.Validating += new System.ComponentModel.CancelEventHandler(this.OnBindPortValidating);
      // 
      // m_BindAddress
      // 
      this.m_BindAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.m_BindAddress.AutoCompleteCustomSource.AddRange(new string[] {
            "127.0.0.1",
            "0.0.0.0"});
      this.m_BindAddress.Location = new System.Drawing.Point(105, 18);
      this.m_BindAddress.Name = "m_BindAddress";
      this.m_BindAddress.Size = new System.Drawing.Size(465, 20);
      this.m_BindAddress.TabIndex = 2;
      this.m_BindAddress.Validating += new System.ComponentModel.CancelEventHandler(this.OnBindAddressValidating);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(16, 46);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(50, 13);
      this.label3.TabIndex = 1;
      this.label3.Text = "Bind Port";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(16, 21);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(69, 13);
      this.label2.TabIndex = 0;
      this.label2.Text = "Bind Address";
      // 
      // m_FilesTab
      // 
      this.m_FilesTab.Controls.Add(this.m_BrowseButton);
      this.m_FilesTab.Controls.Add(this.m_TraceDirectory);
      this.m_FilesTab.Controls.Add(this.m_CaptureDirectoryLabel);
      this.m_FilesTab.Location = new System.Drawing.Point(4, 22);
      this.m_FilesTab.Name = "m_FilesTab";
      this.m_FilesTab.Size = new System.Drawing.Size(588, 347);
      this.m_FilesTab.TabIndex = 3;
      this.m_FilesTab.Text = "Files";
      this.m_FilesTab.UseVisualStyleBackColor = true;
      // 
      // m_BrowseButton
      // 
      this.m_BrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.m_BrowseButton.Location = new System.Drawing.Point(504, 32);
      this.m_BrowseButton.Name = "m_BrowseButton";
      this.m_BrowseButton.Size = new System.Drawing.Size(75, 23);
      this.m_BrowseButton.TabIndex = 2;
      this.m_BrowseButton.Text = "Browse...";
      this.m_BrowseButton.UseVisualStyleBackColor = true;
      this.m_BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
      // 
      // m_TraceDirectory
      // 
      this.m_TraceDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.m_TraceDirectory.Location = new System.Drawing.Point(17, 34);
      this.m_TraceDirectory.Name = "m_TraceDirectory";
      this.m_TraceDirectory.Size = new System.Drawing.Size(481, 20);
      this.m_TraceDirectory.TabIndex = 1;
      this.m_TraceDirectory.Validating += new System.ComponentModel.CancelEventHandler(this.OnTraceDirectoryValidating);
      // 
      // m_CaptureDirectoryLabel
      // 
      this.m_CaptureDirectoryLabel.AutoSize = true;
      this.m_CaptureDirectoryLabel.Location = new System.Drawing.Point(14, 17);
      this.m_CaptureDirectoryLabel.Name = "m_CaptureDirectoryLabel";
      this.m_CaptureDirectoryLabel.Size = new System.Drawing.Size(120, 13);
      this.m_CaptureDirectoryLabel.TabIndex = 0;
      this.m_CaptureDirectoryLabel.Text = "Trace Capture Directory";
      // 
      // OptionsDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(620, 428);
      this.Controls.Add(this.m_CancelButton);
      this.Controls.Add(this.m_OKButton);
      this.Controls.Add(this.m_Tabs);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "OptionsDialog";
      this.Text = "Options";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
      this.m_SuppressionPage.ResumeLayout(false);
      this.m_SuppressionPage.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_SuppressedSymbolList)).EndInit();
      this.m_Tabs.ResumeLayout(false);
      this.m_SymbolRemapping.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_SymbolMappingList)).EndInit();
      this.m_SymbolPathsTab.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_SymbolPathsList)).EndInit();
      this.m_NetworkTab.ResumeLayout(false);
      this.m_NetworkTab.PerformLayout();
      this.m_FilesTab.ResumeLayout(false);
      this.m_FilesTab.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button m_OKButton;
    private System.Windows.Forms.Button m_CancelButton;
    private System.Windows.Forms.TabPage m_SuppressionPage;
    private System.Windows.Forms.CheckBox m_EnableSuppression;
    private BrightIdeasSoftware.ObjectListView m_SuppressedSymbolList;
    private BrightIdeasSoftware.OLVColumn m_SymbolColumn;
    private System.Windows.Forms.Button m_RemoveSymbolButton;
    private System.Windows.Forms.Button m_AddSymbolButton;
    private System.Windows.Forms.TabControl m_Tabs;
    private System.Windows.Forms.TabPage m_NetworkTab;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TabPage m_FilesTab;
    private System.Windows.Forms.TextBox m_BindPort;
    private System.Windows.Forms.TextBox m_BindAddress;
    private System.Windows.Forms.Button m_BrowseButton;
    private System.Windows.Forms.TextBox m_TraceDirectory;
    private System.Windows.Forms.Label m_CaptureDirectoryLabel;
    private System.Windows.Forms.TabPage m_SymbolRemapping;
    private System.Windows.Forms.Button m_RemoveMappingButton;
    private System.Windows.Forms.Button m_AddMappingButton;
    private BrightIdeasSoftware.FastObjectListView m_SymbolMappingList;
    private BrightIdeasSoftware.OLVColumn m_MapPlatformColumn;
    private BrightIdeasSoftware.OLVColumn m_MapSourceColumn;
    private BrightIdeasSoftware.OLVColumn m_MapDestColumn;
    private System.Windows.Forms.TabPage m_SymbolPathsTab;
    private System.Windows.Forms.Button m_RemovePathButton;
    private System.Windows.Forms.Button m_AddPathButton;
    private BrightIdeasSoftware.FastObjectListView m_SymbolPathsList;
    private BrightIdeasSoftware.OLVColumn m_SymPathColumn;
  }
}