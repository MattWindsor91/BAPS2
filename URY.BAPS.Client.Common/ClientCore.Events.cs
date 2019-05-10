using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using JetBrains.Annotations;
using URY.BAPS.Client.Common.Updaters;
using URY.BAPS.Common.Protocol.V2.Commands;

namespace URY.BAPS.Client.Common
{
    public partial class ClientCore
    {
        private DetachableServerUpdater _updater = new DetachableServerUpdater();
        public IServerUpdater Updater => _updater;
        
        /// <summary>
        ///     Subscribes the forwarding <see cref="Subject{T}" /> on the client core to
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