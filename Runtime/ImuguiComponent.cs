using System;
using UnityEngine;
using UnityEngine.Events;

namespace imugui.runtime
{
    public class ImuguiComponent : MonoBehaviour
    {
        public static ImuguiComponent Instance => _instance;
        private static ImuguiComponent _instance;
        private Imugui _imugui;

        public Transform ImuguiRootTrans => _imugui?.ImuguiRoot;

        public float WindowWidth => _imugui?.WindowWidth ?? 0f;
        public float WindowHeight => _imugui?.WindowHeight ?? 0f;
        public Imugui.ImuguiSkin Skin => _imugui?.CurrentSkin ?? default;
        private void Awake()
        {
            _instance = this;
            _imugui = new Imugui();
        }

        public void AddWindow(IImuguiWindow window)
        {
            var attr = Attribute.GetCustomAttribute(window.GetType(), typeof(ImuguiWindowAttribute)) as ImuguiWindowAttribute;
            if (null == attr)
            {
                throw new ImuguiException();
            }
            _imugui.AddWindow(window, attr.AnchorMode, attr.Size, attr.Scale, attr.Offset, attr.Style);
        }
        
        public void EnableWindow(IImuguiWindow window)
        {
            _imugui.EnableWindow(window);
        }
        
        public void DisableWindow(IImuguiWindow window)
        {
            _imugui.DisableWindow(window);
        }
        
        public void RemoveWindow(IImuguiWindow window)
        {
            _imugui.RemoveWindow(window);
        }
        
        public void SyncTrans(IImuguiWindow window, Transform trans)
        {
            _imugui.SyncTrans(window, trans);
        }

        #region Draw commands, called only in IImuguiWindow.OnImugui
        public void Label(string content, params GUILayoutOption[] options)
        {
            _imugui.Label(content, null, options);
        }
        
        public void Label(string content, string style, params GUILayoutOption[] options)
        {
            _imugui.Label(content, style, options);
        }
        
        public void Button(string content, UnityAction callback, params GUILayoutOption[] options)
        {
            _imugui.Button(content, callback, null, options);
        }
        
        public void Button(string content, UnityAction callback, string style, params GUILayoutOption[] options)
        {
            _imugui.Button(content, callback, style, options);
        }

        public void Toggle(string content, bool isOn, UnityAction<bool> callback, params GUILayoutOption[] options)
        {
            _imugui.Toggle(content, isOn, callback, null, options);
        }
        
        public void Toggle(string content, bool isOn, UnityAction<bool> callback, string style, params GUILayoutOption[] options)
        {
            _imugui.Toggle(content, isOn, callback, style, options);
        }
        public void Space(float width)
        {
            _imugui.Space(width);
        }
        
        public void VerticalSpace(float height = GUILayout.DefaultLineHeight)
        {
            _imugui.VerticalSpace(height);
        }

        public void BeginScrollView(float height)
        {
            _imugui.BeginScrollView(height, null);
        }
        
        public void BeginScrollView(float height, string style)
        {
            _imugui.BeginScrollView(height, style);
        }

        public void EndScrollView()
        {
            _imugui.EndScrollView();
        }
        
        public void BeginHorizontalLayout(float height = GUILayout.DefaultLineHeight)
        {
            _imugui.BeginHorizontalLayout(height, null);
        }
        
        public void BeginHorizontalLayout(float height, string style)
        {
            _imugui.BeginHorizontalLayout(height, style);
        }

        public void EndHorizontalLayout()
        {
            _imugui.EndHorizontalLayout();
        }
        
        public void BeginVerticalLayout(float width = 0f)
        {
            _imugui.BeginVerticalLayout(width, null);
        }
        
        public void BeginVerticalLayout(float width, string style)
        {
            _imugui.BeginVerticalLayout(width, style);
        }

        public void EndVerticalLayout()
        {
            _imugui.EndVerticalLayout();
        }

        public void PushSkin(Imugui.ImuguiSkin skin)
        {
            _imugui.PushSkin(skin);
        }

        public void PopSkin()
        {
            _imugui.PopSkin();
        }
        #endregion

        void Update()
        {
            _imugui.Update();
        }
    }
}