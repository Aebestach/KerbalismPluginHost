using System.Collections;
using UnityEngine;

namespace KerbalismPluginHost
{
	/// <summary>
	/// Scans GameData for *.host.xml manifests and loads PluginData DLLs after Kerbalism is present.
	/// </summary>
	[KSPAddon(KSPAddon.Startup.Instantly, false)]
	public class KerbalismPluginHostBootstrap : MonoBehaviour
	{
		private const int MaxWaitFrames = 600;

		private void Start()
		{
			HostedPluginRegistry.DiscoverIfNeeded();
			TryLoadAll();
			StartCoroutine(LoadWhenReadyCoroutine());
		}

		private IEnumerator LoadWhenReadyCoroutine()
		{
			int frames = 0;
			while (!AllLoaded() && frames++ < MaxWaitFrames)
			{
				if (AnyWaitingForKerbalism())
					TryLoadAll();

				yield return null;
			}

			TryLoadAll();
		}

		private static void TryLoadAll()
		{
			foreach (HostedPluginManifest manifest in HostedPluginRegistry.All)
				HostedPluginLoader.TryLoad(manifest);
		}

		private static bool AllLoaded()
		{
			foreach (HostedPluginManifest manifest in HostedPluginRegistry.All)
			{
				if (!manifest.Loaded && HostedPluginLoader.AreDependenciesMet(manifest))
					return false;
			}

			return true;
		}

		private static bool AnyWaitingForKerbalism()
		{
			foreach (HostedPluginManifest manifest in HostedPluginRegistry.All)
			{
				if (!manifest.Loaded && manifest.RequireKerbalism && !KerbalismPresence.IsPresent())
					return true;
			}

			return false;
		}
	}
}
