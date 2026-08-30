using System;
using ParkMinPackages.MediaPipePlugin.Enums;
using ParkMinPackages.MediaPipePlugin.ScriptableObjects;
using R3;
using UnityEngine;

namespace ParkMinPackages.MediaPipePlugin.Objects.Runners
{
	[Serializable]
	public sealed class WebCamSkeletonRunner : SkeletonRunner
	{
		// - Construct -
		public WebCamSkeletonRunner() {
		}

		public WebCamSkeletonRunner(
			MediaPipeModelAsset model,
			float minPoseDetectionConfidence,
			float minPosePresenceConfidence,
			float minTrackingConfidence,
			WebCamTexture webCamTexture
		) : base(model, minPoseDetectionConfidence, minPosePresenceConfidence, minTrackingConfidence) {
			Initialize(webCamTexture);
		}

		// - Public Methods -
		public void Initialize(WebCamTexture webCamTexture) {
			if (webCamTexture == null)
				throw new MissingReferenceException($"{nameof(webCamTexture)} is required.");

			_webCamTexture = webCamTexture;
			_pixelBuffer = null;
			_lastTimestampMilliseconds = -1;
		}

		public void Start() {
			if (IsRunning)
				return;
			if (_webCamTexture == null)
				throw new InvalidOperationException($"{nameof(WebCamSkeletonRunner)} is not initialized with a web camera texture.");
			if (!IsInitialized)
				throw new InvalidOperationException($"{nameof(WebCamSkeletonRunner)} is not initialized with a MediaPipe model.");

			_updateSubscription = Observable.EveryUpdate(UnityFrameProvider.Update).Subscribe(_ => Update());
			IsRunning = true;
		}

		public void Stop() {
			_updateSubscription?.Dispose();
			_updateSubscription = null;
			IsRunning = false;
			_pixelBuffer = null;
		}

		public override void Dispose() {
			Stop();
			_webCamTexture = null;
			base.Dispose();
		}

		// - Public Properties -
		public WebCamTexture WebCamTexture
		{
			get { return _webCamTexture; }
		}

		public bool IsRunning { get; private set; }

		// - Internals -
		protected override PoseLandmarkerRunningMode RunningMode
		{
			get { return PoseLandmarkerRunningMode.Video; }
		}

		[NonSerialized] WebCamTexture _webCamTexture;
		[NonSerialized] Color32[] _pixelBuffer;
		[NonSerialized] IDisposable _updateSubscription;
		[NonSerialized] long _lastTimestampMilliseconds = -1;

		void Update() {
			if (_webCamTexture == null || !_webCamTexture.isPlaying || !_webCamTexture.didUpdateThisFrame || _webCamTexture.width <= 16)
				return;
			if (Skeletons == null)
				return;

			_pixelBuffer = _webCamTexture.GetPixels32(_pixelBuffer);
			long timestampMilliseconds = Math.Max((long)(Time.realtimeSinceStartupAsDouble * 1000d), _lastTimestampMilliseconds + 1);
			_lastTimestampMilliseconds = timestampMilliseconds;
			DetectVideo(_pixelBuffer, _webCamTexture.width, _webCamTexture.height, timestampMilliseconds);
		}
	}
}
