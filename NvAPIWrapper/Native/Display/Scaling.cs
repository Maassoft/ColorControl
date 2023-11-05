using System.ComponentModel;

namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Possible scaling modes
    /// </summary>
    public enum Scaling
    {
        /// <summary>
        ///     No change
        /// </summary>
        Default = 0,

        /// <summary>
        ///     Balanced  - Full Screen
        /// </summary>
        [Description("Display - Full-screen")]
        ToClosest = 1,

        /// <summary>
        ///     Force GPU - Full Screen
        /// </summary>
        [Description("GPU - Full-screen")]
        ToNative = 2,

        /// <summary>
        ///     Force GPU - Centered\No Scaling
        /// </summary>
        [Description("GPU - No Scaling")]
        GPUScanOutToNative = 3,

        /// <summary>
        ///     Force GPU - Aspect Ratio
        /// </summary>
        [Description("GPU - Aspect Ratio")]
        ToAspectScanOutToNative = 5,

        /// <summary>
        ///     Balanced  - Aspect Ratio
        /// </summary>
        [Description("Display - Aspect Ratio")]
        ToAspectScanOutToClosest = 6,

        /// <summary>
        ///     Balanced  - Centered\No Scaling
        /// </summary>
        [Description("Display - No Scaling")]
        GPUScanOutToClosest = 7,

        /// <summary>
        ///     Customized scaling - For future use
        /// </summary>
        Customized = 255
    }
}