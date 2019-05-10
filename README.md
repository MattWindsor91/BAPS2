# Matt's BAPS fork

This repository contains a highly divergent and experimental fork of the
URY Broadcasting and Presenting Suite.  The aims of this fork are to:

- create a new, more modern UI for BAPS based on WPF;
- create a somewhat cleaner and more structured codebase for BAPS, with clear
  distinctions between protocol-specific and protocol-agnostic bits;
- start working towards a cross-platform BAPS server;
- eventually make it easy to transition BAPS to a newer protocol if needed.

This is a part-time project _not_ formally under the auspices of URY's
computing team, so don't expect amazing things.  Contributions and suggestions
are, of course, welcome.

## Build caveats

At time of writing, the BAPS fork uses .NET Core 3.0-preview and some
C# 8-preview features.  The practical upshot of this is that you'll need
Visual Studio 2019 (or something compatible, like Rider 2019.1), and the
pre-release version of the .NET Core SDK.

The Windows-specific parts (the original 'legacy' codebase, as well as the WPF
and Windows-interop projects) currently need .NET Framework 4.7.1, but will
migrate where possible to .NET Core in due time.

Eventually, when .NET Core 3.0 releases (September?), everything in this project
will run on release-quality .NET, and things'll be just fine.
