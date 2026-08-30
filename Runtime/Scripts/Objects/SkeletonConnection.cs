using System;
using ParkMinPackages.MediaPipePlugin.Enums;

namespace ParkMinPackages.MediaPipePlugin.Objects
{
	public readonly struct SkeletonConnection : IEquatable<SkeletonConnection>
	{
		// - Construct -
		public SkeletonConnection(SkeletonLandmarkId start, SkeletonLandmarkId end) {
			Start = start;
			End = end;
		}

		// - Public Methods -
		public bool Equals(SkeletonConnection other) {
			return Start == other.Start && End == other.End;
		}

		public override bool Equals(object obj) {
			return obj is SkeletonConnection other && Equals(other);
		}

		public override int GetHashCode() {
			return HashCode.Combine((int)Start, (int)End);
		}

		// - Public Properties -
		public SkeletonLandmarkId Start { get; }
		public SkeletonLandmarkId End { get; }
	}
}
