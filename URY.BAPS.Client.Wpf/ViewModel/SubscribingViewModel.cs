using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace URY.BAPS.Client.Wpf.ViewModel
{
    /// <summary>
    ///     Base for view models that maintain a set of observer subscriptions.
    /// </summary>
    public abstract class SubscribingViewModel : ViewModelBase, IDisposable
    {
        private readonly IList<IDisposable> _subscriptions = new List<IDisposable>();

        /// <summary>
        ///     Subscribes this view model to an observable using the given callback.
        /// </summary>
        /// <typeparam name="T">Type of observable messages.</typeparam>
        /// <param name="observable">The observable to which we are subscribing.</param>
        /// <param name="callback">The callback to use when the observable fires.</param>
        protected void SubscribeTo<T>(IObservable<T> observable, Action<T> callback)
        {
            _subscriptions.Add(observable.Subscribe(callback));
        }

        public virtual void Dispose()
        {
            foreach (var subscription in _subscriptions)
            {
                subscription.Dispose();
            }
        }
    }
}
