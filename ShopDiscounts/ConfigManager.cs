using BepInEx.Configuration;

namespace ShopDiscounts
{
	public static class ConfigManager
	{
		public static ConfigEntry<bool> UseCustomDiscount;
		public static ConfigEntry<int>  CustomDiscountValue;

		// Very creative names for the discounts, I know :)
		public static ConfigEntry<int> ChanceNoDiscount;
		public static ConfigEntry<int> ChanceLowDiscount;
		public static ConfigEntry<int> ChanceMediumDiscount;
		public static ConfigEntry<int> ChanceHighDiscount;
		public static ConfigEntry<int> ChanceMaxDiscount;
		public static ConfigEntry<int> ChanceUltraDiscount;
		public static ConfigEntry<int> ChanceLegendaryDiscount;

		public static void Init(ConfigFile config)
		{
			UseCustomDiscount  = config.Bind("Custom Discount", "Enable Custom Discount", false, "Skip rolling and just use a fixed discount.");
			CustomDiscountValue = config.Bind("Custom Discount", "Discount Percentage", 0, new ConfigDescription("Fixed discount % to apply.", new AcceptableValueRange<int>(0, 100)));

			ChanceNoDiscount = config.Bind("Chances", "0% Discount Chance", 5, new ConfigDescription ("Weight for no discount.", new AcceptableValueRange<int>(0,100)));
			ChanceLowDiscount = config.Bind("Chances", "1-10% Discount Chance", 40,new ConfigDescription("Weight for small discount.", new AcceptableValueRange<int>(0,100)));
			ChanceMediumDiscount = config.Bind("Chances", "11-25% Discount Chance", 25, new ConfigDescription("Weight for medium discount.", new AcceptableValueRange<int>(0, 100)));
			ChanceHighDiscount = config.Bind("Chances", "26-45% Discount Chance", 15, new ConfigDescription("Weight for high discount.", new AcceptableValueRange<int>(0, 100)));
			ChanceMaxDiscount = config.Bind("Chances", "46-65% Discount Chance", 10, new ConfigDescription("Weight for max discount.", new AcceptableValueRange<int>(0, 100)));
			ChanceUltraDiscount = config.Bind("Chances", "66-85% Discount Chance", 4, new ConfigDescription("Weight for ultra discount.", new AcceptableValueRange<int>(0, 100)));
			ChanceLegendaryDiscount = config.Bind("Chances", "86-100% Discount Chance", 1, new ConfigDescription("Weight for legendary discount.", new AcceptableValueRange<int>(0, 100)));
		}
	}
}