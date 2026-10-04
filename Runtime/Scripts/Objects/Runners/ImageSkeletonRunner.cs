using System;
using ParkMinDev.UPM.MediaPipePlugin.Enums;
using ParkMinDev.UPM.MediaPipePlugin.ScriptableObjects;
using UnityEngine;

namespace ParkMinDev.UPM.MediaPipePlugin.Objects.Runners
{
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceNamespace: "ParkMinPackages.MediaPipePlugin.Objects.Runners", sourceAssembly: "ParkMinPackages.MediaPipePlugin", sourceClassName: "ImageSkeletonRunner")]
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
