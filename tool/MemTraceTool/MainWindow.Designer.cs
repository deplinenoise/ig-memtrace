namespace MemTraceTool
{
  partial class MainWindow
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
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
      this.m_TraceList = new BrightIdeasSoftware.FastObjectListView();
      this.m_FileNameColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.m_StatusColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.m_PlatformColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.m_ExeNameColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.m_WarningCountColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.m_WireSizeColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.m_OutputSizeColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.m_EventCountColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.m_ResolvedColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.m_SourceColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.m_MenuStrip = new System.Windows.Forms.MenuStrip();
      this.m_FileMenu = new System.Windows.Forms.ToolStripMenuItem();
      this.m_QuitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.m_ChangeOptionsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.m_TraceFileLabel = new System.Windows.Forms.Label();
      this.toolStrip1 = new System.Windows.Forms.ToolStrip();
      this.m_HeapViewButton = new System.Windows.Forms.ToolStripButton();
      this.m_SearchButton = new System.Windows.Forms.ToolStripButton();
      this.m_DeltaButton = new System.Windows.Forms.ToolStripButton();
      this.m_ResolveSymbolsButton = new System.Windows.Forms.ToolStripButton();
      this.m_NodeContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.m_StopRecordingMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.m_AddUserTraceMarkMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.m_ShowWarningsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      ((System.ComponentModel.ISupportInitialize)(this.m_TraceList)).BeginInit();
      this.m_MenuStrip.SuspendLayout();
      this.toolStrip1.SuspendLayout();
      this.m_NodeContextMenu.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_TraceList
      // 
      this.m_TraceList.AllColumns.Add(this.m_FileNameColumn);
      this.m_TraceList.AllColumns.Add(this.m_StatusColumn);
      this.m_TraceList.AllColumns.Add(this.m_PlatformColumn);
      this.m_TraceList.AllColumns.Add(this.m_ExeNameColumn);
      this.m_TraceList.AllColumns.Add(this.m_WarningCountColumn);
      this.m_TraceList.AllColumns.Add(this.m_WireSizeColumn);
      this.m_TraceList.AllColumns.Add(this.m_OutputSizeColumn);
      this.m_TraceList.AllColumns.Add(this.m_EventCountColumn);
      this.m_TraceList.AllColumns.Add(this.m_ResolvedColumn);
      this.m_TraceList.AllColumns.Add(this.m_SourceColumn);
      this.m_TraceList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.m_TraceList.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
      this.m_TraceList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.m_FileNameColumn,
            this.m_StatusColumn,
            this.m_PlatformColumn,
            this.m_ExeNameColumn,
            this.m_WarningCountColumn,
            this.m_WireSizeColumn,
            this.m_OutputSizeColumn,
            this.m_EventCountColumn,
            this.m_ResolvedColumn,
            this.m_SourceColumn});
      this.m_TraceList.FullRowSelect = true;
      this.m_TraceList.Location = new System.Drawing.Point(12, 71);
      this.m_TraceList.Name = "m_TraceList";
      this.m_TraceList.ShowGroups = false;
      this.m_TraceList.Size = new System.Drawing.Size(927, 519);
      this.m_TraceList.TabIndex = 1;
      this.m_TraceList.UseCompatibleStateImageBehavior = false;
      this.m_TraceList.View = System.Windows.Forms.View.Details;
      this.m_TraceList.VirtualMode = true;
      this.m_TraceList.CellRightClick += new System.EventHandler<BrightIdeasSoftware.CellRightClickEventArgs>(this.OnCellRightClick);
      this.m_TraceList.SelectedIndexChanged += new System.EventHandler(this.TraceList_ItemSelectionChanged);
      // 
      // m_FileNameColumn
      // 
      this.m_FileNameColumn.AspectName = "FileName";
      this.m_FileNameColumn.CellPadding = null;
      this.m_FileNameColumn.Groupable = false;
      this.m_FileNameColumn.Text = "File Name";
      this.m_FileNameColumn.Width = 187;
      // 
      // m_StatusColumn
      // 
      this.m_StatusColumn.AspectName = "Status";
      this.m_StatusColumn.CellPadding = null;
      this.m_StatusColumn.IsEditable = false;
      this.m_StatusColumn.Text = "Status";
      // 
      // m_PlatformColumn
      // 
      this.m_PlatformColumn.AspectName = "PlatformName";
      this.m_PlatformColumn.CellPadding = null;
      this.m_PlatformColumn.IsEditable = false;
      this.m_PlatformColumn.Text = "Platform";
      // 
      // m_ExeNameColumn
      // 
      this.m_ExeNameColumn.AspectName = "ExecutableName";
      this.m_ExeNameColumn.CellPadding = null;
      this.m_ExeNameColumn.IsEditable = false;
      this.m_ExeNameColumn.Text = "Executable";
      // 
      // m_WarningCountColumn
      // 
      this.m_WarningCountColumn.AspectName = "WarningCount";
      this.m_WarningCountColumn.AspectToStringFormat = "{0:n0}";
      this.m_WarningCountColumn.CellPadding = null;
      this.m_WarningCountColumn.IsEditable = false;
      this.m_WarningCountColumn.Text = "Warnings";
      // 
      // m_WireSizeColumn
      // 
      this.m_WireSizeColumn.AspectName = "WireSizeBytes";
      this.m_WireSizeColumn.CellPadding = null;
      this.m_WireSizeColumn.Groupable = false;
      this.m_WireSizeColumn.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.m_WireSizeColumn.IsEditable = false;
      this.m_WireSizeColumn.Text = "Wire Size";
      this.m_WireSizeColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.m_WireSizeColumn.Width = 72;
      // 
      // m_OutputSizeColumn
      // 
      this.m_OutputSizeColumn.AspectName = "OutputSizeBytes";
      this.m_OutputSizeColumn.CellPadding = null;
      this.m_OutputSizeColumn.Groupable = false;
      this.m_OutputSizeColumn.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.m_OutputSizeColumn.IsEditable = false;
      this.m_OutputSizeColumn.Text = "Output Size";
      this.m_OutputSizeColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.m_OutputSizeColumn.Width = 106;
      // 
      // m_EventCountColumn
      // 
      this.m_EventCountColumn.AspectName = "EventCount";
      this.m_EventCountColumn.AspectToStringFormat = "{0:n0}";
      this.m_EventCountColumn.CellPadding = null;
      this.m_EventCountColumn.Groupable = false;
      this.m_EventCountColumn.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.m_EventCountColumn.IsEditable = false;
      this.m_EventCountColumn.Text = "Event Count";
      this.m_EventCountColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.m_EventCountColumn.Width = 85;
      // 
      // m_ResolvedColumn
      // 
      this.m_ResolvedColumn.AspectName = "IsResolved";
      this.m_ResolvedColumn.CellPadding = null;
      this.m_ResolvedColumn.CheckBoxes = true;
      this.m_ResolvedColumn.IsEditable = false;
      this.m_ResolvedColumn.Searchable = false;
      this.m_ResolvedColumn.Text = "Resolved";
      // 
      // m_SourceColumn
      // 
      this.m_SourceColumn.AspectName = "SourceMachine";
      this.m_SourceColumn.CellPadding = null;
      this.m_SourceColumn.IsEditable = false;
      this.m_SourceColumn.Text = "Machine";
      this.m_SourceColumn.Width = 93;
      // 
      // m_MenuStrip
      // 
      this.m_MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_FileMenu,
            this.optionsToolStripMenuItem});
      this.m_MenuStrip.Location = new System.Drawing.Point(0, 0);
      this.m_MenuStrip.Name = "m_MenuStrip";
      this.m_MenuStrip.Size = new System.Drawing.Size(951, 24);
      this.m_MenuStrip.TabIndex = 2;
      this.m_MenuStrip.Text = "menuStrip1";
      // 
      // m_FileMenu
      // 
      this.m_FileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_QuitMenuItem});
      this.m_FileMenu.Name = "m_FileMenu";
      this.m_FileMenu.Size = new System.Drawing.Size(37, 20);
      this.m_FileMenu.Text = "&File";
      // 
      // m_QuitMenuItem
      // 
      this.m_QuitMenuItem.Name = "m_QuitMenuItem";
      this.m_QuitMenuItem.Size = new System.Drawing.Size(97, 22);
      this.m_QuitMenuItem.Text = "&Quit";
      this.m_QuitMenuItem.Click += new System.EventHandler(this.m_QuitMenuItem_Click);
      // 
      // optionsToolStripMenuItem
      // 
      this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_ChangeOptionsMenuItem});
      this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
      this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
      this.optionsToolStripMenuItem.Text = "&Options";
      // 
      // m_ChangeOptionsMenuItem
      // 
      this.m_ChangeOptionsMenuItem.Name = "m_ChangeOptionsMenuItem";
      this.m_ChangeOptionsMenuItem.Size = new System.Drawing.Size(160, 22);
      this.m_ChangeOptionsMenuItem.Text = "Change &Options";
      this.m_ChangeOptionsMenuItem.Click += new System.EventHandler(this.ChangeOptionsMenuItem_Click);
      // 
      // m_TraceFileLabel
      // 
      this.m_TraceFileLabel.AutoSize = true;
      this.m_TraceFileLabel.Location = new System.Drawing.Point(9, 55);
      this.m_TraceFileLabel.Name = "m_TraceFileLabel";
      this.m_TraceFileLabel.Size = new System.Drawing.Size(59, 13);
      this.m_TraceFileLabel.TabIndex = 4;
      this.m_TraceFileLabel.Text = "Trace Files";
      // 
      // toolStrip1
      // 
      this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_HeapViewButton,
            this.m_SearchButton,
            this.m_DeltaButton,
            this.m_ResolveSymbolsButton});
      this.toolStrip1.Location = new System.Drawing.Point(0, 24);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Size = new System.Drawing.Size(951, 25);
      this.toolStrip1.TabIndex = 5;
      this.toolStrip1.Text = "toolStrip1";
      // 
      // m_HeapViewButton
      // 
      this.m_HeapViewButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.m_HeapViewButton.Enabled = false;
      this.m_HeapViewButton.Image = ((System.Drawing.Image)(resources.GetObject("m_HeapViewButton.Image")));
      this.m_HeapViewButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.m_HeapViewButton.Name = "m_HeapViewButton";
      this.m_HeapViewButton.Size = new System.Drawing.Size(23, 22);
      this.m_HeapViewButton.Text = "Heap View";
      this.m_HeapViewButton.ToolTipText = "Open Heap View";
      this.m_HeapViewButton.Click += new System.EventHandler(this.HeapViewButton_Click);
      // 
      // m_SearchButton
      // 
      this.m_SearchButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.m_SearchButton.Enabled = false;
      this.m_SearchButton.Image = ((System.Drawing.Image)(resources.GetObject("m_SearchButton.Image")));
      this.m_SearchButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.m_SearchButton.Name = "m_SearchButton";
      this.m_SearchButton.Size = new System.Drawing.Size(23, 22);
      this.m_SearchButton.Text = "Range Search";
      this.m_SearchButton.Click += new System.EventHandler(this.SearchButton_Click);
      // 
      // m_DeltaButton
      // 
      this.m_DeltaButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.m_DeltaButton.Enabled = false;
      this.m_DeltaButton.Image = ((System.Drawing.Image)(resources.GetObject("m_DeltaButton.Image")));
      this.m_DeltaButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.m_DeltaButton.Name = "m_DeltaButton";
      this.m_DeltaButton.Size = new System.Drawing.Size(23, 22);
      this.m_DeltaButton.Text = "Delta View";
      this.m_DeltaButton.Click += new System.EventHandler(this.DeltaButton_Click);
      // 
      // m_ResolveSymbolsButton
      // 
      this.m_ResolveSymbolsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.m_ResolveSymbolsButton.Enabled = false;
      this.m_ResolveSymbolsButton.Image = ((System.Drawing.Image)(resources.GetObject("m_ResolveSymbolsButton.Image")));
      this.m_ResolveSymbolsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.m_ResolveSymbolsButton.Name = "m_ResolveSymbolsButton";
      this.m_ResolveSymbolsButton.Size = new System.Drawing.Size(23, 22);
      this.m_ResolveSymbolsButton.Text = "Resolve Symbols";
      this.m_ResolveSymbolsButton.Click += new System.EventHandler(this.ResolveSymbolsButton_Click);
      // 
      // m_NodeContextMenu
      // 
      this.m_NodeContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_StopRecordingMenuItem,
            this.m_AddUserTraceMarkMenuItem,
            this.m_ShowWarningsMenuItem});
      this.m_NodeContextMenu.Name = "m_NodeContextMenu";
      this.m_NodeContextMenu.Size = new System.Drawing.Size(185, 92);
      // 
      // m_StopRecordingMenuItem
      // 
      this.m_StopRecordingMenuItem.Name = "m_StopRecordingMenuItem";
      this.m_StopRecordingMenuItem.Size = new System.Drawing.Size(184, 22);
      this.m_StopRecordingMenuItem.Text = "&Stop Recording";
      this.m_StopRecordingMenuItem.Click += new System.EventHandler(this.StopRecordingMenuItem_Click);
      // 
      // m_AddUserTraceMarkMenuItem
      // 
      this.m_AddUserTraceMarkMenuItem.Name = "m_AddUserTraceMarkMenuItem";
      this.m_AddUserTraceMarkMenuItem.Size = new System.Drawing.Size(184, 22);
      this.m_AddUserTraceMarkMenuItem.Text = "Add User Trace Mark";
      this.m_AddUserTraceMarkMenuItem.Click += new System.EventHandler(this.AddUserTraceMarkMenuItem_Click);
      // 
      // m_ShowWarningsMenuItem
      // 
      this.m_ShowWarningsMenuItem.Name = "m_ShowWarningsMenuItem";
      this.m_ShowWarningsMenuItem.Size = new System.Drawing.Size(184, 22);
      this.m_ShowWarningsMenuItem.Text = "Show Warnings";
      this.m_ShowWarningsMenuItem.Click += new System.EventHandler(this.ShowWarningsMenuItem_Click);
      // 
      // MainWindow
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(951, 602);
      this.Controls.Add(this.toolStrip1);
      this.Controls.Add(this.m_TraceFileLabel);
      this.Controls.Add(this.m_TraceList);
      this.Controls.Add(this.m_MenuStrip);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MainMenuStrip = this.m_MenuStrip;
      this.Name = "MainWindow";
      this.Text = "Memory Trace Tool";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
      ((System.ComponentModel.ISupportInitialize)(this.m_TraceList)).EndInit();
      this.m_MenuStrip.ResumeLayout(false);
      this.m_MenuStrip.PerformLayout();
      this.toolStrip1.ResumeLayout(false);
      this.toolStrip1.PerformLayout();
      this.m_NodeContextMenu.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private BrightIdeasSoftware.FastObjectListView m_TraceList;
    private System.Windows.Forms.MenuStrip m_MenuStrip;
    private System.Windows.Forms.ToolStripMenuItem m_FileMenu;
    private System.Windows.Forms.ToolStripMenuItem m_QuitMenuItem;
    private System.Windows.Forms.Label m_TraceFileLabel;
    private BrightIdeasSoftware.OLVColumn m_FileNameColumn;
    private BrightIdeasSoftware.OLVColumn m_PlatformColumn;
    private BrightIdeasSoftware.OLVColumn m_StatusColumn;
    private BrightIdeasSoftware.OLVColumn m_WireSizeColumn;
    private BrightIdeasSoftware.OLVColumn m_EventCountColumn;
    private BrightIdeasSoftware.OLVColumn m_ResolvedColumn;
    private BrightIdeasSoftware.OLVColumn m_SourceColumn;
    private BrightIdeasSoftware.OLVColumn m_OutputSizeColumn;
    private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem m_ChangeOptionsMenuItem;
    private System.Windows.Forms.ToolStrip toolStrip1;
    private System.Windows.Forms.ToolStripButton m_HeapViewButton;
    private System.Windows.Forms.ToolStripButton m_ResolveSymbolsButton;
    private System.Windows.Forms.ContextMenuStrip m_NodeContextMenu;
    private System.Windows.Forms.ToolStripMenuItem m_StopRecordingMenuItem;
    private System.Windows.Forms.ToolStripButton m_SearchButton;
    private System.Windows.Forms.ToolStripButton m_DeltaButton;
    private BrightIdeasSoftware.OLVColumn m_ExeNameColumn;
    private BrightIdeasSoftware.OLVColumn m_WarningCountColumn;
    private System.Windows.Forms.ToolStripMenuItem m_AddUserTraceMarkMenuItem;
    private System.Windows.Forms.ToolStripMenuItem m_ShowWarningsMenuItem;
  }
}

