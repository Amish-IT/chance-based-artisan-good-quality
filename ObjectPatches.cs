using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Machines;

namespace ait.ChanceBasedArtisanGoodQuality {
	
	public static class ObjectPatches {
		//
		// static data
		//
		
		public const string CASK_ID = "163";
		public const string FISH_SMOKER_ID = "FishSmoker";
		public const string LOOM_ID = "17";
		
		private static readonly List<string> BlacklistedMachineIDs;
		private static readonly List<string> ArtisanIngredientIDs;
		
		private static IMonitor Monitor;
		
		//
		// constructors
		//
		
		static ObjectPatches() {
			BlacklistedMachineIDs = new List<string>() { CASK_ID, FISH_SMOKER_ID, LOOM_ID };
			ArtisanIngredientIDs = new List<string> { "247", "419" };
		}
		
		//
		// static methods
		//
		
		public static bool TryBlacklistMachine(string machineID) {
			if(!BlacklistedMachineIDs.Contains(machineID)) {
				BlacklistedMachineIDs.Add(machineID);
				return true;
			}
			return false;
		}
		
		public static bool TryUnBlacklistMachine(string machineID) {
			if(BlacklistedMachineIDs.Contains(machineID)) {
				BlacklistedMachineIDs.Remove(machineID);
				return true;
			}
			return false;
		}
		
		public static bool TryWhitelistIngredient(string ingredientID) {
			if(ArtisanIngredientIDs.Contains(ingredientID))
				return false;
			ArtisanIngredientIDs.Add(ingredientID);
			return true;
		}
		
		internal static void Init(IMonitor monitor) {
			Monitor = monitor;
		}
		
		internal static void GetOutputItemPostfix(StardewValley.Object machine, MachineItemOutput outputData, Item inputItem, Farmer who, bool probe, ref Item __result) {
			try {
				if(__result == null)
					return;
				if(!(__result.Category == StardewValley.Object.artisanGoodsCategory ||
						ModEntry.Config.ApplyToCookingIngredients && ArtisanIngredientIDs.Contains(__result.ItemId)))
					return;
				if(inputItem == null)
					return;
				if(inputItem.Quality == 0)
					return;
				if(BlacklistedMachineIDs.Contains(machine.ItemId))
					return;
				
				if(machine.ItemId == LOOM_ID)
					__result.Stack = 1;
				
				if(__result.Quality == 0) {
					__result.Quality = RollBaseQuality(inputItem.Quality);
					return;
				}
				
				int minQuality = __result.Quality;
				__result.Quality = RollBaseQuality(inputItem.Quality);
				switch(ModEntry.Config.SpecialQualityOutputMode) {
					case "Duplicate":
						__result.Stack++;
						break;
					case "Minimum":
						if(__result.Quality < minQuality)
							__result.Quality = minQuality;
						break;
					case "Upgrade":
						if(__result.Quality < StardewValley.Object.highQuality)
							__result.Quality++;
						else if(__result.Quality == StardewValley.Object.highQuality)
							__result.Quality = StardewValley.Object.bestQuality;
						break;
					default:
						Monitor.Log(string.Format("Error in {0}, method ObjectPatches.GetOutputItemPostfix: unsupported special quality output mode '{1}'", ModEntry.MOD_NAME, ModEntry.Config.SpecialQualityOutputMode), LogLevel.Error);
						return;
				}
			} catch(Exception e) {
				Monitor.Log(string.Format("Error in {0} Harmony postfix for method StardewValley.MachineDataUtility.GetOutputItem: {1}", ModEntry.MOD_NAME, e.StackTrace), LogLevel.Error);
			}
		}
		
		internal static int RollBaseQuality(int inputQuality) {
			if(inputQuality == StardewValley.Object.bestQuality)
				if(Random.Shared.Next(0, 100) < ModEntry.Config.ChanceToRetainIridium)
					return StardewValley.Object.bestQuality;
				else {
					if(!ModEntry.Config.CascadingDowngrades)
						return StardewValley.Object.highQuality;
					inputQuality = StardewValley.Object.highQuality;
				}
			if(inputQuality == StardewValley.Object.highQuality)
				if(Random.Shared.Next(0, 100) < ModEntry.Config.ChanceToRetainGold)
					return StardewValley.Object.highQuality;
				else {
					if(!ModEntry.Config.CascadingDowngrades)
						return StardewValley.Object.medQuality;
					inputQuality = StardewValley.Object.medQuality;
				}
			if(inputQuality == StardewValley.Object.medQuality)
				if(Random.Shared.Next(0, 100) < ModEntry.Config.ChanceToRetainSilver)
					return StardewValley.Object.medQuality;
			return StardewValley.Object.lowQuality;
		}
	}
}