using DiskDetector;
using DiskDetector.Models;

using Xunit;
using Xunit.Abstractions;

namespace Tests
{
    /// <summary>
    ///     Class to test the DiskDetector.
    ///     Note: As every device has a different hardware specification,
    ///     it`s currently hard to write usable tests.
    /// </summary>
    public class DiskDetectorTests
    {
        private readonly ITestOutputHelper output;

        // As recommended by https://xunit.net/docs/capturing-output
        public DiskDetectorTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData("C")]
        [InlineData("F")]
        public void DetectFixedDriveTest(string driveName)
        {
            var detectedDrive = Detector.DetectFixedDrive(driveName, QueryType.RotationRate, true);
            output.WriteLine("Drive {0}", detectedDrive.Name);
            output.WriteLine("  File type: {0}", detectedDrive.DriveType);
            output.WriteLine("  Volume label: {0}", detectedDrive.VolumeLabel);
            output.WriteLine("  UNC Path: {0}", detectedDrive.UncPath);
            output.WriteLine("  File system: {0}", detectedDrive.DriveFormat);
            output.WriteLine("  Letter: {0}", detectedDrive.DriveLetter);
            output.WriteLine("  HardwareType: {0}", detectedDrive.HardwareType);
            output.WriteLine("  Id: {0}", detectedDrive.Id);
            output.WriteLine(
                "  Available space to current user:{0, 15} bytes",
                detectedDrive.AvailableFreeSpace);

            output.WriteLine(
                "  Total available space:          {0, 15} bytes",
                detectedDrive.TotalFreeSpace);

            output.WriteLine(
                "  Total size of drive:            {0, 15} bytes ",
                detectedDrive.TotalSize);

            Assert.NotNull(detectedDrive);
        }


        [Fact]
        public void DetectFixedDrivesTest()
        {
            var detectedDrives = Detector.DetectFixedDrives(QueryType.RotationRate, true);
            if (detectedDrives.Count != 0)
            {
                foreach (var detectedDrive in detectedDrives)
                {
                    output.WriteLine("Drive {0}", detectedDrive.Name);
                    output.WriteLine("  File type: {0}", detectedDrive.DriveType);
                    output.WriteLine("  Volume label: {0}", detectedDrive.VolumeLabel);
                    output.WriteLine("  UNC Path: {0}", detectedDrive.UncPath);
                    output.WriteLine("  File system: {0}", detectedDrive.DriveFormat);
                    output.WriteLine("  Letter: {0}", detectedDrive.DriveLetter);
                    output.WriteLine("  HardwareType: {0}", detectedDrive.HardwareType);
                    output.WriteLine("  Id: {0}", detectedDrive.Id);
                    output.WriteLine(
                        "  Available space to current user:{0, 15} bytes",
                        detectedDrive.AvailableFreeSpace);

                    output.WriteLine(
                        "  Total available space:          {0, 15} bytes",
                        detectedDrive.TotalFreeSpace);

                    output.WriteLine(
                        "  Total size of drive:            {0, 15} bytes ",
                        detectedDrive.TotalSize);
                }
            }
            Assert.True(detectedDrives.Count > 0);
            /*
            Drive C:\
              File type: Fixed
              Volume label: Windows
              File system: NTFS
              Letter: C
              HardwareType: Ssd
              Id: 0
              Available space to current user:    23861460992 bytes
              Total available space:              23861460992 bytes
              Total size of drive:               255505461248 bytes 
            Drive F:\
              File type: Fixed
              Volume label: Data
              File system: NTFS
              Letter: F
              HardwareType: Hdd
              Id: 1
              Available space to current user:  1250781491200 bytes
              Total available space:            1250781491200 bytes
              Total size of drive:              2000397791232 bytes 
             Drive Z:\
              File type: Network
              Volume label: hubiC
              UNC Path: \\ExpanDrive\hubiC
              File system: EXFS
              Letter: Z
              HardwareType: Unknown
              Id: -1
              Available space to current user:    50000000000 bytes
              Total available space:              50000000000 bytes
              Total size of drive:               100000000000 bytes 
            */
        }

