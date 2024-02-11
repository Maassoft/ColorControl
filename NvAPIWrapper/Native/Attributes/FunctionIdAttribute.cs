using NvAPIWrapper.Native.Helpers;
using System;

namespace NvAPIWrapper.Native.Attributes
{
    [AttributeUsage(AttributeTargets.Delegate)]
    public class FunctionIdAttribute : Attribute
    {
        public FunctionIdAttribute(FunctionId functionId)
        {
            FunctionId = functionId;
        }

        public FunctionId FunctionId { get; set; }
    }
}