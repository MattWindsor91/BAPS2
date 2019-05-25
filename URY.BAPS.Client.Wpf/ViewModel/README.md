# View models

This directory, and its associated namespace, contain the _view models_ used in the WPF BAPS Presenter.
These classes generally receive updates from the BAPS server (through various `ServerUpdater`s), structure them in a
form that is amenable to display in the Presenter's various controls and windows, and provide commands that the UI can
use to talk to the BAPS server (through various `Controller`s).

Some view models have two implementations: the 'actual' model (kept in here, usually given a plain-sounding name ending
in `ViewModel`, and used in the presenter itself), and the 'mock' model (kept in `DesignData`, usually given a name
starting with `Mock`, and used when editing controls and windows in Visual Studio or Blend).
