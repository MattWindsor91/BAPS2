using URY.BAPS.Client.Common.Updaters;

namespace URY.BAPS.Client.Protocol.V2.Core
{
    public partial class ClientCore
    {
        private readonly DetachableServerUpdater _updater = new DetachableServerUpdater();
        public IServerUpdater Updater => _updater;
        
        /// <summary>
        ///     Subscribes the server updater on the client core to
        ///     that on the receiver.
        /// </summary>
        private void SubscribeToReceiver()
        {
            if (_receiver == null) return;
            _updater.Attach(_receiver.ObserveMessage);
        }

        /// <summary>
        ///     Disposes each subscription created by <see cref="SubscribeToReceiver" />.
        /// </summary>
        private void UnsubscribeFromReceiver()
        {
            _updater.Detach();
        }
    }
}