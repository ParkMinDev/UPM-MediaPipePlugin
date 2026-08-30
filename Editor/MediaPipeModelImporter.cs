using System;
using System.IO;
using System.Security.Cryptography;
using ParkMinPackages.MediaPipePlugin.ScriptableObjects;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace ParkMinPackages.MediaPipePlugin.Editor
{
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
