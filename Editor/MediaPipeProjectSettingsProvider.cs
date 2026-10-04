using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ParkMinDev.UPM.MediaPipePlugin.Editor
{
	internal static class MediaPipeProjectSettingsProvider
	{
		// - Statics -
		[SettingsProvider]
		static SettingsProvider CreateProvider() {
			SettingsProvider provider = new SettingsProvider("Project/ParkMinPackages/MediaPipe Plugin", SettingsScope.Project) {
				label = "MediaPipe Plugin",
				keywords = new HashSet<string> { "ParkMinPackages", "MediaPipe", "Model", "Task" },
				guiHandler = _ =>
				{
					MediaPipeProjectSettings settings = MediaPipeProjectSettings.instance;
					SerializedObject serializedSettings = new SerializedObject(settings);
					serializedSettings.Update();
					EditorGUILayout.HelpBox("Only registered MediaPipe models are included in player builds.", MessageType.Info);
					EditorGUILayout.PropertyField(serializedSettings.FindProperty("_registeredModels"), true);

					if (serializedSettings.ApplyModifiedProperties())
						settings.SaveSettings();
				}
			};

			return provider;
		}
	}
}
