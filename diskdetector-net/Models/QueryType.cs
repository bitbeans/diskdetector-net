namespace DiskDetector.Models
{
    /// <summary>
    ///     Possible QueryTypes.
    /// </summary>
    public enum QueryType
    {
        /// <summary>
        ///     Detect the HardwareType by SeekPenalty.
        /// </summary>
        SeekPenalty,

        /// <summary>
        ///     Detect the HardwareType by RotationRate.
        /// </summary>
        RotationRate
    }
}