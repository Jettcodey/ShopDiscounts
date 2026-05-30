using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

namespace ShopDiscounts
{
	internal static class DiscountEvents
	{
		// Please dont conflict with other mods :D
		public const byte EV_DISCOUNT_ROLLED = 156;

		public static void RaiseDiscountRolled(int pct)
		{
			if (!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
			{
				Plugin.Logger?.LogInfo("Not in a room, So Maybe SP or Internet died lol");
				return;
			}

			var options = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
			PhotonNetwork.RaiseEvent(EV_DISCOUNT_ROLLED, new object[] { pct }, options, SendOptions.SendReliable);
			Plugin.Logger?.LogInfo($"sent {pct}% discount to clients");
		}
	}
}