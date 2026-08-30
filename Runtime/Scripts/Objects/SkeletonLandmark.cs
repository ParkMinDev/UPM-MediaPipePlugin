using ParkMinPackages.MediaPipePlugin.Enums;
using UnityEngine;

namespace ParkMinPackages.MediaPipePlugin.Objects
{
	public readonly struct SkeletonLandmark
	{
		// - Construct -
		public SkeletonLandmark(SkeletonLandmarkId id, Vector3 position, bool hasVisibility, float visibility, bool hasPresence, float presence) {
			Id = id;
			Position = position;
			HasVisibility = hasVisibility;
			Visibility = visibility;
			HasPresence = hasPresence;
			Presence = presence;
		}

		// - Public Properties -
		public SkeletonLandmarkId Id { get; }
		public Vector3 Position { get; }
		public bool HasVisibility { get; }
		public float Visibility { get; }
		public bool HasPresence { get; }
		public float Presence { get; }
	}
}
