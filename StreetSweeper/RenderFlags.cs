using System;

namespace StreetSweeper;

[Flags]
public enum RenderFlags
{
    None = 0,
    Model = 1 << 1,
    Nameplate = 1 << 11,
    Invisible = Model | Nameplate,
}
