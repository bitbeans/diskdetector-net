using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using DiskDetector.Exceptions;
using DiskDetector.Models;
using Microsoft.Win32.SafeHandles;

namespace DiskDetector
{
    /// <summary>
    ///     Class to detect the hardware type of a disk.
    /// </summary>
    /// <see cref="https://emoacht.wordpress.com/2012/11/06/csharp-ssd/" />
    public static class Detector
    {
        #region DeviceIoControl (nominal media rotation rate)

        private const uint AtaFlagsDataIn = 0x02;

        #endregion

        /// <summary>
        ///     Check if the application is running as administrator.
        /// </summary>
        /// <returns><c>true</c> if the application is running as administrator otherwise, <c>false</c></returns>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            if (identity == null) return false;
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        ///     Detect a fixed drive by letter.
        /// </summary>
        /// <param name="driveName">A valid drive letter.</param>
        /// <param name="queryType">The QueryType.</param>
        /// <param name="useFallbackQuery">Use QueryType.SeekPenalty as fallback.</param>
        /// <returns>A list of DriveInfoExtended.</returns>
        /// <exception cref="IOException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="DriveNotFoundException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="DetectionFailedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static DriveInfoExtended DetectFixedDrive(string driveName, QueryType queryType = QueryType.SeekPenalty,
            bool useFallbackQuery = true)
        {
            var driveInfoExtended = new DriveInfoExtended();
            var logicalDrive = new DriveInfo(driveName);
            if (logicalDrive.DriveType == DriveType.Fixed)
            {
                if (logicalDrive.IsReady)
                {
                    var tmp = new DriveInfoExtended
                    {
                        DriveFormat = logicalDrive.DriveFormat,
                        VolumeLabel = logicalDrive.VolumeLabel,
                        Name = logicalDrive.Name,
                        DriveType = logicalDrive.DriveType,
                        AvailableFreeSpace = logicalDrive.AvailableFreeSpace,
                        TotalSize = logicalDrive.TotalSize,
                        TotalFreeSpace = logicalDrive.TotalFreeSpace,
                        RootDirectory = logicalDrive.RootDirectory,
                        DriveLetter = logicalDrive.Name.Substring(0, 1).ToCharArray()[0]
                    };

                    var driveId = GetDiskId(tmp.DriveLetter);
                    if (driveId != -1)
                    {
                        tmp.Id = driveId;
                        if (queryType == QueryType.SeekPenalty)
                        {
                            tmp.HardwareType = DetectHardwareTypeBySeekPenalty(driveId);
                        }
                        else
                        {
                            if (IsAdministrator())
                            {
                                tmp.HardwareType = DetectHardwareTypeByRotationRate(driveId);
                            }
                            else
                            {
                                if (useFallbackQuery)
                                {
                                    tmp.HardwareType = DetectHardwareTypeBySeekPenalty(driveId);
                                }
                                else
                                {
                                    throw new SecurityException(
                                        "DetectHardwareTypeBySeekPenalty needs administrative access.");
                                }
                            }
                        }
                        if (tmp.HardwareType != HardwareType.Unknown)
                        {
                            driveInfoExtended = tmp;
                        }
                    }
                }
            }
            return driveInfoExtended;
        }

