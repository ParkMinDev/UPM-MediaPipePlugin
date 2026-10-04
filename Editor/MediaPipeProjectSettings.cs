using System.Collections.Generic;
using ParkMinDev.UPM.MediaPipePlugin.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace ParkMinDev.UPM.MediaPipePlugin.Editor
{
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceNamespace: "ParkMinPackages.MediaPipePlugin.Editor", sourceAssembly: "ParkMinPackages.MediaPipePlugin.Editor", sourceClassName: "MediaPipeProjectSettings")]
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
