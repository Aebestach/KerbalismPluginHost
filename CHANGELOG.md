# Changelog

## 1.0.1

 + Manifest: `RequireAssembly` supports `optional="true"` for soft dependencies (parsed into `OptionalAssemblies`).
 + MainMenu warnings: show `LoadError` details when dependencies are met but plugin load still fails.
 + README: hosted mods list and manifest example updated for Kerbalism Bridge (`zKerbalismBridge`, `zKerbalismProcess`, `zKerbalismNative`, …).

## 1.0.0

 + Initial release: manifest-driven deferred loading for Kerbalism-dependent plugins.
 + Scans `GameData/**/*.host.xml`, loads `PluginData/*.dll` after Kerbalism Bootstrap kbin or debug `Kerbalism.dll` is present.
 + MainMenu dependency warnings via localized keys declared in each manifest.
