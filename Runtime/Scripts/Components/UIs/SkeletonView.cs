using System;
using System.Collections.Generic;
using System.Linq;
using ParkMinPackages.Foundation.Constants;
using ParkMinPackages.MediaPipePlugin.Enums;
using ParkMinPackages.MediaPipePlugin.Objects;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace ParkMinPackages.MediaPipePlugin.Components.UIs
{
	[DisallowMultipleComponent, RequireComponent(typeof(RectTransform))]
	public sealed class SkeletonView : MonoBehaviour
	{
		// - Public Methods -
		public void SetSkeleton(Skeleton skeleton) {
			if (skeleton == null) {
				Clear();
				return;
			}

			if (_renderedLandmarkIds.Count != PoseSkeletonDefinition.LandmarkIds.Count || _renderedConnections.Count != PoseSkeletonDefinition.Connections.Count)
				RebuildVisuals();

			if (_skeleton == skeleton) {
				RenderSkeleton();
				return;
			}

			_skeletonSubscription?.Dispose();
			_skeleton = skeleton;
			_skeletonSubscription = skeleton.Updated.Subscribe(_ => RenderSkeleton());
			RenderSkeleton();
		}

		public void Clear() {
			_skeletonSubscription?.Dispose();
			_skeletonSubscription = null;
			_skeleton = null;
			HideVisuals();
		}

		public void SetDisplayTransform(int rotationAngle, bool horizontallyFlipped, bool verticallyFlipped) {
			rotationAngle = ((rotationAngle % 360) + 360) % 360;

			if (_rotationAngle == rotationAngle && _isHorizontallyFlipped == horizontallyFlipped && _isVerticallyFlipped == verticallyFlipped)
				return;

			_rotationAngle = rotationAngle;
			_isHorizontallyFlipped = horizontallyFlipped;
			_isVerticallyFlipped = verticallyFlipped;
			RenderSkeleton();
		}

		public void RebuildVisuals() {
			RebuildVisuals(PoseSkeletonDefinition.LandmarkIds, PoseSkeletonDefinition.Connections);
		}

		// - Public Properties -
		public Skeleton Skeleton
		{
			get { return _skeleton; }
		}

		public RectTransform RectTransform
		{
			get {
				if (_rectTransform == null)
					_rectTransform = (RectTransform)transform;

				return _rectTransform;
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
		void Awake() {
			_rectTransform = (RectTransform)transform;
			_renderedLandmarkIds = PoseSkeletonDefinition.LandmarkIds.ToArray();
			_renderedConnections = PoseSkeletonDefinition.Connections.ToArray();

			if (_landmarkContainer == null || _lineContainer == null || _landmarkImages.Count != _renderedLandmarkIds.Count || _lineImages.Count != _renderedConnections.Count || _landmarkImages.Any(image => image == null) || _lineImages.Any(image => image == null))
				RebuildVisuals();
		}

		void Reset() {
			RebuildVisuals();
			Clear();
		}

		void OnRectTransformDimensionsChange() {
			if (_skeleton != null)
				RenderSkeleton();
		}

		void OnDestroy() {
			_skeletonSubscription?.Dispose();
			_skeletonSubscription = null;
		}

#if UNITY_EDITOR
		void OnValidate() {
			ApplyStyle();

			if (_skeleton != null)
				RenderSkeleton();
		}
#endif

		// - Internals -
		[Header(Headers.Optional)]
		[SerializeField] Sprite _landmarkSprite;
		[SerializeField] Sprite _lineSprite;

		[Header(Headers.Settings)]
		[SerializeField, Min(1f)] float _landmarkSize = 16f;
		[SerializeField, Min(1f)] float _lineThickness = 5f;
		[SerializeField, Range(0f, 1f)] float _minimumVisibility = 0.5f;
		[SerializeField] Color _landmarkColor = new Color(0f, 0.85f, 1f, 1f);
		[SerializeField] Color _lineColor = Color.white;

		[SerializeField, HideInInspector] RectTransform _lineContainer;
		[SerializeField, HideInInspector] RectTransform _landmarkContainer;
		[SerializeField, HideInInspector] List<Image> _landmarkImages = new List<Image>();
		[SerializeField, HideInInspector] List<Image> _lineImages = new List<Image>();

		Skeleton _skeleton;
		IDisposable _skeletonSubscription;
		RectTransform _rectTransform;
		IReadOnlyList<SkeletonLandmarkId> _renderedLandmarkIds = Array.Empty<SkeletonLandmarkId>();
		IReadOnlyList<SkeletonConnection> _renderedConnections = Array.Empty<SkeletonConnection>();
		int _rotationAngle;
		bool _isHorizontallyFlipped;
		bool _isVerticallyFlipped;

		void RenderSkeleton() {
			if (_skeleton == null || !_skeleton.IsDetected) {
				HideVisuals();
				return;
			}

			Rect rect = RectTransform.rect;

			for (int index = 0; index < _renderedLandmarkIds.Count; index++) {
				Image landmarkImage = _landmarkImages[index];
				bool visible = _skeleton.TryGetLandmark(_renderedLandmarkIds[index], out SkeletonLandmark landmark) && IsVisible(landmark);
				landmarkImage.gameObject.SetActive(visible);

				if (visible)
					landmarkImage.rectTransform.anchoredPosition = ToAnchoredPosition(landmark.Position, rect);
			}

			for (int index = 0; index < _renderedConnections.Count; index++) {
				SkeletonConnection connection = _renderedConnections[index];
				Image lineImage = _lineImages[index];
				bool hasStart = _skeleton.TryGetLandmark(connection.Start, out SkeletonLandmark start);
				bool hasEnd = _skeleton.TryGetLandmark(connection.End, out SkeletonLandmark end);
				bool visible = hasStart && IsVisible(start) && hasEnd && IsVisible(end);
				lineImage.gameObject.SetActive(visible);

				if (!visible)
					continue;

				Vector2 startPosition = ToAnchoredPosition(start.Position, rect);
				Vector2 endPosition = ToAnchoredPosition(end.Position, rect);
				Vector2 direction = endPosition - startPosition;
				RectTransform lineTransform = lineImage.rectTransform;
				lineTransform.anchoredPosition = (startPosition + endPosition) * 0.5f;
				lineTransform.sizeDelta = new Vector2(direction.magnitude, _lineThickness);
				lineTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
			}
		}

		void HideVisuals() {
			foreach (Image landmarkImage in _landmarkImages)
				if (landmarkImage != null)
					landmarkImage.gameObject.SetActive(false);

			foreach (Image lineImage in _lineImages)
				if (lineImage != null)
					lineImage.gameObject.SetActive(false);
		}

		void RebuildVisuals(IReadOnlyList<SkeletonLandmarkId> landmarkIds, IReadOnlyList<SkeletonConnection> connections) {
			_lineContainer = GetOrCreateContainer(_lineContainer, "Lines", 0);
			_landmarkContainer = GetOrCreateContainer(_landmarkContainer, "Landmarks", 1);
			DestroyChildren(_lineContainer);
			DestroyChildren(_landmarkContainer);
			_landmarkImages.Clear();
			_lineImages.Clear();

			foreach (SkeletonConnection connection in connections)
				_lineImages.Add(CreateImage(_lineContainer, $"{connection.Start} - {connection.End}"));

			foreach (SkeletonLandmarkId landmarkId in landmarkIds)
				_landmarkImages.Add(CreateImage(_landmarkContainer, landmarkId.ToString()));

			_renderedLandmarkIds = landmarkIds.ToArray();
			_renderedConnections = connections.ToArray();
			ApplyStyle();
		}

		RectTransform GetOrCreateContainer(RectTransform container, string containerName, int siblingIndex) {
			if (container == null) {
				GameObject containerObject = new GameObject(containerName, typeof(RectTransform));
				containerObject.layer = gameObject.layer;
				container = containerObject.GetComponent<RectTransform>();
				container.SetParent(transform, false);
			}

			container.name = containerName;
			container.SetSiblingIndex(siblingIndex);
			container.anchorMin = Vector2.zero;
			container.anchorMax = Vector2.one;
			container.pivot = new Vector2(0.5f, 0.5f);
			container.offsetMin = Vector2.zero;
			container.offsetMax = Vector2.zero;
			return container;
		}

		Image CreateImage(RectTransform parent, string imageName) {
			GameObject imageObject = new GameObject(imageName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
			imageObject.layer = gameObject.layer;
			RectTransform imageTransform = imageObject.GetComponent<RectTransform>();
			imageTransform.SetParent(parent, false);
			imageTransform.anchorMin = new Vector2(0.5f, 0.5f);
			imageTransform.anchorMax = new Vector2(0.5f, 0.5f);
			imageTransform.pivot = new Vector2(0.5f, 0.5f);
			Image image = imageObject.GetComponent<Image>();
			image.raycastTarget = false;
			return image;
		}

		void DestroyChildren(RectTransform container) {
			for (int index = container.childCount - 1; index >= 0; index--) {
				GameObject child = container.GetChild(index).gameObject;

				if (Application.isPlaying)
					Destroy(child);
				else
					DestroyImmediate(child);
			}
		}

		void ApplyStyle() {
			foreach (Image landmarkImage in _landmarkImages) {
				if (landmarkImage == null)
					continue;
				landmarkImage.sprite = _landmarkSprite;
				landmarkImage.color = _landmarkColor;
				landmarkImage.rectTransform.sizeDelta = new Vector2(_landmarkSize, _landmarkSize);
			}

			foreach (Image lineImage in _lineImages) {
				if (lineImage == null)
					continue;
				lineImage.sprite = _lineSprite;
				lineImage.color = _lineColor;
			}
		}

		bool IsVisible(SkeletonLandmark landmark) {
			return (!landmark.HasVisibility || landmark.Visibility >= _minimumVisibility) && (!landmark.HasPresence || landmark.Presence >= _minimumVisibility);
		}

		Vector2 ToAnchoredPosition(Vector3 normalizedPosition, Rect rect) {
			float x = (normalizedPosition.x - 0.5f) * rect.width;
			float y = (0.5f - normalizedPosition.y) * rect.height;

			if (_isHorizontallyFlipped)
				x = -x;
			if (_isVerticallyFlipped)
				y = -y;

			float rotationRadians = -_rotationAngle * Mathf.Deg2Rad;
			float cosine = Mathf.Cos(rotationRadians);
			float sine = Mathf.Sin(rotationRadians);
			return new Vector2(x * cosine - y * sine, x * sine + y * cosine);
		}
	}
}
