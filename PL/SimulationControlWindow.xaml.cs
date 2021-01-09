using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BLAPI;

namespace PL
{
    /// <summary>
    /// Interaction logic for SimulationControlWindow.xaml
    /// </summary>
    public partial class SimulationControlWindow : Window
    {
        IBL bl;
        public static readonly DependencyProperty SimulatorInactiveProperty = DependencyProperty.Register("SimulatorInactive", typeof(Boolean), typeof(SimulationControlWindow));
        private bool SimulatorInactive
        {
            get => (bool)GetValue(SimulatorInactiveProperty);
            set => SetValue(SimulatorInactiveProperty, value); 
        }

        BackgroundWorker timerWorker;
        Thread workerThread;
        public SimulationControlWindow(IBL bl)
        {
            SimulatorInactive = true;
            this.bl = bl;
            timerWorker = new BackgroundWorker();
            InitializeComponent();
            timerWorker.DoWork += (s, e) =>
            {
                workerThread = Thread.CurrentThread;
                while (!timerWorker.CancellationPending) try { Thread.Sleep(1000000); } catch (Exception ex) { }
            };
            timerWorker.ProgressChanged += timer_ProgressChanged;
            timerWorker.WorkerReportsProgress = true;
            timerWorker.WorkerSupportsCancellation = true;
        }

        private void timer_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            TimeSpan time = (TimeSpan)e.UserState;
            timerTextBox.Text = String.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours,time.Minutes,time.Seconds);
        }

        private void simulatorButton_Click(object sender, RoutedEventArgs e)
        {
            if (SimulatorInactive)
            {
                TimeSpan startTime;
                int rate;
                if (!TimeSpan.TryParse(timerTextBox.Text, out startTime) || !int.TryParse(rateTextBox.Text, out rate))
                {
                    MessageBox.Show("Wrong timer format");
                    return;
                }
                timerWorker.RunWorkerAsync();
                SimulatorInactive = false;
                ((Button)sender).Content = "Stop";
                bl.StartSimulator(startTime, rate, (time) => timerWorker.ReportProgress(0, time));
            }
            else
            {
                SimulatorInactive = true;
                ((Button)sender).Content = "Start";
                bl.StopSimulator();
                timerWorker.CancelAsync();
                workerThread.Interrupt();
            }
        }
        private void rateTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e) => e.Handled = e.Text == null || !e.Text.All(char.IsDigit);
    }
}
