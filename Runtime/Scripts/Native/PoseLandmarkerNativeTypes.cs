using System;
using System.Runtime.InteropServices;

namespace ParkMinDev.UPM.MediaPipePlugin.Native
{
	// - Class Struct Enum -
	internal enum MpStatus
	{
		Ok = 0,
		Cancelled = 1,
		Unknown = 2,
		InvalidArgument = 3,
		DeadlineExceeded = 4,
		NotFound = 5,
		AlreadyExists = 6,
		PermissionDenied = 7,
		ResourceExhausted = 8,
		FailedPrecondition = 9,
		Aborted = 10,
		OutOfRange = 11,
		Unimplemented = 12,
		Internal = 13,
		Unavailable = 14,
		DataLoss = 15,
		Unauthenticated = 16
	}

	internal enum MpDelegate
	{
		Cpu = 0
	}

	internal enum MpHostEnvironment
	{
		Unknown = 0
	}

	internal enum MpHostSystem
	{
		Unknown = 0,
		Windows = 3,
		Android = 5
	}

	internal enum MpRunningMode
	{
		Image = 1,
		Video = 2,
		LiveStream = 3
	}

	internal enum MpImageFormat
	{
		Srgba = 2
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct MpBaseOptions
	{
		internal IntPtr ModelAssetBuffer;
		internal uint ModelAssetBufferCount;
		internal IntPtr ModelAssetPath;
		internal int FileDescriptor;
		internal MpDelegate Delegate;
		internal MpHostEnvironment HostEnvironment;
		internal MpHostSystem HostSystem;
		internal IntPtr HostVersion;
		internal IntPtr CaBundlePath;
		internal IntPtr AppId;
		internal IntPtr AppVersion;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct MpPoseLandmarkerOptions
	{
		internal MpBaseOptions BaseOptions;
		internal MpRunningMode RunningMode;
		internal int NumPoses;
		internal float MinPoseDetectionConfidence;
		internal float MinPosePresenceConfidence;
		internal float MinTrackingConfidence;
		[MarshalAs(UnmanagedType.I1)] internal bool OutputSegmentationMasks;
		internal IntPtr ResultCallback;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct MpPoseLandmarkerResult
	{
		internal IntPtr SegmentationMasks;
		internal uint SegmentationMasksCount;
		internal IntPtr PoseLandmarks;
		internal uint PoseLandmarksCount;
		internal IntPtr PoseWorldLandmarks;
		internal uint PoseWorldLandmarksCount;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct MpNormalizedLandmarks
	{
		internal IntPtr Landmarks;
		internal uint LandmarksCount;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct MpLandmarks
	{
		internal IntPtr Landmarks;
		internal uint LandmarksCount;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct MpNormalizedLandmark
	{
		internal float X;
		internal float Y;
		internal float Z;
		[MarshalAs(UnmanagedType.I1)] internal bool HasVisibility;
		internal float Visibility;
		[MarshalAs(UnmanagedType.I1)] internal bool HasPresence;
		internal float Presence;
		internal IntPtr Name;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct MpLandmark
	{
		internal float X;
		internal float Y;
		internal float Z;
		[MarshalAs(UnmanagedType.I1)] internal bool HasVisibility;
		internal float Visibility;
		[MarshalAs(UnmanagedType.I1)] internal bool HasPresence;
		internal float Presence;
		internal IntPtr Name;
	}
}
