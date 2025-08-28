using System;
using Dalamud.Configuration;

namespace StreetSweeper;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool HideWeeabooPolice { get; set; } = true;
}
