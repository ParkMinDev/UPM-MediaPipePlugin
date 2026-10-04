using UnityEngine;

namespace ParkMinDev.UPM.MediaPipePlugin.Objects.Selectors
{
	public sealed class ClosestToCenterSkeletonSelector : SkeletonSelector
	{
		// - Construct -
		public ClosestToCenterSkeletonSelector(float minimumVisibility = 0.5f) : this(new Vector2(0.5f, 0.5f), minimumVisibility) {
		}

		public ClosestToCenterSkeletonSelector(Vector2 screenCenter, float minimumVisibility = 0.5f) : base(minimumVisibility) {
			ScreenCenter = screenCenter;
		}

		// - Public Properties -
		public Vector2 ScreenCenter { get; set; }

		// - Internals -
		protected override float GetScore(Skeleton skeleton, Rect bounds) {
			return -(bounds.center - ScreenCenter).sqrMagnitude;
		}
	}
}
