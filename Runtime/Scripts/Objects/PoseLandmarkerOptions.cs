using System;
using ParkMinPackages.MediaPipePlugin.Enums;

namespace ParkMinPackages.MediaPipePlugin.Objects
{
	public sealed class PoseLandmarkerOptions
	{
		// - Construct -
		public PoseLandmarkerOptions(string modelPath) {
			if (string.IsNullOrWhiteSpace(modelPath))
				throw new ArgumentException("A pose landmarker model path is required.", nameof(modelPath));

			ModelPath = modelPath;
		}

		public PoseLandmarkerOptions(byte[] modelBuffer) {
			if (modelBuffer == null || modelBuffer.Length == 0)
				throw new ArgumentException("A pose landmarker model buffer is required.", nameof(modelBuffer));

			ModelBuffer = modelBuffer;
		}

		// - Public Properties -
		public string ModelPath { get; }
		public byte[] ModelBuffer { get; }
		public PoseLandmarkerRunningMode RunningMode { get; set; } = PoseLandmarkerRunningMode.Image;
		public int NumPoses { get; set; } = 1;
		public float MinPoseDetectionConfidence { get; set; } = 0.5f;
		public float MinPosePresenceConfidence { get; set; } = 0.5f;
		public float MinTrackingConfidence { get; set; } = 0.5f;
	}
}
