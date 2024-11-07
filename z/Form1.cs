using NAudio.CoreAudioApi;

namespace SoundView {
  public partial class Form1 : Form {
    MMDevice device = null;

    public Form1() {
      InitializeComponent();
      MMDeviceEnumerator enumerator = new();
      var devices = enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
      device = devices.ToArray()[0];
    }

    private void timer1_Tick(object sender, EventArgs e) {
      double difference = device.AudioMeterInformation.PeakValues[0] - device.AudioMeterInformation.PeakValues[1];
      double es = +0.06249999;

      if (difference <= -es) {
        panel2.BackColor = Color.Red;
      } else {
        panel2.BackColor = SystemColors.Control;
      }

      if (difference >= +es) {
        panel1.BackColor = Color.Red;
      } else {
        panel1.BackColor = SystemColors.Control;
      }
    }
  }
}
