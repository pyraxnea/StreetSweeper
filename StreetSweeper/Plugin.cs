using System.Text.RegularExpressions;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using StreetSweeper.Windows;

namespace StreetSweeper;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class Plugin : IDalamudPlugin
{
    [PluginService]
    internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    
    [PluginService]
    internal static ICommandManager CommandManager { get; private set; } = null!;

    [PluginService]
    internal static IObjectTable ObjectTable { get; private set; } = null!;

    [PluginService]
    internal static IFramework Framework { get; private set; } = null!;

    [PluginService]
    internal static IClientState ClientState { get; private set; } = null!;

    [PluginService]
    internal static IPluginLog Log { get; private set; } = null!;

    private readonly Regex regex = new Regex("[^a-zA-Z0-9 -]");

    private const string CommandName = "/streetsweeper";
    private const string CommandHelpMessage = "Shows the StreetSweeper configuration.";

    private Configuration Configuration { get; init; }
    private WindowSystem WindowSystem { get; init; }
    private ConfigurationWindow ConfigurationWindow { get; init; }

    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        WindowSystem = new WindowSystem("StreetSweeper");

        ConfigurationWindow = new ConfigurationWindow(PluginInterface, Configuration);
        WindowSystem.AddWindow(ConfigurationWindow);
        PluginInterface.UiBuilder.OpenConfigUi += OnOpenConfigUi;

        PluginInterface.UiBuilder.Draw += OnDraw;

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = CommandHelpMessage,
        });

        Framework.Update += OnFrameworkUpdate;

        ClientState.TerritoryChanged += OnTerritoryChanged;
        
        Log.Information("Plugin initialized.");
    }

    private void OnCommand(string command, string args)
    {
        ConfigurationWindow.Toggle();
    }

    private void OnOpenConfigUi()
    {
        ConfigurationWindow.Toggle();
    }

    private void OnDraw()
    {
        WindowSystem.Draw();
    }

    private void OnFrameworkUpdate(IFramework framework)
    {
        foreach (var battleChara in ObjectTable.PlayerObjects)
        {
            if (battleChara.ObjectKind != ObjectKind.Player)
            {
                continue;
            }

            unsafe
            {
                var character = (Character*)battleChara.Address;

                var name = Dalamud.Utility.Util.GetUTF8String(character->Name.ToArray());

                var names = name.Split(" ");

                if (names.Length != 2)
                {
                    continue;
                }

                var firstName = names[0];
                firstName = regex.Replace(firstName, "");

                var lastName = names[1];
                lastName = regex.Replace(lastName, "");

                if (!firstName.StartsWith("Weeaboo") || !lastName.StartsWith("Police"))
                {
                    continue;
                }

                if (Configuration.HideWeeabooPolice)
                {
                    character->RenderFlags |= (int)RenderFlags.Invisible; // Hide
                }
                else
                {
                    character->RenderFlags &= ~(int)RenderFlags.Invisible; // Show
                }
            }
        }
    }

    private void OnTerritoryChanged(ushort territoryId)
    {
        Log.Information($"Territory {territoryId} has been changed.");
    }

    public void Dispose()
    {
        CommandManager.RemoveHandler(CommandName);

        WindowSystem.RemoveAllWindows();
        ConfigurationWindow.Dispose();

        Log.Information("Plugin disposed.");
    }
}
