# `URY.BAPS.Common.Protocol.V2`

This project describes the BAPS protocol used mainly by BAPS2, and
often just called 'BapsNet'.  It's the stable, well-used, binary
protocol that's served URY for over a decade.

This project _doesn't_ contain the specific code needed by clients
and servers to make sense of the protocol; see
`URY.BAPS.Client.Protocol.V2` and `URY.BAPS.Server.Protocol.V2`.

## Characteristics of this protocol

- Binary, command/length/arguments layout protocol, based on a handful
  of primitive types
- Not self-describing; to understand a BAPS message, one must look up the
  initial command word (meaning that we need a large amount of decoder logic,
  but not much encoder logic).
- Specific to BAPS
