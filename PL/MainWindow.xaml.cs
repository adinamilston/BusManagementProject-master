using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

using BLAPI;
using BO;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IBL bl = BLFactory.GetBL("1");
        BackgroundWorker panelWorker; // bkg worker for updating arriving lines
        List<LineTiming> panelLines = new List<LineTiming>(); // collection of arriving lines

        public MainWindow()
        {
            InitializeComponent();
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            busStationListView.DataContext = bl.GetAllBusStations();
            panelWorker = new BackgroundWorker();
            panelWorker.DoWork += (s, e) =>
            { // Fake bkg worker - it must run in order for progressChanged to work properly
                while (!panelWorker.CancellationPending) try { Thread.Sleep(1000000); } catch (Exception ex) { }
            };
            panelWorker.ProgressChanged += panel_ProgressChanged;
            panelWorker.WorkerReportsProgress = true;
            panelWorker.WorkerSupportsCancellation = true;
            panelWorker.RunWorkerAsync();
        }

        private void busStationListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BO.BusStation busStation = (sender as ListView).SelectedItem as BO.BusStation;
            busLineListView.ItemsSource = bl.GetBusLineNumbers(busStation.Code).OrderBy(n => n);
            panelLines = new List<LineTiming>();
            lineTimingListView.ItemsSource = null;
            bl.SetStationPanel(busStation.Code, stationObserver);
        }

        private void panel_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            LineTiming lineTiming = (LineTiming)e.UserState;
            int index = panelLines.IndexOf(lineTiming);
            if (index == -1)
            { // It's a new line bus coming soon here
                if (lineTiming.Timing == TimeSpan.Zero) return;
                panelLines.Add(lineTiming);
                panelLines.Sort((lnTime1, lnTime2) => (int)(lnTime1.Timing - lnTime2.Timing).TotalMilliseconds);
            }
            else
            {
                if (lineTiming.Timing == TimeSpan.Zero)
                    panelLines.Remove(lineTiming);
                else
                    panelLines.Sort((lt1, lt2) => (int)(lt1.Timing - lt2.Timing).TotalMilliseconds);
            }
            lineTimingListView.ItemsSource = null;
            int count = (panelLines.Count < 5) ? panelLines.Count : 5;
            lineTimingListView.ItemsSource = panelLines.GetRange(0, count);
        }

        private void stationObserver(LineTiming lineTiming)
            => panelWorker.ReportProgress(0, lineTiming);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            new SimulationControlWindow(bl).Show();
        }
    }
}
