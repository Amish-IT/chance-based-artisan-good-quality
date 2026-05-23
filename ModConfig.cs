using StardewModdingAPI;

namespace ait.ChanceBasedArtisanGoodQuality {
	
	public class ModConfig {
		//
		// static data
		//
		
		private const string REPLACE_LOOM_BEHAVIOR_FIELD_ID = "rlb24389";
		private const int CHANCE_TO_RETAIN_SILVER_DEFAULT = 50;
		private const int CHANCE_TO_RETAIN_GOLD_DEFAULT = 25;
		private const int CHANCE_TO_RETAIN_IRIDIUM_DEFAULT = 12;
		private const bool CASCADING_DOWNGRADES_DEFAULT = false;
		private const string SPECIAL_QUALITY_OUTPUT_MODE_DEFAULT = "Duplicate";
		private const bool REPLACE_LOOM_BEHAVIOR_DEFAULT = false;
		
		private static readonly string[] SpecialQualityOutputModes;
		
		//
		// instance data
		//
		
		public int ChanceToRetainSilver { get; set; }
		public int ChanceToRetainGold { get; set; }
		public int ChanceToRetainIridium { get; set; }
		public bool CascadingDowngrades { get; set; }
		public string SpecialQualityOutputMode { get; set; }
		public bool ReplaceLoomBehavior { get; set; }
		
		//
		// constructors
		//
		
		static ModConfig() {
			SpecialQualityOutputModes = new string[] { "Duplicate", "Minimum", "Upgrade" };
		}
		
		public ModConfig() {
			ChanceToRetainSilver = CHANCE_TO_RETAIN_SILVER_DEFAULT;
			ChanceToRetainGold = CHANCE_TO_RETAIN_GOLD_DEFAULT;
			ChanceToRetainIridium = CHANCE_TO_RETAIN_IRIDIUM_DEFAULT;
			CascadingDowngrades = CASCADING_DOWNGRADES_DEFAULT;
			SpecialQualityOutputMode = SPECIAL_QUALITY_OUTPUT_MODE_DEFAULT;
		}
		
		//
		// instance methods
		//
		
		internal void Init() {
			if(ReplaceLoomBehavior)
				ObjectPatches.TryUnBlacklistMachine(ObjectPatches.LOOM_ID);
		}
		
		internal void BuildGenericConfigMenu(IManifest manifest, GenericModConfigMenu.IGenericModConfigMenuApi configMenu) {
			// ChanceToRetainIridium:
			configMenu.AddNumberOption(manifest,
					() => { return ChanceToRetainIridium; },
					(int value) => { ChanceToRetainIridium = value; },
					() => { return "Chance To Retain Iridium Quality"; },
					() => { return string.Format("The chance that an iridium input item will produce an iridium output item; otherwise, it will produce a gold item (default: {0}%)",
							CHANCE_TO_RETAIN_IRIDIUM_DEFAULT); },
					0, 100, 1);
			// ChanceToRetainGold:
			configMenu.AddNumberOption(manifest,
					() => { return ChanceToRetainGold; },
					(int value) => { ChanceToRetainGold = value; },
					() => { return "Chance To Retain Gold Quality"; },
					() => { return string.Format("The chance that a gold input item will produce a gold output item; otherwise, it will produce a silver item (default: {0}%)",
							CHANCE_TO_RETAIN_GOLD_DEFAULT); },
					0, 100, 1);
			// ChanceToRetainSilver:
			configMenu.AddNumberOption(manifest,
					() => { return ChanceToRetainSilver; },
					(int value) => { ChanceToRetainSilver = value; },
					() => { return "Chance To Retain Silver Quality"; },
					() => { return string.Format("The chance that a silver input item will produce a silver output item; otherwise, it will produce a normal item (default: {0}%)",
							CHANCE_TO_RETAIN_SILVER_DEFAULT); },
					0, 100, 1);
			// CascadingDowngrades:
			configMenu.AddBoolOption(manifest,
					() => { return CascadingDowngrades; },
					(bool value) => { CascadingDowngrades = value; },
					() => { return "Cascading Downgrades"; },
					() => { return string.Format("If true, when an artisan good fails to retain its quality and is downgraded, it may be downgraded again (according to the chances above) until it retains a quality level or becomes normal quality (default: {0})",
							CASCADING_DOWNGRADES_DEFAULT); });
			// SpecialQualityOutputMode:
			configMenu.AddTextOption(manifest,
					() => { return SpecialQualityOutputMode; },
					(string value) => { SpecialQualityOutputMode = value; },
					() => { return "Special Quality Output Mode"; },
					() => { return string.Format("The mode for handling special artisan goods that have quality (e.g.: large eggs in a mayonnaise machine).  Duplicate: create 1 additional output; Minimum: output quality will be at least the vanilla quality for that input; Upgrade: increase output's final quality by 1 level (default: {0})",
							SPECIAL_QUALITY_OUTPUT_MODE_DEFAULT); },
					SpecialQualityOutputModes);
			// ReplaceLoomBehavior
			configMenu.AddBoolOption(manifest,
					() => { return ReplaceLoomBehavior; },
					(bool value) => { ReplaceLoomBehavior = value; },
					() => { return "Replace Loom Behavior"; },
					() => { return string.Format("If true, replace the vanilla quality-based chance to double Loom products with this mod's chance to inherit quality (default: {0})",
							REPLACE_LOOM_BEHAVIOR_DEFAULT); }, REPLACE_LOOM_BEHAVIOR_FIELD_ID);
		}
		
		//
		// static methods
		//
		
		internal static void OnFieldChanged(string fieldID, object newValue) {
			if(fieldID == REPLACE_LOOM_BEHAVIOR_FIELD_ID)
				if((bool)newValue)
					ObjectPatches.TryUnBlacklistMachine(ObjectPatches.LOOM_ID);
				else
					ObjectPatches.TryBlacklistMachine(ObjectPatches.LOOM_ID);
		}
	}
}