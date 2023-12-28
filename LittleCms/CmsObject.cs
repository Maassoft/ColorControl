using System;

namespace LittleCms
{
    public abstract class CmsObject
    {
        private IntPtr handle;
        private bool isOwner;

        public IntPtr Handle => handle;
        public bool IsOwner => isOwner;

        public abstract CmsContext Context { get; }


        protected CmsObject() { }

        protected CmsObject(IntPtr handle, bool moveOwnership)
        {
            AttachObject(handle, isOwner);
        }


        protected void AttachObject(IntPtr handle, bool moveOwnership)
        {
            if (handle != IntPtr.Zero && isOwner)
            {
                FreeObject();
            }
            if (handle == IntPtr.Zero)
            {
                throw new NullReferenceException();
            }
            this.handle = handle;
            this.isOwner = moveOwnership;
        }

        ~CmsObject() {
            if (handle != IntPtr.Zero && isOwner) {
                FreeObject();
            }
        }

        protected abstract void FreeObject();
    }
}
