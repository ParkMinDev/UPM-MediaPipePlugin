using System;
using System.Collections;
using System.Collections.Generic;
using ParkMinPackages.MediaPipePlugin.Enums;
using R3;
using UnityEngine;

namespace ParkMinPackages.MediaPipePlugin.Objects
{
	public sealed class Skeleton : IDisposable
	{
		// - Class Struct Enum -
		sealed class LandmarkCollection : IReadOnlyList<SkeletonLandmark>
		{
			// - Construct -
			internal LandmarkCollection(Skeleton skeleton) {
				_skeleton = skeleton;
			}

			// - Public Methods -
			public IEnumerator<SkeletonLandmark> GetEnumerator() {
				for (int index = 0; index < Count; index++)
					yield return _skeleton._activeLandmarks[index];
			}

			IEnumerator IEnumerable.GetEnumerator() {
				return GetEnumerator();
			}

			// - Public Properties -
			public int Count
			{
				get { return _skeleton._activeLandmarkCount; }
			}

			public SkeletonLandmark this[int index]
			{
				get {
					if (index < 0 || index >= Count)
						throw new ArgumentOutOfRangeException(nameof(index));

					return _skeleton._activeLandmarks[index];
				}
			}

			// - Internals -
			readonly Skeleton _skeleton;
		}

		// - Statics -
		const int LandmarkCapacity = (int)SkeletonLandmarkId.HipCenter + 1;

		// - Construct -
		public Skeleton() {
			_landmarks = new LandmarkCollection(this);
		}

		public Skeleton(IEnumerable<SkeletonLandmark> landmarks) : this() {
			if (landmarks == null)
				throw new ArgumentNullException(nameof(landmarks));

			BeginUpdate(-1);

			foreach (SkeletonLandmark landmark in landmarks)
				SetLandmark(landmark);

			CompleteUpdate();
		}

		// - Public Methods -
		public bool TryGetLandmark(SkeletonLandmarkId id, out SkeletonLandmark landmark) {
			int index = (int)id;

			if (index < 0 || index >= LandmarkCapacity || !_hasLandmark[index]) {
				landmark = default;
				return false;
			}

			landmark = _landmarksById[index];
			return true;
		}

		public bool Contains(SkeletonLandmarkId id) {
			int index = (int)id;
			return index >= 0 && index < LandmarkCapacity && _hasLandmark[index];
		}

		public void Dispose() {
			_updated.OnCompleted();
			_updated.Dispose();
		}

		public Skeleton CreateSnapshot() {
			Skeleton snapshot = new Skeleton();
			Array.Copy(_landmarksById, snapshot._landmarksById, LandmarkCapacity);
			Array.Copy(_activeLandmarks, snapshot._activeLandmarks, _activeLandmarkCount);
			Array.Copy(_hasLandmark, snapshot._hasLandmark, LandmarkCapacity);
			snapshot._activeLandmarkCount = _activeLandmarkCount;
			snapshot.TimestampMilliseconds = TimestampMilliseconds;
			snapshot.IsDetected = IsDetected;
			snapshot.Version = Version;
			return snapshot;
		}

		// - Public Properties -
		public IReadOnlyList<SkeletonLandmark> Landmarks
		{
			get { return _landmarks; }
		}

		public IReadOnlyList<SkeletonConnection> Connections
		{
			get { return PoseSkeletonDefinition.Connections; }
		}

		public long TimestampMilliseconds { get; private set; } = -1;
		public bool IsDetected { get; private set; }
		public int Version { get; private set; }
		public Observable<Unit> Updated
		{
			get { return _updated; }
		}

		// - Internals -
		readonly SkeletonLandmark[] _landmarksById = new SkeletonLandmark[LandmarkCapacity];
		readonly SkeletonLandmark[] _activeLandmarks = new SkeletonLandmark[LandmarkCapacity];
		readonly bool[] _hasLandmark = new bool[LandmarkCapacity];
		readonly LandmarkCollection _landmarks;
		readonly Subject<Unit> _updated = new Subject<Unit>();
		int _activeLandmarkCount;

		internal void BeginUpdate(long timestampMilliseconds) {
			Array.Clear(_hasLandmark, 0, _hasLandmark.Length);
			_activeLandmarkCount = 0;
			TimestampMilliseconds = timestampMilliseconds;
			IsDetected = false;
		}

		internal void SetLandmark(SkeletonLandmark landmark) {
			int index = (int)landmark.Id;

			if (index < 0 || index >= LandmarkCapacity)
				throw new ArgumentOutOfRangeException(nameof(landmark));

			_landmarksById[index] = landmark;
			_hasLandmark[index] = true;
		}

		internal void CompleteUpdate() {
			AddDerivedLandmarks();

			for (int index = 0; index < LandmarkCapacity; index++)
				if (_hasLandmark[index])
					_activeLandmarks[_activeLandmarkCount++] = _landmarksById[index];

			IsDetected = _activeLandmarkCount > 0;
			Version++;
			_updated.OnNext(Unit.Default);
		}

		internal void Clear() {
			BeginUpdate(-1);
			CompleteUpdate();
		}

		void AddDerivedLandmarks() {
			if (TryGetLandmark(SkeletonLandmarkId.LeftShoulder, out SkeletonLandmark leftShoulder) && TryGetLandmark(SkeletonLandmarkId.RightShoulder, out SkeletonLandmark rightShoulder))
				SetLandmark(CreateMidpoint(SkeletonLandmarkId.Neck, leftShoulder, rightShoulder));

			if (TryGetLandmark(SkeletonLandmarkId.LeftHip, out SkeletonLandmark leftHip) && TryGetLandmark(SkeletonLandmarkId.RightHip, out SkeletonLandmark rightHip))
				SetLandmark(CreateMidpoint(SkeletonLandmarkId.HipCenter, leftHip, rightHip));

			if (TryGetLandmark(SkeletonLandmarkId.Neck, out SkeletonLandmark neck) && TryGetLandmark(SkeletonLandmarkId.HipCenter, out SkeletonLandmark hipCenter))
				SetLandmark(CreateMidpoint(SkeletonLandmarkId.Chest, neck, hipCenter));
		}

		SkeletonLandmark CreateMidpoint(SkeletonLandmarkId id, SkeletonLandmark first, SkeletonLandmark second) {
			return new SkeletonLandmark(
				id,
				(first.Position + second.Position) * 0.5f,
				first.HasVisibility && second.HasVisibility,
				Mathf.Min(first.Visibility, second.Visibility),
				first.HasPresence && second.HasPresence,
				Mathf.Min(first.Presence, second.Presence)
			);
		}
	}
}
