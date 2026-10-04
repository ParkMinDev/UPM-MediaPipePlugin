namespace ParkMinDev.UPM.MediaPipePlugin.Objects
{
	public sealed class MediaPipeModelData
	{
		// - Construct -
		internal MediaPipeModelData(string filePath, byte[] buffer) {
			FilePath = filePath;
			Buffer = buffer;
		}

		// - Public Properties -
		public string FilePath { get; }
		public byte[] Buffer { get; }
		public bool IsBuffer
		{
			get { return Buffer != null; }
		}
	}
}
