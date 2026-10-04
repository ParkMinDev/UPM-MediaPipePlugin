using System;
using System.Runtime.InteropServices;

namespace ParkMinDev.UPM.MediaPipePlugin.Native
{
	internal static class PoseLandmarkerNativeMethods
	{
		// - Statics -
		[DllImport(MediaPipeNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		internal static extern MpStatus MpImageCreateFromUint8Data(MpImageFormat format, int width, int height, byte[] pixelData, int pixelDataSize, out IntPtr image, out IntPtr errorMessage);

		[DllImport(MediaPipeNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void MpImageFree(IntPtr image);

		[DllImport(MediaPipeNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		internal static extern MpStatus MpPoseLandmarkerCreate(ref MpPoseLandmarkerOptions options, out IntPtr landmarker, out IntPtr errorMessage);

		[DllImport(MediaPipeNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		internal static extern MpStatus MpPoseLandmarkerDetectImage(IntPtr landmarker, IntPtr image, IntPtr imageProcessingOptions, out MpPoseLandmarkerResult result, out IntPtr errorMessage);

		[DllImport(MediaPipeNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		internal static extern MpStatus MpPoseLandmarkerDetectForVideo(IntPtr landmarker, IntPtr image, IntPtr imageProcessingOptions, long timestampMilliseconds, out MpPoseLandmarkerResult result, out IntPtr errorMessage);

		[DllImport(MediaPipeNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void MpPoseLandmarkerCloseResult(ref MpPoseLandmarkerResult result);

		[DllImport(MediaPipeNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		internal static extern MpStatus MpPoseLandmarkerClose(IntPtr landmarker, out IntPtr errorMessage);
	}
}
