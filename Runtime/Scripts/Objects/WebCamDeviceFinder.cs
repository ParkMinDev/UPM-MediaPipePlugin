using System;
using UnityEngine;

namespace ParkMinDev.UPM.MediaPipePlugin.Objects
{
	public static class WebCamDeviceFinder
	{
		// - Statics -
		public static WebCamDevice Find(string preferredDeviceName, bool fallbackToFirstAvailable) {
			WebCamDevice[] devices = WebCamTexture.devices;

			if (devices.Length == 0)
				throw new InvalidOperationException("No web camera is available.");
			if (string.IsNullOrWhiteSpace(preferredDeviceName))
				return devices[0];

			for (int index = 0; index < devices.Length; index++)
				if (devices[index].name == preferredDeviceName)
					return devices[index];

			if (fallbackToFirstAvailable)
				return devices[0];

			throw new InvalidOperationException($"Web camera '{preferredDeviceName}' was not found.");
		}
	}
}
