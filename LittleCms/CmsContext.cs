using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleCms
{
    public class CmsContext
    {
        public IntPtr Handle { get; }
        public bool OwnedByGC { get; private set; }


        static readonly ConcurrentDictionary<IntPtr, CmsContext> GCOwnedContext = new();

        public static CmsContext Default { get; } = new CmsContext(IntPtr.Zero, false);


        private void MoveToGC()
        {
            OwnedByGC = true;

            GCOwnedContext.TryAdd(Handle, this);
        }

        public CmsContext() {
            Handle = CmsNative.cmsCreateContext(IntPtr.Zero, IntPtr.Zero);
            MoveToGC();
        }

        private CmsContext(IntPtr handle, bool moveOwnership){
            Handle = handle;
            if (moveOwnership)
            {
                MoveToGC();
            }
        }

        public static CmsContext GetFromHandle(IntPtr handle, bool moveOwnership = false)
        {
            if (handle == IntPtr.Zero) return Default;
            if (GCOwnedContext.TryGetValue(handle, out var x)) return x;
            return new CmsContext(handle, moveOwnership);
        }

        public double SetAdaptionState(double state)
        {
            return CmsNative.cmsSetAdaptationStateTHR(Handle, state);
        }


        ~CmsContext()
        {
            if (OwnedByGC && Handle != IntPtr.Zero )
            {
                if (GCOwnedContext.TryRemove(new(Handle, this))) {
                    CmsNative.cmsDeleteContext(Handle);
                }
            }
        }
    }
}
