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
				var temp = helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
				if(temp != null) { // if GMCM installed
					IGenericModConfigMenuApi configMenu = temp as IGenericModConfigMenuApi;
					configMenu.Register(ModManifest, () => { Config = new ModConfig(); }, () => { Helper.WriteConfig<ModConfig>(Config); });
					Config.BuildGenericConfigMenu(ModManifest, configMenu);
					configMenu.OnFieldChanged(ModManifest, ModConfig.OnFieldChanged);
				}
			};
			
			// hook-in harmony patch:
			ObjectPatches.Init(Monitor);
			new Harmony(ModManifest.UniqueID).Patch(AccessTools.Method(typeof(StardewValley.MachineDataUtility), nameof(StardewValley.MachineDataUtility.GetOutputItem)),
					null, new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.GetOutputItemPostfix)));
		}
	}
}