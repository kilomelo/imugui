using UnityEngine;

namespace imugui.runtime
{
    public partial class Imugui
    {
        public void SyncTrans(IImuguiWindow window, Transform trans)
        {
            if (!AddedWindows.TryGetValue(window, out var enable))
                throw new ImuguiException();
            if (!enable) return;
            if (!UguiViews.TryGetValue(window, out var uguiView))
                throw new ImuguiException();
            uguiView.SyncTrans(trans);
        }
    }
}
