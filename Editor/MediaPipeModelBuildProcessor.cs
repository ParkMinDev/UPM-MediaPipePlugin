using System;
using System.Collections.Generic;
using System.IO;
using ParkMinPackages.MediaPipePlugin.ScriptableObjects;
using UnityEditor;
using UnityEditor.Build;

namespace ParkMinPackages.MediaPipePlugin.Editor
{
	internal sealed class MediaPipeModelBuildProcessor : BuildPlayerProcessor
	{
		// - Public Methods -
		public override void PrepareForBuild(BuildPlayerContext buildPlayerContext) {
			IReadOnlyList<MediaPipeModelAsset> registeredModels = MediaPipeProjectSettings.instance.RegisteredModels;
			HashSet<string> registeredAssetPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			HashSet<string> runtimePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			for (int index = 0; index < registeredModels.Count; index++) {
				MediaPipeModelAsset model = registeredModels[index];

				if (model == null)
					throw new BuildFailedException($"Registered MediaPipe model at index {index} is missing.");

				string assetPath = AssetDatabase.GetAssetPath(model);

				if (string.IsNullOrWhiteSpace(assetPath) || !File.Exists(assetPath))
					throw new BuildFailedException($"MediaPipe model source file was not found: {model.ModelName}");
				if (!assetPath.EndsWith(".task", StringComparison.OrdinalIgnoreCase))
					throw new BuildFailedException($"MediaPipe model must use the .task extension: {assetPath}");
				if (!registeredAssetPaths.Add(assetPath))
					continue;
				if (!runtimePaths.Add(model.RuntimeRelativePath))
					throw new BuildFailedException($"MediaPipe model runtime path is duplicated: {model.RuntimeRelativePath}");

				buildPlayerContext.AddAdditionalPathToStreamingAssets(assetPath, model.RuntimeRelativePath);
			}

			foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
				if (!scene.enabled)
					continue;

				string[] dependencies = AssetDatabase.GetDependencies(scene.path, true);

				foreach (string dependency in dependencies) {
					MediaPipeModelAsset model = AssetDatabase.LoadAssetAtPath<MediaPipeModelAsset>(dependency);

					if (model != null && !registeredAssetPaths.Contains(dependency))
						throw new BuildFailedException($"MediaPipe model used by '{scene.path}' is not registered in Project Settings: {dependency}");
				}
			}
		}
	}
}
