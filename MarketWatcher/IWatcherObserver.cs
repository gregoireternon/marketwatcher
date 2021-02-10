namespace MarketWatcher
{
    public interface IWatcherObserver
    {
        void HandleQuote(Quote quote);

        void HandleWatchingState();
        void HandleStopWatchingState();
    }
}
