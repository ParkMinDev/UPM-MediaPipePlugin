# ParkMinPackages.MediaPipePlugin

Unity-facing MediaPipe pose inference APIs, task runners, source adapters, platform settings, model management, and UGUI views.

The package consumes platform binaries and model assets from `ParkMinPackages.MediaPipe.NativeRuntime`. Native build sources remain outside the installable `UPMPackage` directory in the `ParkMinDev/MediaPipe-NativeRuntime` repository.

## Features

- Image, video, and web camera pose inference runners
- Reusable single-person and multi-person Skeleton destinations
- Skeleton selection by on-screen size or proximity to the screen center
- UGUI Skeleton, video, and web camera views
- Project-wide MediaPipe model registration and build-time inclusion

## Dependency

Install the native runtime package through ParkMinPackages Package Manager before using this package.

```text
https://github.com/ParkMinDev/MediaPipe-NativeRuntime.git?path=/UPMPackage
```
