using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Delegates;
using StardewValley.GameData.Buildings;
using StardewValley.Internal;
using StardewValley.Objects;

namespace ait.ChanceBasedArtisanGoodQuality {
	
	public static class MillPatches {
		//
		// static data
		//
		
		private const string MILL_BUILDING_TYPE = "Mill";
		
		private static IMonitor Monitor;
		private static Dictionary<string, RecipeOutput[]> MillRecipes;
		private static List<ItemQualities> InputItems;
		
		//
		// constructor
		//
		
		static MillPatches() {
			InputItems = new List<ItemQualities>(3);
			MillRecipes = new Dictionary<string, RecipeOutput[]>();
		}
		
		//
		// static methods
		//
		
		public static void AddMillRecipe(string inputID, params RecipeOutput[] outputs) {
			if(outputs.Length == 0) {
				Monitor.Log(string.Format("Attempted to add a mill recipe with input item '{0}' but 0 output items!", inputID), LogLevel.Error);
				return;
			}
			if(MillRecipes.ContainsKey(inputID)) {
				Monitor.Log(string.Format("Attempted to add a mill recipe with input item '{0}', but there is already a recipe for that input!", inputID), LogLevel.Error);
				return;
			}
			
			for(int index = 0; index < outputs.Length; index++)
				if(!StardewValley.Game1.objectData.ContainsKey(outputs[index].OutputID)) {
					List<RecipeOutput> tempList = new List<RecipeOutput>(outputs);
					tempList.RemoveAt(index--);
					outputs = tempList.ToArray();
				}
			MillRecipes.Add(inputID, outputs);
		}
		
		internal static void Init(IMonitor monitor) {
			Monitor = monitor;
			AddMillRecipe("262", new RecipeOutput("246")); // wheat > wheat flour
			AddMillRecipe("284", new RecipeOutput("245", 3), new RecipeOutput("Cornucopia_Molasses")); // beet > sugar, molasses
			AddMillRecipe("271", new RecipeOutput("423")); // unmilled rice > rice
			AddMillRecipe("Cornucopia_Buckwheat", new RecipeOutput("Cornucopia_BuckwheatFlour"));
			AddMillRecipe("Cornucopia_Durum", new RecipeOutput("Cornucopia_SemolinaFlour"));
			AddMillRecipe("Cornucopia_Sugarcane", new RecipeOutput("245", 2), new RecipeOutput("Cornucopia_Molasses"));
			AddMillRecipe("Cornucopia_SugarBeet", new RecipeOutput("245", 4), new RecipeOutput("Cornucopia_Molasses"));
			AddMillRecipe("Cornucopia_Barley", new RecipeOutput("Cornucopia_WholeGrainFlour"));
		}
		
		internal static void CheckItemConversionRulePrefix(BuildingItemConversion conversion, ItemQueryContext itemQueryContext, Building __instance) {
			if(!ModEntry.Config.ApplyToCookingIngredients)
				return;
			if(!__instance.buildingType.Contains(MILL_BUILDING_TYPE))
				return;
			
			InputItems.Clear();
			foreach(Item i in __instance.buildingChests[0].Items) {
				if(i != null && MillRecipes.ContainsKey(i.ItemId)) {
					foreach(ItemQualities iq in InputItems)
						if(__instance.id == iq.Mill.id && iq.ItemID == i.ItemId) {
							iq.Counts[i.Quality] += i.Stack;
							goto nextItem;
						}
					InputItems.Add(new ItemQualities(__instance, i.ItemId));
					InputItems[^1].Counts[i.Quality] += i.Stack;
				}
				nextItem:
				;
			}
		}
		
		internal static void CheckItemConversionRulePostfix(BuildingItemConversion conversion, ItemQueryContext itemQueryContext, Building __instance) {
			if(!ModEntry.Config.ApplyToCookingIngredients)
				return;
			if(!__instance.buildingType.Contains(MILL_BUILDING_TYPE))
				return;
			
			foreach(ItemQualities iq in InputItems) {
				// remove vanilla, quality-less output:
				for(int outputIndex = 0; outputIndex < MillRecipes[iq.ItemID].Length; outputIndex++) {
					int outputToRemove = 0;
					foreach(int i in iq.Counts)
						outputToRemove += i;
					outputToRemove *= MillRecipes[iq.ItemID][outputIndex].OutputCount;
					foreach(Item i in iq.Mill.buildingChests[1].Items)
						if(i == null)
							continue;
						else if(i.ItemId == MillRecipes[iq.ItemID][outputIndex].OutputID && i.Quality == StardewValley.Object.lowQuality) {
							if(i.Stack >= outputToRemove) {
								i.Stack -= outputToRemove;
								break;
							} else {
								outputToRemove -= i.Stack;
								i.ConsumeStack(i.Stack);
							}
						}
				}
				
				// roll qualities of all output items:
				int[,] outputQualities = new int[MillRecipes[iq.ItemID].Length,StardewValley.Object.bestQuality + 1];
				for(int qualityIndex = 0; qualityIndex < iq.Counts.Length; qualityIndex++)
					for(int outputIndex = 0; outputIndex < MillRecipes[iq.ItemID].Length; outputIndex++)
						while(iq.Counts[qualityIndex]-- > 0)
							for(int rollNum = 0; rollNum < MillRecipes[iq.ItemID][outputIndex].OutputCount; rollNum++)
								outputQualities[outputIndex,ObjectPatches.RollBaseQuality(qualityIndex)]++;
				
				// add output items to chest:
				for(int outputIndex = 0; outputIndex < MillRecipes[iq.ItemID].Length; outputIndex++)
					for(int qualityIndex = 0; qualityIndex < outputQualities.Length; qualityIndex++)
						if(outputQualities[outputIndex,qualityIndex] > 0)
							iq.Mill.buildingChests[1].addItem(ItemRegistry.Create(MillRecipes[iq.ItemID][outputIndex].OutputID,
									outputQualities[outputIndex,qualityIndex], qualityIndex));
			}
		}
		
		//
		// inner type
		//
		
		private class ItemQualities {
			// instance data
			
			public readonly Building Mill;
			public readonly string ItemID;
			public readonly int[] Counts;
			
			// constructor
			
			public ItemQualities(Building mill, string itemID) {
				Mill = mill;
				ItemID = itemID;
				Counts = new int[StardewValley.Object.bestQuality + 1];
			}
		}
		
		public class RecipeOutput {
			// instance data
			
			public readonly string OutputID;
			public readonly int OutputCount;
			
			// constructor
			
			public RecipeOutput(string outputID, int outputCount=1) {
				OutputID = outputID;
				OutputCount = outputCount;
			}
		}
	}
	
}