using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ParkMinDev.UPM.MediaPipePlugin.Enums;
using ParkMinDev.UPM.MediaPipePlugin.Objects;
using ParkMinDev.UPM.MediaPipePlugin.ScriptableObjects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ParkMinDev.UPM.MediaPipePlugin.Objects.Runners
{
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceNamespace: "ParkMinPackages.MediaPipePlugin.Objects.Runners", sourceAssembly: "ParkMinPackages.MediaPipePlugin", sourceClassName: "SkeletonRunner")]
	[Serializable]
	public abstract class SkeletonRunner : IDisposable
	{
		// - Construct -
		protected SkeletonRunner() {
		}

		protected SkeletonRunner(
			MediaPipeModelAsset model,
			float minPoseDetectionConfidence,
			float minPosePresenceConfidence,
			float minTrackingConfidence
		) {
			Model = model;
			MinPoseDetectionConfidence = minPoseDetectionConfidence;
			MinPosePresenceConfidence = minPosePresenceConfidence;
			MinTrackingConfidence = minTrackingConfidence;
		}

		// - Public Methods -
		public async UniTask InitializeAsync(CancellationToken cancellationToken) {
			if (IsDisposed)
				throw new ObjectDisposedException(GetType().Name);
			if (IsInitialized)
				return;

			if (IsInitializing) {
				await UniTask.WaitUntil(() => !IsInitializing, cancellationToken: cancellationToken);

				if (IsInitialized)
					return;
			}

			SkeletonCollection skeletons = _skeletons;

			if (skeletons == null)
				throw new MissingReferenceException($"{nameof(Skeletons)} is required.");
			if (_model == null)
				throw new MissingReferenceException($"{GetType().Name} requires a {nameof(MediaPipeModelAsset)}.");

			IsInitializing = true;

			try {
				MediaPipeModelData modelData = await MediaPipeModelLoader.LoadAsync(_model, cancellationToken);
				cancellationToken.ThrowIfCancellationRequested();

				if (IsDisposed)
					throw new ObjectDisposedException(GetType().Name);

				PoseLandmarkerOptions options = modelData.IsBuffer
					? new PoseLandmarkerOptions(modelData.Buffer)
					: new PoseLandmarkerOptions(modelData.FilePath);
				options.RunningMode = RunningMode;
				options.NumPoses = skeletons.Length;
				options.MinPoseDetectionConfidence = _minPoseDetectionConfidence;
				options.MinPosePresenceConfidence = _minPosePresenceConfidence;
				options.MinTrackingConfidence = _minTrackingConfidence;
				_poseLandmarker = new PoseLandmarker(options);
				_initializedPoseCount = skeletons.Length;
				IsInitialized = true;
			}
			finally {
				IsInitializing = false;
			}
		}

		public virtual void Dispose() {
			if (IsDisposed)
				return;

			IsDisposed = true;
			_poseLandmarker?.Dispose();
			_poseLandmarker = null;
			_skeletons = null;
			_initializedPoseCount = 0;
			IsInitialized = false;
		}

		// - Public Properties -
		public MediaPipeModelAsset Model
		{
			get { return _model; }
			set
			{
				if (IsDisposed)
					throw new ObjectDisposedException(GetType().Name);
				if (IsInitialized || IsInitializing)
					throw new InvalidOperationException($"{nameof(Model)} cannot be changed after initialization has started.");

				_model = value;
			}
		}

		public float MinPoseDetectionConfidence
		{
			get { return _minPoseDetectionConfidence; }
			set
			{
				if (IsDisposed)
					throw new ObjectDisposedException(GetType().Name);
				if (IsInitialized || IsInitializing)
					throw new InvalidOperationException($"{nameof(MinPoseDetectionConfidence)} cannot be changed after initialization has started.");

				_minPoseDetectionConfidence = Mathf.Clamp01(value);
			}
		}

		public float MinPosePresenceConfidence
		{
			get { return _minPosePresenceConfidence; }
			set
			{
				if (IsDisposed)
					throw new ObjectDisposedException(GetType().Name);
				if (IsInitialized || IsInitializing)
					throw new InvalidOperationException($"{nameof(MinPosePresenceConfidence)} cannot be changed after initialization has started.");

				_minPosePresenceConfidence = Mathf.Clamp01(value);
			}
		}

		public float MinTrackingConfidence
		{
			get { return _minTrackingConfidence; }
			set
			{
				if (IsDisposed)
					throw new ObjectDisposedException(GetType().Name);
				if (IsInitialized || IsInitializing)
					throw new InvalidOperationException($"{nameof(MinTrackingConfidence)} cannot be changed after initialization has started.");

				_minTrackingConfidence = Mathf.Clamp01(value);
			}
		}

		public SkeletonCollection Skeletons
		{
			get { return _skeletons; }
			set
			{
				if (IsDisposed)
					throw new ObjectDisposedException(GetType().Name);
				if (IsInitializing)
					throw new InvalidOperationException($"{nameof(Skeletons)} cannot be changed while initialization is in progress.");
				if (IsInitialized && value != null && value.Length != _initializedPoseCount)
					throw new InvalidOperationException($"{nameof(Skeletons)} length must remain {_initializedPoseCount} after initialization.");

				_skeletons = value;
			}
		}

		public bool IsInitializing { get; private set; }
		public bool IsInitialized { get; private set; }
		public bool IsDisposed { get; private set; }

		// - Internals -
		[SerializeField, Required] MediaPipeModelAsset _model;
		[SerializeField, Range(0f, 1f)] float _minPoseDetectionConfidence = 0.5f;
		[SerializeField, Range(0f, 1f)] float _minPosePresenceConfidence = 0.5f;
		[SerializeField, Range(0f, 1f)] float _minTrackingConfidence = 0.5f;

		protected abstract PoseLandmarkerRunningMode RunningMode { get; }

		protected int DetectImage(Texture2D texture) {
			if (Skeletons == null)
				return 0;
			if (!IsInitialized)
				throw new InvalidOperationException($"{GetType().Name} is not initialized.");

			return _poseLandmarker.Detect(texture, Skeletons);
		}

		protected int DetectVideo(Texture2D texture, long timestampMilliseconds) {
			if (Skeletons == null)
				return 0;
			if (!IsInitialized)
				throw new InvalidOperationException($"{GetType().Name} is not initialized.");

			return _poseLandmarker.DetectForVideo(texture, timestampMilliseconds, Skeletons);
		}

		protected int DetectVideo(Color32[] pixels, int width, int height, long timestampMilliseconds) {
			if (Skeletons == null)
				return 0;
			if (!IsInitialized)
				throw new InvalidOperationException($"{GetType().Name} is not initialized.");

			return _poseLandmarker.DetectForVideo(pixels, width, height, timestampMilliseconds, Skeletons);
		}

		[NonSerialized] SkeletonCollection _skeletons;
		[NonSerialized] PoseLandmarker _poseLandmarker;
		[NonSerialized] int _initializedPoseCount;
	}
}
