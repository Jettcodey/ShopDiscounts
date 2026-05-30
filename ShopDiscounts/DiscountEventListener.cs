using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace ShopDiscounts
{
	internal class DiscountEventListener : MonoBehaviourPunCallbacks, IOnEventCallback
	{
		static bool _created;

		public static void Ensure()
		{
			if (_created) return;
			_created = true;

			var listenObj = new GameObject("ShopDiscounts_EventListener");
			DontDestroyOnLoad(listenObj);
			listenObj.AddComponent<DiscountEventListener>();

			Plugin.Logger?.LogInfo("Discount event listener ready.");
		}

		public override void OnJoinedRoom() => DiscountManager.ResetDiscount();
		public override void OnLeftRoom()   => DiscountManager.ResetDiscount();

		public void OnEvent(EventData photonEvent)
		{
			if (photonEvent.Code != DiscountEvents.EV_DISCOUNT_ROLLED) return;

			if (photonEvent.CustomData is not object[] data || data.Length < 1) return;

			int discount = (int)data[0];
			DiscountManager.ReceiveDiscount(discount);
			Plugin.Logger?.LogInfo($"Got discount from host: {discount}%");
		}
	}
}