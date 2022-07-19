using System.Collections.Generic;
using UnityEngine;

namespace imugui.runtime
{
    public partial class Imugui
    {
        private readonly Dictionary<IImuguiWindow, bool> AddedWindows = new Dictionary<IImuguiWindow, bool>();
        public void AddWindow(IImuguiWindow window, EAnchorMode anchorMode, Vector2 size, float scale, Vector2 offset, string style, Vector3 transScale)
        {
            if (AddedWindows.ContainsKey(window))
                throw new ImuguiException();
            AddedWindows.Add(window, false);
            CreateUgui(window, anchorMode, size, scale, offset, style, transScale);
        }
        public void RemoveWindow(IImuguiWindow window)
        {
            if (!AddedWindows.ContainsKey(window))
                throw new ImuguiException();
            AddedWindows.Remove(window);
            DestroyUgui(window);
        }

        public void EnableWindow(IImuguiWindow window)
        {
            if (!AddedWindows.ContainsKey(window))
                throw new ImuguiException();
            AddedWindows[window] = true;
            SetUguiActive(window, true);
        }

        public void DisableWindow(IImuguiWindow window)
        {
            if (!AddedWindows.ContainsKey(window))
                throw new ImuguiException();
            AddedWindows[window] = false;
            SetUguiActive(window, false);
        }

        public void Update()
        {
            UpdateDrawCommand();
        }
    }
}