using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using ParkMinPackages.MediaPipePlugin.ScriptableObjects;
using UnityEngine;
#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Networking;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ParkMinPackages.MediaPipePlugin.Objects
{
	public static class MediaPipeModelLoader
	{
		// - Statics -
		public static async UniTask<MediaPipeModelData> LoadAsync(MediaPipeModelAsset model, CancellationToken cancellationToken) {
			if (model == null)
				throw new ArgumentNullException(nameof(model));

			cancellationToken.ThrowIfCancellationRequested();
			string cacheKey = string.IsNullOrWhiteSpace(model.ContentHash) ? model.RuntimeRelativePath : model.ContentHash;

			if (!ModelCache.TryGetValue(cacheKey, out Task<MediaPipeModelData> loadTask)) {
				loadTask = LoadModelAsync(model, Application.exitCancellationToken).AsTask();
				ModelCache.Add(cacheKey, loadTask);
			}

			try {
				return await loadTask.AsUniTask().AttachExternalCancellation(cancellationToken);
			}
			catch {
				if ((loadTask.IsCanceled || loadTask.IsFaulted) && ModelCache.TryGetValue(cacheKey, out Task<MediaPipeModelData> cachedTask) && ReferenceEquals(loadTask, cachedTask))
					ModelCache.Remove(cacheKey);

				throw;
			}
		}

		public static void ClearCache() {
			ModelCache.Clear();
		}

		static readonly Dictionary<string, Task<MediaPipeModelData>> ModelCache = new Dictionary<string, Task<MediaPipeModelData>>(StringComparer.Ordinal);

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		static void ResetCache() {
			ClearCache();
		}

		static UniTask<MediaPipeModelData> LoadModelAsync(MediaPipeModelAsset model, CancellationToken cancellationToken) {
#if UNITY_EDITOR
			string assetPath = AssetDatabase.GetAssetPath(model);

			if (string.IsNullOrWhiteSpace(assetPath))
				throw new InvalidOperationException($"{model.name} does not have a source asset path.");

			return UniTask.FromResult(new MediaPipeModelData(Path.GetFullPath(assetPath), null));
#elif UNITY_ANDROID
			return LoadBufferAsync(model, cancellationToken);
#else
			string modelPath = Path.Combine(Application.streamingAssetsPath, model.RuntimeRelativePath);

			if (!File.Exists(modelPath))
				throw new FileNotFoundException("The MediaPipe model was not included in StreamingAssets.", modelPath);

			return UniTask.FromResult(new MediaPipeModelData(modelPath, null));
#endif
		}

#if UNITY_ANDROID && !UNITY_EDITOR
		static async UniTask<MediaPipeModelData> LoadBufferAsync(MediaPipeModelAsset model, CancellationToken cancellationToken) {
			string modelUri = $"{Application.streamingAssetsPath}/{model.RuntimeRelativePath.Replace('\\', '/')}";
			using UnityWebRequest request = UnityWebRequest.Get(modelUri);
			await request.SendWebRequest().WithCancellation(cancellationToken);

			if (request.result != UnityWebRequest.Result.Success)
				throw new InvalidOperationException($"Failed to load MediaPipe model '{model.ModelName}': {request.error}");

			return new MediaPipeModelData(null, request.downloadHandler.data);
		}
#endif
	}
}
