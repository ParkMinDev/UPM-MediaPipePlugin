using System;
using System.IO;
using System.Security.Cryptography;
using ParkMinDev.UPM.MediaPipePlugin.ScriptableObjects;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace ParkMinDev.UPM.MediaPipePlugin.Editor
{
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceNamespace: "ParkMinPackages.MediaPipePlugin.Editor", sourceAssembly: "ParkMinPackages.MediaPipePlugin.Editor", sourceClassName: "MediaPipeModelImporter")]
	[ScriptedImporter(1, "task")]
	public sealed class MediaPipeModelImporter : ScriptedImporter
	{
		// - Public Methods -
		public override void OnImportAsset(AssetImportContext context) {
			byte[] modelBytes = File.ReadAllBytes(context.assetPath);
			string contentHash;

			using (SHA256 sha256 = SHA256.Create()) {
				byte[] hashBytes = sha256.ComputeHash(modelBytes);
				contentHash = BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLowerInvariant();
			}

			MediaPipeModelAsset modelAsset = ScriptableObject.CreateInstance<MediaPipeModelAsset>();
			string fileName = Path.GetFileName(context.assetPath);
			modelAsset.name = Path.GetFileNameWithoutExtension(context.assetPath);
			modelAsset.Initialize(modelAsset.name, $"ParkMinPackages/MediaPipe/Models/{fileName}", contentHash, modelBytes.LongLength);
			context.AddObjectToAsset("Model", modelAsset);
			context.SetMainObject(modelAsset);
		}
	}
}
