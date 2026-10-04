using UnityEngine;

namespace ParkMinDev.UPM.MediaPipePlugin.ScriptableObjects
{
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceNamespace: "ParkMinPackages.MediaPipePlugin.ScriptableObjects", sourceAssembly: "ParkMinPackages.MediaPipePlugin", sourceClassName: "MediaPipeModelAsset")]
	public sealed class MediaPipeModelAsset : ScriptableObject
	{
		// - Public Properties -
		public string ModelName
		{
			get { return _modelName; }
		}

		public string RuntimeRelativePath
		{
			get { return _runtimeRelativePath; }
		}

		public string ContentHash
		{
			get { return _contentHash; }
		}

		public long FileSize
		{
			get { return _fileSize; }
		}

		// - Internals -
		[SerializeField] string _modelName;
		[SerializeField] string _runtimeRelativePath;
		[SerializeField] string _contentHash;
		[SerializeField] long _fileSize;

		internal void Initialize(string modelName, string runtimeRelativePath, string contentHash, long fileSize) {
			_modelName = modelName;
			_runtimeRelativePath = runtimeRelativePath;
			_contentHash = contentHash;
			_fileSize = fileSize;
		}
	}
}
