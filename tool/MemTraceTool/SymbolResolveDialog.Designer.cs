namespace MemTraceTool
{
  partial class SymbolResolveDialog
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SymbolResolveDialog));
      this.m_LogText = new System.Windows.Forms.TextBox();
      this.m_ProgressBar = new System.Windows.Forms.ProgressBar();
      this.m_StatusLabel = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // m_LogText
      // 
      this.m_LogText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.m_LogText.Location = new System.Drawing.Point(10, 58);
      this.m_LogText.MaxLength = 128000;
      this.m_LogText.Multiline = true;
      this.m_LogText.Name = "m_LogText";
      this.m_LogText.ReadOnly = true;
      this.m_LogText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.m_LogText.Size = new System.Drawing.Size(606, 354);
      this.m_LogText.TabIndex = 0;
      // 
      // m_ProgressBar
      // 
      this.m_ProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.m_ProgressBar.Location = new System.Drawing.Point(10, 29);
      this.m_ProgressBar.Maximum = 1000;
      this.m_ProgressBar.Name = "m_ProgressBar";
      this.m_ProgressBar.Size = new System.Drawing.Size(605, 23);
      this.m_ProgressBar.TabIndex = 1;
      // 
      // m_StatusLabel
      // 
      this.m_StatusLabel.AutoSize = true;
      this.m_StatusLabel.Location = new System.Drawing.Point(9, 13);
      this.m_StatusLabel.Name = "m_StatusLabel";
      this.m_StatusLabel.Size = new System.Drawing.Size(37, 13);
      this.m_StatusLabel.TabIndex = 2;
      this.m_StatusLabel.Text = "Status";
      // 
      // SymbolResolveDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(627, 422);
      this.Controls.Add(this.m_StatusLabel);
      this.Controls.Add(this.m_ProgressBar);
      this.Controls.Add(this.m_LogText);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "SymbolResolveDialog";
      this.Text = "Resolving Symbols..";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
      this.Shown += new System.EventHandler(this.OnFormShown);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox m_LogText;
    private System.Windows.Forms.ProgressBar m_ProgressBar;
    private System.Windows.Forms.Label m_StatusLabel;
  }
}