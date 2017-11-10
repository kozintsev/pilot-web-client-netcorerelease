using System;
using System.Runtime.InteropServices;

namespace MuPDF
{
    internal class NativeXps : IDisposable
    {
        private GCHandle _dataBufferHandle;
        public IntPtr[] Pages { get; private set; }
        public IntPtr XpsPtr { get; private set; }

        public NativeXps(IntPtr docNativePtr, int pageCount, GCHandle dataHandle)
        {
            _dataBufferHandle = dataHandle;
            XpsPtr = docNativePtr;
            Pages = new IntPtr[pageCount];
        }

        public void Dispose()
        {
            _dataBufferHandle.Free();

            if (XpsPtr == IntPtr.Zero)
                return;

            foreach (var page in Pages)
            {
                try
                {
                    RenderLibrary.close_page(XpsPtr, page);
                }
                catch
                {
                }
            }
            Pages = null;

            try
            {
                RenderLibrary.close_xps(XpsPtr);
            }
            finally
            {
                XpsPtr = IntPtr.Zero;
            }
        }
    }
}