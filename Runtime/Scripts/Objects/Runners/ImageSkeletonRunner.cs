using System;
using ParkMinPackages.MediaPipePlugin.Enums;
using ParkMinPackages.MediaPipePlugin.ScriptableObjects;
using UnityEngine;

namespace ParkMinPackages.MediaPipePlugin.Objects.Runners
{
	[Serializable]
	public sealed class ImageSkeletonRunner : SkeletonRunner
	{
		// - Construct -
		public ImageSkeletonRunner() {
		}

		public ImageSkeletonRunner(
			MediaPipeModelAsset model,
			float minPoseDetectionConfidence,
			float minPosePresenceConfidence,
			float minTrackingConfidence
		) : base(model, minPoseDetectionConfidence, minPosePresenceConfidence, minTrackingConfidence) {
		}

		// - Public Methods -
		public int Detect(Texture2D image) {
			return DetectImage(image);
		}

		// - Internals -
		protected override PoseLandmarkerRunningMode RunningMode
		{
			get { return PoseLandmarkerRunningMode.Image; }
		}
	}
}
