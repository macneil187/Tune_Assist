namespace AutoTune
{
  public partial class HelpForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HelpForm));
      this.HelpTextBox = new System.Windows.Forms.RichTextBox();
      this.SuspendLayout();
      // 
      // HelpTextBox
      // 
      this.HelpTextBox.BackColor = System.Drawing.SystemColors.Control;
      this.HelpTextBox.Cursor = System.Windows.Forms.Cursors.Default;
      this.HelpTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.HelpTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.HelpTextBox.Location = new System.Drawing.Point(0, 0);
      this.HelpTextBox.Margin = new System.Windows.Forms.Padding(10);
      this.HelpTextBox.MaxLength = 83647;
      this.HelpTextBox.Name = "HelpTextBox";
      this.HelpTextBox.ReadOnly = true;
      this.HelpTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
      this.HelpTextBox.Size = new System.Drawing.Size(593, 243);
      this.HelpTextBox.TabIndex = 0;
      this.HelpTextBox.Text = resources.GetString("HelpTextBox.Text");
      // 
      // HelpForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.ControlLight;
      this.ClientSize = new System.Drawing.Size(593, 243);
      this.Controls.Add(this.HelpTextBox);
      this.ForeColor = System.Drawing.SystemColors.ControlText;
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Name = "HelpForm";
      this.ShowIcon = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Help";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.RichTextBox HelpTextBox;
  }
}