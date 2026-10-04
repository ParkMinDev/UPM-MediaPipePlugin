using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ParkMinDev.UPM.MediaPipePlugin.Enums;
using ParkMinDev.UPM.MediaPipePlugin.Native;
using UnityEngine;

namespace ParkMinDev.UPM.MediaPipePlugin.Objects
{
	public sealed class PoseLandmarkerResult
	{
		// - Class Struct Enum -
		[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceNamespace: "ParkMinPackages.MediaPipePlugin.Objects", sourceAssembly: "ParkMinPackages.MediaPipePlugin", sourceClassName: "PoseLandmarkerResult+Landmark")]
		[Serializable]
		public readonly struct Landmark
		{
			// - Construct -
			internal Landmark(Vector3 position, bool hasVisibility, float visibility, bool hasPresence, float presence, string name) {
				Position = position;
				HasVisibility = hasVisibility;
				Visibility = visibility;
				HasPresence = hasPresence;
				Presence = presence;
				Name = name;
			}

			// - Public Properties -
			public Vector3 Position { get; }
			public bool HasVisibility { get; }
			public float Visibility { get; }
			public bool HasPresence { get; }
			public float Presence { get; }
			public string Name { get; }
		}

		public sealed class Pose
		{
			// - Construct -
			internal Pose(IReadOnlyList<Landmark> normalizedLandmarks, IReadOnlyList<Landmark> worldLandmarks) {
				NormalizedLandmarks = normalizedLandmarks;
				WorldLandmarks = worldLandmarks;
			}

			// - Public Properties -
			public IReadOnlyList<Landmark> NormalizedLandmarks { get; }
			public IReadOnlyList<Landmark> WorldLandmarks { get; }
		}

		// - Construct -
		internal PoseLandmarkerResult(IReadOnlyList<Pose> poses) {
			Poses = poses;
		}

		// - Public Properties -
		public IReadOnlyList<Pose> Poses { get; }

		// - Internals -
		internal static PoseLandmarkerResult Create(MpPoseLandmarkerResult result) {
			int poseCount = Math.Max(checked((int)result.PoseLandmarksCount), checked((int)result.PoseWorldLandmarksCount));
			Pose[] poses = new Pose[poseCount];
			int normalizedCollectionSize = Marshal.SizeOf<MpNormalizedLandmarks>();
			int worldCollectionSize = Marshal.SizeOf<MpLandmarks>();

			for (int poseIndex = 0; poseIndex < poseCount; poseIndex++) {
				IReadOnlyList<Landmark> normalizedLandmarks = Array.Empty<Landmark>();
				IReadOnlyList<Landmark> worldLandmarks = Array.Empty<Landmark>();

				if (poseIndex < result.PoseLandmarksCount) {
					IntPtr collectionAddress = IntPtr.Add(result.PoseLandmarks, poseIndex * normalizedCollectionSize);
					MpNormalizedLandmarks collection = Marshal.PtrToStructure<MpNormalizedLandmarks>(collectionAddress);
					normalizedLandmarks = CreateNormalizedLandmarks(collection);
				}

				if (poseIndex < result.PoseWorldLandmarksCount) {
					IntPtr collectionAddress = IntPtr.Add(result.PoseWorldLandmarks, poseIndex * worldCollectionSize);
					MpLandmarks collection = Marshal.PtrToStructure<MpLandmarks>(collectionAddress);
					worldLandmarks = CreateWorldLandmarks(collection);
				}

				poses[poseIndex] = new Pose(normalizedLandmarks, worldLandmarks);
			}

			return new PoseLandmarkerResult(poses);
		}

		internal static bool CopyNormalizedPoseToSkeleton(MpPoseLandmarkerResult result, int poseIndex, long timestampMilliseconds, Skeleton destination) {
			if (destination == null)
				throw new ArgumentNullException(nameof(destination));

			destination.BeginUpdate(timestampMilliseconds);

			if (poseIndex < 0 || poseIndex >= result.PoseLandmarksCount) {
				destination.CompleteUpdate();
				return false;
			}

			int collectionSize = Marshal.SizeOf<MpNormalizedLandmarks>();
			IntPtr collectionAddress = IntPtr.Add(result.PoseLandmarks, poseIndex * collectionSize);
			MpNormalizedLandmarks collection = Marshal.PtrToStructure<MpNormalizedLandmarks>(collectionAddress);
			int landmarkSize = Marshal.SizeOf<MpNormalizedLandmark>();
			int landmarkCount = Math.Min(33, checked((int)collection.LandmarksCount));

			for (int index = 0; index < landmarkCount; index++) {
				IntPtr landmarkAddress = IntPtr.Add(collection.Landmarks, index * landmarkSize);
				MpNormalizedLandmark landmark = Marshal.PtrToStructure<MpNormalizedLandmark>(landmarkAddress);
				destination.SetLandmark(new SkeletonLandmark((SkeletonLandmarkId)index, new Vector3(landmark.X, landmark.Y, landmark.Z), landmark.HasVisibility, landmark.Visibility, landmark.HasPresence, landmark.Presence));
			}

			destination.CompleteUpdate();
			return destination.IsDetected;
		}

		internal static int CopyNormalizedPosesToSkeletons(MpPoseLandmarkerResult result, long timestampMilliseconds, SkeletonCollection destination) {
			if (destination == null)
				throw new ArgumentNullException(nameof(destination));

			int detectedCount = Math.Min(checked((int)result.PoseLandmarksCount), destination.Length);

			for (int index = 0; index < detectedCount; index++)
				CopyNormalizedPoseToSkeleton(result, index, timestampMilliseconds, destination[index]);

			for (int index = detectedCount; index < destination.Length; index++)
				if (destination[index].IsDetected)
					destination[index].Clear();

			return detectedCount;
		}

		static IReadOnlyList<Landmark> CreateNormalizedLandmarks(MpNormalizedLandmarks collection) {
			Landmark[] landmarks = new Landmark[checked((int)collection.LandmarksCount)];
			int landmarkSize = Marshal.SizeOf<MpNormalizedLandmark>();

			for (int index = 0; index < landmarks.Length; index++) {
				IntPtr landmarkAddress = IntPtr.Add(collection.Landmarks, index * landmarkSize);
				MpNormalizedLandmark landmark = Marshal.PtrToStructure<MpNormalizedLandmark>(landmarkAddress);
				landmarks[index] = new Landmark(new Vector3(landmark.X, landmark.Y, landmark.Z), landmark.HasVisibility, landmark.Visibility, landmark.HasPresence, landmark.Presence, landmark.Name == IntPtr.Zero ? null : Marshal.PtrToStringUTF8(landmark.Name));
			}

			return landmarks;
		}

		static IReadOnlyList<Landmark> CreateWorldLandmarks(MpLandmarks collection) {
			Landmark[] landmarks = new Landmark[checked((int)collection.LandmarksCount)];
			int landmarkSize = Marshal.SizeOf<MpLandmark>();

			for (int index = 0; index < landmarks.Length; index++) {
				IntPtr landmarkAddress = IntPtr.Add(collection.Landmarks, index * landmarkSize);
				MpLandmark landmark = Marshal.PtrToStructure<MpLandmark>(landmarkAddress);
				landmarks[index] = new Landmark(new Vector3(landmark.X, landmark.Y, landmark.Z), landmark.HasVisibility, landmark.Visibility, landmark.HasPresence, landmark.Presence, landmark.Name == IntPtr.Zero ? null : Marshal.PtrToStringUTF8(landmark.Name));
			}

			return landmarks;
		}
	}
}
