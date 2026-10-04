using ParkMinDev.UPM.MediaPipePlugin.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace ParkMinDev.UPM.MediaPipePlugin.Editor
{
	internal static class MediaPipeProjectSettingsCommands
	{
		// - Statics -
		[MenuItem("Assets/ParkMinPackages/Register MediaPipe Models", false, 2100)]
		static void RegisterSelectedModels() {
			foreach (Object selectedObject in Selection.objects)
				if (selectedObject is MediaPipeModelAsset model)
					MediaPipeProjectSettings.instance.Register(model);
		}

		[MenuItem("Assets/ParkMinPackages/Register MediaPipe Models", true)]
		static bool CanRegisterSelectedModels() {
			foreach (Object selectedObject in Selection.objects)
				if (selectedObject is MediaPipeModelAsset)
					return true;

			return false;
		}
	}
}
