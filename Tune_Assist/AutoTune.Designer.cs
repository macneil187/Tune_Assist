 using System.Drawing;

namespace AutoTune
{
  partial class AutoTune
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AutoTune));
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
      //this.helpForm = new AutoTune.HelpForm();
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.openFileToolStripMenuItem_Open = new System.Windows.Forms.ToolStripMenuItem();
      this.closeFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
      this.fileToolStripMenuItem_Exit = new System.Windows.Forms.ToolStripMenuItem();
      this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.viewHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.aboutUsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.statusStrip1 = new System.Windows.Forms.StatusStrip();
      this.ProgressBar = new System.Windows.Forms.ToolStripProgressBar();
      this.StatusBox = new System.Windows.Forms.ToolStripStatusLabel();
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.tabPage1 = new System.Windows.Forms.TabPage();
      this.buffDV1 = new Buffer.BuffDV();
      this.tabPage2 = new System.Windows.Forms.TabPage();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.buffDVmaf1 = new Buffer.BuffDV();
      this.buffDVmaf2 = new Buffer.BuffDV();
      this.textBox_MAF1 = new System.Windows.Forms.TextBox();
      this.textBox_MAF2 = new System.Windows.Forms.TextBox();
      this.btnCancelParse = new System.Windows.Forms.Button();
      this.menuStrip1.SuspendLayout();
      this.statusStrip1.SuspendLayout();
      this.tabControl1.SuspendLayout();
      this.tabPage1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.buffDV1)).BeginInit();
      this.tabPage2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.buffDVmaf1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.buffDVmaf2)).BeginInit();
      this.SuspendLayout();
      // 
      // helpForm
      // 
    /*  this.helpForm.BackColor = System.Drawing.SystemColors.ControlLight;
      this.helpForm.ClientSize = new System.Drawing.Size(593, 239);
      this.helpForm.ForeColor = System.Drawing.SystemColors.ControlText;
      this.helpForm.Location = new System.Drawing.Point(156, 156);
      this.helpForm.Name = "helpForm";
      this.helpForm.Opacity = 0.95D;
      this.helpForm.ShowIcon = false;
      this.helpForm.Text = "Help";
      this.helpForm.Visible = false;  */
      // 
      // menuStrip1
      // 
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Size = new System.Drawing.Size(800, 24);
      this.menuStrip1.Stretch = false;
      this.menuStrip1.TabIndex = 0;
      this.menuStrip1.Text = "menuStrip1";
      // 
      // fileToolStripMenuItem
      // 
      this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFileToolStripMenuItem_Open,
            this.closeFileToolStripMenuItem,
            this.toolStripSeparator1,
            this.toolStripMenuItem1,
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
      this.closeFileToolStripMenuItem.Enabled = false;
      this.closeFileToolStripMenuItem.Name = "closeFileToolStripMenuItem";
      this.closeFileToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
      this.closeFileToolStripMenuItem.Text = "Close File";
      this.closeFileToolStripMenuItem.Click += new System.EventHandler(this.closeFileToolStripMenuItem_Click);
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
      // 
      // toolStripMenuItem1
      // 
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
      this.toolStripMenuItem1.Text = "Options";
      this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
      // 
      // toolStripMenuItem2
      // 
      this.toolStripMenuItem2.Name = "toolStripMenuItem2";
      this.toolStripMenuItem2.Size = new System.Drawing.Size(177, 6);
      // 
      // fileToolStripMenuItem_Exit
      // 
      this.fileToolStripMenuItem_Exit.Name = "fileToolStripMenuItem_Exit";
      this.fileToolStripMenuItem_Exit.Size = new System.Drawing.Size(180, 22);
      this.fileToolStripMenuItem_Exit.Text = "Exit";
      this.fileToolStripMenuItem_Exit.Click += new System.EventHandler(this.fileToolStripMenuItem_Exit_Click);
      // 
      // helpToolStripMenuItem
      // 
      this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewHelpToolStripMenuItem,
            this.aboutUsToolStripMenuItem});
      this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
      this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
      this.helpToolStripMenuItem.Text = "Help";
      // 
      // viewHelpToolStripMenuItem
      // 
      this.viewHelpToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("viewHelpToolStripMenuItem.Image")));
      this.viewHelpToolStripMenuItem.Name = "viewHelpToolStripMenuItem";
      this.viewHelpToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
      this.viewHelpToolStripMenuItem.Text = "View Help";
      this.viewHelpToolStripMenuItem.Click += new System.EventHandler(this.viewHelpToolStripMenuItem_Click);
      // 
      // aboutUsToolStripMenuItem
      // 
      this.aboutUsToolStripMenuItem.Name = "aboutUsToolStripMenuItem";
      this.aboutUsToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
      this.aboutUsToolStripMenuItem.Text = "About Us";
      this.aboutUsToolStripMenuItem.Click += new System.EventHandler(this.aboutUsToolStripMenuItem_Click);
      // 
      // statusStrip1
      // 
      this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ProgressBar,
            this.StatusBox});
      this.statusStrip1.Location = new System.Drawing.Point(0, 428);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Size = new System.Drawing.Size(800, 22);
      this.statusStrip1.TabIndex = 1;
      this.statusStrip1.Text = "statusStrip1";
      // 
      // ProgressBar
      // 
      this.ProgressBar.Name = "ProgressBar";
      this.ProgressBar.Size = new System.Drawing.Size(300, 16);
      // 
      // StatusBox
      // 
      this.StatusBox.Name = "StatusBox";
      this.StatusBox.Size = new System.Drawing.Size(0, 17);
      // 
      // tabControl1
      // 
      this.tabControl1.Controls.Add(this.tabPage1);
      this.tabControl1.Controls.Add(this.tabPage2);
      this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl1.Location = new System.Drawing.Point(0, 24);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(800, 404);
      this.tabControl1.TabIndex = 2;
      // 
      // tabPage1
      // 
      this.tabPage1.Controls.Add(this.buffDV1);
      this.tabPage1.Location = new System.Drawing.Point(4, 22);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage1.Size = new System.Drawing.Size(792, 378);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "User Log";
      this.tabPage1.UseVisualStyleBackColor = true;
      // 
      // buffDV1
      // 
      this.buffDV1.AllowUserToAddRows = false;
      this.buffDV1.AllowUserToDeleteRows = false;
      this.buffDV1.AllowUserToResizeRows = false;
      this.buffDV1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
      dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      dataGridViewCellStyle1.BackColor = System.Drawing.Color.Navy;
      dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Bold);
      dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
      dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
      this.buffDV1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
      this.buffDV1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
      dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
      dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
      dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
      this.buffDV1.DefaultCellStyle = dataGridViewCellStyle2;
      this.buffDV1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.buffDV1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
      this.buffDV1.GridColor = System.Drawing.Color.Black;
      this.buffDV1.Location = new System.Drawing.Point(3, 3);
      this.buffDV1.Name = "buffDV1";
      this.buffDV1.ReadOnly = true;
      this.buffDV1.RowHeadersVisible = false;
      this.buffDV1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
      this.buffDV1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
      this.buffDV1.Size = new System.Drawing.Size(786, 372);
      this.buffDV1.TabIndex = 0;
      this.buffDV1.Visible = false;
      this.buffDV1.VisibleChanged += new System.EventHandler(this.buffDV1_VisibleChanged);
      this.buffDV1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.buffDVmaf_KeyDown);
      this.buffDV1.Leave += new System.EventHandler(this.buffDV1_Leave);
      // 
      // tabPage2
      // 
      this.tabPage2.Controls.Add(this.splitContainer1);
      this.tabPage2.Location = new System.Drawing.Point(4, 22);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage2.Size = new System.Drawing.Size(792, 378);
      this.tabPage2.TabIndex = 1;
      this.tabPage2.Text = "Fuel";
      this.tabPage2.UseVisualStyleBackColor = true;
      this.tabPage2.Enter += new System.EventHandler(this.tabPage2_Enter);
      this.tabPage2.Leave += new System.EventHandler(this.tabPage2_Leave);
      // 
      // splitContainer1
      // 
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Left;
      this.splitContainer1.IsSplitterFixed = true;
      this.splitContainer1.Location = new System.Drawing.Point(3, 3);
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.buffDVmaf1);
      this.splitContainer1.Panel1MinSize = 173;
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.buffDVmaf2);
      this.splitContainer1.Panel2MinSize = 173;
      this.splitContainer1.Size = new System.Drawing.Size(506, 372);
      this.splitContainer1.SplitterDistance = 253;
      this.splitContainer1.SplitterWidth = 1;
      this.splitContainer1.TabIndex = 4;
      // 
      // buffDVmaf1
      // 
      this.buffDVmaf1.AllowUserToAddRows = false;
      this.buffDVmaf1.AllowUserToDeleteRows = false;
      this.buffDVmaf1.AllowUserToResizeColumns = false;
      this.buffDVmaf1.AllowUserToResizeRows = false;
      this.buffDVmaf1.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
      this.buffDVmaf1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
      dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
      dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
      dataGridViewCellStyle3.Format = "N0";
      dataGridViewCellStyle3.NullValue = null;
      dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
      this.buffDVmaf1.DefaultCellStyle = dataGridViewCellStyle3;
      this.buffDVmaf1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.buffDVmaf1.Location = new System.Drawing.Point(0, 0);
      this.buffDVmaf1.Name = "buffDVmaf1";
      this.buffDVmaf1.RowHeadersVisible = false;
      this.buffDVmaf1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
      this.buffDVmaf1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
      this.buffDVmaf1.ShowCellToolTips = false;
      this.buffDVmaf1.Size = new System.Drawing.Size(253, 372);
      this.buffDVmaf1.TabIndex = 0;
      this.buffDVmaf1.Visible = false;
      this.buffDVmaf1.CellParsing += new System.Windows.Forms.DataGridViewCellParsingEventHandler(this.buffDVmaf_CellParsing);
      this.buffDVmaf1.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.buffDVmaf1_CellValidated);
      this.buffDVmaf1.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.buffDVmaf_EditingControlShowing);
      this.buffDVmaf1.VisibleChanged += new System.EventHandler(this.buffDVmaf1_VisibleChanged);
      this.buffDVmaf1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.buffDVmaf_KeyDown);
      this.buffDVmaf1.Leave += new System.EventHandler(this.buffDVmaf1_Leave);
      // 
      // buffDVmaf2
      // 
      this.buffDVmaf2.AllowUserToAddRows = false;
      this.buffDVmaf2.AllowUserToDeleteRows = false;
      this.buffDVmaf2.AllowUserToResizeColumns = false;
      this.buffDVmaf2.AllowUserToResizeRows = false;
      this.buffDVmaf2.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
      this.buffDVmaf2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
      dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
      dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
      dataGridViewCellStyle4.Format = "N0";
      dataGridViewCellStyle4.NullValue = null;
      dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
      this.buffDVmaf2.DefaultCellStyle = dataGridViewCellStyle4;
      this.buffDVmaf2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.buffDVmaf2.Location = new System.Drawing.Point(0, 0);
      this.buffDVmaf2.Name = "buffDVmaf2";
      this.buffDVmaf2.RowHeadersVisible = false;
      this.buffDVmaf2.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
      this.buffDVmaf2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
      this.buffDVmaf2.ShowCellToolTips = false;
      this.buffDVmaf2.Size = new System.Drawing.Size(252, 372);
      this.buffDVmaf2.TabIndex = 1;
      this.buffDVmaf2.Visible = false;
      this.buffDVmaf2.CellParsing += new System.Windows.Forms.DataGridViewCellParsingEventHandler(this.buffDVmaf_CellParsing);
      this.buffDVmaf2.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.buffDVmaf2_CellValidated);
      this.buffDVmaf2.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.buffDVmaf_EditingControlShowing);
      this.buffDVmaf2.VisibleChanged += new System.EventHandler(this.buffDVmaf2_VisibleChanged);
      this.buffDVmaf2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.buffDVmaf_KeyDown);
      this.buffDVmaf2.Leave += new System.EventHandler(this.buffDVmaf2_Leave);
      // 
      // textBox_MAF1
      // 
      this.textBox_MAF1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.textBox_MAF1.BackColor = System.Drawing.SystemColors.AppWorkspace;
      this.textBox_MAF1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.textBox_MAF1.Cursor = System.Windows.Forms.Cursors.Default;
      this.textBox_MAF1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBox_MAF1.Location = new System.Drawing.Point(4, 427);
      this.textBox_MAF1.MaxLength = 12;
      this.textBox_MAF1.Name = "textBox_MAF1";
      this.textBox_MAF1.Size = new System.Drawing.Size(256, 20);
      this.textBox_MAF1.TabIndex = 2;
      this.textBox_MAF1.Text = "MAF Bank 1";
      this.textBox_MAF1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      this.textBox_MAF1.Visible = false;
      // 
      // textBox_MAF2
      // 
      this.textBox_MAF2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.textBox_MAF2.BackColor = System.Drawing.SystemColors.AppWorkspace;
      this.textBox_MAF2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.textBox_MAF2.Cursor = System.Windows.Forms.Cursors.Default;
      this.textBox_MAF2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBox_MAF2.Location = new System.Drawing.Point(261, 427);
      this.textBox_MAF2.MaxLength = 12;
      this.textBox_MAF2.Name = "textBox_MAF2";
      this.textBox_MAF2.Size = new System.Drawing.Size(252, 20);
      this.textBox_MAF2.TabIndex = 3;
      this.textBox_MAF2.Text = "MAF Bank 2";
      this.textBox_MAF2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      this.textBox_MAF2.Visible = false;
      // 
      // btnCancelParse
      // 
      this.btnCancelParse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnCancelParse.Location = new System.Drawing.Point(681, 427);
      this.btnCancelParse.Name = "btnCancelParse";
      this.btnCancelParse.Size = new System.Drawing.Size(107, 23);
      this.btnCancelParse.TabIndex = 1;
      this.btnCancelParse.Text = "Cancel Load";
      this.btnCancelParse.UseVisualStyleBackColor = true;
      this.btnCancelParse.Visible = false;
      this.btnCancelParse.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // AutoTune
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(800, 450);
      this.Controls.Add(this.btnCancelParse);
      this.Controls.Add(this.textBox_MAF2);
      this.Controls.Add(this.textBox_MAF1);
      this.Controls.Add(this.tabControl1);
      this.Controls.Add(this.statusStrip1);
      this.Controls.Add(this.menuStrip1);
      this.MainMenuStrip = this.menuStrip1;
      this.Name = "AutoTune";
      this.Text = "Tune Assist";
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      this.statusStrip1.ResumeLayout(false);
      this.statusStrip1.PerformLayout();
      this.tabControl1.ResumeLayout(false);
      this.tabPage1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.buffDV1)).EndInit();
      this.tabPage2.ResumeLayout(false);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.buffDVmaf1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.buffDVmaf2)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem openFileToolStripMenuItem_Open;
    private System.Windows.Forms.ToolStripMenuItem closeFileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem_Exit;
    private System.Windows.Forms.StatusStrip statusStrip1;
    private System.Windows.Forms.ToolStripProgressBar ProgressBar;
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.TabPage tabPage2;
    public Buffer.BuffDV buffDV1;
    private Parser parser;
    public HelpForm helpForm;
    public About about;
    private System.Windows.Forms.Button btnCancelParse;
    private System.Windows.Forms.ToolStripStatusLabel StatusBox;
    private Buffer.BuffDV buffDVmaf1;
    private System.Windows.Forms.TextBox textBox_MAF1;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.TextBox textBox_MAF2;
    private Buffer.BuffDV buffDVmaf2;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
    private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem viewHelpToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem aboutUsToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
  }
}

