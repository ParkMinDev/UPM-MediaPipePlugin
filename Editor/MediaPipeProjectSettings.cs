using System.Collections.Generic;
using ParkMinPackages.MediaPipePlugin.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace ParkMinPackages.MediaPipePlugin.Editor
{
	[FilePath("ProjectSettings/ParkMinPackages.MediaPipePluginSettings.asset", FilePathAttribute.Location.ProjectFolder)]
	internal sealed class MediaPipeProjectSettings : ScriptableSingleton<MediaPipeProjectSettings>
	{
		// - Public Properties -
		internal IReadOnlyList<MediaPipeModelAsset> RegisteredModels
		{
			get { return _registeredModels; }
		}

		// - Internals -
		[SerializeField] List<MediaPipeModelAsset> _registeredModels = new List<MediaPipeModelAsset>();

		internal bool Contains(MediaPipeModelAsset model) {
			return model != null && _registeredModels.Contains(model);
		}

		internal void Register(MediaPipeModelAsset model) {
			if (model == null || _registeredModels.Contains(model))
				return;

			_registeredModels.Add(model);
			SaveSettings();
		}

		internal void SaveSettings() {
			Save(true);
		}
	}
}
