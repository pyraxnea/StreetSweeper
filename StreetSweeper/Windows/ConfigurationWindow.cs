using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;

namespace StreetSweeper.Windows;

public sealed class ConfigurationWindow : Window, IDisposable
{
    private IDalamudPluginInterface PluginInterface { get; init; }
    private Configuration Configuration { get; init; }

    public ConfigurationWindow(IDalamudPluginInterface pluginInterface, Configuration configuration)
        : base("Street Sweeper", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoResize)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue),
        };

        PluginInterface = pluginInterface;
        Configuration = configuration;
    }

    public override void Draw()
    {
        var hideWeeabooPolice = Configuration.HideWeeabooPolice;

        if (ImGui.Checkbox("Hide Weeaboo Police", ref hideWeeabooPolice))
        {
            Configuration.HideWeeabooPolice = hideWeeabooPolice;
            PluginInterface.SavePluginConfig(Configuration);
        }
    }

    public void Dispose()
    {
    }
}
