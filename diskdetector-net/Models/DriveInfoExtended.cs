using System.IO;

namespace DiskDetector.Models
{
    public class DriveInfoExtended
    {
        public string Name { get; set; }
        public char DriveLetter { get; set; }
        public DriveType DriveType { get; set; }
        public int Id { get; set; }
        public string VolumeLabel { get; set; }
        public string DriveFormat { get; set; }
        public long TotalFreeSpace { get; set; }
        public long TotalSize { get; set; }
        public long AvailableFreeSpace { get; set; }
        public HardwareType HardwareType { get; set; }
        public DirectoryInfo RootDirectory { get; set; }

    }
}