using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine.SceneManagement;

namespace ShopDiscounts
{
	[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
	[BepInDependency("Jettcodey.MoreShopItems", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("Jettcodey.JettsShopConfig", BepInDependency.DependencyFlags.SoftDependency)]
	public class Plugin : BaseUnityPlugin
	{
		public static Plugin Instance { get; private set; }
		internal static new ManualLogSource Logger { get; private set; }

		Harmony _harmony;

		void Awake()
		{
			Instance = this;
			Logger = base.Logger;

			ConfigManager.Init(Config);

			_harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
			_harmony.PatchAll();

			// gotta wait for everything to properly init before we can register our listener, otherwise we get stuck in a Loding Screen.
			SceneManager.sceneLoaded += OnFirstScene;

			Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} v{MyPluginInfo.PLUGIN_VERSION} loaded successfully.");
		}

		void OnFirstScene(Scene scene, LoadSceneMode mode)
		{
			DiscountEventListener.Ensure();
			SceneManager.sceneLoaded -= OnFirstScene;
		}
	}
}