using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace KerbalismPluginHost
{
	internal static class AddonLoaderWrapper
	{
		private static readonly MethodInfo KSPStartAddon;
		internal static bool IsValid => KSPStartAddon != null;

		static AddonLoaderWrapper()
		{
			KSPStartAddon = typeof(AddonLoader).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(method =>
			{
				ParameterInfo[] parameters = method.GetParameters();
				if (parameters.Length != 4)
					return false;
				if (parameters[0].ParameterType != typeof(AssemblyLoader.LoadedAssembly))
					return false;
				if (parameters[1].ParameterType != typeof(Type))
					return false;
				if (parameters[2].ParameterType != typeof(KSPAddon))
					return false;
				return parameters[3].ParameterType == typeof(KSPAddon.Startup);
			});
		}

		internal static void StartAddon(AssemblyLoader.LoadedAssembly assembly, Type addonType, KSPAddon addon, KSPAddon.Startup startup)
		{
			if (!IsValid)
			{
				Debug.LogError("[KerbalismPluginHost] AddonLoader.StartAddon not found; hosted plugin addons may not start.");
				return;
			}

			KSPStartAddon.Invoke(AddonLoader.Instance, new object[] { assembly, addonType, addon, startup });
		}
	}
}
