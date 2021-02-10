using System;
using System.IO;
using System.Linq;
using System.Media;
using System.Net.Http;
using System.Reflection;
using System.Threading;

namespace MarketWatcher
{
    public class Watcher
    {
        private static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();
        private IWatcherObserver _observer;
        private RootEntity _root;
        private Quote lastQuote=null;
        public const string BACKUPFILE_LOCATION = "backup.data";
        public Watcher(IWatcherObserver observer, RootEntity root)
        {
            this._observer = observer;
            _root = root;
        }

        public bool Watching { get; set; } = false;
        public void StopWatch()
        {
            Watching = false;
        }

        public async void Watch()
        {
            if (Watching) return;

            _observer.HandleWatchingState();
            Watching = true;
            HttpClient clientHttp = new HttpClient();
            string path = "https://www.boursorama.com/bourse/action/graph/ws/UpdateCharts?symbol=2rP" + _root.Instrument + "&period=-1";
            while (Watching)
            {
                Thread.Sleep(2000);
                try
                {
                    HttpResponseMessage response = await clientHttp.GetAsync(path);
                    if (response.IsSuccessStatusCode)
                    {
                        QuoteHolder res = await response.Content.ReadAsAsync<QuoteHolder>();
                        _log.Info("Received:" + res);
                        ManagerAlert(res);
                        _observer.HandleQuote(res.Quotes.First());
                    }

                }
                catch (Exception e)
                {
                    _log.Error(e,"Error");
                }
                
                _log.Info("Waiting...");
            }
            _observer.HandleStopWatchingState();
            _log.Info("Stop was called");
        }

        private void ManagerAlert(QuoteHolder res)
        {
            Quote newQuote = res.Quotes.First();
            if (lastQuote != null)
            {
                if(_root.Alerts!=null && _root.Alerts.Count > 0)
                {
                    foreach(AlertSetupEntity alert in _root.Alerts)
                    {
                        try
                        {
                            if (alert.Type == Type.Montant)
                            {
                                if (newQuote.Cours >= alert.Seuil && lastQuote.Cours < alert.Seuil)
                                {
                                    PlaySound();
                                }
                            }
                            if (alert.Type == Type.Descendant)
                            {
                                if (newQuote.Cours <= alert.Seuil && lastQuote.Cours > alert.Seuil)
                                {
                                    PlaySound();
                                }
                            }
                        }
                        catch(Exception e)
                        {
                            _log.Warn(e,"Error applying rule");
                        }
                        
                    }
                }
            }
            lastQuote = newQuote;
        }

        private void PlaySound()
        {
            var resourceName = "MarketWatcher.Resources.bell.wav";

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {

                SoundPlayer player = new SoundPlayer(stream);
                player.Load();
                player.Play();
            }
        }
    }
}
