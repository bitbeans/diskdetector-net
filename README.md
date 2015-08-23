#diskdetector-net [![NuGet Version](https://img.shields.io/nuget/v/diskdetector-net.svg?style=flat-square)](https://www.nuget.org/packages/diskdetector-net/) [![License](http://img.shields.io/badge/license-MIT-green.svg?style=flat-square)](https://github.com/bitbeans/diskdetector-net/blob/master/LICENSE.md)


Class to detect the hardware type (SSD or HDD) of a hard disk on windows based systems.

This implementation is based on [emoacht.wordpress.com](https://emoacht.wordpress.com/2012/11/06/csharp-ssd/).



## Installation

There is a [NuGet package](https://www.nuget.org/packages/diskdetector-net/) available.

## Example

```csharp
public void DetectFixedDrivesTest()
{
    var detectedDrives = Detector.DetectFixedDrives(QueryType.SeekPenalty);
    if (detectedDrives.Count != 0)
    {
        foreach (var detectedDrive in detectedDrives)
        {
            Console.WriteLine("Drive {0}", detectedDrive.Name);
            Console.WriteLine("  File type: {0}", detectedDrive.DriveType);

            Console.WriteLine("  Volume label: {0}", detectedDrive.VolumeLabel);
            Console.WriteLine("  File system: {0}", detectedDrive.DriveFormat);
            Console.WriteLine("  Letter: {0}", detectedDrive.DriveLetter);
            Console.WriteLine("  HardwareType: {0}", detectedDrive.HardwareType);
            Console.WriteLine("  Id: {0}", detectedDrive.Id);
            Console.WriteLine(
                "  Available space to current user:{0, 15} bytes",
                detectedDrive.AvailableFreeSpace);

            Console.WriteLine(
                "  Total available space:          {0, 15} bytes",
                detectedDrive.TotalFreeSpace);

            Console.WriteLine(
                "  Total size of drive:            {0, 15} bytes ",
                detectedDrive.TotalSize);
        }
    }

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
    */

```

## License
[MIT](https://en.wikipedia.org/wiki/MIT_License)