using System;
using ParkMinDev.UPM.MediaPipePlugin.Native;

namespace ParkMinDev.UPM.MediaPipePlugin.Objects
{
	public sealed class MediaPipeNativeException : Exception
	{
		// - Construct -
		internal MediaPipeNativeException(MpStatus status, string message) : base(message) {
			StatusCode = (int)status;
		}

		// - Public Properties -
		public int StatusCode { get; }
	}
}
