namespace MemTraceTool
{
  partial class RangeQueryWindow
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RangeQueryWindow));
      this.m_StartAddress = new System.Windows.Forms.TextBox();
      this.m_SizeBytes = new System.Windows.Forms.TextBox();
      this.m_SearchButton = new System.Windows.Forms.Button();
      this.m_SearchResults = new BrightIdeasSoftware.FastObjectListView();
      this.m_EventType = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.m_AddressColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.m_SizeColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.m_TimeColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.statusStrip1 = new System.Windows.Forms.StatusStrip();
      this.m_StatusText = new System.Windows.Forms.ToolStripStatusLabel();
      this.m_ProgressBar = new System.Windows.Forms.ToolStripProgressBar();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.m_SearchResults)).BeginInit();
      this.statusStrip1.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_StartAddress
      // 
      this.m_StartAddress.Location = new System.Drawing.Point(93, 8);
      this.m_StartAddress.Name = "m_StartAddress";
      this.m_StartAddress.Size = new System.Drawing.Size(154, 20);
      this.m_StartAddress.TabIndex = 0;
      // 
      // m_SizeBytes
      // 
      this.m_SizeBytes.Location = new System.Drawing.Point(320, 8);
      this.m_SizeBytes.Name = "m_SizeBytes";
      this.m_SizeBytes.Size = new System.Drawing.Size(154, 20);
      this.m_SizeBytes.TabIndex = 1;
      // 
      // m_SearchButton
      // 
      this.m_SearchButton.Location = new System.Drawing.Point(527, 7);
      this.m_SearchButton.Name = "m_SearchButton";
      this.m_SearchButton.Size = new System.Drawing.Size(75, 23);
      this.m_SearchButton.TabIndex = 2;
      this.m_SearchButton.Text = "Search";
      this.m_SearchButton.UseVisualStyleBackColor = true;
      this.m_SearchButton.Click += new System.EventHandler(this.SearchButton_Click);
      // 
      // m_SearchResults
      // 
      this.m_SearchResults.AllColumns.Add(this.m_EventType);
      this.m_SearchResults.AllColumns.Add(this.m_AddressColumn);
      this.m_SearchResults.AllColumns.Add(this.m_SizeColumn);
      this.m_SearchResults.AllColumns.Add(this.m_TimeColumn);
      this.m_SearchResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.m_SearchResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.m_EventType,
            this.m_AddressColumn,
            this.m_SizeColumn,
            this.m_TimeColumn});
      this.m_SearchResults.Location = new System.Drawing.Point(13, 40);
      this.m_SearchResults.Name = "m_SearchResults";
      this.m_SearchResults.ShowGroups = false;
      this.m_SearchResults.Size = new System.Drawing.Size(606, 350);
      this.m_SearchResults.TabIndex = 3;
      this.m_SearchResults.UseCompatibleStateImageBehavior = false;
      this.m_SearchResults.View = System.Windows.Forms.View.Details;
      this.m_SearchResults.VirtualMode = true;
      this.m_SearchResults.CellToolTipShowing += new System.EventHandler<BrightIdeasSoftware.ToolTipShowingEventArgs>(this.OnToolTipShowing);
      // 
      // m_EventType
      // 
      this.m_EventType.AspectName = "EventType";
      this.m_EventType.CellPadding = null;
      this.m_EventType.Text = "Event";
      this.m_EventType.Width = 87;
      // 
      // m_AddressColumn
      // 
      this.m_AddressColumn.AspectName = "Address";
      this.m_AddressColumn.AspectToStringFormat = "{0:x16}";
      this.m_AddressColumn.CellPadding = null;
      this.m_AddressColumn.IsEditable = false;
      this.m_AddressColumn.Text = "Address";
      this.m_AddressColumn.Width = 151;
      // 
      // m_SizeColumn
      // 
      this.m_SizeColumn.AspectName = "Size";
      this.m_SizeColumn.AspectToStringFormat = "{0:n0}";
      this.m_SizeColumn.CellPadding = null;
      this.m_SizeColumn.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.m_SizeColumn.IsEditable = false;
      this.m_SizeColumn.Text = "Size";
      this.m_SizeColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.m_SizeColumn.Width = 113;
      // 
      // m_TimeColumn
      // 
      this.m_TimeColumn.AspectName = "Time";
      this.m_TimeColumn.AspectToStringFormat = "{0:n3}";
      this.m_TimeColumn.CellPadding = null;
      this.m_TimeColumn.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.m_TimeColumn.IsEditable = false;
      this.m_TimeColumn.Text = "Time (s)";
      this.m_TimeColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.m_TimeColumn.Width = 84;
      // 
      // statusStrip1
      // 
      this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_StatusText,
            this.m_ProgressBar});
      this.statusStrip1.Location = new System.Drawing.Point(0, 403);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Size = new System.Drawing.Size(631, 22);
      this.statusStrip1.TabIndex = 4;
      this.statusStrip1.Text = "statusStrip1";
      // 
      // m_StatusText
      // 
      this.m_StatusText.Name = "m_StatusText";
      this.m_StatusText.Size = new System.Drawing.Size(333, 17);
      this.m_StatusText.Spring = true;
      this.m_StatusText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // m_ProgressBar
      // 
      this.m_ProgressBar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
      this.m_ProgressBar.Maximum = 10000;
      this.m_ProgressBar.Name = "m_ProgressBar";
      this.m_ProgressBar.Size = new System.Drawing.Size(250, 16);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(13, 12);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(72, 13);
      this.label1.TabIndex = 5;
      this.label1.Text = "Base Address";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(288, 12);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(27, 13);
      this.label2.TabIndex = 6;
      this.label2.Text = "Size";
      // 
      // RangeQueryWindow
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(631, 425);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.statusStrip1);
      this.Controls.Add(this.m_SearchResults);
      this.Controls.Add(this.m_SearchButton);
      this.Controls.Add(this.m_SizeBytes);
      this.Controls.Add(this.m_StartAddress);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "RangeQueryWindow";
      this.Text = "Address Range Query";
      ((System.ComponentModel.ISupportInitialize)(this.m_SearchResults)).EndInit();
      this.statusStrip1.ResumeLayout(false);
      this.statusStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox m_StartAddress;
    private System.Windows.Forms.TextBox m_SizeBytes;
    private System.Windows.Forms.Button m_SearchButton;
    private BrightIdeasSoftware.FastObjectListView m_SearchResults;
    private System.Windows.Forms.StatusStrip statusStrip1;
    private System.Windows.Forms.ToolStripStatusLabel m_StatusText;
    private System.Windows.Forms.ToolStripProgressBar m_ProgressBar;
    private BrightIdeasSoftware.OLVColumn m_AddressColumn;
    private BrightIdeasSoftware.OLVColumn m_SizeColumn;
    private BrightIdeasSoftware.OLVColumn m_TimeColumn;
    private BrightIdeasSoftware.OLVColumn m_EventType;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
  }
}