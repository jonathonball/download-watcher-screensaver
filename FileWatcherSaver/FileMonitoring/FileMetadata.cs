using System;

namespace FileMonitoring
{
    /// <summary>
    /// A simple data class to store metadata for a file.
    /// </summary>
    public class FileMetadata
    {
        public string Name { get; set; } = "";
        public string FullPath { get; set; } = "";
        public long SizeInBytes { get; set; }
        public DateTime LastModifiedUtc { get; set; }
    }
}
