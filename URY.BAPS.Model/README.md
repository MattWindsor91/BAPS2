# URY.BAPS.Model

This project contains the common BAPS model---the various pieces of data sent
between the BAPS client and the BAPS server.  This includes:

- track information;
- playback state;
- server configuration;
- the various high-level 'messages' sent between client and server.

This project doesn't specify the low-level protocol used by the client and
server, and tries to be as protocol-agnostic as possible.

