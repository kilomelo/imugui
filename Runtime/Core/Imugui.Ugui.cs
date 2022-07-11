using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace imugui.runtime
{
    public partial class Imugui
    {
        public float WindowWidth => _drawingHandler.DrawingView == null ? 0f : _drawingHandler.DrawingView.Width;
        public float WindowHeight => _drawingHandler.DrawingView == null ? 0f : _drawingHandler.DrawingView.Height;
        // limit complexity of single window
        private const int WindowStructureDepthLimit = 8;
        private DrawingHandler _drawingHandler;
        internal const float ImuguiWindowDefaultScale = 0.002f;
        public Transform ImuguiRoot
        {
            get
            {
                _imuguiRoot ??= new GameObject("ImuguiRoot").transform;
                Object.DontDestroyOnLoad(_imuguiRoot.gameObject);
                return _imuguiRoot;
            }
        }
        private Transform _imuguiRoot;
        private readonly Dictionary<IImuguiWindow, UguiView> UguiViews =
            new Dictionary<IImuguiWindow, UguiView>();
        public enum EAnchorMode : byte
        {
            LeftTop,
            CenterTop,
            RightTop,
            LeftCenter,
            Center,
            RightCenter,
            LeftBottom,
            CenterBottom,
            RightBottom,
        }
        private void CreateUgui(IImuguiWindow window, EAnchorMode anchorMode, Vector2 size, float scale, Vector2 offset, string style)
        {
            if (UguiViews.ContainsKey(window))
                throw new ImuguiException();
            var newUguiView = CreateUguiWindow(anchorMode, size, scale, offset, style);
            UguiViews.Add(window, newUguiView);
        }
        private void DestroyUgui(IImuguiWindow window)
        {
            if (!UguiViews.TryGetValue(window, out var uguiView))
                throw new ImuguiException();
            // todo reuse
            if (null == uguiView) return;
            uguiView.Destroy();
            UguiViews.Remove(window);
        }
        private void SetUguiActive(IImuguiWindow window, bool active)
        {
            if (!UguiViews.TryGetValue(window, out var uguiView))
                throw new ImuguiException();
            if (null == uguiView) return;
            uguiView.CanvasEnabled = active;
        }

        private UguiView CreateUguiWindow(EAnchorMode anchorMode, Vector2 size, float scale, Vector2 offset, string style)
        {
            var prefabPath = string.IsNullOrEmpty(style) ? ImuguiComponent.Instance.Skin.Canvas : style;
            var root = Object.Instantiate(Resources.Load(prefabPath)) as GameObject;
            var rectTrans = root.transform as RectTransform; 
            root.transform.SetParent(ImuguiRoot);
            rectTrans.sizeDelta = size;
            rectTrans.localScale = Vector3.one * scale;
            return root.GetComponent<UguiView>();
        }

        #region internal draw commands, called between BeginImugui() and EndImugui
        private void DrawLabel(int serialNum, string content, string style, params GUILayoutOption[] options)
        {
            if (null == _drawingHandler.DrawingView) throw new ImuguiException();
            var layoutElement = _drawingHandler.DrawingView.AddLabel(serialNum, _drawingHandler.CurrentDrawLayer, content, style);
            _drawingHandler.DrawingView.ApplyLayoutOption(layoutElement, options);
        }

        private void DrawButton(int serialNum, string content, UnityAction callback, string style, params GUILayoutOption[] options)
        {
            if (null == _drawingHandler.DrawingView) throw new ImuguiException();
            var layoutElement = _drawingHandler.DrawingView.AddButton(serialNum, _drawingHandler.CurrentDrawLayer, content, callback, style);
            _drawingHandler.DrawingView.ApplyLayoutOption(layoutElement, options);
        }

        private void DrawSpace(int serialNum, float width)
        {
            if (null == _drawingHandler.DrawingView) throw new ImuguiException();
            _drawingHandler.DrawingView.AddSpace(serialNum, _drawingHandler.CurrentDrawLayer, width);
        }
        
        private void DrawVerticalSpace(int serialNum, float height)
        {
            if (null == _drawingHandler.DrawingView) throw new ImuguiException();
            _drawingHandler.DrawingView.AddVerticalSpace(serialNum, _drawingHandler.CurrentDrawLayer, height);
        }
        
        private void DrawToggle(int serialNum, string content, bool isOn, UnityAction<bool> callback, string style, params GUILayoutOption[] options)
        {
            if (null == _drawingHandler.DrawingView) throw new ImuguiException();
            var layoutElement = _drawingHandler.DrawingView.AddToggle(serialNum, _drawingHandler.CurrentDrawLayer, content, isOn, callback, style);
            _drawingHandler.DrawingView.ApplyLayoutOption(layoutElement, options);
        }

        private void BeginScrollViewInternal(int serialNum, float height, string style)
        {
            if (null == _drawingHandler.DrawingView) throw new ImuguiException();
            if (_drawingHandler.DrawLayerStack.Count() >= WindowStructureDepthLimit) throw new ImuguiException();
            var flexible = height <= 0f;
            // nested flexible content in one dimension
            if (flexible && _drawingHandler.FlexibleHeight) throw new ImuguiException();
            _drawingHandler.PushLayout(_drawingHandler.DrawingView.AddScrollView(
                serialNum, _drawingHandler.CurrentLayoutHashCode, _drawingHandler.CurrentDrawLayer, height, style), flexible);
        }

        private void BeginHorizontalLayoutInternal(int serialNum, float height, string style)
        {
            if (null == _drawingHandler.DrawingView) throw new ImuguiException();
            if (_drawingHandler.DrawLayerStack.Count() >= WindowStructureDepthLimit) throw new ImuguiException();
            var flexible = height <= 0f;
            // nested flexible content in one dimension
            if (flexible && _drawingHandler.FlexibleHeight) throw new ImuguiException();
            _drawingHandler.PushLayout(_drawingHandler.DrawingView.AddHorizontalLayout(
                serialNum, _drawingHandler.CurrentLayoutHashCode, _drawingHandler.CurrentDrawLayer, height, style), flexible);

        }
        private void BeginVerticalLayoutInternal(int serialNum, float width, string style)
        {
            if (null == _drawingHandler.DrawingView) throw new ImuguiException();
            if (_drawingHandler.DrawLayerStack.Count() >= WindowStructureDepthLimit) throw new ImuguiException();
            var flexible = width <= 0f;
            // nested flexible content in one dimension
            if (flexible && _drawingHandler.FlexibleWidth) throw new ImuguiException();
            _drawingHandler.PushLayout(_drawingHandler.DrawingView.AddVerticalLayout(
                serialNum, _drawingHandler.CurrentLayoutHashCode, _drawingHandler.CurrentDrawLayer, width, style), flexible);

        }
        private void EndLayoutInternal()
        {
            // todo end command pair check
            _drawingHandler.PopLayout();
        }

        #endregion

        private void ClearUguiWindow(IImuguiWindow window)
        {
            if (!UguiViews.TryGetValue(window, out var uguiView) || null == uguiView)
                throw new ImuguiException();
            uguiView.Clear();
        }
        
        private void BeginImugui(IImuguiWindow window)
        {
            if (!UguiViews.TryGetValue(window, out var uguiView) || null == uguiView)
                throw new ImuguiException();
            _drawingHandler.Init(uguiView);
        }

        private void EndImugui(IImuguiWindow window)
        {
            if (_drawingHandler.DrawLayerStack.Any())
            {
                _drawingHandler.Clear();
                throw new ImuguiException();
            }
            _drawingHandler.Clear();
        }

        private struct DrawingHandler
        {
            public UguiView DrawingView;
            public Stack<RectTransform> DrawLayerStack;
            public RectTransform CurrentDrawLayer;
            public bool FlexibleHeight;
            public bool FlexibleWidth;

            private int _currentLayoutIdx;
            private List<int> _windowLayoutStructure;

            public int CurrentLayoutHashCode
            {
                get
                {
                    var hashCode = 0;
                    foreach (var layoutIdx in _windowLayoutStructure)
                    {
                        hashCode += layoutIdx.GetHashCode();
                    }
                    return hashCode + _currentLayoutIdx.GetHashCode();
                }
            }
            
            public void Init(UguiView drawingView)
            {
                DrawingView = drawingView;
                if (null == DrawLayerStack) DrawLayerStack = new Stack<RectTransform>();
                else DrawLayerStack.Clear();
                CurrentDrawLayer = drawingView.transform as RectTransform;
                FlexibleHeight = false;
                FlexibleWidth = false;
                
                _windowLayoutStructure = new List<int>();
                _currentLayoutIdx = 0;
                DrawingView.OnImuguiBegin();
            }

            public void Clear()
            {
                if (null != DrawingView) DrawingView.OnImuguiEnd();
                DrawingView = null;
                DrawLayerStack.Clear();
                CurrentDrawLayer = null;
                FlexibleHeight = false;
                FlexibleWidth = false;
            }

            public void PushLayout(RectTransform newLayer, bool flexible)
            {
                DrawLayerStack.Push(CurrentDrawLayer);
                CurrentDrawLayer = newLayer;
                _windowLayoutStructure.Add(_currentLayoutIdx);
                _currentLayoutIdx = 0;
                // FlexibleHeight = flexible;
            }

            public void PopLayout()
            {
                if (null == DrawingView) throw new ImuguiException();
                if (!DrawLayerStack.Any() || !_windowLayoutStructure.Any()) throw new ImuguiException();
                CurrentDrawLayer = DrawLayerStack.Pop();
                _currentLayoutIdx = _windowLayoutStructure[^1];
                _windowLayoutStructure.RemoveAt(_windowLayoutStructure.Count() - 1);
            }
        }
    }
}