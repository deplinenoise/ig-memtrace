namespace MemTraceTool
{
  partial class HeapWindow
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HeapWindow));
      this.m_SymbolContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.m_ExcludeSymbolMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.m_RefreshButton = new System.Windows.Forms.Button();
      this.m_Perspective = new System.Windows.Forms.ComboBox();
      this.m_StatusStrip = new System.Windows.Forms.StatusStrip();
      this.m_StripStatus = new System.Windows.Forms.ToolStripStatusLabel();
      this.m_ReplayProgress = new System.Windows.Forms.ToolStripProgressBar();
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.m_HeapTab = new System.Windows.Forms.TabPage();
      this.m_HeapTree = new MemTrace.Widgets.HeapTreeList();
      this.m_FragTab = new System.Windows.Forms.TabPage();
      this.label2 = new System.Windows.Forms.Label();
      this.m_CallstackList = new BrightIdeasSoftware.FastObjectListView();
      this.m_CsSymbolColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.m_CsSourceFileColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.m_CsLineNumberColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.m_BlockVisualizerLabel = new System.Windows.Forms.Label();
      this.m_FragPanel = new System.Windows.Forms.Panel();
      this.m_FragWidget = new MemTrace.Widgets.FragmentationWidget();
      this.m_HeapCombo = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.m_BlockView = new MemTrace.Widgets.MemBlockView();
      this.m_AllocInfo = new BrightIdeasSoftware.FastObjectListView();
      this.m_FragAddressColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.m_FragSizeColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.m_FragAgeColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.m_FragColorColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.label4 = new System.Windows.Forms.Label();
      this.m_TimeControl = new MemTrace.Widgets.TimeControl();
      this.m_SymbolContextMenu.SuspendLayout();
      this.m_StatusStrip.SuspendLayout();
      this.tabControl1.SuspendLayout();
      this.m_HeapTab.SuspendLayout();
      this.m_FragTab.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_CallstackList)).BeginInit();
      this.m_FragPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_AllocInfo)).BeginInit();
      this.SuspendLayout();
      // 
      // m_SymbolContextMenu
      // 
      this.m_SymbolContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_ExcludeSymbolMenuItem});
      this.m_SymbolContextMenu.Name = "m_SymbolContextMenu";
      this.m_SymbolContextMenu.Size = new System.Drawing.Size(158, 26);
      // 
      // m_ExcludeSymbolMenuItem
      // 
      this.m_ExcludeSymbolMenuItem.Name = "m_ExcludeSymbolMenuItem";
      this.m_ExcludeSymbolMenuItem.Size = new System.Drawing.Size(157, 22);
      this.m_ExcludeSymbolMenuItem.Text = "E&xclude Symbol";
      this.m_ExcludeSymbolMenuItem.Click += new System.EventHandler(this.ExcludeSymbolMenuItem_Click);
      // 
      // m_RefreshButton
      // 
      this.m_RefreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.m_RefreshButton.Location = new System.Drawing.Point(929, 14);
      this.m_RefreshButton.Name = "m_RefreshButton";
      this.m_RefreshButton.Size = new System.Drawing.Size(58, 23);
      this.m_RefreshButton.TabIndex = 6;
      this.m_RefreshButton.Text = "Refresh";
      this.m_RefreshButton.UseVisualStyleBackColor = true;
      this.m_RefreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
      // 
      // m_Perspective
      // 
      this.m_Perspective.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.m_Perspective.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.m_Perspective.FormattingEnabled = true;
      this.m_Perspective.Location = new System.Drawing.Point(743, 15);
      this.m_Perspective.Name = "m_Perspective";
      this.m_Perspective.Size = new System.Drawing.Size(180, 21);
      this.m_Perspective.TabIndex = 7;
      this.m_Perspective.SelectedIndexChanged += new System.EventHandler(this.m_Perspective_SelectedIndexChanged);
      // 
      // m_StatusStrip
      // 
      this.m_StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_StripStatus,
            this.m_ReplayProgress});
      this.m_StatusStrip.Location = new System.Drawing.Point(0, 727);
      this.m_StatusStrip.Name = "m_StatusStrip";
      this.m_StatusStrip.Size = new System.Drawing.Size(991, 22);
      this.m_StatusStrip.TabIndex = 9;
      // 
      // m_StripStatus
      // 
      this.m_StripStatus.Name = "m_StripStatus";
      this.m_StripStatus.Size = new System.Drawing.Size(543, 17);
      this.m_StripStatus.Spring = true;
      this.m_StripStatus.Text = "OK";
      this.m_StripStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // m_ReplayProgress
      // 
      this.m_ReplayProgress.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
      this.m_ReplayProgress.Maximum = 1000;
      this.m_ReplayProgress.Name = "m_ReplayProgress";
      this.m_ReplayProgress.Size = new System.Drawing.Size(400, 16);
      // 
      // tabControl1
      // 
      this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl1.Controls.Add(this.m_HeapTab);
      this.tabControl1.Controls.Add(this.m_FragTab);
      this.tabControl1.Location = new System.Drawing.Point(4, 47);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(987, 677);
      this.tabControl1.TabIndex = 12;
      // 
      // m_HeapTab
      // 
      this.m_HeapTab.Controls.Add(this.m_HeapTree);
      this.m_HeapTab.Location = new System.Drawing.Point(4, 22);
      this.m_HeapTab.Name = "m_HeapTab";
      this.m_HeapTab.Padding = new System.Windows.Forms.Padding(3);
      this.m_HeapTab.Size = new System.Drawing.Size(979, 651);
      this.m_HeapTab.TabIndex = 0;
      this.m_HeapTab.Text = "Heap";
      this.m_HeapTab.UseVisualStyleBackColor = true;
      // 
      // m_HeapTree
      // 
      this.m_HeapTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.m_HeapTree.Location = new System.Drawing.Point(0, 0);
      this.m_HeapTree.Name = "m_HeapTree";
      this.m_HeapTree.Size = new System.Drawing.Size(979, 651);
      this.m_HeapTree.TabIndex = 0;
      // 
      // m_FragTab
      // 
      this.m_FragTab.Controls.Add(this.label2);
      this.m_FragTab.Controls.Add(this.m_CallstackList);
      this.m_FragTab.Controls.Add(this.m_BlockVisualizerLabel);
      this.m_FragTab.Controls.Add(this.m_FragPanel);
      this.m_FragTab.Controls.Add(this.m_HeapCombo);
      this.m_FragTab.Controls.Add(this.label1);
      this.m_FragTab.Controls.Add(this.m_BlockView);
      this.m_FragTab.Controls.Add(this.m_AllocInfo);
      this.m_FragTab.Controls.Add(this.label4);
      this.m_FragTab.Location = new System.Drawing.Point(4, 22);
      this.m_FragTab.Name = "m_FragTab";
      this.m_FragTab.Padding = new System.Windows.Forms.Padding(3);
      this.m_FragTab.Size = new System.Drawing.Size(979, 651);
      this.m_FragTab.TabIndex = 1;
      this.m_FragTab.Text = "Fragmentation";
      this.m_FragTab.UseVisualStyleBackColor = true;
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(920, 376);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(55, 13);
      this.label2.TabIndex = 13;
      this.label2.Text = "Call Stack";
      // 
      // m_CallstackList
      // 
      this.m_CallstackList.AllColumns.Add(this.m_CsSymbolColumn);
      this.m_CallstackList.AllColumns.Add(this.m_CsSourceFileColumn);
      this.m_CallstackList.AllColumns.Add(this.m_CsLineNumberColumn);
      this.m_CallstackList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.m_CallstackList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.m_CsSymbolColumn,
            this.m_CsSourceFileColumn,
            this.m_CsLineNumberColumn});
      this.m_CallstackList.FullRowSelect = true;
      this.m_CallstackList.GridLines = true;
      this.m_CallstackList.Location = new System.Drawing.Point(277, 392);
      this.m_CallstackList.Name = "m_CallstackList";
      this.m_CallstackList.ShowGroups = false;
      this.m_CallstackList.Size = new System.Drawing.Size(698, 256);
      this.m_CallstackList.TabIndex = 12;
      this.m_CallstackList.UseCompatibleStateImageBehavior = false;
      this.m_CallstackList.View = System.Windows.Forms.View.Details;
      this.m_CallstackList.VirtualMode = true;
      // 
      // m_CsSymbolColumn
      // 
      this.m_CsSymbolColumn.AspectName = "Symbol";
      this.m_CsSymbolColumn.CellPadding = null;
      this.m_CsSymbolColumn.FillsFreeSpace = true;
      this.m_CsSymbolColumn.IsEditable = false;
      this.m_CsSymbolColumn.Text = "Symbol";
      this.m_CsSymbolColumn.Width = 413;
      // 
      // m_CsSourceFileColumn
      // 
      this.m_CsSourceFileColumn.AspectName = "FileName";
      this.m_CsSourceFileColumn.CellPadding = null;
      this.m_CsSourceFileColumn.IsEditable = false;
      this.m_CsSourceFileColumn.Text = "Source File";
      this.m_CsSourceFileColumn.Width = 199;
      // 
      // m_CsLineNumberColumn
      // 
      this.m_CsLineNumberColumn.AspectName = "LineNumber";
      this.m_CsLineNumberColumn.CellPadding = null;
      this.m_CsLineNumberColumn.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.m_CsLineNumberColumn.IsEditable = false;
      this.m_CsLineNumberColumn.Text = "Line Number";
      this.m_CsLineNumberColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.m_CsLineNumberColumn.Width = 76;
      // 
      // m_BlockVisualizerLabel
      // 
      this.m_BlockVisualizerLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.m_BlockVisualizerLabel.AutoSize = true;
      this.m_BlockVisualizerLabel.Location = new System.Drawing.Point(12, 378);
      this.m_BlockVisualizerLabel.Name = "m_BlockVisualizerLabel";
      this.m_BlockVisualizerLabel.Size = new System.Drawing.Size(81, 13);
      this.m_BlockVisualizerLabel.TabIndex = 11;
      this.m_BlockVisualizerLabel.Text = "Block Visualizer";
      // 
      // m_FragPanel
      // 
      this.m_FragPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.m_FragPanel.AutoScroll = true;
      this.m_FragPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.m_FragPanel.Controls.Add(this.m_FragWidget);
      this.m_FragPanel.Location = new System.Drawing.Point(14, 39);
      this.m_FragPanel.Name = "m_FragPanel";
      this.m_FragPanel.Size = new System.Drawing.Size(405, 336);
      this.m_FragPanel.TabIndex = 10;
      // 
      // m_FragWidget
      // 
      this.m_FragWidget.AutoSize = true;
      this.m_FragWidget.BackColor = System.Drawing.SystemColors.Window;
      this.m_FragWidget.FragmentationData = null;
      this.m_FragWidget.Location = new System.Drawing.Point(0, 0);
      this.m_FragWidget.MaxAddress = ((ulong)(0ul));
      this.m_FragWidget.MinAddress = ((ulong)(0ul));
      this.m_FragWidget.Name = "m_FragWidget";
      this.m_FragWidget.Size = new System.Drawing.Size(384, 317);
      this.m_FragWidget.TabIndex = 9;
      this.m_FragWidget.OnBlockSelected += new MemTrace.Widgets.FragmentationWidget.BlockSelectedHandler(this.OnFragBlockSelected);
      // 
      // m_HeapCombo
      // 
      this.m_HeapCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.m_HeapCombo.FormattingEnabled = true;
      this.m_HeapCombo.Location = new System.Drawing.Point(89, 11);
      this.m_HeapCombo.Name = "m_HeapCombo";
      this.m_HeapCombo.Size = new System.Drawing.Size(151, 21);
      this.m_HeapCombo.TabIndex = 5;
      this.m_HeapCombo.SelectionChangeCommitted += new System.EventHandler(this.OnHeapComboCommit);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 15);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(71, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "Virtual Range";
      // 
      // m_BlockView
      // 
      this.m_BlockView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.m_BlockView.BackColor = System.Drawing.Color.Silver;
      this.m_BlockView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.m_BlockView.Location = new System.Drawing.Point(14, 392);
      this.m_BlockView.MaximumSize = new System.Drawing.Size(256, 256);
      this.m_BlockView.MinimumSize = new System.Drawing.Size(256, 256);
      this.m_BlockView.Name = "m_BlockView";
      this.m_BlockView.Size = new System.Drawing.Size(256, 256);
      this.m_BlockView.TabIndex = 9;
      // 
      // m_AllocInfo
      // 
      this.m_AllocInfo.AllColumns.Add(this.m_FragAddressColumn);
      this.m_AllocInfo.AllColumns.Add(this.m_FragSizeColumn);
      this.m_AllocInfo.AllColumns.Add(this.m_FragAgeColumn);
      this.m_AllocInfo.AllColumns.Add(this.m_FragColorColumn);
      this.m_AllocInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.m_AllocInfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.m_FragAddressColumn,
            this.m_FragSizeColumn,
            this.m_FragAgeColumn,
            this.m_FragColorColumn});
      this.m_AllocInfo.FullRowSelect = true;
      this.m_AllocInfo.GridLines = true;
      this.m_AllocInfo.Location = new System.Drawing.Point(429, 39);
      this.m_AllocInfo.Name = "m_AllocInfo";
      this.m_AllocInfo.ShowGroups = false;
      this.m_AllocInfo.Size = new System.Drawing.Size(546, 336);
      this.m_AllocInfo.TabIndex = 7;
      this.m_AllocInfo.UseCellFormatEvents = true;
      this.m_AllocInfo.UseCompatibleStateImageBehavior = false;
      this.m_AllocInfo.View = System.Windows.Forms.View.Details;
      this.m_AllocInfo.VirtualMode = true;
      this.m_AllocInfo.FormatCell += new System.EventHandler<BrightIdeasSoftware.FormatCellEventArgs>(this.OnFormatFragCell);
      this.m_AllocInfo.SelectionChanged += new System.EventHandler(this.OnAllocSelectionChanged);
      // 
      // m_FragAddressColumn
      // 
      this.m_FragAddressColumn.CellPadding = null;
      this.m_FragAddressColumn.IsEditable = false;
      this.m_FragAddressColumn.Text = "Address";
      this.m_FragAddressColumn.Width = 196;
      // 
      // m_FragSizeColumn
      // 
      this.m_FragSizeColumn.CellPadding = null;
      this.m_FragSizeColumn.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.m_FragSizeColumn.IsEditable = false;
      this.m_FragSizeColumn.Text = "Size";
      this.m_FragSizeColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.m_FragSizeColumn.Width = 93;
      // 
      // m_FragAgeColumn
      // 
      this.m_FragAgeColumn.CellPadding = null;
      this.m_FragAgeColumn.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.m_FragAgeColumn.Text = "Age";
      this.m_FragAgeColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.m_FragAgeColumn.Width = 101;
      // 
      // m_FragColorColumn
      // 
      this.m_FragColorColumn.CellPadding = null;
      this.m_FragColorColumn.Text = "Color";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(429, 22);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(117, 13);
      this.label4.TabIndex = 6;
      this.label4.Text = "Contributing Allocations";
      // 
      // m_TimeControl
      // 
      this.m_TimeControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.m_TimeControl.Location = new System.Drawing.Point(4, 9);
      this.m_TimeControl.MaxTime = 0D;
      this.m_TimeControl.MinTime = 0D;
      this.m_TimeControl.Name = "m_TimeControl";
      this.m_TimeControl.Size = new System.Drawing.Size(685, 32);
      this.m_TimeControl.TabIndex = 11;
      this.m_TimeControl.TimeLabel = "Time";
      this.m_TimeControl.TimeChanged += new MemTrace.Widgets.TimeControl.TimeChangedDelegate(this.OnTimeChanged);
      // 
      // HeapWindow
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(991, 749);
      this.Controls.Add(this.tabControl1);
      this.Controls.Add(this.m_TimeControl);
      this.Controls.Add(this.m_StatusStrip);
      this.Controls.Add(this.m_Perspective);
      this.Controls.Add(this.m_RefreshButton);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "HeapWindow";
      this.Text = "MemTrace Heap View";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
      this.Shown += new System.EventHandler(this.OnFormShown);
      this.m_SymbolContextMenu.ResumeLayout(false);
      this.m_StatusStrip.ResumeLayout(false);
      this.m_StatusStrip.PerformLayout();
      this.tabControl1.ResumeLayout(false);
      this.m_HeapTab.ResumeLayout(false);
      this.m_FragTab.ResumeLayout(false);
      this.m_FragTab.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_CallstackList)).EndInit();
      this.m_FragPanel.ResumeLayout(false);
      this.m_FragPanel.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_AllocInfo)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ContextMenuStrip m_SymbolContextMenu;
    private System.Windows.Forms.ToolStripMenuItem m_ExcludeSymbolMenuItem;
    private System.Windows.Forms.Button m_RefreshButton;
    private System.Windows.Forms.ComboBox m_Perspective;
    private System.Windows.Forms.StatusStrip m_StatusStrip;
    private System.Windows.Forms.ToolStripProgressBar m_ReplayProgress;
    private System.Windows.Forms.ToolStripStatusLabel m_StripStatus;
    private MemTrace.Widgets.TimeControl m_TimeControl;
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage m_HeapTab;
    private System.Windows.Forms.TabPage m_FragTab;
    private System.Windows.Forms.ComboBox m_HeapCombo;
    private System.Windows.Forms.Label label1;
    private MemTrace.Widgets.MemBlockView m_BlockView;
    private BrightIdeasSoftware.FastObjectListView m_AllocInfo;
    private BrightIdeasSoftware.OLVColumn m_FragAddressColumn;
    private BrightIdeasSoftware.OLVColumn m_FragColorColumn;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Panel m_FragPanel;
    private MemTrace.Widgets.FragmentationWidget m_FragWidget;
    private System.Windows.Forms.Label m_BlockVisualizerLabel;
    private BrightIdeasSoftware.OLVColumn m_FragSizeColumn;
    private BrightIdeasSoftware.OLVColumn m_FragAgeColumn;
    private System.Windows.Forms.Label label2;
    private BrightIdeasSoftware.FastObjectListView m_CallstackList;
    private BrightIdeasSoftware.OLVColumn m_CsSymbolColumn;
    private BrightIdeasSoftware.OLVColumn m_CsSourceFileColumn;
    private BrightIdeasSoftware.OLVColumn m_CsLineNumberColumn;
    private MemTrace.Widgets.HeapTreeList m_HeapTree;
  }
}

