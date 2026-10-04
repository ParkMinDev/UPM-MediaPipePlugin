using ParkMinDev.UPM.Foundation.Components;
using ParkMinDev.UPM.Foundation.Constants;
using ParkMinDev.UPM.Foundation.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace ParkMinDev.UPM.MediaPipePlugin.Components.UIs
{
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceNamespace: "ParkMinPackages.MediaPipePlugin.Components.UIs", sourceAssembly: "ParkMinPackages.MediaPipePlugin", sourceClassName: "WebCamView")]
	public sealed class WebCamView : ExtendedBehaviour, IR3Updatable
	{
		// - Public Methods -
		public void SetWebCamTexture(WebCamTexture webCamTexture) {
			if (webCamTexture == null)
				throw new MissingReferenceException($"{nameof(webCamTexture)} is required.");

			_webCamTexture = webCamTexture;
			_rawImage.texture = webCamTexture;
			ApplyDisplaySettings();
		}

		public void Clear() {
			if (_rawImage.texture == _webCamTexture)
				_rawImage.texture = null;

			_webCamTexture = null;
			_rotationAngle = 0;
			_isHorizontallyFlipped = false;
			_isVerticallyFlipped = false;
			_textureWidth = 0;
			_textureHeight = 0;
			_rawImage.uvRect = new Rect(0f, 0f, 1f, 1f);
			_rawImage.rectTransform.localEulerAngles = Vector3.zero;
		}

		// - Public Properties -
		public RawImage RawImage
		{
			get { return _rawImage; }
		}

		public AspectRatioFitter AspectRatioFitter
		{
			get { return _aspectRatioFitter; }
			set {
				_aspectRatioFitter = value;
				ApplyDisplaySettings();
			}
		}

		public WebCamTexture WebCamTexture
		{
			get { return _webCamTexture; }
		}

		public bool ApplyDeviceRotation
		{
			get { return _applyDeviceRotation; }
			set {
				_applyDeviceRotation = value;
				ApplyDisplaySettings();
			}
		}

		public bool ApplyDeviceVerticalFlip
		{
			get { return _applyDeviceVerticalFlip; }
			set {
				_applyDeviceVerticalFlip = value;
				ApplyDisplaySettings();
			}
		}

		public bool FlipHorizontal
		{
			get { return _flipHorizontal; }
			set {
				_flipHorizontal = value;
				ApplyDisplaySettings();
			}
		}

		public bool FlipVertical
		{
			get { return _flipVertical; }
			set {
				_flipVertical = value;
				ApplyDisplaySettings();
			}
		}

		public int RotationAngle
		{
			get { return _rotationAngle; }
		}

		public bool IsHorizontallyFlipped
		{
			get { return _isHorizontallyFlipped; }
		}

		public bool IsVerticallyFlipped
		{
			get { return _isVerticallyFlipped; }
		}

		// - Handler -
		void IR3Updatable.R3Update() {
			if (_webCamTexture == null || _webCamTexture.width <= 16)
				return;

			ApplyDisplaySettings();
		}

		// - Internals -
		[Header(Headers.Required)]
		[SerializeField, Required] RawImage _rawImage;

		[Header(Headers.Optional)]
		[SerializeField] AspectRatioFitter _aspectRatioFitter;

		[Header(Headers.Settings)]
		[SerializeField] bool _applyDeviceRotation = true;
		[SerializeField] bool _applyDeviceVerticalFlip = true;
		[SerializeField] bool _flipHorizontal;
		[SerializeField] bool _flipVertical;

		WebCamTexture _webCamTexture;
		int _rotationAngle;
		int _textureWidth;
		int _textureHeight;
		bool _isHorizontallyFlipped;
		bool _isVerticallyFlipped;

		void ApplyDisplaySettings() {
			if (_rawImage == null || _webCamTexture == null)
				return;

			int rotationAngle = _applyDeviceRotation ? ((_webCamTexture.videoRotationAngle % 360) + 360) % 360 : 0;
			bool isHorizontallyFlipped = _flipHorizontal;
			bool isVerticallyFlipped = (_applyDeviceVerticalFlip && _webCamTexture.videoVerticallyMirrored) != _flipVertical;
			int textureWidth = _webCamTexture.width;
			int textureHeight = _webCamTexture.height;

			if (_rotationAngle == rotationAngle && _isHorizontallyFlipped == isHorizontallyFlipped && _isVerticallyFlipped == isVerticallyFlipped && _textureWidth == textureWidth && _textureHeight == textureHeight)
				return;

			_rotationAngle = rotationAngle;
			_isHorizontallyFlipped = isHorizontallyFlipped;
			_isVerticallyFlipped = isVerticallyFlipped;
			_textureWidth = textureWidth;
			_textureHeight = textureHeight;
			_rawImage.rectTransform.localEulerAngles = new Vector3(0f, 0f, -rotationAngle);
			_rawImage.uvRect = new Rect(isHorizontallyFlipped ? 1f : 0f, isVerticallyFlipped ? 1f : 0f, isHorizontallyFlipped ? -1f : 1f, isVerticallyFlipped ? -1f : 1f);

			if (_aspectRatioFitter != null && textureWidth > 0 && textureHeight > 0)
				_aspectRatioFitter.aspectRatio = rotationAngle % 180 == 0 ? (float)textureWidth / textureHeight : (float)textureHeight / textureWidth;
		}
	}
}
