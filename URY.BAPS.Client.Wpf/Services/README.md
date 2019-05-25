# Services

This directory, and its associated namespace, contains various classes that provide some sort of service to the rest
of the WPF BAPS Presenter (but aren't view models or converters).  We generally use one instance of each service,
and instantiate it in the dependency injection system.