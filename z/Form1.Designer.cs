namespace SoundView {
  partial class Form1 {
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code
    private void InitializeComponent() {
      int wide = 2;
      int high = 2;

      components = new System.ComponentModel.Container();
      timer = new System.Windows.Forms.Timer(components);
      panel1 = new Panel();
      panel2 = new Panel();
      SuspendLayout();

      timer.Enabled = true;
      timer.Interval = 10;
      timer.Tick += timer1_Tick;

      panel2.Anchor = AnchorStyles.Top;
      panel2.BackColor = Color.Transparent;
      panel2.Location = new Point(((Screen.PrimaryScreen.Bounds.Width / +256) * +129) - (wide / +2), (Screen.PrimaryScreen.Bounds.Height / +2) - (high / +2));
      panel2.Name = "panel2";
      panel2.Size = new Size(wide, high);
      panel2.TabIndex = 2;

      panel1.Anchor = AnchorStyles.Top;
      panel1.BackColor = Color.Transparent;
      panel1.Location = new Point(((Screen.PrimaryScreen.Bounds.Width / +256) * +127) - (wide / +2), (Screen.PrimaryScreen.Bounds.Height / +2) - (high / +2));
      panel1.Name = "panel1";
      panel1.Size = new Size(wide, high);
      panel1.TabIndex = 1;

      AutoScaleMode = AutoScaleMode.Inherit;
      BackColor = Color.Black;
      ClientSize = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
      Controls.Add(panel2);
      Controls.Add(panel1);
      DoubleBuffered = true;
      FormBorderStyle = FormBorderStyle.None;
      Name = "Form1";
      Opacity = +1.0;
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
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Panel panel2;
  }
}
