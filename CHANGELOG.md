# Changelog

## [0.3.0] - 2026-09-17

### Changed
- Moved VideoSkeletonRunner serialization into the reusable VideoSkeletonRunner.Setting type.
- Constructed each VideoSkeletonRunner with independent runtime state while copying configuration from its Setting.
- Added inspector support for serialized VideoSkeletonRunner.Setting fields.

## [0.2.1] - 2026-09-12

### Fixed
- Released VideoSkeletonRunner subscriptions and owned frame textures without stopping the injected VideoPlayer during disposal.
- Destroyed the owned WebCamTexture during disposal without querying playback state or calling Stop; explicit Close behavior is unchanged.

## [0.2.0] - 2026-08-30

- Added native Pose Landmarker bindings for image and video inference.
- Added project settings, model importing, model loading, and build-time model inclusion.
- Added reusable Skeleton, SkeletonCollection, landmark, and pose result APIs.
- Added image, video, and web camera Skeleton runners and source utilities.
- Added Skeleton, video, and web camera UGUI views with rotation, mirroring, and aspect-ratio support.
- Added largest-person and center-nearest Skeleton selectors.
- Added Foundation, R3, UniTask, UGUI, and Unity Video dependency declarations.

## [0.1.0] - 2026-08-27

- Added the initial Unity package structure.
- Added Runtime and Editor assembly definitions.
- Added the ParkMinPackages native runtime dependency declaration.
- Configured the native runtime dependency to install from its `UPMPackage` subdirectory.
