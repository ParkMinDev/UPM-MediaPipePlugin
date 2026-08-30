using System;
using ParkMinPackages.MediaPipePlugin.Native;

namespace ParkMinPackages.MediaPipePlugin.Objects
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
