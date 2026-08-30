using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ParkMinPackages.MediaPipePlugin.Enums;
using ParkMinPackages.MediaPipePlugin.Objects;
using ParkMinPackages.MediaPipePlugin.ScriptableObjects;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Video;

namespace ParkMinPackages.MediaPipePlugin.Objects.Runners
{
	[Serializable]
	public sealed class VideoSkeletonRunner : SkeletonRunner
	{
		// - Construct -
		public VideoSkeletonRunner() {
		}

		public VideoSkeletonRunner(
			MediaPipeModelAsset model,
			float minPoseDetectionConfidence,
			float minPosePresenceConfidence,
			float minTrackingConfidence,
			VideoPlayer videoPlayer,
			bool loop
		) : base(model, minPoseDetectionConfidence, minPosePresenceConfidence, minTrackingConfidence) {
			VideoPlayer = videoPlayer;
			Loop = loop;
		}

		// - Public Methods -
		public async UniTask PlayAsync(VideoClip videoClip, CancellationToken cancellationToken) {
			if (videoClip == null)
				throw new ArgumentNullException(nameof(videoClip));

			await InitializeAsync(cancellationToken);
			Stop();
			_videoPlayer.playOnAwake = false;
			_videoPlayer.renderMode = VideoRenderMode.APIOnly;
			_videoPlayer.isLooping = _loop;
			_videoPlayer.clip = videoClip;
			_lastFrame = -1;
			_lastTimestampMilliseconds = -1;
			_playRequested = true;
			_videoPlayer.Prepare();
			Start();
		}

		public void Start() {
			if (IsRunning)
				return;
			if (!IsInitialized)
				throw new InvalidOperationException($"{nameof(VideoSkeletonRunner)} is not initialized with a MediaPipe model.");
			if (_videoPlayer == null)
				throw new MissingReferenceException($"{nameof(VideoPlayer)} is required.");

			_updateSubscription = Observable.EveryUpdate(UnityFrameProvider.Update).Subscribe(_ => Update());
			IsRunning = true;
		}

		public void Stop() {
			_updateSubscription?.Dispose();
			_updateSubscription = null;
			IsRunning = false;
			_playRequested = false;
			_videoPlayer?.Stop();
		}

		public override void Dispose() {
			Stop();

			if (_frameTexture != null)
				UnityEngine.Object.Destroy(_frameTexture);

			_frameTexture = null;
			base.Dispose();
		}

		// - Public Properties -
		public VideoPlayer VideoPlayer
		{
			get { return _videoPlayer; }
			set { _videoPlayer = value; }
		}

		public bool Loop
		{
			get { return _loop; }
			set { _loop = value; }
		}

		public bool IsRunning { get; private set; }

		// - Internals -
		[SerializeField, Required] VideoPlayer _videoPlayer;
		[SerializeField] bool _loop = true;

		protected override PoseLandmarkerRunningMode RunningMode
		{
			get { return PoseLandmarkerRunningMode.Video; }
		}

		[NonSerialized] Texture2D _frameTexture;
		[NonSerialized] IDisposable _updateSubscription;
		[NonSerialized] long _lastFrame = -1;
		[NonSerialized] long _lastTimestampMilliseconds = -1;
		[NonSerialized] bool _playRequested;

		void Update() {
			if (!_videoPlayer.isPrepared)
				return;

			if (_playRequested) {
				_playRequested = false;
				_videoPlayer.Play();
			}

			if (_videoPlayer.texture == null || _videoPlayer.frame < 0 || _videoPlayer.frame == _lastFrame)
				return;
			if (Skeletons == null)
				return;

			_lastFrame = _videoPlayer.frame;
			long timestampMilliseconds = Math.Max((long)(Time.realtimeSinceStartupAsDouble * 1000d), _lastTimestampMilliseconds + 1);
			_lastTimestampMilliseconds = timestampMilliseconds;
			DetectVideo(ReadTexture(_videoPlayer.texture), timestampMilliseconds);
		}

		Texture2D ReadTexture(Texture sourceTexture) {
			if (_frameTexture == null || _frameTexture.width != sourceTexture.width || _frameTexture.height != sourceTexture.height) {
				if (_frameTexture != null)
					UnityEngine.Object.Destroy(_frameTexture);

				_frameTexture = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.RGBA32, false);
			}

			RenderTexture temporaryTexture = RenderTexture.GetTemporary(sourceTexture.width, sourceTexture.height, 0, RenderTextureFormat.ARGB32);
			RenderTexture previousTexture = RenderTexture.active;

			try {
				Graphics.Blit(sourceTexture, temporaryTexture);
				RenderTexture.active = temporaryTexture;
				_frameTexture.ReadPixels(new Rect(0f, 0f, sourceTexture.width, sourceTexture.height), 0, 0, false);
				_frameTexture.Apply(false, false);
			}
			finally {
				RenderTexture.active = previousTexture;
				RenderTexture.ReleaseTemporary(temporaryTexture);
			}

			return _frameTexture;
		}
	}
}
