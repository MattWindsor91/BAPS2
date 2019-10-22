# URY.BAPS.Client.ViewModels

This project contains various 'view models' for the BAPS client.  These are
part of the _MVVM_ (Model-View-ViewModel) architecture, and represent a
two-direction projection of a model (the state of the BAPS server) onto which a
view (the BAPS WPF client, etc) can bind controls.

These classes all depend on ReactiveUI, which is a layer on top of the Reactive
Extensions the rest of the BAPS client projects use for pushing messages around
the client.  Not all client implementations'll need to use them.  While they're
mostly designed for WPF, which is heavily MVVM-centric, they don't target a
specific platform.