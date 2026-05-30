using HarmonyLib;

namespace ShopDiscounts.Patches
{
	[HarmonyPatch(typeof(ShopManager))]
	public class ShopManagerPatches
	{
		// get us the Discount!
		[HarmonyPostfix]
		[HarmonyPatch("ShopInitialize")]
		public static void GenerateDiscountOnLoad()
		{
			if (!SemiFunc.RunIsShop() || !SemiFunc.IsMasterClientOrSingleplayer()) return;
			DiscountManager.RollDiscount();
			DiscountManager.SyncToClients();
		}

		// literally just applies the discount to the item
		[HarmonyPrefix]
		[HarmonyPatch("ShoppingListItemAdd")]
		public static void OnItemAdded(ItemAttributes item)
		{
			if (!SemiFunc.RunIsShop() || !DiscountManager.IsDiscountSynced) return;
			DiscountManager.ApplyDiscountToItem(item);
		}

		// literally just restores the Item price to the original
		[HarmonyPrefix]
		[HarmonyPatch("ShoppingListItemRemove")]
		public static void OnItemRemoved(ItemAttributes item)
		{
			if (!SemiFunc.RunIsShop()) return;
			DiscountManager.RestoreItemPrice(item);
		}

		// runs before the game checks the price total so we can be sure the discount is applied correctly
		[HarmonyPrefix]
		[HarmonyPatch("ShopCheck")]
		public static void EnsureDiscountBeforeCheck()
		{
			if (!SemiFunc.RunIsShop()) return;
			if (!DiscountManager.IsDiscountSynced)
				DiscountManager.CurrentDiscount = 0;
			DiscountManager.ApplyDiscountToAllItems();
		}
	}
}