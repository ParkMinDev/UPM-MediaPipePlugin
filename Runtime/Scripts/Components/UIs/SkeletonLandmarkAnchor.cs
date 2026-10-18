using System;
using ParkMinDev.UPM.Foundation.Constants;
using ParkMinDev.UPM.MediaPipePlugin.Enums;
using ParkMinDev.UPM.MediaPipePlugin.Objects;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ParkMinDev.UPM.MediaPipePlugin.Components.UIs
{
    [ExecuteAlways, DisallowMultipleComponent]
    public sealed class SkeletonLandmarkAnchor : MonoBehaviour
    {
        // - Public Methods -
        public void SetSkeleton(Skeleton skeleton) {
            _subscription?.Dispose();
            _subscription = null;
            _skeleton = skeleton;
            if (isActiveAndEnabled && skeleton != null)
                _subscription = skeleton.Updated.Subscribe(_ => Refresh());
            Refresh();
        }

        public void Clear() {
            SetSkeleton(null);
        }

        public void SetDisplayTransform(int rotationAngle, bool horizontallyFlipped, bool verticallyFlipped) {
            _rotationAngle = ((rotationAngle % 360) + 360) % 360;
            _horizontallyFlipped = horizontallyFlipped;
            _verticallyFlipped = verticallyFlipped;
            Refresh();
        }

        public void Refresh() {
            IsTracked = isActiveAndEnabled && _coordinateSpace != null && _target != null && _skeleton != null && _skeleton.IsDetected;
            if (!IsTracked)
                return;

            IsTracked = _skeleton.TryGetLandmark(_landmark, out SkeletonLandmark landmark) && (!landmark.HasVisibility || landmark.Visibility >= _minimumVisibility) && (!landmark.HasPresence || landmark.Presence >= _minimumVisibility);
            if (!IsTracked)
                return;

            // Convert the displayed landmark into the target parent's coordinate space
            Rect rect = _coordinateSpace.rect;
            float x = (landmark.Position.x - 0.5f) * rect.width * (_horizontallyFlipped ? -1f : 1f);
            float y = (0.5f - landmark.Position.y) * rect.height * (_verticallyFlipped ? -1f : 1f);
            float radians = -_rotationAngle * Mathf.Deg2Rad;
            Vector2 point = rect.center + new Vector2(x * Mathf.Cos(radians) - y * Mathf.Sin(radians), x * Mathf.Sin(radians) + y * Mathf.Cos(radians)) + _offset;
            _target.position = _coordinateSpace.TransformPoint(point);
        }

        // - Public Properties -
        public bool IsTracked { get; private set; }

        // - Handler -
        void OnEnable() {
            if (_skeleton != null)
                _subscription = _skeleton.Updated.Subscribe(_ => Refresh());
            Refresh();
        }

        void OnDisable() {
            _subscription?.Dispose();
            _subscription = null;
            IsTracked = false;
        }

        void OnDestroy() {
            _subscription?.Dispose();
        }

        void LateUpdate() {
            Refresh();
        }

        // - Private & Protected -
        [Title(Headers.Required)]
        [SerializeField, Required] RectTransform _coordinateSpace;
        [SerializeField, Required] RectTransform _target;

        [Title(Headers.Settings)]
        [SerializeField] SkeletonLandmarkId _landmark = SkeletonLandmarkId.LeftElbow;
        [SerializeField, Tooltip("Offset in the display coordinate space, after rotation and mirroring.")] Vector2 _offset = new Vector2(100f, 0f);
        [SerializeField, Range(0f, 1f)] float _minimumVisibility = 0.5f;

        Skeleton _skeleton;
        IDisposable _subscription;
        int _rotationAngle;
        bool _horizontallyFlipped;
        bool _verticallyFlipped;
    }
}
