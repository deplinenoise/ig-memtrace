namespace MemTraceTool
{
  partial class DeltaWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeltaWindow));
            this.m_StartTimeControl = new MemTrace.Widgets.TimeControl();
            this.m_EndTimeControl = new MemTrace.Widgets.TimeControl();
            this.m_HeapTree = new MemTrace.Widgets.HeapTreeList();
            this.m_RefreshButton = new System.Windows.Forms.Button();
            this.m_Perspective = new System.Windows.Forms.ComboBox();
            this.m_StatusStrip = new System.Windows.Forms.StatusStrip();
            this.m_StripStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_ReplayProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.m_StatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_StartTimeControl
            // 
            this.m_StartTimeControl.Location = new System.Drawing.Point(12, 9);
            this.m_StartTimeControl.MaxTime = 0D;
            this.m_StartTimeControl.MinTime = 0D;
            this.m_StartTimeControl.Name = "m_StartTimeControl";
            this.m_StartTimeControl.Size = new System.Drawing.Size(327, 32);
            this.m_StartTimeControl.TabIndex = 0;
            this.m_StartTimeControl.TimeLabel = "Start";
            // 
            // m_EndTimeControl
            // 
            this.m_EndTimeControl.Location = new System.Drawing.Point(415, 9);
            this.m_EndTimeControl.MaxTime = 0D;
            this.m_EndTimeControl.MinTime = 0D;
            this.m_EndTimeControl.Name = "m_EndTimeControl";
            this.m_EndTimeControl.Size = new System.Drawing.Size(327, 32);
            this.m_EndTimeControl.TabIndex = 1;
            this.m_EndTimeControl.TimeLabel = "End";
            // 
            // m_HeapTree
            // 
            this.m_HeapTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_HeapTree.Location = new System.Drawing.Point(2, 51);
            this.m_HeapTree.Name = "m_HeapTree";
            this.m_HeapTree.Size = new System.Drawing.Size(1117, 694);
            this.m_HeapTree.TabIndex = 2;
            this.m_HeapTree.CellDblClick += new System.EventHandler(this.HeapTree_CellDblClick);
            // 
            // m_RefreshButton
            // 
            this.m_RefreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_RefreshButton.Location = new System.Drawing.Point(1033, 12);
            this.m_RefreshButton.Name = "m_RefreshButton";
            this.m_RefreshButton.Size = new System.Drawing.Size(75, 23);
            this.m_RefreshButton.TabIndex = 3;
            this.m_RefreshButton.Text = "Refresh";
            this.m_RefreshButton.UseVisualStyleBackColor = true;
            this.m_RefreshButton.Click += new System.EventHandler(this.OnRefreshButtonClick);
            // 
            // m_Perspective
            // 
            this.m_Perspective.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_Perspective.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_Perspective.FormattingEnabled = true;
            this.m_Perspective.Location = new System.Drawing.Point(822, 14);
            this.m_Perspective.Name = "m_Perspective";
            this.m_Perspective.Size = new System.Drawing.Size(180, 21);
            this.m_Perspective.TabIndex = 8;
            // 
            // m_StatusStrip
            // 
            this.m_StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_StripStatus,
            this.m_ReplayProgress});
            this.m_StatusStrip.Location = new System.Drawing.Point(0, 724);
            this.m_StatusStrip.Name = "m_StatusStrip";
            this.m_StatusStrip.Size = new System.Drawing.Size(1120, 22);
            this.m_StatusStrip.TabIndex = 10;
            // 
            // m_StripStatus
            // 
            this.m_StripStatus.Name = "m_StripStatus";
            this.m_StripStatus.Size = new System.Drawing.Size(703, 17);
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
            // DeltaWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1120, 746);
            this.Controls.Add(this.m_StatusStrip);
            this.Controls.Add(this.m_Perspective);
            this.Controls.Add(this.m_RefreshButton);
            this.Controls.Add(this.m_HeapTree);
            this.Controls.Add(this.m_EndTimeControl);
            this.Controls.Add(this.m_StartTimeControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DeltaWindow";
            this.Text = "Memory Delta View";
            this.m_StatusStrip.ResumeLayout(false);
            this.m_StatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private MemTrace.Widgets.TimeControl m_StartTimeControl;
    private MemTrace.Widgets.TimeControl m_EndTimeControl;
    private MemTrace.Widgets.HeapTreeList m_HeapTree;
    private System.Windows.Forms.Button m_RefreshButton;
    private System.Windows.Forms.ComboBox m_Perspective;
    private System.Windows.Forms.StatusStrip m_StatusStrip;
    private System.Windows.Forms.ToolStripStatusLabel m_StripStatus;
    private System.Windows.Forms.ToolStripProgressBar m_ReplayProgress;
  }
}