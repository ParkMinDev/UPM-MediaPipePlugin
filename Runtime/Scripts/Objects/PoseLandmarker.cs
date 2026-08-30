using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using ParkMinPackages.MediaPipePlugin.Enums;
using ParkMinPackages.MediaPipePlugin.Native;
using Unity.Collections;
using UnityEngine;

namespace ParkMinPackages.MediaPipePlugin.Objects
{
	public sealed class PoseLandmarker : IDisposable
	{
		// - Construct -
		public PoseLandmarker(PoseLandmarkerOptions options) {
			if (options == null)
				throw new ArgumentNullException(nameof(options));

			bool hasModelPath = !string.IsNullOrWhiteSpace(options.ModelPath);
			bool hasModelBuffer = options.ModelBuffer != null && options.ModelBuffer.Length > 0;

			if (hasModelPath == hasModelBuffer)
				throw new ArgumentException("Specify either a model path or a model buffer.", nameof(options));
			if (hasModelPath && !File.Exists(options.ModelPath))
				throw new FileNotFoundException("The pose landmarker model could not be found.", options.ModelPath);
			if (options.NumPoses < 1)
				throw new ArgumentOutOfRangeException(nameof(options.NumPoses));

			IntPtr modelPath = hasModelPath ? Marshal.StringToCoTaskMemUTF8(Path.GetFullPath(options.ModelPath)) : IntPtr.Zero;
			GCHandle modelBufferHandle = hasModelBuffer ? GCHandle.Alloc(options.ModelBuffer, GCHandleType.Pinned) : default;

			try {
				MpPoseLandmarkerOptions nativeOptions = new MpPoseLandmarkerOptions {
					BaseOptions = new MpBaseOptions {
						ModelAssetBuffer = hasModelBuffer ? modelBufferHandle.AddrOfPinnedObject() : IntPtr.Zero,
						ModelAssetBufferCount = hasModelBuffer ? checked((uint)options.ModelBuffer.Length) : 0,
						ModelAssetPath = modelPath,
						FileDescriptor = -1,
						Delegate = MpDelegate.Cpu,
						HostEnvironment = MpHostEnvironment.Unknown,
#if UNITY_ANDROID && !UNITY_EDITOR
						HostSystem = MpHostSystem.Android
#else
						HostSystem = MpHostSystem.Windows
#endif
					},
					RunningMode = options.RunningMode == PoseLandmarkerRunningMode.Image ? MpRunningMode.Image : MpRunningMode.Video,
					NumPoses = options.NumPoses,
					MinPoseDetectionConfidence = options.MinPoseDetectionConfidence,
					MinPosePresenceConfidence = options.MinPosePresenceConfidence,
					MinTrackingConfidence = options.MinTrackingConfidence,
					OutputSegmentationMasks = false
				};

				MpStatus status = PoseLandmarkerNativeMethods.MpPoseLandmarkerCreate(ref nativeOptions, out _landmarker, out IntPtr errorMessage);
				MediaPipeNativeLibrary.ThrowIfFailed(status, errorMessage);
				_runningMode = options.RunningMode;
			}
			finally {
				if (modelPath != IntPtr.Zero)
					Marshal.FreeCoTaskMem(modelPath);
				if (modelBufferHandle.IsAllocated)
					modelBufferHandle.Free();
			}
		}

		// - Public Methods -
		public PoseLandmarkerResult Detect(Texture2D texture) {
			if (_runningMode != PoseLandmarkerRunningMode.Image)
				throw new InvalidOperationException($"{nameof(Detect)} requires {nameof(PoseLandmarkerRunningMode.Image)} mode.");

			return Detect(texture, 0, false);
		}

		public bool Detect(Texture2D texture, Skeleton destination) {
			if (_runningMode != PoseLandmarkerRunningMode.Image)
				throw new InvalidOperationException($"{nameof(Detect)} requires {nameof(PoseLandmarkerRunningMode.Image)} mode.");

			return Detect(texture, 0, false, destination);
		}

		public int Detect(Texture2D texture, SkeletonCollection destination) {
			if (_runningMode != PoseLandmarkerRunningMode.Image)
				throw new InvalidOperationException($"{nameof(Detect)} requires {nameof(PoseLandmarkerRunningMode.Image)} mode.");

			return Detect(texture, 0, false, destination);
		}

