namespace MemTraceTool
{
  partial class WarningWindow
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WarningWindow));
      this.m_TextBox = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // m_TextBox
      // 
      this.m_TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.m_TextBox.Location = new System.Drawing.Point(13, 13);
      this.m_TextBox.Multiline = true;
      this.m_TextBox.Name = "m_TextBox";
      this.m_TextBox.ReadOnly = true;
      this.m_TextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.m_TextBox.Size = new System.Drawing.Size(629, 399);
      this.m_TextBox.TabIndex = 0;
      // 
      // WarningWindow
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(654, 424);
      this.Controls.Add(this.m_TextBox);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "WarningWindow";
      this.Text = "Warnings";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox m_TextBox;
  }
}