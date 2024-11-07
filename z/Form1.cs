using NAudio.CoreAudioApi;

namespace SoundView {
  public partial class Form1 : Form {
    MMDevice device = null;

    public Form1() {
      InitializeComponent();
      MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
      var devices = enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
      device = devices.ToArray()[0];
    }

    private void timer1_Tick(object sender, EventArgs e) {
      if (device != null) {
        // Calculate the difference between left and right channels, emphasizing the difference
        var leftRightDifference = device.AudioMeterInformation.PeakValues[0] - device.AudioMeterInformation.PeakValues[1];
        
        // Increase sensitivity by amplifying the difference
        var val = (int)(((leftRightDifference * 50) + 50)); // Higher multiplier for more sensitivity

        Console.WriteLine(val);

        // Adjust thresholds for more sensitivity
        if (val < 25) {
          panel2.BackColor = Color.Red;
        } else {
          panel2.BackColor = SystemColors.Control;
        }

        if (val > 75) {
          panel1.BackColor = Color.Red;
        } else {
          panel1.BackColor = SystemColors.Control;
        }
      }
    }
  }
}
