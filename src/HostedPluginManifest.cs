using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace KerbalismPluginHost
{
	public sealed class HostedPluginManifest
	{
		public string ManifestPath;
		public string ModRoot;
		public string Id;
		public string AssemblyName;
		public string DllFileName;
		public string InitType;
		public string InitMethod = "Initialize";
		public bool RequireKerbalism;
		public string WarnTitleLoc;
		public string WarnMessageLoc;
		public string QuitButtonLoc;
		public List<string> RequiredAssemblies = new List<string>();
		public bool Loaded;
		public string LoadError;

		public string DllPath => Path.Combine(ModRoot, "PluginData", DllFileName);

		public static HostedPluginManifest Parse(string manifestPath)
		{
			var doc = new XmlDocument();
			using (XmlTextReader reader = new XmlTextReader(manifestPath))
			{
				reader.Namespaces = false;
				doc.Load(reader);
			}

			XmlNode pluginNode = doc.DocumentElement?.SelectSingleNode("Plugin");
			if (pluginNode?.Attributes == null)
				return null;

			var manifest = new HostedPluginManifest
			{
				ManifestPath = manifestPath,
				ModRoot = Path.GetDirectoryName(manifestPath),
				Id = GetAttribute(pluginNode, "id"),
				AssemblyName = GetAttribute(pluginNode, "assembly"),
				DllFileName = GetAttribute(pluginNode, "dll"),
				InitType = GetAttribute(pluginNode, "initType"),
				InitMethod = GetAttributeOrDefault(pluginNode, "initMethod", "Initialize")
			};

			if (string.IsNullOrEmpty(manifest.Id)
				|| string.IsNullOrEmpty(manifest.AssemblyName)
				|| string.IsNullOrEmpty(manifest.DllFileName))
				return null;

			XmlNode kerbalismNode = doc.DocumentElement.SelectSingleNode("RequireKerbalism");
			if (kerbalismNode?.Attributes != null)
			{
				manifest.RequireKerbalism = true;
				manifest.WarnTitleLoc = GetAttribute(kerbalismNode, "warnTitleLoc");
				manifest.WarnMessageLoc = GetAttribute(kerbalismNode, "warnMessageLoc");
				manifest.QuitButtonLoc = GetAttribute(kerbalismNode, "quitButtonLoc");
			}

			XmlNodeList assemblyNodes = doc.DocumentElement.SelectNodes("RequireAssembly");
			if (assemblyNodes != null)
			{
				foreach (XmlNode node in assemblyNodes)
				{
					string name = GetAttribute(node, "name");
					if (!string.IsNullOrEmpty(name))
						manifest.RequiredAssemblies.Add(name);
				}
			}

			return manifest;
		}

		private static string GetAttribute(XmlNode node, string name)
		{
			return node.Attributes?[name]?.Value;
		}

		private static string GetAttributeOrDefault(XmlNode node, string name, string defaultValue)
		{
			string value = GetAttribute(node, name);
			return string.IsNullOrEmpty(value) ? defaultValue : value;
		}
	}
}
