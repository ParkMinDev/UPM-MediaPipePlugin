using System;
using ParkMinDev.UPM.MediaPipePlugin.Enums;
using ParkMinDev.UPM.UGUI.Components.Graphics;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ParkMinDev.UPM.MediaPipePlugin.Objects
{
    [Serializable]
    public sealed class SkeletonAngleOptions
    {
        // - Public Properties -
        public SkeletonLandmarkId FirstLandmark { get { return _firstLandmark; } set { _firstLandmark = value; } }
        public SkeletonLandmarkId VertexLandmark { get { return _vertexLandmark; } set { _vertexLandmark = value; } }
        public SkeletonLandmarkId LastLandmark { get { return _lastLandmark; } set { _lastLandmark = value; } }
        public AngleGraphicOptions GraphicOptions { get { return _graphicOptions; } set { _graphicOptions = value ?? throw new ArgumentNullException(nameof(value)); } }

        // - Private & Protected -
        [Title("Landmarks")]
        [SerializeField] SkeletonLandmarkId _firstLandmark = SkeletonLandmarkId.LeftShoulder;
        [SerializeField] SkeletonLandmarkId _vertexLandmark = SkeletonLandmarkId.LeftElbow;
        [SerializeField] SkeletonLandmarkId _lastLandmark = SkeletonLandmarkId.LeftWrist;

        [Title("Graphic")]
        [SerializeField, InlineProperty, HideLabel] AngleGraphicOptions _graphicOptions = new AngleGraphicOptions();
    }
}
