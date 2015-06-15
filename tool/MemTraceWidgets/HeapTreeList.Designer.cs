namespace MemTrace.Widgets
{
  partial class HeapTreeList
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HeapTreeList));
      this.m_TreeList = new BrightIdeasSoftware.TreeListView();
      this.m_KeyColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.m_FileNameColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.m_LineNumberColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.m_SizeColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.m_CountColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.m_ImageList = new System.Windows.Forms.ImageList(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.m_TreeList)).BeginInit();
      this.SuspendLayout();
      // 
      // m_TreeList
      // 
      this.m_TreeList.AllColumns.Add(this.m_KeyColumn);
      this.m_TreeList.AllColumns.Add(this.m_FileNameColumn);
      this.m_TreeList.AllColumns.Add(this.m_LineNumberColumn);
      this.m_TreeList.AllColumns.Add(this.m_SizeColumn);
      this.m_TreeList.AllColumns.Add(this.m_CountColumn);
      this.m_TreeList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.m_TreeList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.m_KeyColumn,
            this.m_FileNameColumn,
            this.m_LineNumberColumn,
            this.m_SizeColumn,
            this.m_CountColumn});
      this.m_TreeList.FullRowSelect = true;
      this.m_TreeList.GridLines = true;
      this.m_TreeList.Location = new System.Drawing.Point(0, 0);
      this.m_TreeList.Name = "m_TreeList";
      this.m_TreeList.OwnerDraw = true;
      this.m_TreeList.ShowGroups = false;
      this.m_TreeList.Size = new System.Drawing.Size(1000, 500);
      this.m_TreeList.SmallImageList = this.m_ImageList;
      this.m_TreeList.TabIndex = 2;
      this.m_TreeList.UseCompatibleStateImageBehavior = false;
      this.m_TreeList.View = System.Windows.Forms.View.Details;
      this.m_TreeList.VirtualMode = true;
      // 
      // m_KeyColumn
      // 
      this.m_KeyColumn.AspectName = "Key";
      this.m_KeyColumn.CellPadding = null;
      this.m_KeyColumn.IsEditable = false;
      this.m_KeyColumn.MinimumWidth = 200;
      this.m_KeyColumn.Text = "Item";
      this.m_KeyColumn.Width = 253;
      // 
      // m_FileNameColumn
      // 
      this.m_FileNameColumn.AspectName = "FileName";
      this.m_FileNameColumn.CellPadding = null;
      this.m_FileNameColumn.IsEditable = false;
      this.m_FileNameColumn.Text = "File Name";
      this.m_FileNameColumn.Width = 177;
      // 
      // m_LineNumberColumn
      // 
      this.m_LineNumberColumn.AspectName = "LineNumber";
      this.m_LineNumberColumn.AspectToStringFormat = "{0:n0}";
      this.m_LineNumberColumn.CellPadding = null;
      this.m_LineNumberColumn.IsEditable = false;
      this.m_LineNumberColumn.Text = "Line Number";
      // 
      // m_SizeColumn
      // 
      this.m_SizeColumn.AspectName = "SizeBytes";
      this.m_SizeColumn.AspectToStringFormat = "{0:n0}";
      this.m_SizeColumn.CellPadding = null;
      this.m_SizeColumn.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.m_SizeColumn.IsEditable = false;
      this.m_SizeColumn.MinimumWidth = 80;
      this.m_SizeColumn.Text = "Size";
      this.m_SizeColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.m_SizeColumn.Width = 112;
      // 
      // m_CountColumn
      // 
      this.m_CountColumn.AspectName = "Count";
      this.m_CountColumn.AspectToStringFormat = "{0:n0}";
      this.m_CountColumn.CellPadding = null;
      this.m_CountColumn.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.m_CountColumn.IsEditable = false;
      this.m_CountColumn.MinimumWidth = 80;
      this.m_CountColumn.Text = "Count";
      this.m_CountColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.m_CountColumn.Width = 105;
      // 
      // m_ImageList
      // 
      this.m_ImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("m_ImageList.ImageStream")));
      this.m_ImageList.TransparentColor = System.Drawing.Color.Transparent;
      this.m_ImageList.Images.SetKeyName(0, "folder");
      this.m_ImageList.Images.SetKeyName(1, "sourcefile");
      this.m_ImageList.Images.SetKeyName(2, "heap");
      this.m_ImageList.Images.SetKeyName(3, "asset");
      this.m_ImageList.Images.SetKeyName(4, "namespace");
      this.m_ImageList.Images.SetKeyName(5, "component");
      this.m_ImageList.Images.SetKeyName(6, "arrow");
      this.m_ImageList.Images.SetKeyName(7, "symbol");
      // 
      // HeapTreeList
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.m_TreeList);
      this.Name = "HeapTreeList";
      this.Size = new System.Drawing.Size(1000, 500);
      ((System.ComponentModel.ISupportInitialize)(this.m_TreeList)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private BrightIdeasSoftware.TreeListView m_TreeList;
    private BrightIdeasSoftware.OLVColumn m_KeyColumn;
    private BrightIdeasSoftware.OLVColumn m_FileNameColumn;
    private BrightIdeasSoftware.OLVColumn m_LineNumberColumn;
    private BrightIdeasSoftware.OLVColumn m_SizeColumn;
    private BrightIdeasSoftware.OLVColumn m_CountColumn;
    private System.Windows.Forms.ImageList m_ImageList;

  }
}
