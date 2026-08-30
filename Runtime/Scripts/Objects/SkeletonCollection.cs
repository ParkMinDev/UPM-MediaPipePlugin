using System;
using System.Collections;
using System.Collections.Generic;

namespace ParkMinPackages.MediaPipePlugin.Objects
{
	public sealed class SkeletonCollection : IReadOnlyList<Skeleton>, IDisposable
	{
		// - Construct -
		public SkeletonCollection(int length) {
			if (length < 1)
				throw new ArgumentOutOfRangeException(nameof(length));

			_skeletons = new Skeleton[length];

			for (int index = 0; index < length; index++)
				_skeletons[index] = new Skeleton();
		}

		// - Public Methods -
		public IEnumerator<Skeleton> GetEnumerator() {
			return ((IEnumerable<Skeleton>)_skeletons).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public void Dispose() {
			if (_isDisposed)
				return;

			_isDisposed = true;

			foreach (Skeleton skeleton in _skeletons)
				skeleton.Dispose();
		}

		// - Public Properties -
		public int Length
		{
			get { return _skeletons.Length; }
		}

		public int Count
		{
			get { return _skeletons.Length; }
		}

		public Skeleton this[int index]
		{
			get { return _skeletons[index]; }
		}

		// - Internals -
		readonly Skeleton[] _skeletons;
		bool _isDisposed;
	}
}
