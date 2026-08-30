using System;
using System.Collections.Generic;
using System.Linq;
using ParkMinPackages.MediaPipePlugin.Enums;

namespace ParkMinPackages.MediaPipePlugin.Objects
{
	public static class PoseSkeletonDefinition
	{
		// - Statics -
		public static IReadOnlyList<SkeletonLandmarkId> LandmarkIds { get; } = Enum.GetValues(typeof(SkeletonLandmarkId)).Cast<SkeletonLandmarkId>().ToArray();

		public static IReadOnlyList<SkeletonConnection> Connections { get; } = new SkeletonConnection[] {
			new SkeletonConnection(SkeletonLandmarkId.LeftEar, SkeletonLandmarkId.LeftEyeOuter),
			new SkeletonConnection(SkeletonLandmarkId.LeftEyeOuter, SkeletonLandmarkId.LeftEye),
			new SkeletonConnection(SkeletonLandmarkId.LeftEye, SkeletonLandmarkId.LeftEyeInner),
			new SkeletonConnection(SkeletonLandmarkId.LeftEyeInner, SkeletonLandmarkId.Nose),
			new SkeletonConnection(SkeletonLandmarkId.Nose, SkeletonLandmarkId.RightEyeInner),
			new SkeletonConnection(SkeletonLandmarkId.RightEyeInner, SkeletonLandmarkId.RightEye),
			new SkeletonConnection(SkeletonLandmarkId.RightEye, SkeletonLandmarkId.RightEyeOuter),
			new SkeletonConnection(SkeletonLandmarkId.RightEyeOuter, SkeletonLandmarkId.RightEar),
			new SkeletonConnection(SkeletonLandmarkId.LeftMouth, SkeletonLandmarkId.RightMouth),
			new SkeletonConnection(SkeletonLandmarkId.LeftShoulder, SkeletonLandmarkId.Neck),
			new SkeletonConnection(SkeletonLandmarkId.Neck, SkeletonLandmarkId.RightShoulder),
			new SkeletonConnection(SkeletonLandmarkId.Neck, SkeletonLandmarkId.Chest),
			new SkeletonConnection(SkeletonLandmarkId.Chest, SkeletonLandmarkId.HipCenter),
			new SkeletonConnection(SkeletonLandmarkId.LeftShoulder, SkeletonLandmarkId.LeftElbow),
			new SkeletonConnection(SkeletonLandmarkId.LeftElbow, SkeletonLandmarkId.LeftWrist),
			new SkeletonConnection(SkeletonLandmarkId.LeftWrist, SkeletonLandmarkId.LeftPinky),
			new SkeletonConnection(SkeletonLandmarkId.LeftWrist, SkeletonLandmarkId.LeftIndex),
			new SkeletonConnection(SkeletonLandmarkId.LeftWrist, SkeletonLandmarkId.LeftThumb),
			new SkeletonConnection(SkeletonLandmarkId.RightShoulder, SkeletonLandmarkId.RightElbow),
			new SkeletonConnection(SkeletonLandmarkId.RightElbow, SkeletonLandmarkId.RightWrist),
			new SkeletonConnection(SkeletonLandmarkId.RightWrist, SkeletonLandmarkId.RightPinky),
			new SkeletonConnection(SkeletonLandmarkId.RightWrist, SkeletonLandmarkId.RightIndex),
			new SkeletonConnection(SkeletonLandmarkId.RightWrist, SkeletonLandmarkId.RightThumb),
			new SkeletonConnection(SkeletonLandmarkId.HipCenter, SkeletonLandmarkId.LeftHip),
			new SkeletonConnection(SkeletonLandmarkId.LeftHip, SkeletonLandmarkId.LeftKnee),
			new SkeletonConnection(SkeletonLandmarkId.LeftKnee, SkeletonLandmarkId.LeftAnkle),
			new SkeletonConnection(SkeletonLandmarkId.LeftAnkle, SkeletonLandmarkId.LeftHeel),
			new SkeletonConnection(SkeletonLandmarkId.LeftHeel, SkeletonLandmarkId.LeftFootIndex),
			new SkeletonConnection(SkeletonLandmarkId.LeftFootIndex, SkeletonLandmarkId.LeftAnkle),
			new SkeletonConnection(SkeletonLandmarkId.HipCenter, SkeletonLandmarkId.RightHip),
			new SkeletonConnection(SkeletonLandmarkId.RightHip, SkeletonLandmarkId.RightKnee),
			new SkeletonConnection(SkeletonLandmarkId.RightKnee, SkeletonLandmarkId.RightAnkle),
			new SkeletonConnection(SkeletonLandmarkId.RightAnkle, SkeletonLandmarkId.RightHeel),
			new SkeletonConnection(SkeletonLandmarkId.RightHeel, SkeletonLandmarkId.RightFootIndex),
			new SkeletonConnection(SkeletonLandmarkId.RightFootIndex, SkeletonLandmarkId.RightAnkle)
		};
	}
}