        [Fact]
        public void DetectDriveTest()
        {
            var detectedDrive = Detector.DetectDrive("Z", QueryType.RotationRate, true);
            output.WriteLine("Drive {0}", detectedDrive.Name);
            output.WriteLine("  File type: {0}", detectedDrive.DriveType);
            output.WriteLine("  Volume label: {0}", detectedDrive.VolumeLabel);
            output.WriteLine("  UNC Path: {0}", detectedDrive.UncPath);
            output.WriteLine("  File system: {0}", detectedDrive.DriveFormat);
            output.WriteLine("  Letter: {0}", detectedDrive.DriveLetter);
            output.WriteLine("  HardwareType: {0}", detectedDrive.HardwareType);
            output.WriteLine("  Id: {0}", detectedDrive.Id);
            output.WriteLine(
                "  Available space to current user:{0, 15} bytes",
                detectedDrive.AvailableFreeSpace);

            output.WriteLine(
                "  Total available space:          {0, 15} bytes",
                detectedDrive.TotalFreeSpace);

            output.WriteLine(
                "  Total size of drive:            {0, 15} bytes ",
                detectedDrive.TotalSize);

            Assert.NotNull(detectedDrive);
        }

        [Fact]
        public void DetectDrivesTest()
        {
            var detectedDrives = Detector.DetectDrives(QueryType.RotationRate, true);
            if (detectedDrives.Count != 0)
            {
                foreach (var detectedDrive in detectedDrives)
                {
                    output.WriteLine("Drive {0}", detectedDrive.Name);
                    output.WriteLine("  File type: {0}", detectedDrive.DriveType);
                    output.WriteLine("  Volume label: {0}", detectedDrive.VolumeLabel);
                    output.WriteLine("  UNC Path: {0}", detectedDrive.UncPath);
                    output.WriteLine("  File system: {0}", detectedDrive.DriveFormat);
                    output.WriteLine("  Letter: {0}", detectedDrive.DriveLetter);
                    output.WriteLine("  HardwareType: {0}", detectedDrive.HardwareType);
                    output.WriteLine("  Id: {0}", detectedDrive.Id);
                    output.WriteLine(
                        "  Available space to current user:{0, 15} bytes",
                        detectedDrive.AvailableFreeSpace);

                    output.WriteLine(
                        "  Total available space:          {0, 15} bytes",
                        detectedDrive.TotalFreeSpace);

                    output.WriteLine(
                        "  Total size of drive:            {0, 15} bytes ",
                        detectedDrive.TotalSize);
                }
            }
            Assert.True(detectedDrives.Count > 0);
        }

        [Fact]
        public void DetectHardwareTypeByRotationRate1Test()
        {
            // Note: Test requires administrator privileges.
            var ssd = Detector.DetectHardwareTypeByRotationRate(0);
            var hdd = Detector.DetectHardwareTypeByRotationRate(1);
            Assert.Equal(HardwareType.Ssd, ssd);
            Assert.Equal(HardwareType.Hdd, hdd);
        }

        [Fact]
        public void DetectHardwareTypeByRotationRate2Test()
        {
            var driveLetterStringSsd = "C";
            char driveLetterSsd = driveLetterStringSsd[0];

            var driveLetterStringHdd = "E";
            char driveLetterHdd = driveLetterStringHdd[0];
            // Note: Test requires administrator privileges.
            var ssd = Detector.DetectHardwareTypeByRotationRate(driveLetterSsd);
            var hdd = Detector.DetectHardwareTypeByRotationRate(driveLetterHdd);
            Assert.Equal(HardwareType.Ssd, ssd);
            Assert.Equal(HardwareType.Hdd, hdd);
        }

        [Fact]
        public void DetectHardwareTypeBySeekPenalty1Test()
        {
            var ssd = Detector.DetectHardwareTypeBySeekPenalty(0);
            var hdd = Detector.DetectHardwareTypeBySeekPenalty(1);
            Assert.Equal(HardwareType.Ssd, ssd);
            Assert.Equal(HardwareType.Hdd, hdd);
        }

        [Fact]
        public void DetectHardwareTypeBySeekPenalty2Test()
        {
            var driveLetterStringSsd = "C";
            char driveLetterSsd = driveLetterStringSsd[0];

            var driveLetterStringHdd = "E";
            char driveLetterHdd = driveLetterStringHdd[0];
            var ssd = Detector.DetectHardwareTypeBySeekPenalty(driveLetterSsd);
            var hdd = Detector.DetectHardwareTypeBySeekPenalty(driveLetterHdd);
            Assert.Equal(HardwareType.Ssd, ssd);
            Assert.Equal(HardwareType.Hdd, hdd);
        }

        [Fact]
        public void IsAdministratorTest()
        {
            var isAdministrator = Detector.IsAdministrator();
            Assert.True(isAdministrator);
        }
    }
}
