# Kerbalism Plugin Host

Shared deferred loader for Kerbalism add-on mods. Kerbalism release builds ship `KerbalismBootstrap.dll` plus versioned `*.kbin` files instead of `Kerbalism.dll`. Plugins that reference Kerbalism at compile time cannot sit in `Plugins/` during KSP startup — AssemblyLoader runs before Bootstrap loads the kbin.

This mod scans `GameData` for `*.host.xml` manifests, waits until Kerbalism (kbin or debug `Kerbalism.dll`) is loaded, then loads each registered plugin from that mod's `PluginData/` folder.

## Dependencies

* [Kerbalism](https://github.com/Kerbalism/Kerbalism) 3.32+ (Bootstrap / kbin workflow)

## Installation

Copy `GameData/zKerbalismPluginHost` into your KSP `GameData` folder. Build output:

* `GameData/zKerbalismPluginHost/Plugins/zKerbalismPluginHost.dll`

## Manifest format

Each hosted mod ships a manifest next to its `PluginData` folder, for example `GameData/zKerbalismNative/zKerbalismNative.host.xml`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<KerbalismHostedPlugin>
  <Plugin id="KerbalismNative"
          assembly="zKerbalismNative"
          dll="zKerbalismNative.dll"
          initType="KerbalismNative.KerbalismNativeCoreInit" />
  <RequireKerbalism warnTitleLoc="#LOC_KerbalismBridge_KerbalismNotFound_Title"
                    warnMessageLoc="#LOC_KerbalismBridge_KerbalismNotFound_Msg"
                    quitButtonLoc="#LOC_KerbalismBridge_Quit" />
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

Maintained in [KerbalismBridge](https://github.com/Aebestach/KerbalismBridge) (one repo; separate `GameData` installs):

* **zKerbalismBridge** — shared runtime (background thermal sim, editor sim)
* **zKerbalismProcess** — Process layer (converters, harvesters, radiators)
* **zKerbalismNative** — Native layer (fission, NFE capacitors, SpaceDust, …)
* **zKerbalismFFT** — Far Future Technologies integration
* **zKerbalismDynamicRadiation** — optional dynamic radiation (requires Native and/or FFT patches)

Install **zKerbalismBridge** plus **zKerbalismProcess** and/or **zKerbalismNative** for SystemHeat integration. Legacy folders `zKerbalismSystemHeat` and `zKerbalismNFE` are not part of Kerbalism Bridge.

## Building

Open `src/zKerbalismPluginHost.sln` in Visual Studio. Requires KSP DLL references at `../../../KSPDLL/` (same layout as sibling repos).
