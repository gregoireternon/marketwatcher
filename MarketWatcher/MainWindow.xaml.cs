using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MarketWatcher
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window, IWatcherObserver
    {
        private static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();
        public Watcher _watcher;
        RootEntity rootEntity; 


        public MainWindow()
        {
            _log.Info("create Main window");

            InitializeComponent();
            try
            {
                rootEntity = JsonConvert.DeserializeObject<RootEntity>(File.ReadAllText(Watcher.BACKUPFILE_LOCATION));
                
                instrument.Text = rootEntity.Instrument;
            }
            catch (Exception e)
            {
                rootEntity = new RootEntity()
                {
                    Alerts = new ObservableCollection<AlertSetupEntity>()
                };
            }
            
            
            dataGrid.ItemsSource = rootEntity.Alerts;
            _watcher = new Watcher(this, rootEntity);

            rootEntity.Alerts.CollectionChanged += AlertSetupEntity_Changed;

        }

        private void AlertSetupEntity_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            return;
        }

        private void instrument_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (rootEntity != null)
            {
                rootEntity.Instrument = instrument.Text;
            }
        }

        private void startWatch_Click(object sender, RoutedEventArgs e)
        {
            if (!_watcher.Watching)
            {
                rootEntity.Instrument = instrument.Text;
                
                Thread t = new Thread(_watcher.Watch);
                t.Start();
            }
            else
            {
                _log.Info("Already watching");
            }

        }

        private void stopWatch_Click(object sender, RoutedEventArgs e)
        {
            _watcher.StopWatch();
        }

        public void HandleQuote(Quote quote)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    float formerValue = float.Parse(QuoteValue.Content.ToString());
                    if (formerValue < quote.Cours)
                    {
                        QuoteValue.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else if (formerValue > quote.Cours)
                    {
                        QuoteValue.Foreground = new SolidColorBrush(Colors.Red);
                    }
                }
                catch (Exception e)
                {
                    _log.Debug("Error updating quote display");
                }
                QuoteValue.Content = quote.Cours;


            }));
        }




        private void DeleteItem(object sender, RoutedEventArgs e)
        {
            var selectedItem = dataGrid.SelectedItem;
            if (selectedItem != null)
            {
                rootEntity.Alerts.Remove(selectedItem as AlertSetupEntity);
                dataGrid.SelectedItem = null;
            }
        }

        public void HandleWatchingState()
        {
            

            Dispatcher.BeginInvoke(new Action(() =>
            {
                File.WriteAllText(Watcher.BACKUPFILE_LOCATION, JsonConvert.SerializeObject(rootEntity, Formatting.Indented));
                Watch.IsEnabled = false;
                stopWatch.IsEnabled = true;
                instrument.IsEnabled = false;
            }));
        }

        public void HandleStopWatchingState()
        {
            

            Dispatcher.BeginInvoke(new Action(() =>
            {
                File.WriteAllText(Watcher.BACKUPFILE_LOCATION, JsonConvert.SerializeObject(rootEntity, Formatting.Indented));
                Watch.IsEnabled = true;
                stopWatch.IsEnabled = false;
                instrument.IsEnabled = true;
            }));
        }
    }
}