		public PoseLandmarkerResult DetectForVideo(Texture2D texture, long timestampMilliseconds) {
			if (_runningMode != PoseLandmarkerRunningMode.Video)
				throw new InvalidOperationException($"{nameof(DetectForVideo)} requires {nameof(PoseLandmarkerRunningMode.Video)} mode.");
			if (timestampMilliseconds <= _lastTimestampMilliseconds)
				throw new ArgumentOutOfRangeException(nameof(timestampMilliseconds), "Video timestamps must be monotonically increasing.");

			PoseLandmarkerResult result = Detect(texture, timestampMilliseconds, true);
			_lastTimestampMilliseconds = timestampMilliseconds;
			return result;
		}

		public bool DetectForVideo(Texture2D texture, long timestampMilliseconds, Skeleton destination) {
			if (_runningMode != PoseLandmarkerRunningMode.Video)
				throw new InvalidOperationException($"{nameof(DetectForVideo)} requires {nameof(PoseLandmarkerRunningMode.Video)} mode.");
			if (timestampMilliseconds <= _lastTimestampMilliseconds)
				throw new ArgumentOutOfRangeException(nameof(timestampMilliseconds), "Video timestamps must be monotonically increasing.");

			bool isDetected = Detect(texture, timestampMilliseconds, true, destination);
			_lastTimestampMilliseconds = timestampMilliseconds;
			return isDetected;
		}

		public int DetectForVideo(Texture2D texture, long timestampMilliseconds, SkeletonCollection destination) {
			if (_runningMode != PoseLandmarkerRunningMode.Video)
				throw new InvalidOperationException($"{nameof(DetectForVideo)} requires {nameof(PoseLandmarkerRunningMode.Video)} mode.");
			if (timestampMilliseconds <= _lastTimestampMilliseconds)
				throw new ArgumentOutOfRangeException(nameof(timestampMilliseconds), "Video timestamps must be monotonically increasing.");

			int detectedCount = Detect(texture, timestampMilliseconds, true, destination);
			_lastTimestampMilliseconds = timestampMilliseconds;
			return detectedCount;
		}

		public bool DetectForVideo(Color32[] pixels, int width, int height, long timestampMilliseconds, Skeleton destination) {
			if (_runningMode != PoseLandmarkerRunningMode.Video)
				throw new InvalidOperationException($"{nameof(DetectForVideo)} requires {nameof(PoseLandmarkerRunningMode.Video)} mode.");
			if (timestampMilliseconds <= _lastTimestampMilliseconds)
				throw new ArgumentOutOfRangeException(nameof(timestampMilliseconds), "Video timestamps must be monotonically increasing.");

			bool isDetected = Detect(pixels, width, height, timestampMilliseconds, destination);
			_lastTimestampMilliseconds = timestampMilliseconds;
			return isDetected;
		}

		public int DetectForVideo(Color32[] pixels, int width, int height, long timestampMilliseconds, SkeletonCollection destination) {
			if (_runningMode != PoseLandmarkerRunningMode.Video)
				throw new InvalidOperationException($"{nameof(DetectForVideo)} requires {nameof(PoseLandmarkerRunningMode.Video)} mode.");
			if (timestampMilliseconds <= _lastTimestampMilliseconds)
				throw new ArgumentOutOfRangeException(nameof(timestampMilliseconds), "Video timestamps must be monotonically increasing.");

			int detectedCount = Detect(pixels, width, height, timestampMilliseconds, destination);
			_lastTimestampMilliseconds = timestampMilliseconds;
			return detectedCount;
		}

		public void Dispose() {
			if (_isDisposed)
				return;

			_isDisposed = true;
			IntPtr landmarker = _landmarker;
			_landmarker = IntPtr.Zero;

			if (landmarker == IntPtr.Zero)
				return;

			MpStatus status = PoseLandmarkerNativeMethods.MpPoseLandmarkerClose(landmarker, out IntPtr errorMessage);
			MediaPipeNativeLibrary.ThrowIfFailed(status, errorMessage);
		}

		// - Internals -
		readonly PoseLandmarkerRunningMode _runningMode;
		IntPtr _landmarker;
		byte[] _pixelBuffer = Array.Empty<byte>();
		long _lastTimestampMilliseconds = -1;
		bool _isDisposed;

