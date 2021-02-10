namespace MarketWatcher
{
    public enum Type { Montant, Descendant };
    public class AlertSetupEntity
    {
        public float Seuil { get; set; }
        public Type Type { get; set; }
    }
}
