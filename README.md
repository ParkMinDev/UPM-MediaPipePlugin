# ParkMinDev.UPM.MediaPipePlugin

Unity-facing MediaPipe pose inference APIs, task runners, source adapters, platform settings, model management, and UGUI views.

The package consumes platform binaries and model assets from `ParkMinPackages.MediaPipe.NativeRuntime`. Native build sources remain outside the installable `UPM` directory in the `ParkMinDev/MediaPipeNativeRuntime` repository.

## Features

- Image, video, and web camera pose inference runners
- Reusable single-person and multi-person Skeleton destinations
- Skeleton selection by on-screen size or proximity to the screen center
- UGUI Skeleton, video, and web camera views
- Configurable skeleton angle overlays with reusable child AngleGraphics
- Project-wide MediaPipe model registration and build-time inclusion

## Dependency

Install the native runtime package through ParkMinPackages Package Manager before using this package.

```text
https://github.com/ParkMinDev/MediaPipeNativeRuntime.git?path=/UPM
```

## Skeleton angle overlays

Add `SkeletonAngleView` to a RectTransform matching the skeleton display area. Configure its `Angles` array in the Inspector: each `SkeletonAngleOptions` contains a first landmark, a vertex landmark, a last landmark, and `AngleGraphicOptions`. No scene references to individual graphics are needed.

Call `SetSkeleton(skeleton)` with the displayed Skeleton. The view creates and reuses one child AngleGraphic per configured item and subscribes to Skeleton updates. Use `SetDisplayTransform(rotationAngle, horizontallyFlipped, verticallyFlipped)` with the same values as the SkeletonView. Angles are measured in the displayed 2D coordinate system, using the smaller signed sweep between the two rays.

Missing or low-confidence landmarks and coincident points hide the affected graphic. `Clear()` releases the Skeleton subscription and hides all graphics. Disabling the view also releases its subscription; enabling it restores the current Skeleton binding and reuses its children. Removing the view removes the children it owns. Runtime configuration can be replaced with `SetAngles(SkeletonAngleOptions[])`; after editing existing option objects from code, call `RebuildVisuals()` to apply their appearance and layout.

AngleGraphic and AngleGraphicOptions are provided by `com.parkmindev.upm.ugui`, declared in `parkmin-upm.json`.

## Landmark anchors and angle callouts

`SkeletonLandmarkAnchor` places an arbitrary target RectTransform at a selected landmark plus an offset in the display coordinate space. Assign the display RectTransform to `Coordinate Space` and the element to move to `Target`. Bind the displayed Skeleton with `SetSkeleton` and use the same `SetDisplayTransform` values as the skeleton view. The offset applies after rotation and mirroring. Target anchors, pivots and parents may differ from the display's; the component converts positions through world space. It does not modify text or visibility. `IsTracked` reports whether the selected landmark meets the visibility and presence thresholds. Keep the component on an active owner when hiding the target, so tracking can recover. `Clear()` releases the subscription. `Refresh()` can be called after layout changes; LateUpdate also refreshes placement.

`SkeletonAngleView.Graphics` exposes the generated graphics in configuration order for composition with external presentation components. UGUI's `AngleCalloutLineGraphic` connects an AngleGraphic's arc midpoint to a target RectTransform boundary, accounting for their transforms and source scale. It supports a straight line or an elbow with a configurable end segment. Place its RectTransform over the complete connector drawing area, beneath the label. Missing or inactive references clear the line automatically. These components do not depend on a text system; a controller may update any label and hide it when the angle or anchor is unavailable.