		PoseLandmarkerResult Detect(Texture2D texture, long timestampMilliseconds, bool isVideo) {
			IntPtr image = CreateImage(texture);

			try {
				MpStatus detectStatus = isVideo
					? PoseLandmarkerNativeMethods.MpPoseLandmarkerDetectForVideo(_landmarker, image, IntPtr.Zero, timestampMilliseconds, out MpPoseLandmarkerResult nativeResult, out IntPtr detectErrorMessage)
					: PoseLandmarkerNativeMethods.MpPoseLandmarkerDetectImage(_landmarker, image, IntPtr.Zero, out nativeResult, out detectErrorMessage);
				MediaPipeNativeLibrary.ThrowIfFailed(detectStatus, detectErrorMessage);

				try {
					return PoseLandmarkerResult.Create(nativeResult);
				}
				finally {
					PoseLandmarkerNativeMethods.MpPoseLandmarkerCloseResult(ref nativeResult);
				}
			}
			finally {
				PoseLandmarkerNativeMethods.MpImageFree(image);
			}
		}

		bool Detect(Texture2D texture, long timestampMilliseconds, bool isVideo, Skeleton destination) {
			if (destination == null)
				throw new ArgumentNullException(nameof(destination));

			IntPtr image = CreateImage(texture);

			try {
				MpStatus detectStatus = isVideo
					? PoseLandmarkerNativeMethods.MpPoseLandmarkerDetectForVideo(_landmarker, image, IntPtr.Zero, timestampMilliseconds, out MpPoseLandmarkerResult nativeResult, out IntPtr detectErrorMessage)
					: PoseLandmarkerNativeMethods.MpPoseLandmarkerDetectImage(_landmarker, image, IntPtr.Zero, out nativeResult, out detectErrorMessage);
				MediaPipeNativeLibrary.ThrowIfFailed(detectStatus, detectErrorMessage);

				try {
					return PoseLandmarkerResult.CopyNormalizedPoseToSkeleton(nativeResult, 0, isVideo ? timestampMilliseconds : -1, destination);
				}
				finally {
					PoseLandmarkerNativeMethods.MpPoseLandmarkerCloseResult(ref nativeResult);
				}
			}
			finally {
				PoseLandmarkerNativeMethods.MpImageFree(image);
			}
		}

		int Detect(Texture2D texture, long timestampMilliseconds, bool isVideo, SkeletonCollection destination) {
			if (destination == null)
				throw new ArgumentNullException(nameof(destination));

			IntPtr image = CreateImage(texture);

			try {
				MpStatus detectStatus = isVideo
					? PoseLandmarkerNativeMethods.MpPoseLandmarkerDetectForVideo(_landmarker, image, IntPtr.Zero, timestampMilliseconds, out MpPoseLandmarkerResult nativeResult, out IntPtr detectErrorMessage)
					: PoseLandmarkerNativeMethods.MpPoseLandmarkerDetectImage(_landmarker, image, IntPtr.Zero, out nativeResult, out detectErrorMessage);
				MediaPipeNativeLibrary.ThrowIfFailed(detectStatus, detectErrorMessage);

				try {
					return PoseLandmarkerResult.CopyNormalizedPosesToSkeletons(nativeResult, isVideo ? timestampMilliseconds : -1, destination);
				}
				finally {
					PoseLandmarkerNativeMethods.MpPoseLandmarkerCloseResult(ref nativeResult);
				}
			}
			finally {
				PoseLandmarkerNativeMethods.MpImageFree(image);
			}
		}

		bool Detect(IReadOnlyList<Color32> pixels, int width, int height, long timestampMilliseconds, Skeleton destination) {
			if (destination == null)
				throw new ArgumentNullException(nameof(destination));

			IntPtr image = CreateImage(pixels, width, height);

			try {
				MpStatus detectStatus = PoseLandmarkerNativeMethods.MpPoseLandmarkerDetectForVideo(_landmarker, image, IntPtr.Zero, timestampMilliseconds, out MpPoseLandmarkerResult nativeResult, out IntPtr detectErrorMessage);
				MediaPipeNativeLibrary.ThrowIfFailed(detectStatus, detectErrorMessage);

				try {
					return PoseLandmarkerResult.CopyNormalizedPoseToSkeleton(nativeResult, 0, timestampMilliseconds, destination);
				}
				finally {
					PoseLandmarkerNativeMethods.MpPoseLandmarkerCloseResult(ref nativeResult);
				}
			}
			finally {
				PoseLandmarkerNativeMethods.MpImageFree(image);
			}
		}

