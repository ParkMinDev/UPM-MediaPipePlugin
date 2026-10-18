using System;
using System.Collections.Generic;
using System.Linq;
using ParkMinDev.UPM.Foundation.Constants;
using ParkMinDev.UPM.MediaPipePlugin.Objects;
using ParkMinDev.UPM.UGUI.Components.Graphics;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ParkMinDev.UPM.MediaPipePlugin.Components.UIs
{
    [DisallowMultipleComponent, RequireComponent(typeof(RectTransform))]
    public sealed class SkeletonAngleView : MonoBehaviour
    {
        // - Public Methods -
        public void SetSkeleton(Skeleton skeleton) {
            if (skeleton == null) {
                Clear();
                return;
            }

            if (_skeleton == skeleton) {
                RenderAngles();
                return;
            }

            _skeletonSubscription?.Dispose();
            _skeletonSubscription = null;
            _skeleton = skeleton;
            if (isActiveAndEnabled) {
                _skeletonSubscription = skeleton.Updated.Subscribe(_ => RenderAngles());
                RebuildVisuals();
            }
        }

        public void Clear() {
            _skeletonSubscription?.Dispose();
            _skeletonSubscription = null;
            _skeleton = null;
            HideVisuals();
        }

        public void SetAngles(SkeletonAngleOptions[] angles) {
            _angles = angles?.ToArray() ?? throw new ArgumentNullException(nameof(angles));
            RebuildVisuals();
        }

        public void SetDisplayTransform(int rotationAngle, bool horizontallyFlipped, bool verticallyFlipped) {
            rotationAngle = ((rotationAngle % 360) + 360) % 360;
            if (_rotationAngle == rotationAngle && _isHorizontallyFlipped == horizontallyFlipped && _isVerticallyFlipped == verticallyFlipped)
                return;

            _rotationAngle = rotationAngle;
            _isHorizontallyFlipped = horizontallyFlipped;
            _isVerticallyFlipped = verticallyFlipped;
            RenderAngles();
        }

        public void RebuildVisuals() {
            if (!isActiveAndEnabled)
                return;

            // Remove unused graphics
            for (int index = _graphics.Count - 1; index >= _angles.Length; index--) {
                DestroyGraphic(_graphics[index]);
                _graphics.RemoveAt(index);
            }

            // Create and reuse owned children
            for (int index = 0; index < _angles.Length; index++) {
                if (_graphics.Count <= index)
                    _graphics.Add(null);

                SkeletonAngleOptions angle = _angles[index];
                if (angle == null) {
                    if (_graphics[index] != null)
                        _graphics[index].gameObject.SetActive(false);
                    continue;
                }

                AngleGraphic graphic = _graphics[index];
                if (graphic == null) {
                    GameObject graphicObject = new GameObject("AngleGraphic", typeof(RectTransform), typeof(CanvasRenderer), typeof(AngleGraphic));
                    graphicObject.layer = gameObject.layer;
                    RectTransform graphicTransform = graphicObject.GetComponent<RectTransform>();
                    graphicTransform.SetParent(transform, false);
                    graphicTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    graphicTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    graphicTransform.pivot = new Vector2(0.5f, 0.5f);
                    graphic = graphicObject.GetComponent<AngleGraphic>();
                    graphic.raycastTarget = false;
                    _graphics[index] = graphic;
                }

                graphic.name = $"Angle_{index}_{angle.FirstLandmark}_{angle.VertexLandmark}_{angle.LastLandmark}";
                graphic.ApplyOptions(angle.GraphicOptions);
                float extent = Mathf.Max(Mathf.Max(0f, angle.GraphicOptions.Radius) + Mathf.Max(angle.GraphicOptions.GuideExtension, angle.GraphicOptions.ArcThickness * 0.5f), angle.GraphicOptions.VertexRadius) + Mathf.Max(0f, angle.GraphicOptions.GuideThickness) * 0.5f;
                graphic.rectTransform.sizeDelta = Vector2.one * Mathf.Max(1f, extent * 2f);
            }

            _refreshRequested = false;
            RenderAngles();
        }

        // - Public Properties -
        public IReadOnlyList<SkeletonAngleOptions> Angles { get { return _angles; } }
        public IReadOnlyList<AngleGraphic> Graphics { get { return _graphics; } }
        public Skeleton Skeleton { get { return _skeleton; } }
        public int RotationAngle { get { return _rotationAngle; } }
        public bool IsHorizontallyFlipped { get { return _isHorizontallyFlipped; } }
        public bool IsVerticallyFlipped { get { return _isVerticallyFlipped; } }

        public RectTransform RectTransform
        {
            get {
                if (_rectTransform == null)
                    _rectTransform = (RectTransform)transform;
                return _rectTransform;
            }
        }

        // - Handler -
        void OnEnable() {
            if (_skeleton != null)
                _skeletonSubscription = _skeleton.Updated.Subscribe(_ => RenderAngles());
            RebuildVisuals();
        }

        void OnDisable() {
            _skeletonSubscription?.Dispose();
            _skeletonSubscription = null;
            HideVisuals();
        }

        void OnDestroy() {
            _skeletonSubscription?.Dispose();
            _skeletonSubscription = null;
            foreach (AngleGraphic graphic in _graphics)
                DestroyGraphic(graphic);
            _graphics.Clear();
        }

        void OnRectTransformDimensionsChange() {
            if (_skeleton != null)
                RenderAngles();
        }

        void LateUpdate() {
            if (_refreshRequested)
                RebuildVisuals();
        }

#if UNITY_EDITOR
        void OnValidate() {
            _refreshRequested = true;
        }
#endif

        // - Private & Protected -
        [Title(Headers.Settings)]
        [SerializeField] SkeletonAngleOptions[] _angles = Array.Empty<SkeletonAngleOptions>();
        [SerializeField, Range(0f, 1f)] float _minimumVisibility = 0.5f;

        [SerializeField, HideInInspector] List<AngleGraphic> _graphics = new List<AngleGraphic>();

        Skeleton _skeleton;
        IDisposable _skeletonSubscription;
        RectTransform _rectTransform;
        int _rotationAngle;
        bool _isHorizontallyFlipped;
        bool _isVerticallyFlipped;
        bool _refreshRequested;

        void RenderAngles() {
            if (!isActiveAndEnabled || _skeleton == null || !_skeleton.IsDetected) {
                HideVisuals();
                return;
            }

            Rect rect = RectTransform.rect;
            for (int index = 0; index < _graphics.Count; index++) {
                AngleGraphic graphic = _graphics[index];
                if (graphic == null)
                    continue;
                if (index >= _angles.Length || _angles[index] == null) {
                    graphic.gameObject.SetActive(false);
                    continue;
                }

                SkeletonAngleOptions angle = _angles[index];
                bool hasFirst = _skeleton.TryGetLandmark(angle.FirstLandmark, out SkeletonLandmark first);
                bool hasVertex = _skeleton.TryGetLandmark(angle.VertexLandmark, out SkeletonLandmark vertex);
                bool hasLast = _skeleton.TryGetLandmark(angle.LastLandmark, out SkeletonLandmark last);
                bool visible = hasFirst && IsVisible(first) && hasVertex && IsVisible(vertex) && hasLast && IsVisible(last);
                if (visible) {
                    Vector2 vertexPosition = ToAnchoredPosition(vertex.Position, rect);
                    graphic.rectTransform.anchoredPosition = vertexPosition;
                    visible = graphic.SetAngle(ToAnchoredPosition(first.Position, rect), vertexPosition, ToAnchoredPosition(last.Position, rect));
                }
                graphic.gameObject.SetActive(visible);
            }
        }

        void HideVisuals() {
            foreach (AngleGraphic graphic in _graphics)
                if (graphic != null)
                    graphic.gameObject.SetActive(false);
        }

        void DestroyGraphic(AngleGraphic graphic) {
            if (graphic == null)
                return;
            graphic.gameObject.SetActive(false);
            if (Application.isPlaying)
                Destroy(graphic.gameObject);
            else
                DestroyImmediate(graphic.gameObject);
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
