using System;
using UnityEngine;

namespace ParkMinPackages.MediaPipePlugin.Objects.Selectors
{
	public abstract class SkeletonSelector
	{
		// - Construct -
		protected SkeletonSelector(float minimumVisibility = 0.5f) {
			MinimumVisibility = minimumVisibility;
		}

		// - Public Methods -
		public Skeleton Select(SkeletonCollection skeletons) {
			if (skeletons == null)
				throw new ArgumentNullException(nameof(skeletons));

			Skeleton selectedSkeleton = null;
			float selectedScore = float.NegativeInfinity;

			foreach (Skeleton skeleton in skeletons) {
				if (!skeleton.IsDetected || !TryGetBounds(skeleton, out Rect bounds))
					continue;

				float score = GetScore(skeleton, bounds);

				if (float.IsNaN(score) || selectedSkeleton != null && score <= selectedScore)
					continue;

				selectedSkeleton = skeleton;
				selectedScore = score;
			}

			return selectedSkeleton;
		}

		// - Public Properties -
		public float MinimumVisibility
		{
			get { return _minimumVisibility; }
			set { _minimumVisibility = Mathf.Clamp01(value); }
		}

		// - Internals -
		float _minimumVisibility;

		protected abstract float GetScore(Skeleton skeleton, Rect bounds);

		protected bool TryGetBounds(Skeleton skeleton, out Rect bounds) {
			float minimumX = float.PositiveInfinity;
			float minimumY = float.PositiveInfinity;
			float maximumX = float.NegativeInfinity;
			float maximumY = float.NegativeInfinity;

			foreach (SkeletonLandmark landmark in skeleton.Landmarks) {
				if (!IsLandmarkAvailable(landmark))
					continue;

				Vector3 position = landmark.Position;

				if (float.IsNaN(position.x) || float.IsNaN(position.y) || float.IsInfinity(position.x) || float.IsInfinity(position.y))
					continue;

				float x = Mathf.Clamp01(position.x);
				float y = Mathf.Clamp01(position.y);
				minimumX = Mathf.Min(minimumX, x);
				minimumY = Mathf.Min(minimumY, y);
				maximumX = Mathf.Max(maximumX, x);
				maximumY = Mathf.Max(maximumY, y);
			}

			if (float.IsPositiveInfinity(minimumX)) {
				bounds = default;
				return false;
			}

			bounds = Rect.MinMaxRect(minimumX, minimumY, maximumX, maximumY);
			return true;
		}

		protected bool IsLandmarkAvailable(SkeletonLandmark landmark) {
			return (!landmark.HasVisibility || landmark.Visibility >= _minimumVisibility) && (!landmark.HasPresence || landmark.Presence >= _minimumVisibility);
		}
	}
}