		int Detect(IReadOnlyList<Color32> pixels, int width, int height, long timestampMilliseconds, SkeletonCollection destination) {
			if (destination == null)
				throw new ArgumentNullException(nameof(destination));

			IntPtr image = CreateImage(pixels, width, height);

			try {
				MpStatus detectStatus = PoseLandmarkerNativeMethods.MpPoseLandmarkerDetectForVideo(_landmarker, image, IntPtr.Zero, timestampMilliseconds, out MpPoseLandmarkerResult nativeResult, out IntPtr detectErrorMessage);
				MediaPipeNativeLibrary.ThrowIfFailed(detectStatus, detectErrorMessage);

				try {
					return PoseLandmarkerResult.CopyNormalizedPosesToSkeletons(nativeResult, timestampMilliseconds, destination);
				}
				finally {
					PoseLandmarkerNativeMethods.MpPoseLandmarkerCloseResult(ref nativeResult);
				}
			}
			finally {
				PoseLandmarkerNativeMethods.MpImageFree(image);
			}
		}

		IntPtr CreateImage(Texture2D texture) {
			if (_isDisposed)
				throw new ObjectDisposedException(nameof(PoseLandmarker));
			if (texture == null)
				throw new ArgumentNullException(nameof(texture));
			if (!texture.isReadable)
				throw new InvalidOperationException($"{texture.name} must have Read/Write enabled.");

			int pixelCount = texture.width * texture.height;

			if (_pixelBuffer.Length != pixelCount * 4)
				_pixelBuffer = new byte[pixelCount * 4];

			if (texture.format == TextureFormat.RGBA32) {
				NativeArray<Color32> colors = texture.GetRawTextureData<Color32>();
				CopyPixels(colors, texture.width, texture.height);
			}
			else {
				Color32[] colors = texture.GetPixels32();
				CopyPixels(colors, texture.width, texture.height);
			}

			MpStatus imageStatus = PoseLandmarkerNativeMethods.MpImageCreateFromUint8Data(MpImageFormat.Srgba, texture.width, texture.height, _pixelBuffer, _pixelBuffer.Length, out IntPtr image, out IntPtr imageErrorMessage);
			MediaPipeNativeLibrary.ThrowIfFailed(imageStatus, imageErrorMessage);
			return image;
		}

		IntPtr CreateImage(IReadOnlyList<Color32> pixels, int width, int height) {
			if (_isDisposed)
				throw new ObjectDisposedException(nameof(PoseLandmarker));
			if (pixels == null)
				throw new ArgumentNullException(nameof(pixels));
			if (width < 1)
				throw new ArgumentOutOfRangeException(nameof(width));
			if (height < 1)
				throw new ArgumentOutOfRangeException(nameof(height));

			int pixelCount = checked(width * height);

			if (pixels.Count < pixelCount)
				throw new ArgumentException("The pixel buffer is smaller than the requested image size.", nameof(pixels));
			if (_pixelBuffer.Length != checked(pixelCount * 4))
				_pixelBuffer = new byte[pixelCount * 4];

			CopyPixels(pixels, width, height);
			MpStatus imageStatus = PoseLandmarkerNativeMethods.MpImageCreateFromUint8Data(MpImageFormat.Srgba, width, height, _pixelBuffer, _pixelBuffer.Length, out IntPtr image, out IntPtr imageErrorMessage);
			MediaPipeNativeLibrary.ThrowIfFailed(imageStatus, imageErrorMessage);
			return image;
		}

		void CopyPixels(IReadOnlyList<Color32> colors, int width, int height) {
			for (int y = 0; y < height; y++) {
				int sourceRow = (height - 1 - y) * width;
				int destinationRow = y * width * 4;

				for (int x = 0; x < width; x++) {
					Color32 color = colors[sourceRow + x];
					int destinationIndex = destinationRow + x * 4;
					_pixelBuffer[destinationIndex] = color.r;
					_pixelBuffer[destinationIndex + 1] = color.g;
					_pixelBuffer[destinationIndex + 2] = color.b;
					_pixelBuffer[destinationIndex + 3] = color.a;
				}
			}
		}

		void CopyPixels(NativeArray<Color32> colors, int width, int height) {
			for (int y = 0; y < height; y++) {
				int sourceRow = (height - 1 - y) * width;
				int destinationRow = y * width * 4;

				for (int x = 0; x < width; x++) {
					Color32 color = colors[sourceRow + x];
					int destinationIndex = destinationRow + x * 4;
					_pixelBuffer[destinationIndex] = color.r;
					_pixelBuffer[destinationIndex + 1] = color.g;
					_pixelBuffer[destinationIndex + 2] = color.b;
					_pixelBuffer[destinationIndex + 3] = color.a;
				}
			}
		}

	}
}
