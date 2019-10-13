using System;
using System.Reactive.Disposables;
using ReactiveUI;

namespace URY.BAPS.Client.ViewModel
{
    /// <summary>
    ///     Abstract base class for view models.
    ///     <para>
    ///         View models follow the ReactiveUI format, and descend
    ///         ultimately from <see cref="ReactiveObject"/>.  They do not
    ///         (yet) implement the 'WhenActivated' pattern; instead, they
    ///         provide a method <see cref="AddSubscription"/> to track
    ///         disposable subscriptions, and rely on the top-level
    ///         dependency injection container disposing them when finished.
    ///     </para>
    /// </summary>
    public class ViewModelBase : ReactiveObject, IDisposable
    {
        private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

        /// <summary>
        ///     Adds an <see cref="IObservable{T}"/> subscription to the view model's internal pile of
        ///     subscriptions, ready for disposal when the view model is disposed.
        /// </summary>
        /// <param name="subscription">
        ///     The subscription to track.
        /// </param>
        protected void AddSubscription(IDisposable subscription)
        {
            _subscriptions.Add(subscription);
        }

        /// <summary>
        ///     Disposes of a <see cref="ViewModelBase"/> by disposing of its
        ///     registered subscriptions.
        /// </summary>
        public virtual void Dispose()
        {
            _subscriptions?.Dispose();
        }
    }
}