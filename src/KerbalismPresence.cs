using System.Reflection;

namespace KerbalismPluginHost
{
	internal static class KerbalismPresence
	{
		internal static bool IsPresent()
		{
			foreach (AssemblyLoader.LoadedAssembly loadedAssembly in AssemblyLoader.loadedAssemblies)
			{
				string name = new AssemblyName(loadedAssembly.assembly.FullName).Name;
				if (name == "Kerbalism" || (name.StartsWith("Kerbalism1") && name != "KerbalismBootstrap"))
					return true;
			}

			return false;
		}
	}
}
