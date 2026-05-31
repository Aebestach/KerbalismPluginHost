using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace KerbalismPluginHost
{
	internal static class HostedPluginLoader
	{
		internal static bool AreDependenciesMet(HostedPluginManifest manifest)
		{
			if (manifest.RequireKerbalism && !KerbalismPresence.IsPresent())
				return false;

			foreach (string requiredAssembly in manifest.RequiredAssemblies)
			{
				if (!IsAssemblyLoaded(requiredAssembly))
					return false;
			}

			return true;
		}

		internal static bool TryLoad(HostedPluginManifest manifest)
		{
			if (manifest.Loaded)
				return true;

			if (manifest.RequireKerbalism && !KerbalismPresence.IsPresent())
				return false;

			foreach (string requiredAssembly in manifest.RequiredAssemblies)
			{
				if (!IsAssemblyLoaded(requiredAssembly))
				{
					manifest.LoadError = "Required assembly not loaded: " + requiredAssembly;
					return false;
				}
			}

			if (IsAssemblyLoaded(manifest.AssemblyName))
			{
				manifest.Loaded = true;
				return true;
			}

			if (!File.Exists(manifest.DllPath))
			{
				manifest.LoadError = "Plugin DLL not found: " + manifest.DllPath;
				Debug.LogError("[KerbalismPluginHost] " + manifest.Id + ": " + manifest.LoadError);
				return false;
			}

			try
			{
				AssemblyLoader.LoadPlugin(new FileInfo(manifest.DllPath), manifest.DllPath, null);
				AssemblyLoader.LoadedAssembly loaded = AssemblyLoader.loadedAssemblies
					.FirstOrDefault(a => a.name == manifest.AssemblyName);
				if (loaded == null)
				{
					manifest.LoadError = "AssemblyLoader failed to register " + manifest.AssemblyName;
					Debug.LogError("[KerbalismPluginHost] " + manifest.Id + ": " + manifest.LoadError);
					return false;
				}

				loaded.Load();
				RegisterLoadedTypes(loaded);
				StartCoreAddons(loaded);
				InvokeInit(loaded.assembly, manifest);
				manifest.Loaded = true;
				Debug.Log("[KerbalismPluginHost] Loaded hosted plugin '" + manifest.Id + "' from " + manifest.DllPath);
				return true;
			}
			catch (Exception ex)
			{
				manifest.LoadError = ex.Message;
				Debug.LogError("[KerbalismPluginHost] " + manifest.Id + " load failed: " + ex);
				return false;
			}
		}

		private static bool IsAssemblyLoaded(string assemblyName)
		{
			foreach (AssemblyLoader.LoadedAssembly loadedAssembly in AssemblyLoader.loadedAssemblies)
			{
				if (new AssemblyName(loadedAssembly.assembly.FullName).Name == assemblyName)
					return true;
			}

			return false;
		}

		private static void RegisterLoadedTypes(AssemblyLoader.LoadedAssembly loaded)
		{
			foreach (Type type in loaded.assembly.GetTypes())
			{
				foreach (Type loadedType in AssemblyLoader.loadedTypes)
				{
					if (!loadedType.IsAssignableFrom(type))
						continue;

					loaded.types.Add(loadedType, type);
					PropertyInfo temp = typeof(AssemblyLoader.LoadedAssembly).GetProperty("typesDictionary");
					if (temp == null)
						continue;

					var dict = (Dictionary<Type, Dictionary<string, Type>>)temp.GetValue(loaded, null);
					if (!dict.ContainsKey(loadedType))
						dict[loadedType] = new Dictionary<string, Type>();
					dict[loadedType][type.Name] = type;
				}
			}
		}

		private static void StartCoreAddons(AssemblyLoader.LoadedAssembly loaded)
		{
			if (!AddonLoaderWrapper.IsValid)
				return;

			foreach (Type type in loaded.assembly.GetTypes())
			{
				if (!type.IsSubclassOf(typeof(MonoBehaviour)))
					continue;

				object[] attrs = type.GetCustomAttributes(typeof(KSPAddon), true);
				if (attrs == null || attrs.Length == 0)
					continue;

				foreach (KSPAddon addon in attrs)
				{
					if (addon.startup == KSPAddon.Startup.MainMenu)
						AddonLoaderWrapper.StartAddon(loaded, type, addon, KSPAddon.Startup.MainMenu);
				}
			}
		}

		private static void InvokeInit(Assembly assembly, HostedPluginManifest manifest)
		{
			if (string.IsNullOrEmpty(manifest.InitType))
				return;

			Type initType = assembly.GetType(manifest.InitType);
			if (initType == null)
			{
				Debug.LogError("[KerbalismPluginHost] " + manifest.Id + ": init type not found: " + manifest.InitType);
				return;
			}

			MethodInfo init = initType.GetMethod(manifest.InitMethod, BindingFlags.Static | BindingFlags.Public);
			if (init == null)
			{
				Debug.LogError("[KerbalismPluginHost] " + manifest.Id + ": init method not found: "
					+ manifest.InitType + "." + manifest.InitMethod);
				return;
			}

			init.Invoke(null, null);
		}
	}
}
