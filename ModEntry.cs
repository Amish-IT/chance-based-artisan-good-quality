using System.Reflection;
using GenericModConfigMenu;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ait.ChanceBasedArtisanGoodQuality {
	
	public class ModEntry : Mod {
		//
		// static data
		//
		
		public const string MOD_NAME = "ChanceBasedArtisanGoodQuality";
		
		public static ModConfig Config { get; private set; }
		
		//
		// instance methods
		//
		
		public override void Entry(IModHelper helper) {
			Config = Helper.ReadConfig<ModConfig>();
			Config.Init();
			
			helper.Events.GameLoop.GameLaunched += (object? sender, GameLaunchedEventArgs args) => {
				IGenericModConfigMenuApi? configMenu = helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
				if(configMenu != null) { // if GMCM installed
					configMenu.Register(ModManifest, () => { Config = new ModConfig(); }, () => { Helper.WriteConfig<ModConfig>(Config); });
					Config.BuildGenericConfigMenu(ModManifest, configMenu);
					configMenu.OnFieldChanged(ModManifest, ModConfig.OnFieldChanged);
				}
				
				MillPatches.Init(Monitor);
				ObjectPatches.Init(Monitor);
			};
			
			Harmony harmony = new Harmony(ModManifest.UniqueID);
			harmony.Patch(AccessTools.Method(typeof(StardewValley.MachineDataUtility), nameof(StardewValley.MachineDataUtility.GetOutputItem)),
					null, new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.GetOutputItemPostfix)));
			harmony.Patch(AccessTools.Method(typeof(StardewValley.Buildings.Building), nameof(StardewValley.Buildings.Building.CheckItemConversionRule)),
					new HarmonyMethod(typeof(MillPatches), nameof(MillPatches.CheckItemConversionRulePrefix)),
					new HarmonyMethod(typeof(MillPatches), nameof(MillPatches.CheckItemConversionRulePostfix)));
		}
	}
}