        /// <summary>
        ///     Detect all fixed drives.
        /// </summary>
        /// <param name="queryType">The QueryType.</param>
        /// <param name="useFallbackQuery">Use QueryType.SeekPenalty as fallback.</param>
        /// <returns>A list of DriveInfoExtended.</returns>
        /// <exception cref="IOException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="DriveNotFoundException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="DetectionFailedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static List<DriveInfoExtended> DetectFixedDrives(QueryType queryType = QueryType.SeekPenalty,
            bool useFallbackQuery = true)
        {
            var driveInfoExtended = new List<DriveInfoExtended>();
            var logicalDrives = DriveInfo.GetDrives();

            foreach (var logicalDrive in logicalDrives)
            {
                if (logicalDrive.DriveType == DriveType.Fixed)
                {
                    if (logicalDrive.IsReady)
                    {
                        var tmp = new DriveInfoExtended
                        {
                            DriveFormat = logicalDrive.DriveFormat,
                            VolumeLabel = logicalDrive.VolumeLabel,
                            Name = logicalDrive.Name,
                            DriveType = logicalDrive.DriveType,
                            AvailableFreeSpace = logicalDrive.AvailableFreeSpace,
                            TotalSize = logicalDrive.TotalSize,
                            TotalFreeSpace = logicalDrive.TotalFreeSpace,
                            RootDirectory = logicalDrive.RootDirectory,
                            DriveLetter = logicalDrive.Name.Substring(0, 1).ToCharArray()[0]
                        };

                        var driveId = GetDiskId(tmp.DriveLetter);
                        if (driveId != -1)
                        {
                            tmp.Id = driveId;
                            if (queryType == QueryType.SeekPenalty)
                            {
                                tmp.HardwareType = DetectHardwareTypeBySeekPenalty(driveId);
                            }
                            else
                            {
                                if (IsAdministrator())
                                {
                                    tmp.HardwareType = DetectHardwareTypeByRotationRate(driveId);
                                }
                                else
                                {
                                    if (useFallbackQuery)
                                    {
                                        tmp.HardwareType = DetectHardwareTypeBySeekPenalty(driveId);
                                    }
                                    else
                                    {
                                        throw new SecurityException(
                                            "DetectHardwareTypeBySeekPenalty needs administrative access.");
                                    }
                                }
                            }
                            if (tmp.HardwareType != HardwareType.Unknown)
                            {
                                driveInfoExtended.Add(tmp);
                            }
                        }
                    }
                }
            }
            return driveInfoExtended;
        }

        /// <summary>
        ///     DeviceIoControl to get disk extents
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="dwIoControlCode"></param>
        /// <param name="lpInBuffer"></param>
        /// <param name="nInBufferSize"></param>
        /// <param name="lpOutBuffer"></param>
        /// <param name="nOutBufferSize"></param>
        /// <param name="lpBytesReturned"></param>
        /// <param name="lpOverlapped"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", EntryPoint = "DeviceIoControl",
            SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            uint dwIoControlCode,
            IntPtr lpInBuffer,
            uint nInBufferSize,
            ref VolumeDiskExtents lpOutBuffer,
            uint nOutBufferSize,
            out uint lpBytesReturned,
            IntPtr lpOverlapped);

        /// <summary>
        ///     Gets the device ID by drive letter.
        /// </summary>
        /// <param name="driveLetter">A valid drive letter.</param>
        /// <returns>The device ID.</returns>
        /// <exception cref="DetectionFailedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        private static int GetDiskId(char driveLetter)
        {
            var di = new DriveInfo(driveLetter.ToString());
            if (di.DriveType != DriveType.Fixed)
            {
                throw new DetectionFailedException(string.Format("This drive is not fixed drive: {0}",
                    driveLetter));
            }

            var sDrive = "\\\\.\\" + driveLetter + ":";

            var hDrive = CreateFileW(
                sDrive,
                0, // No access to drive
                FileShareRead | FileShareWrite,
                IntPtr.Zero,
                OpenExisting,
                FileAttributeNormal,
                IntPtr.Zero);

            if (hDrive == null || hDrive.IsInvalid)
            {
                throw new DetectionFailedException(string.Format("Could not detect Disk Id of {0}",
                    driveLetter));
            }

            var ioctlVolumeGetVolumeDiskExtents = CTL_CODE(
                IoctlVolumeBase, 0,
                MethodBuffered, FileAnyAccess); // From winioctl.h

            var queryDiskExtents =
                new VolumeDiskExtents();

            uint returnedQueryDiskExtentsSize;

            var queryDiskExtentsResult = DeviceIoControl(
                hDrive,
                ioctlVolumeGetVolumeDiskExtents,
                IntPtr.Zero,
                0,
                ref queryDiskExtents,
                (uint) Marshal.SizeOf(queryDiskExtents),
                out returnedQueryDiskExtentsSize,
                IntPtr.Zero);

            hDrive.Close();

            if (queryDiskExtentsResult == false ||
                queryDiskExtents.Extents.Length != 1)
            {
                throw new DetectionFailedException(string.Format("Could not detect Disk Id of {0}",
                    driveLetter));
            }

            return (int) queryDiskExtents.Extents[0].DiskNumber;
        }

        /// <summary>
        ///     Detect the HardwareType by SeekPenalty.
        /// </summary>
        /// <param name="driveLetter">A valid drive letter.</param>
        /// <returns>The detected HardwareType.</returns>
        public static HardwareType DetectHardwareTypeBySeekPenalty(char driveLetter)
        {
            try
            {
                return DetectHardwareTypeBySeekPenalty(GetDiskId(driveLetter));
            }
            catch (DetectionFailedException)
            {
                return HardwareType.Unknown;
            }
        }

        /// <summary>
        ///     Detect the HardwareType by SeekPenalty.
        /// </summary>
        /// <param name="driveId">A valid drive Id.</param>
        /// <returns>The detected HardwareType.</returns>
        public static HardwareType DetectHardwareTypeBySeekPenalty(int driveId)
        {
            var physicalDriveName = "\\\\.\\PhysicalDrive" + driveId;

            try
            {
                return HasDriveSeekPenalty(physicalDriveName) ? HardwareType.Hdd : HardwareType.Ssd;
            }
            catch (DetectionFailedException)
            {
                return HardwareType.Unknown;
            }
        }

        /// <summary>
        ///     Detect the HardwareType by RotationRate.
        /// </summary>
        /// <param name="driveLetter">A valid drive letter.</param>
        /// <returns>The detected HardwareType.</returns>
        public static HardwareType DetectHardwareTypeByRotationRate(char driveLetter)
        {
            try
            {
                return DetectHardwareTypeByRotationRate(GetDiskId(driveLetter));
            }
            catch (DetectionFailedException)
            {
                return HardwareType.Unknown;
            }
        }

        /// <summary>
        ///     Detect the HardwareType by RotationRate.
        /// </summary>
        /// <param name="driveId">A valid drive Id.</param>
        /// <returns>The detected HardwareType.</returns>
        /// <remarks>Administrative privilege is required!</remarks>
        public static HardwareType DetectHardwareTypeByRotationRate(int driveId)
        {
            var physicalDriveName = "\\\\.\\PhysicalDrive" + driveId;

            try
            {
                return HasDriveNominalMediaRotationRate(physicalDriveName) ? HardwareType.Hdd : HardwareType.Ssd;
            }
            catch (DetectionFailedException)
            {
                return HardwareType.Unknown;
            }
        }

        /// <summary>
        ///     CreateFile to get handle to drive.
        /// </summary>
        /// <param name="lpFileName"></param>
        /// <param name="dwDesiredAccess"></param>
        /// <param name="dwShareMode"></param>
        /// <param name="lpSecurityAttributes"></param>
        /// <param name="dwCreationDisposition"></param>
        /// <param name="dwFlagsAndAttributes"></param>
        /// <param name="hTemplateFile"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern SafeFileHandle CreateFileW(
            [MarshalAs(UnmanagedType.LPWStr)] string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        private static uint CTL_CODE(uint DeviceType, uint Function,
            uint Method, uint Access)
        {
            return ((DeviceType << 16) | (Access << 14) |
                    (Function << 2) | Method);
        }

        /// <summary>
        ///     DeviceIoControl to check no seek penalty.
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="dwIoControlCode"></param>
        /// <param name="lpInBuffer"></param>
        /// <param name="nInBufferSize"></param>
        /// <param name="lpOutBuffer"></param>
        /// <param name="nOutBufferSize"></param>
        /// <param name="lpBytesReturned"></param>
        /// <param name="lpOverlapped"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", EntryPoint = "DeviceIoControl",
            SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            uint dwIoControlCode,
            ref StoragePropertyQuery lpInBuffer,
            uint nInBufferSize,
            ref DeviceSeekPenaltyDescriptor lpOutBuffer,
            uint nOutBufferSize,
            out uint lpBytesReturned,
            IntPtr lpOverlapped);

        /// <summary>
        ///     DeviceIoControl to check nominal media rotation rate.
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="dwIoControlCode"></param>
        /// <param name="lpInBuffer"></param>
        /// <param name="nInBufferSize"></param>
        /// <param name="lpOutBuffer"></param>
        /// <param name="nOutBufferSize"></param>
        /// <param name="lpBytesReturned"></param>
        /// <param name="lpOverlapped"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", EntryPoint = "DeviceIoControl",
            SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            uint dwIoControlCode,
            ref AtaIdentifyDeviceQuery lpInBuffer,
            uint nInBufferSize,
            ref AtaIdentifyDeviceQuery lpOutBuffer,
            uint nOutBufferSize,
            out uint lpBytesReturned,
            IntPtr lpOverlapped);

        /// <summary>
        ///     Check if the drive has a seek penalty.
        /// </summary>
        /// <param name="physicalDriveName">A valid physicalDriveName.</param>
        /// <returns><c> true </c> if the drive has a seek penalty otherwise, <c> false </c></returns>
        /// <remarks>Administrative privilege is required!</remarks>
        /// <exception cref="DetectionFailedException"></exception>
        private static bool HasDriveSeekPenalty(string physicalDriveName)
        {
            var hDrive = CreateFileW(
                physicalDriveName,
                0, // No access to drive
                FileShareRead | FileShareWrite,
                IntPtr.Zero,
                OpenExisting,
                FileAttributeNormal,
                IntPtr.Zero);

            if (hDrive == null || hDrive.IsInvalid)
            {
                throw new DetectionFailedException(string.Format("Could not detect SeekPenalty of {0}",
                    physicalDriveName));
            }

            var ioctlStorageQueryProperty = CTL_CODE(
                IoctlStorageBase, 0x500,
                MethodBuffered, FileAnyAccess); // From winioctl.h

            var querySeekPenalty =
                new StoragePropertyQuery
                {
                    PropertyId = StorageDeviceSeekPenaltyProperty,
                    QueryType = PropertyStandardQuery
                };

            var querySeekPenaltyDesc =
                new DeviceSeekPenaltyDescriptor();

            uint returnedQuerySeekPenaltySize;

            var querySeekPenaltyResult = DeviceIoControl(
                hDrive,
                ioctlStorageQueryProperty,
                ref querySeekPenalty,
                (uint) Marshal.SizeOf(querySeekPenalty),
                ref querySeekPenaltyDesc,
                (uint) Marshal.SizeOf(querySeekPenaltyDesc),
                out returnedQuerySeekPenaltySize,
                IntPtr.Zero);

            hDrive.Close();

            if (querySeekPenaltyResult == false)
            {
                throw new DetectionFailedException(string.Format("Could not detect SeekPenalty of {0}",
                    physicalDriveName));
            }
            if (querySeekPenaltyDesc.IncursSeekPenalty == false)
            {
                //This drive has NO SEEK penalty
                return false;
            }
            //This drive has SEEK penalty
            return true;
        }

        /// <summary>
        ///     Check if the drive has a nominal media rotation rate.
        /// </summary>
        /// <param name="physicalDriveName">A valid physicalDriveName.</param>
        /// <returns><c> true </c> if the drive has a media rotation rate otherwise, <c> false </c></returns>
        /// <remarks>Administrative privilege is required!</remarks>
        /// <exception cref="DetectionFailedException"></exception>
        private static bool HasDriveNominalMediaRotationRate(string physicalDriveName)
        {
            var hDrive = CreateFileW(
                physicalDriveName,
                GenericRead | GenericWrite, // Administrative privilege is required
                FileShareRead | FileShareWrite,
                IntPtr.Zero,
                OpenExisting,
                FileAttributeNormal,
                IntPtr.Zero);

            if (hDrive == null || hDrive.IsInvalid)
            {
                throw new DetectionFailedException(string.Format("Could not detect NominalMediaRotationRate of {0}",
                    physicalDriveName));
            }

            var ioctlAtaPassThrough = CTL_CODE(
                IoctlScsiBase, 0x040b, MethodBuffered,
                FileReadAccess | FileWriteAccess); // From ntddscsi.h

            var idQuery = new AtaIdentifyDeviceQuery {data = new ushort[256]};

            idQuery.header.Length = (ushort) Marshal.SizeOf(idQuery.header);
            idQuery.header.AtaFlags = (ushort) AtaFlagsDataIn;
            idQuery.header.DataTransferLength =
                (uint) (idQuery.data.Length*2); // Size of "data" in bytes
            idQuery.header.TimeOutValue = 3; // Sec
            idQuery.header.DataBufferOffset = Marshal.OffsetOf(
                typeof (AtaIdentifyDeviceQuery), "data");
            idQuery.header.PreviousTaskFile = new byte[8];
            idQuery.header.CurrentTaskFile = new byte[8];
            idQuery.header.CurrentTaskFile[6] = 0xec; // ATA IDENTIFY DEVICE

            uint retvalSize;

            var result = DeviceIoControl(
                hDrive,
                ioctlAtaPassThrough,
                ref idQuery,
                (uint) Marshal.SizeOf(idQuery),
                ref idQuery,
                (uint) Marshal.SizeOf(idQuery),
                out retvalSize,
                IntPtr.Zero);

            hDrive.Close();

            if (result == false)
            {
                throw new DetectionFailedException(string.Format("Could not detect NominalMediaRotationRate of {0}",
                    physicalDriveName));
            }
            // Word index of nominal media rotation rate
            // (1 means non-rotate device)
            const int kNominalMediaRotRateWordIndex = 217;

            if (idQuery.data[kNominalMediaRotRateWordIndex] == 1)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        ///     For DeviceIoControl to get disk extents
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct DiskExtent
        {
            public readonly uint DiskNumber;
            public readonly long StartingOffset;
            public readonly long ExtentLength;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct VolumeDiskExtents
        {
            public readonly uint NumberOfDiskExtents;
            [MarshalAs(UnmanagedType.ByValArray)] public readonly DiskExtent[] Extents;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct StoragePropertyQuery
        {
            public uint PropertyId;
            public uint QueryType;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)] public readonly byte[] AdditionalParameters;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DeviceSeekPenaltyDescriptor
        {
            public readonly uint Version;
            public readonly uint Size;
            [MarshalAs(UnmanagedType.U1)] public readonly bool IncursSeekPenalty;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct AtaPassThroughEx
        {
            public ushort Length;
            public ushort AtaFlags;
            public readonly byte PathId;
            public readonly byte TargetId;
            public readonly byte Lun;
            public readonly byte ReservedAsUchar;
            public uint DataTransferLength;
            public uint TimeOutValue;
            public readonly uint ReservedAsUlong;
            public IntPtr DataBufferOffset;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)] public byte[] PreviousTaskFile;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)] public byte[] CurrentTaskFile;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct AtaIdentifyDeviceQuery
        {
            public AtaPassThroughEx header;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] public ushort[] data;
        }

        #region CreateFile (get handle to drive)

        private const uint GenericRead = 0x80000000;
        private const uint GenericWrite = 0x40000000;
        private const uint FileShareRead = 0x00000001;
        private const uint FileShareWrite = 0x00000002;
        private const uint OpenExisting = 3;
        private const uint FileAttributeNormal = 0x00000080;

        #endregion

        #region Control Codes

        private const uint FileDeviceMassStorage = 0x0000002d;
        private const uint IoctlStorageBase = FileDeviceMassStorage;
        private const uint FileDeviceController = 0x00000004;
        private const uint IoctlScsiBase = FileDeviceController;
        private const uint MethodBuffered = 0;
        private const uint FileAnyAccess = 0;
        private const uint FileReadAccess = 0x00000001;
        private const uint FileWriteAccess = 0x00000002;
        private const uint IoctlVolumeBase = 0x00000056;

        #endregion

        #region DeviceIoControl (seek penalty)

        private const uint StorageDeviceSeekPenaltyProperty = 7;
        private const uint PropertyStandardQuery = 0;

        #endregion
    }
}