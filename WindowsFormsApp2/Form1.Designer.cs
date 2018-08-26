namespace WindowsFormsApp2
{
  partial class Form1
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
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.openFileToolStripMenuItem_Open = new System.Windows.Forms.ToolStripMenuItem();
      this.closeFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
      this.fileToolStripMenuItem_Exit = new System.Windows.Forms.ToolStripMenuItem();
      this.statusStrip1 = new System.Windows.Forms.StatusStrip();
      this.StatusBox = new System.Windows.Forms.ToolStripStatusLabel();
      this.ProgressBar = new System.Windows.Forms.ToolStripProgressBar();
      this.menuStrip1.SuspendLayout();
      this.statusStrip1.SuspendLayout();
      this.SuspendLayout();
      // 
      // menuStrip1
      // 
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Size = new System.Drawing.Size(800, 24);
      this.menuStrip1.TabIndex = 0;
      this.menuStrip1.Text = "menuStrip1";
      // 
      // fileToolStripMenuItem
      // 
      this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFileToolStripMenuItem_Open,
            this.closeFileToolStripMenuItem,
            this.toolStripMenuItem2,
            this.fileToolStripMenuItem_Exit});
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
      this.fileToolStripMenuItem.Text = "File";
      // 
      // openFileToolStripMenuItem_Open
      // 
      this.openFileToolStripMenuItem_Open.Name = "openFileToolStripMenuItem_Open";
      this.openFileToolStripMenuItem_Open.Size = new System.Drawing.Size(180, 22);
      this.openFileToolStripMenuItem_Open.Text = "Open File";
      this.openFileToolStripMenuItem_Open.Click += new System.EventHandler(this.openFileToolStripMenuItem_Open_Click);
      // 
      // closeFileToolStripMenuItem
      // 
      this.closeFileToolStripMenuItem.Name = "closeFileToolStripMenuItem";
      this.closeFileToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
      this.closeFileToolStripMenuItem.Text = "Close File";
      // 
      // toolStripMenuItem2
      // 
      this.toolStripMenuItem2.Name = "toolStripMenuItem2";
      this.toolStripMenuItem2.Size = new System.Drawing.Size(180, 22);
      this.toolStripMenuItem2.Text = " ";
      // 
      // fileToolStripMenuItem_Exit
      // 
      this.fileToolStripMenuItem_Exit.Name = "fileToolStripMenuItem_Exit";
      this.fileToolStripMenuItem_Exit.Size = new System.Drawing.Size(180, 22);
      this.fileToolStripMenuItem_Exit.Text = "Exit";
      this.fileToolStripMenuItem_Exit.Click += new System.EventHandler(this.fileToolStripMenuItem1_Click);
      // 
      // statusStrip1
      // 
      this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusBox,
            this.ProgressBar});
      this.statusStrip1.Location = new System.Drawing.Point(0, 428);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Size = new System.Drawing.Size(800, 22);
      this.statusStrip1.TabIndex = 1;
      this.statusStrip1.Text = "statusStrip1";
      // 
      // StatusBox
      // 
      this.StatusBox.Name = "StatusBox";
      this.StatusBox.Size = new System.Drawing.Size(48, 17);
      this.StatusBox.Text = "Status : ";
      // 
      // ProgressBar
      // 
      this.ProgressBar.Name = "ProgressBar";
      this.ProgressBar.Size = new System.Drawing.Size(300, 16);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(800, 450);
      this.Controls.Add(this.statusStrip1);
      this.Controls.Add(this.menuStrip1);
      this.MainMenuStrip = this.menuStrip1;
      this.Name = "Form1";
      this.Text = "Form1";
      this.Load += new System.EventHandler(this.Form1_Load);
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      this.statusStrip1.ResumeLayout(false);
      this.statusStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem openFileToolStripMenuItem_Open;
    private System.Windows.Forms.ToolStripMenuItem closeFileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem_Exit;
    private System.Windows.Forms.StatusStrip statusStrip1;
    private System.Windows.Forms.ToolStripStatusLabel StatusBox;
    private System.Windows.Forms.ToolStripProgressBar ProgressBar;
  }
}

