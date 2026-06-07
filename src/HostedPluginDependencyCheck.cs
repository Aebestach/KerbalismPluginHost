using KSP.Localization;
using UnityEngine;

namespace KerbalismPluginHost
{
	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	public class HostedPluginDependencyCheck : MonoBehaviour
	{
		public void Start()
		{
			foreach (HostedPluginManifest manifest in HostedPluginRegistry.All)
			{
				if (manifest.Loaded)
					continue;

				if (!HostedPluginLoader.AreDependenciesMet(manifest))
				{
					ShowWarning(manifest, null);
					continue;
				}

				if (!string.IsNullOrEmpty(manifest.LoadError))
					ShowWarning(manifest, manifest.LoadError);
			}
		}

		private static void ShowWarning(HostedPluginManifest manifest, string loadErrorDetail)
		{
			string title = LocalizeOrFallback(manifest.WarnTitleLoc, manifest.Id + ": dependency missing");
			string message = LocalizeOrFallback(manifest.WarnMessageLoc, manifest.Id + " could not load. Check mod dependencies and restart.");
			if (!string.IsNullOrEmpty(loadErrorDetail))
				message += "\n\n" + loadErrorDetail;
			string quit = LocalizeOrFallback(manifest.QuitButtonLoc, "Quit");

			PopupDialog.SpawnPopupDialog(
				new Vector2(0.5f, 0.5f),
				new Vector2(0.5f, 0.5f),
				new MultiOptionDialog(
					"KerbalismPluginHost_" + manifest.Id,
					message,
					title,
					HighLogic.UISkin,
					new Rect(0.5f, 0.5f, 520f, 60f),
					new DialogGUIFlexibleSpace(),
					new DialogGUIHorizontalLayout(
						new DialogGUIFlexibleSpace(),
						new DialogGUIButton(quit, Application.Quit, 140.0f, 30.0f, true),
						new DialogGUIFlexibleSpace()
					)
				),
				true,
				HighLogic.UISkin);
		}

		private static string LocalizeOrFallback(string locKey, string fallback)
		{
			if (string.IsNullOrEmpty(locKey))
				return fallback;

			string localized = Localizer.Format(locKey);
			return localized == locKey ? fallback : localized;
		}
	}
}
