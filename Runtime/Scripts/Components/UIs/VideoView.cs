using ParkMinDev.UPM.Foundation.Components;
using ParkMinDev.UPM.Foundation.Constants;
using ParkMinDev.UPM.Foundation.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace ParkMinDev.UPM.MediaPipePlugin.Components.UIs
{
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceNamespace: "ParkMinPackages.MediaPipePlugin.Components.UIs", sourceAssembly: "ParkMinPackages.MediaPipePlugin", sourceClassName: "VideoView")]
	public sealed class VideoView : ExtendedBehaviour, IR3Updatable
	{
		// - Public Methods -
		public void SetVideoPlayer(VideoPlayer videoPlayer) {
			if (videoPlayer == null)
				throw new MissingReferenceException($"{nameof(videoPlayer)} is required.");

			_videoPlayer = videoPlayer;
			_isActive = true;
			ApplyDisplaySettings();
		}

		public void Clear() {
			_rawImage.texture = null;
			_texture = null;
			_isActive = false;
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

		public VideoPlayer VideoPlayer
		{
			get { return _videoPlayer; }
		}

		public Texture Texture
		{
			get { return _texture; }
		}

		public int RotationAngle
		{
			get { return _rotationAngle; }
			set {
				_rotationAngle = ((value % 360) + 360) % 360;
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

		// - Handler -
		void IR3Updatable.R3Update() {
			ApplyDisplaySettings();
		}

		// - Internals -
		[Header(Headers.Required)]
		[SerializeField, Required] RawImage _rawImage;
		[SerializeField, Required] VideoPlayer _videoPlayer;

		[Header(Headers.Optional)]
		[SerializeField] AspectRatioFitter _aspectRatioFitter;

		[Header(Headers.Settings)]
		[SerializeField] int _rotationAngle;
		[SerializeField] bool _flipHorizontal;
		[SerializeField] bool _flipVertical;

		Texture _texture;
		int _textureWidth;
		int _textureHeight;
		int _appliedRotationAngle = -1;
		bool _isHorizontallyFlipped;
		bool _isVerticallyFlipped;
		bool _isActive;

		void ApplyDisplaySettings() {
			if (!_isActive || _rawImage == null || _videoPlayer == null)
				return;

			Texture texture = _videoPlayer.texture;

			if (texture == null)
				return;

			int textureWidth = texture.width;
			int textureHeight = texture.height;

			if (_texture == texture && _textureWidth == textureWidth && _textureHeight == textureHeight && _appliedRotationAngle == _rotationAngle && _isHorizontallyFlipped == _flipHorizontal && _isVerticallyFlipped == _flipVertical)
				return;

			_texture = texture;
			_textureWidth = textureWidth;
			_textureHeight = textureHeight;
			_appliedRotationAngle = _rotationAngle;
			_isHorizontallyFlipped = _flipHorizontal;
			_isVerticallyFlipped = _flipVertical;
			_rawImage.texture = texture;
			_rawImage.rectTransform.localEulerAngles = new Vector3(0f, 0f, -_rotationAngle);
			_rawImage.uvRect = new Rect(_flipHorizontal ? 1f : 0f, _flipVertical ? 1f : 0f, _flipHorizontal ? -1f : 1f, _flipVertical ? -1f : 1f);

			if (_aspectRatioFitter != null && textureWidth > 0 && textureHeight > 0)
				_aspectRatioFitter.aspectRatio = _rotationAngle % 180 == 0 ? (float)textureWidth / textureHeight : (float)textureHeight / textureWidth;
		}
	}
}
