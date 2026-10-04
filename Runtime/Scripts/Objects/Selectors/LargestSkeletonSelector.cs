using UnityEngine;

namespace ParkMinDev.UPM.MediaPipePlugin.Objects.Selectors
{
	public sealed class LargestSkeletonSelector : SkeletonSelector
	{
		// - Construct -
		public LargestSkeletonSelector(float minimumVisibility = 0.5f) : base(minimumVisibility) {
		}

		// - Internals -
		protected override float GetScore(Skeleton skeleton, Rect bounds) {
			return bounds.width * bounds.height;
		}
	}
}
