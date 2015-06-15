namespace MemTrace.Widgets
{
  partial class TimeControl
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
      this.m_Label = new System.Windows.Forms.Label();
      this.m_Time = new System.Windows.Forms.ComboBox();
      this.m_Rev2Button = new System.Windows.Forms.Button();
      this.m_Rev1Button = new System.Windows.Forms.Button();
      this.m_Fwd2Button = new System.Windows.Forms.Button();
      this.m_Fwd1Button = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // m_Label
      // 
      this.m_Label.AutoSize = true;
      this.m_Label.Location = new System.Drawing.Point(-3, 9);
      this.m_Label.Name = "m_Label";
      this.m_Label.Size = new System.Drawing.Size(30, 13);
      this.m_Label.TabIndex = 0;
      this.m_Label.Text = "Time";
      // 
      // m_Time
      // 
      this.m_Time.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.m_Time.FormattingEnabled = true;
      this.m_Time.Location = new System.Drawing.Point(33, 5);
      this.m_Time.Name = "m_Time";
      this.m_Time.Size = new System.Drawing.Size(406, 21);
      this.m_Time.TabIndex = 1;
      this.m_Time.SelectionChangeCommitted += new System.EventHandler(this.OnTimeValueSelected);
      this.m_Time.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnComboKeyDown);
      // 
      // m_Rev2Button
      // 
      this.m_Rev2Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.m_Rev2Button.Location = new System.Drawing.Point(445, 4);
      this.m_Rev2Button.Name = "m_Rev2Button";
      this.m_Rev2Button.Size = new System.Drawing.Size(34, 23);
      this.m_Rev2Button.TabIndex = 3;
      this.m_Rev2Button.Text = "<<";
      this.m_Rev2Button.UseVisualStyleBackColor = true;
      this.m_Rev2Button.Click += new System.EventHandler(this.OnRev2Button_Click);
      // 
      // m_Rev1Button
      // 
      this.m_Rev1Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.m_Rev1Button.Location = new System.Drawing.Point(486, 4);
      this.m_Rev1Button.Name = "m_Rev1Button";
      this.m_Rev1Button.Size = new System.Drawing.Size(34, 23);
      this.m_Rev1Button.TabIndex = 4;
      this.m_Rev1Button.Text = "<";
      this.m_Rev1Button.UseVisualStyleBackColor = true;
      this.m_Rev1Button.Click += new System.EventHandler(this.OnRev1Button_Click);
      // 
      // m_Fwd2Button
      // 
      this.m_Fwd2Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.m_Fwd2Button.Location = new System.Drawing.Point(574, 4);
      this.m_Fwd2Button.Name = "m_Fwd2Button";
      this.m_Fwd2Button.Size = new System.Drawing.Size(34, 23);
      this.m_Fwd2Button.TabIndex = 6;
      this.m_Fwd2Button.Text = ">>";
      this.m_Fwd2Button.UseVisualStyleBackColor = true;
      this.m_Fwd2Button.Click += new System.EventHandler(this.OnFwd2Button_Click);
      // 
      // m_Fwd1Button
      // 
      this.m_Fwd1Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.m_Fwd1Button.Location = new System.Drawing.Point(533, 4);
      this.m_Fwd1Button.Name = "m_Fwd1Button";
      this.m_Fwd1Button.Size = new System.Drawing.Size(34, 23);
      this.m_Fwd1Button.TabIndex = 5;
      this.m_Fwd1Button.Text = ">";
      this.m_Fwd1Button.UseVisualStyleBackColor = true;
      this.m_Fwd1Button.Click += new System.EventHandler(this.OnFwd1Button_Click);
      // 
      // TimeControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.m_Fwd2Button);
      this.Controls.Add(this.m_Fwd1Button);
      this.Controls.Add(this.m_Rev1Button);
      this.Controls.Add(this.m_Rev2Button);
      this.Controls.Add(this.m_Time);
      this.Controls.Add(this.m_Label);
      this.Name = "TimeControl";
      this.Size = new System.Drawing.Size(609, 32);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label m_Label;
    private System.Windows.Forms.ComboBox m_Time;
    private System.Windows.Forms.Button m_Rev2Button;
    private System.Windows.Forms.Button m_Rev1Button;
    private System.Windows.Forms.Button m_Fwd2Button;
    private System.Windows.Forms.Button m_Fwd1Button;
  }
}
