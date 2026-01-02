namespace FileMonitoring
{
    /// <summary>
    /// A simple data class to store metadata for a file.
    /// </summary>
    public class FileMetadata
    {
        public string Name { get; set; } = "";
        public string FullPath { get; set; } = "";
        public string? SizeInBytes { get; set; }
        public string? LastModifiedUtc { get; set; }
    }
}
