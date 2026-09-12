using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ParkMinPackages.MediaPipePlugin.Objects
{
	[Serializable]
	public sealed class WebCamTextureSource : IDisposable
	{
		// - Construct -
		public WebCamTextureSource() {
		}

		public WebCamTextureSource(
			string preferredDeviceName,
			bool fallbackToFirstAvailable,
			Vector2Int requestedResolution,
			int requestedFrameRate
		) {
			PreferredDeviceName = preferredDeviceName;
			FallbackToFirstAvailable = fallbackToFirstAvailable;
			RequestedResolution = requestedResolution;
			RequestedFrameRate = requestedFrameRate;
		}

		// - Public Methods -
		public async UniTask<WebCamTexture> OpenAsync(CancellationToken cancellationToken) {
			if (IsOpen)
				return _webCamTexture;

			Close();

			if (!Application.HasUserAuthorization(UserAuthorization.WebCam)) {
				AsyncOperation authorizationRequest = Application.RequestUserAuthorization(UserAuthorization.WebCam);
				await authorizationRequest.ToUniTask(cancellationToken: cancellationToken);

				if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
					throw new UnauthorizedAccessException("Web camera permission was denied.");
			}

			WebCamDevice device = WebCamDeviceFinder.Find(_preferredDeviceName, _fallbackToFirstAvailable);
			_webCamTexture = new WebCamTexture(device.name, _requestedResolution.x, _requestedResolution.y, _requestedFrameRate);
			_webCamTexture.Play();

			try {
				await UniTask.WaitUntil(() => _webCamTexture != null && _webCamTexture.width > 16, cancellationToken: cancellationToken);
			}
			catch {
				Close();
				throw;
			}

			return _webCamTexture;
		}

		public void Dispose() {
			if (_webCamTexture != null)
				UnityEngine.Object.Destroy(_webCamTexture);

			_webCamTexture = null;
		}

		public void Close() {
			if (_webCamTexture == null)
				return;

			if (_webCamTexture.isPlaying)
				_webCamTexture.Stop();

			UnityEngine.Object.Destroy(_webCamTexture);
			_webCamTexture = null;
		}

		// - Public Properties -
		public string PreferredDeviceName
		{
			get { return _preferredDeviceName; }
			set { _preferredDeviceName = value; }
		}

		public bool FallbackToFirstAvailable
		{
			get { return _fallbackToFirstAvailable; }
			set { _fallbackToFirstAvailable = value; }
		}

		public Vector2Int RequestedResolution
		{
			get { return _requestedResolution; }
			set { _requestedResolution = new Vector2Int(Mathf.Max(1, value.x), Mathf.Max(1, value.y)); }
		}

		public int RequestedFrameRate
		{
			get { return _requestedFrameRate; }
			set { _requestedFrameRate = Mathf.Max(1, value); }
		}

		public WebCamTexture Texture
		{
			get { return _webCamTexture; }
		}

		public bool IsOpen
		{
			get { return _webCamTexture != null && _webCamTexture.isPlaying; }
		}

		// - Internals -
		[SerializeField] string _preferredDeviceName;
		[SerializeField] bool _fallbackToFirstAvailable = true;
		[SerializeField] Vector2Int _requestedResolution = new Vector2Int(1280, 720);
		[SerializeField, Min(1)] int _requestedFrameRate = 30;

		[NonSerialized] WebCamTexture _webCamTexture;
	}
}
