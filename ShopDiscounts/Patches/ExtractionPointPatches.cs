using HarmonyLib;

namespace ShopDiscounts.Patches
{
	[HarmonyPatch(typeof(ExtractionPoint))]
	public class ExtractionPointPatches
	{
		[HarmonyPostfix]
		[HarmonyPatch("SetHaulText")]
		public static void AddDiscountTextToScreen(ExtractionPoint __instance)
		{
			if (!SemiFunc.RunIsShop()) return;

			int d = DiscountManager.CurrentDiscount;
			if (d <= 0) return;

			// dont double-append if already shown lmao
			if (!__instance.haulGoalScreen.text.Contains("(-"))
				__instance.haulGoalScreen.text += $" <color=green>(-{d}%)</color>";
		}

		[HarmonyPostfix]
		[HarmonyPatch("DestroyAllPhysObjectsInShoppingList")]
		public static void RevertItemPrices()
		{
			if (!SemiFunc.RunIsShop()) return;
			DiscountManager.RestoreAllItemPrices();
		}
	}
}