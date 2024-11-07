namespace SoundView {
  partial class Form1 {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      components = new System.ComponentModel.Container();
      timer = new System.Windows.Forms.Timer(components);
      label1 = new Label();
      label2 = new Label();
      panel1 = new Panel();
      panel2 = new Panel();
      SuspendLayout();
      // 
      // timer
      // 
      timer.Enabled = true;
      timer.Interval = +10;
      timer.Tick += timer1_Tick;
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Font = new Font("Microsoft Sans Serif", 13F, FontStyle.Regular, GraphicsUnit.Point, 0);
      label1.Location = new Point(113, 67);
      label1.Margin = new Padding(2, 0, 2, 0);
      label1.Name = "label1";
      label1.Size = new Size(40, 22);
      label1.TabIndex = 4;
      label1.Text = "Left";
      // 
      // label2
      // 
      label2.AutoSize = true;
      label2.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
      label2.Location = new Point(361, 67);
      label2.Margin = new Padding(2, 0, 2, 0);
      label2.Name = "label2";
      label2.Size = new Size(47, 20);
      label2.TabIndex = 5;
      label2.Text = "Right";
      // 
      // panel1
      // 
      panel1.BackColor = Color.Black;
      panel1.BorderStyle = BorderStyle.FixedSingle;
      panel1.Dock = DockStyle.Left;
      panel1.Location = new Point(0, 0);
      panel1.Margin = new Padding(2);
      panel1.Name = "panel1";
      panel1.Size = new Size(64, 1080);
      panel1.TabIndex = 6;
      // 
      // panel2
      // 
      panel2.BackColor = Color.Black;
      panel2.BorderStyle = BorderStyle.FixedSingle;
      panel2.Dock = DockStyle.Right;
      panel2.Location = new Point(1856, 0);
      panel2.Margin = new Padding(2);
      panel2.Name = "panel2";
      panel2.Size = new Size(64, 1080);
      panel2.TabIndex = 7;
      // 
      // Form1
      // 
      AutoScaleMode = AutoScaleMode.Inherit;
      BackColor = Color.Black;
      ClientSize = new Size(1920, 1080);
      Controls.Add(panel2);
      Controls.Add(panel1);
      Controls.Add(label2);
      Controls.Add(label1);
      DoubleBuffered = true;
      FormBorderStyle = FormBorderStyle.None;
      Margin = new Padding(2);
      Name = "Form1";
      ShowIcon = false;
      Text = "Form1";
      TopMost = true;
      TransparencyKey = Color.Black;
      WindowState = FormWindowState.Maximized;
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private System.Windows.Forms.Timer timer;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Panel panel2;
  }
}

