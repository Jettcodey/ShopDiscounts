using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

namespace ShopDiscounts
{
	public static class DiscountManager
	{
		public static int CurrentDiscount { get; internal set; }
		public static bool IsDiscountSynced { get; private set; }

		static readonly Dictionary<ItemAttributes, int> _originalPrices = new();

		// Rolls a new discount based on config weights or sets a custom discount if enabled
		// Could probably be optimized? but aslong as it works and doesnt break its fineeee with me 
		public static void RollDiscount()
		{
			if (ConfigManager.UseCustomDiscount.Value)
			{
				CurrentDiscount = Mathf.Clamp(ConfigManager.CustomDiscountValue.Value, 0, 100);
				IsDiscountSynced = true;
				Plugin.Logger?.LogInfo($"Custom discount set to {CurrentDiscount}%");
				return;
			}

			int wNo = ConfigManager.ChanceNoDiscount.Value;
			int wLow = ConfigManager.ChanceLowDiscount.Value;
			int wMed = ConfigManager.ChanceMediumDiscount.Value;
			int wHigh = ConfigManager.ChanceHighDiscount.Value;
			int wMax = ConfigManager.ChanceMaxDiscount.Value;
			int wUltr = ConfigManager.ChanceUltraDiscount.Value;
			int wLeg = ConfigManager.ChanceLegendaryDiscount.Value;

			int total = wNo + wLow + wMed + wHigh + wMax + wUltr + wLeg;
			if (total <= 0)
			{
				CurrentDiscount = 0;
				IsDiscountSynced = true;
				Plugin.Logger?.LogWarning("All discount weights are 0, defaulting to no discount");
				return;
			}

			int roll = Random.Range(0, total);

			if (roll < wNo)
				CurrentDiscount = 0;
			else if (roll < wNo + wLow)
				CurrentDiscount = Random.Range(1, 11);
			else if (roll < wNo + wLow + wMed)
				CurrentDiscount = Random.Range(11, 26);
			else if (roll < wNo + wLow + wMed + wHigh)
				CurrentDiscount = Random.Range(26, 46);
			else if (roll < wNo + wLow + wMed + wHigh + wMax)
				CurrentDiscount = Random.Range(46, 66);
			else if (roll < wNo + wLow + wMed + wHigh + wMax + wUltr)
				CurrentDiscount = Random.Range(66, 86);
			else
				CurrentDiscount = Random.Range(86, 101);

			IsDiscountSynced = true;
			Plugin.Logger?.LogInfo($"rolled {CurrentDiscount}% shop discount");
		}

		public static void SyncToClients()
		{
			if (!SemiFunc.IsMasterClientOrSingleplayer()) return;

			if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
				DiscountEvents.RaiseDiscountRolled(CurrentDiscount);
			else
				ApplyDiscountToAllItems();
		}

		public static void ReceiveDiscount(int discount)
		{
			CurrentDiscount = discount;
			IsDiscountSynced = true;
			ApplyDiscountToAllItems();
		}

		public static void ResetDiscount()
		{
			CurrentDiscount = 0;
			IsDiscountSynced = false;
			_originalPrices.Clear();
		}

		public static void ApplyDiscountToItem(ItemAttributes item)
		{
			if (item == null || CurrentDiscount <= 0) return;
			if (_originalPrices.ContainsKey(item)) return;

			_originalPrices[item] = item.value;
			int discounted = Mathf.RoundToInt(item.value * (1f - CurrentDiscount / 100f));
			if (discounted < 1 && item.value > 0) discounted = 1;
			item.value = discounted;

			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				var view = item.GetComponent<PhotonView>();
				if (view != null && PhotonNetwork.IsConnected)
					view.RPC("GetValueRPC", RpcTarget.Others, discounted);
			}
		}

		public static void RestoreItemPrice(ItemAttributes item)
		{
			if (item == null) return;
			if (!_originalPrices.TryGetValue(item, out int orig)) return;

			item.value = orig;
			_originalPrices.Remove(item);

			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				var view = item.GetComponent<PhotonView>();
				if (view != null && PhotonNetwork.IsConnected)
					view.RPC("GetValueRPC", RpcTarget.Others, orig);
			}
		}

		public static void ApplyDiscountToAllItems()
		{
			if (CurrentDiscount <= 0) return;
			foreach (var item in ShopManager.instance.shoppingList)
				ApplyDiscountToItem(item);
		}

		public static void RestoreAllItemPrices()
		{
			foreach (var kvp in _originalPrices)
			{
				if (kvp.Key == null) continue;
				kvp.Key.value = kvp.Value;

				if (SemiFunc.IsMasterClientOrSingleplayer())
				{
					var view = kvp.Key.GetComponent<PhotonView>();
					if (view != null && PhotonNetwork.IsConnected)
						view.RPC("GetValueRPC", RpcTarget.Others, kvp.Value);
				}
			}
			_originalPrices.Clear();
		}
	}
}