# Kerbalism Plugin Host

Shared deferred loader for Kerbalism add-on mods. Kerbalism release builds ship `KerbalismBootstrap.dll` plus versioned `*.kbin` files instead of `Kerbalism.dll`. Plugins that reference Kerbalism at compile time cannot sit in `Plugins/` during KSP startup — AssemblyLoader runs before Bootstrap loads the kbin.

This mod scans `GameData` for `*.host.xml` manifests, waits until Kerbalism (kbin or debug `Kerbalism.dll`) is loaded, then loads each registered plugin from that mod's `PluginData/` folder.

## Dependencies

* [Kerbalism](https://github.com/Kerbalism/Kerbalism) 3.32+ (Bootstrap / kbin workflow)

## Installation

Copy `GameData/zKerbalismPluginHost` into your KSP `GameData` folder. Build output:

* `GameData/zKerbalismPluginHost/Plugins/zKerbalismPluginHost.dll`

## Manifest format

Each hosted mod ships a manifest next to its `PluginData` folder, for example `GameData/zKerbalismNFE/zKerbalismNFE.host.xml`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<KerbalismHostedPlugin>
  <Plugin id="KerbalismNFE"
          assembly="zKerbalismNFE"
          dll="zKerbalismNFE.dll"
          initType="KerbalismNFE.KerbalismNFECoreInit" />
  <RequireKerbalism warnTitleLoc="#LOC_KerbalismNFE_DependencyNotFound_Title"
                    warnMessageLoc="#LOC_KerbalismNFE_DependencyNotFound_Msg"
                    quitButtonLoc="#LOC_KerbalismNFE_Quit" />
  <RequireAssembly name="NearFutureElectrical" />
</KerbalismHostedPlugin>
```

| Attribute / element | Purpose |
|---------------------|---------|
| `Plugin/@assembly` | Loaded assembly name (must match DLL) |
| `Plugin/@dll` | File name under `PluginData/` |
| `Plugin/@initType` | Optional static init class (calls `initMethod`, default `Initialize`) |
| `RequireKerbalism` | Wait for Kerbalism; show localized warning at MainMenu if missing |
| `RequireAssembly` | Additional assembly that must be loaded before the plugin |

## Hosted mods

* [KerbalismNFE](https://github.com/Aebestach/KerbalismNFE)
* [KerbalismSystemHeat](https://github.com/Aebestach/KerbalismSystemHeat)
* [KerbalismDynamicRadiation](https://github.com/Aebestach/KerbalismDynamicRadiation)

## Building

Open `src/zKerbalismPluginHost.sln` in Visual Studio. Requires KSP DLL references at `../../../KSPDLL/` (same layout as sibling repos).
