using System;
using ParkMinPackages.MediaPipePlugin.Enums;
using ParkMinPackages.MediaPipePlugin.Objects;

namespace ParkMinPackages.MediaPipePlugin.Extensions
{
	public static class PoseLandmarkerResultExtensions
	{
		// - Statics -
		public static Skeleton ToSkeleton(this PoseLandmarkerResult.Pose pose) {
			if (pose == null)
				throw new ArgumentNullException(nameof(pose));

			Skeleton skeleton = new Skeleton();
			skeleton.BeginUpdate(-1);
			int landmarkCount = Math.Min(33, pose.NormalizedLandmarks.Count);

			for (int index = 0; index < landmarkCount; index++) {
				PoseLandmarkerResult.Landmark source = pose.NormalizedLandmarks[index];
				skeleton.SetLandmark(new SkeletonLandmark((SkeletonLandmarkId)index, source.Position, source.HasVisibility, source.Visibility, source.HasPresence, source.Presence));
			}

			skeleton.CompleteUpdate();
			return skeleton;
		}
	}
}
