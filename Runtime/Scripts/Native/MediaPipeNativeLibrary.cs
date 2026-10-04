using System;
using System.Runtime.InteropServices;
using ParkMinDev.UPM.MediaPipePlugin.Objects;

namespace ParkMinDev.UPM.MediaPipePlugin.Native
{
	internal static class MediaPipeNativeLibrary
	{
		// - Statics -
#if UNITY_ANDROID && !UNITY_EDITOR
		internal const string Name = "mediapipe";
#else
		internal const string Name = "libmediapipe";
#endif

		[DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void MpErrorFree(IntPtr errorMessage);

		internal static void ThrowIfFailed(MpStatus status, IntPtr errorMessage) {
			string message = errorMessage == IntPtr.Zero
				? status.ToString()
				: Marshal.PtrToStringUTF8(errorMessage);

			if (errorMessage != IntPtr.Zero)
				MpErrorFree(errorMessage);

			if (status != MpStatus.Ok)
				throw new MediaPipeNativeException(status, message);
		}
	}
}
