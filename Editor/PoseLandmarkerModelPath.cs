using System;
using System.IO;
using System.Linq;
using UnityEditor.PackageManager;

namespace ParkMinDev.UPM.MediaPipePlugin.Editor
{
	public static class PoseLandmarkerModelPath
	{
		// - Statics -
		const string NativeRuntimePackageName = "com.parkmindev.mediapipenativeruntime.upm";
		const string LiteModelRelativePath = "Runtime/Models/PoseLandmarker/pose_landmarker_lite.task";

		public static string Lite
		{
			get
			{
				PackageInfo packageInfo = PackageInfo.GetAllRegisteredPackages().FirstOrDefault(info => info.name == NativeRuntimePackageName);

				if (packageInfo == null)
					throw new InvalidOperationException($"{NativeRuntimePackageName} is not installed.");

				string modelPath = Path.Combine(packageInfo.resolvedPath, LiteModelRelativePath);

				if (!File.Exists(modelPath))
					throw new FileNotFoundException("The pose landmarker lite model could not be found.", modelPath);

				return modelPath;
			}
		}
	}
}
