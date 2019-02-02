using System;
using BAPSClientCommon;
using BAPSClientCommon.Events;
using GalaSoft.MvvmLight.Messaging;

namespace BAPSPresenterNG
{
    /// <summary>
    ///     An object that listens to channel control events on the message
    ///     bus and forwards them to the appropriate channel controller.
    /// </summary>
    public class ChannelControllerMessengerAdapter
    {
        private readonly Func<ushort, ChannelController> _controllerFunc;
        private readonly IMessenger _messenger;

        /// <summary>
        ///     Constructs a <see cref="ChannelControllerMessengerAdapter" />.
        ///     <para>
        ///         This constructor registers the appropriate message handlers
        ///         on <paramref name="messenger" />.
        ///     </para>
        /// </summary>
        /// <param name="controllerFunc">A function that, given a channel ID, retrieves that channel's controller.</param>
        /// <param name="messenger">The messenger bus to forward onto.</param>
        public ChannelControllerMessengerAdapter(Func<ushort, ChannelController> controllerFunc, IMessenger messenger)
        {
            _controllerFunc = controllerFunc;
            _messenger = messenger;
        }

        /// <summary>
        ///     Registers the adapter as handling each request event on the message bus.
        /// </summary>
        public void Register()
        {
            _messenger.Register<Requests.MarkerEventArgs>(this, HandleChannelMarker);
        }

        private void HandleChannelMarker(Requests.MarkerEventArgs args)
        {
            _controllerFunc(args.ChannelId).SetMarker(args.Marker, args.NewValue);
        }
    }
}