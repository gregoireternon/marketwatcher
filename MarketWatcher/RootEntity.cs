using System.Collections.ObjectModel;

namespace MarketWatcher
{
    public class RootEntity
    {
        public ObservableCollection<AlertSetupEntity> Alerts { get; set; }
        public string Instrument { get; set; }
    }
}
