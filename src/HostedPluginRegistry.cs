using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace KerbalismPluginHost
{
	internal static class HostedPluginRegistry
	{
		private static readonly List<HostedPluginManifest> Manifests = new List<HostedPluginManifest>();
		private static bool discovered;

		internal static IReadOnlyList<HostedPluginManifest> All => Manifests;

		internal static void DiscoverIfNeeded()
		{
			if (discovered)
				return;

			discovered = true;
			string gameData = Path.Combine(KSPUtil.ApplicationRootPath, "GameData");
			if (!Directory.Exists(gameData))
			{
				Debug.LogWarning("[KerbalismPluginHost] GameData folder not found: " + gameData);
				return;
			}

			foreach (string manifestPath in Directory.GetFiles(gameData, "*.host.xml", SearchOption.AllDirectories))
			{
				HostedPluginManifest manifest = HostedPluginManifest.Parse(manifestPath);
				if (manifest == null)
				{
					Debug.LogWarning("[KerbalismPluginHost] Invalid manifest: " + manifestPath);
					continue;
				}

				Manifests.Add(manifest);
				Debug.Log("[KerbalismPluginHost] Registered hosted plugin '" + manifest.Id + "' from " + manifestPath);
			}
		}
	}
}
