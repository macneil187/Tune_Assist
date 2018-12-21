namespace AutoTune
{
  partial class OptionForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionForm));
      this.OptionTabControl = new System.Windows.Forms.TabControl();
      this.OptionTab1 = new System.Windows.Forms.TabPage();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.Minimal_MAF_checkbox = new System.Windows.Forms.CheckBox();
      this.checkBoxAirTemp = new System.Windows.Forms.CheckBox();
      this.checkBoxAccelChange = new System.Windows.Forms.CheckBox();
      this.checkBoxOpenLoop = new System.Windows.Forms.CheckBox();
      this.checkBoxClosedLoop = new System.Windows.Forms.CheckBox();
      this.richTextBoxOptions = new System.Windows.Forms.RichTextBox();
      this.OptionTab2 = new System.Windows.Forms.TabPage();
      this.statusStrip1 = new System.Windows.Forms.StatusStrip();
      this.buttonSave = new System.Windows.Forms.Button();
      this.buttonCancel = new System.Windows.Forms.Button();
      this.OptionTabControl.SuspendLayout();
      this.OptionTab1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.SuspendLayout();
      // 
      // OptionTabControl
      // 
      this.OptionTabControl.Controls.Add(this.OptionTab1);
      this.OptionTabControl.Controls.Add(this.OptionTab2);
      this.OptionTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.OptionTabControl.Location = new System.Drawing.Point(0, 0);
      this.OptionTabControl.Name = "OptionTabControl";
      this.OptionTabControl.SelectedIndex = 0;
      this.OptionTabControl.Size = new System.Drawing.Size(473, 224);
      this.OptionTabControl.TabIndex = 0;
      // 
      // OptionTab1
      // 
      this.OptionTab1.BackColor = System.Drawing.SystemColors.Control;
      this.OptionTab1.Controls.Add(this.splitContainer1);
      this.OptionTab1.Location = new System.Drawing.Point(4, 22);
      this.OptionTab1.Name = "OptionTab1";
      this.OptionTab1.Padding = new System.Windows.Forms.Padding(3);
      this.OptionTab1.Size = new System.Drawing.Size(465, 198);
      this.OptionTab1.TabIndex = 0;
      this.OptionTab1.Text = "MAF";
      // 
      // splitContainer1
      // 
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer1.Location = new System.Drawing.Point(3, 3);
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.Minimal_MAF_checkbox);
      this.splitContainer1.Panel1.Controls.Add(this.checkBoxAirTemp);
      this.splitContainer1.Panel1.Controls.Add(this.checkBoxAccelChange);
      this.splitContainer1.Panel1.Controls.Add(this.checkBoxOpenLoop);
      this.splitContainer1.Panel1.Controls.Add(this.checkBoxClosedLoop);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.richTextBoxOptions);
      this.splitContainer1.Size = new System.Drawing.Size(459, 192);
      this.splitContainer1.SplitterDistance = 198;
      this.splitContainer1.TabIndex = 0;
      // 
      // Minimal_MAF_checkbox
      // 
      this.Minimal_MAF_checkbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.Minimal_MAF_checkbox.AutoSize = true;
      this.Minimal_MAF_checkbox.Checked = global::AutoTune.Properties.Settings.Default.Maf_MINIMAL;
      this.Minimal_MAF_checkbox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.Minimal_MAF_checkbox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::AutoTune.Properties.Settings.Default, "Maf_MINIMAL", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.Minimal_MAF_checkbox.Location = new System.Drawing.Point(3, 145);
      this.Minimal_MAF_checkbox.Name = "Minimal_MAF_checkbox";
      this.Minimal_MAF_checkbox.Size = new System.Drawing.Size(136, 17);
      this.Minimal_MAF_checkbox.TabIndex = 4;
      this.Minimal_MAF_checkbox.Text = "Minimize MAF Changes";
      this.Minimal_MAF_checkbox.UseVisualStyleBackColor = true;
      // 
      // checkBoxAirTemp
      // 
      this.checkBoxAirTemp.AutoSize = true;
      this.checkBoxAirTemp.Checked = global::AutoTune.Properties.Settings.Default.MAF_IAT;
      this.checkBoxAirTemp.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkBoxAirTemp.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::AutoTune.Properties.Settings.Default, "MAF_IAT", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.checkBoxAirTemp.Location = new System.Drawing.Point(3, 85);
      this.checkBoxAirTemp.Name = "checkBoxAirTemp";
      this.checkBoxAirTemp.Size = new System.Drawing.Size(171, 17);
      this.checkBoxAirTemp.TabIndex = 3;
      this.checkBoxAirTemp.Text = "Filter Intake Air Temp Changes";
      this.checkBoxAirTemp.UseVisualStyleBackColor = true;
      // 
      // checkBoxAccelChange
      // 
      this.checkBoxAccelChange.AutoSize = true;
      this.checkBoxAccelChange.Checked = global::AutoTune.Properties.Settings.Default.MAF_ACCEL;
      this.checkBoxAccelChange.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkBoxAccelChange.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::AutoTune.Properties.Settings.Default, "MAF_ACCEL", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.checkBoxAccelChange.Location = new System.Drawing.Point(3, 62);
      this.checkBoxAccelChange.Name = "checkBoxAccelChange";
      this.checkBoxAccelChange.Size = new System.Drawing.Size(154, 17);
      this.checkBoxAccelChange.TabIndex = 2;
      this.checkBoxAccelChange.Text = "Filter Quick Accel Changes";
      this.checkBoxAccelChange.UseVisualStyleBackColor = true;
      // 
      // checkBoxOpenLoop
      // 
      this.checkBoxOpenLoop.AutoSize = true;
      this.checkBoxOpenLoop.Checked = global::AutoTune.Properties.Settings.Default.MAF_OL;
      this.checkBoxOpenLoop.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkBoxOpenLoop.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::AutoTune.Properties.Settings.Default, "MAF_OL", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.checkBoxOpenLoop.Location = new System.Drawing.Point(116, 3);
      this.checkBoxOpenLoop.Name = "checkBoxOpenLoop";
      this.checkBoxOpenLoop.Size = new System.Drawing.Size(79, 17);
      this.checkBoxOpenLoop.TabIndex = 1;
      this.checkBoxOpenLoop.Text = "Open Loop";
      this.checkBoxOpenLoop.UseVisualStyleBackColor = true;
      // 
      // checkBoxClosedLoop
      // 
      this.checkBoxClosedLoop.AutoSize = true;
      this.checkBoxClosedLoop.Checked = global::AutoTune.Properties.Settings.Default.MAF_CL;
      this.checkBoxClosedLoop.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkBoxClosedLoop.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::AutoTune.Properties.Settings.Default, "MAF_CL", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.checkBoxClosedLoop.Location = new System.Drawing.Point(3, 3);
      this.checkBoxClosedLoop.Name = "checkBoxClosedLoop";
      this.checkBoxClosedLoop.Size = new System.Drawing.Size(85, 17);
      this.checkBoxClosedLoop.TabIndex = 0;
      this.checkBoxClosedLoop.Text = "Closed Loop";
      this.checkBoxClosedLoop.UseVisualStyleBackColor = true;
      this.checkBoxClosedLoop.CheckedChanged += new System.EventHandler(this.checkBoxClosedLoop_CheckedChanged);
      // 
      // richTextBoxOptions
      // 
      this.richTextBoxOptions.BackColor = System.Drawing.SystemColors.Control;
      this.richTextBoxOptions.Dock = System.Windows.Forms.DockStyle.Fill;
      this.richTextBoxOptions.Location = new System.Drawing.Point(0, 0);
      this.richTextBoxOptions.Margin = new System.Windows.Forms.Padding(6);
      this.richTextBoxOptions.Name = "richTextBoxOptions";
      this.richTextBoxOptions.Size = new System.Drawing.Size(257, 192);
      this.richTextBoxOptions.TabIndex = 0;
      this.richTextBoxOptions.Text = resources.GetString("richTextBoxOptions.Text");
      // 
      // OptionTab2
      // 
      this.OptionTab2.BackColor = System.Drawing.SystemColors.Control;
      this.OptionTab2.Location = new System.Drawing.Point(4, 22);
      this.OptionTab2.Name = "OptionTab2";
      this.OptionTab2.Padding = new System.Windows.Forms.Padding(3);
      this.OptionTab2.Size = new System.Drawing.Size(465, 198);
      this.OptionTab2.TabIndex = 1;
      this.OptionTab2.Text = "Fuel Comp";
      // 
      // statusStrip1
      // 
      this.statusStrip1.Location = new System.Drawing.Point(0, 202);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Size = new System.Drawing.Size(473, 22);
      this.statusStrip1.TabIndex = 1;
      this.statusStrip1.Text = "statusStrip1";
      // 
      // buttonSave
      // 
      this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonSave.Location = new System.Drawing.Point(281, 201);
      this.buttonSave.Name = "buttonSave";
      this.buttonSave.Size = new System.Drawing.Size(75, 23);
      this.buttonSave.TabIndex = 2;
      this.buttonSave.Text = "Save";
      this.buttonSave.UseVisualStyleBackColor = true;
      this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
      // 
      // buttonCancel
      // 
      this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.buttonCancel.Location = new System.Drawing.Point(374, 202);
      this.buttonCancel.Name = "buttonCancel";
      this.buttonCancel.Size = new System.Drawing.Size(75, 23);
      this.buttonCancel.TabIndex = 3;
      this.buttonCancel.Text = "Cancel";
      this.buttonCancel.UseVisualStyleBackColor = true;
      this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
      // 
      // OptionForm
      // 
      this.AcceptButton = this.buttonSave;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Control;
      this.CancelButton = this.buttonCancel;
      this.ClientSize = new System.Drawing.Size(473, 224);
      this.Controls.Add(this.buttonCancel);
      this.Controls.Add(this.buttonSave);
      this.Controls.Add(this.statusStrip1);
      this.Controls.Add(this.OptionTabControl);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Name = "OptionForm";
      this.ShowIcon = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Options";
      this.OptionTabControl.ResumeLayout(false);
      this.OptionTab1.ResumeLayout(false);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel1.PerformLayout();
      this.splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TabControl OptionTabControl;
    private System.Windows.Forms.TabPage OptionTab1;
    private System.Windows.Forms.TabPage OptionTab2;
    private System.Windows.Forms.StatusStrip statusStrip1;
    private System.Windows.Forms.Button buttonSave;
    private System.Windows.Forms.Button buttonCancel;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.CheckBox checkBoxAirTemp;
    private System.Windows.Forms.CheckBox checkBoxAccelChange;
    private System.Windows.Forms.CheckBox checkBoxOpenLoop;
    private System.Windows.Forms.CheckBox checkBoxClosedLoop;
    private System.Windows.Forms.RichTextBox richTextBoxOptions;
    private System.Windows.Forms.CheckBox Minimal_MAF_checkbox;
  }
}