# ParkMinPackages.MediaPipePlugin

Unity-facing MediaPipe runtime APIs, task runners, source adapters, platform settings, and lifecycle management.

The package consumes platform binaries and model assets from `ParkMinPackages.MediaPipe.NativeRuntime`. Native build sources remain outside the installable `UPMPackage` directory in the `ParkMinPackages/MediaPipe-NativeRuntime` repository.

## Status

This repository currently contains the initial Unity package structure. Runtime APIs and runners will be implemented after the native ABI and artifact layout are established.

## Dependency

Install the native runtime package through ParkMinPackages Package Manager before using this package.

```text
https://github.com/ParkMinPackages/MediaPipe-NativeRuntime.git?path=/UPMPackage
```
