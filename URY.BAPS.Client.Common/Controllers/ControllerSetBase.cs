using System;
using System.Collections.Concurrent;
using JetBrains.Annotations;

namespace URY.BAPS.Client.Common.Controllers
{
    /// <summary>
    ///     Abstract base class for objects that govern a collection of
    ///     controllers, each for an item identified by numeric ID.
    ///     <para>
    ///         The controller set constructs controllers on demand,
    ///         using a factory method provided by the implementation.
    ///     </para>
    /// </summary>
    /// <typeparam name="TController">
    ///     Type of controller being instantiated.
    /// </typeparam>
    public abstract class ControllerSetBase<TController>
    {
        [NotNull] private readonly ConcurrentDictionary<byte, TController> _controllers =
            new ConcurrentDictionary<byte, TController>();

        /// <summary>
        ///     The client core.
        /// </summary>
        [NotNull] protected readonly IClientCore Core;

        /// <summary>
        ///     Abstract base constructor for controller sets.
        /// </summary>
        /// <param name="core">
        ///     The client core.
        /// </param>
        protected ControllerSetBase([CanBeNull] IClientCore core)
        {
            Core = core ?? throw new ArgumentNullException(nameof(core));
        }

        /// <summary>
        ///     Gets a controller for an item with the given ID.
        /// </summary>
        /// <param name="id">The item ID.</param>
        /// <returns>
        ///     A <see cref="TController" /> for <paramref name="id" />.
        ///     Any controllers this set previously constructed for the given ID will be reused.
        /// </returns>
        [NotNull]
        public TController ControllerFor(byte id)
        {
            return _controllers.GetOrAdd(id, MakeController);
        }

        /// <summary>
        ///     Factory method for producing <see cref="TController" />s.
        /// </summary>
        /// <param name="id">
        ///     The ID of the item whose controller is being constructed.
        /// </param>
        /// <returns>
        ///     A <see cref="TController" /> for the item with ID
        ///     <see cref="id" />.
        /// </returns>
        [NotNull]
        protected abstract TController MakeController(byte id);
    }
}