using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Object = UnityEngine.Object;
using Toggle = UnityEngine.UI.Toggle;

namespace imugui.runtime
{
    public class UguiView : MonoBehaviour
    {
        [SerializeField] private Transform _rootTrans;
        [SerializeField] private Canvas _rootCanvas;

        public float Width => (_rootTrans as RectTransform).sizeDelta.x;
        public float Height => (_rootTrans as RectTransform).sizeDelta.y;
        // key: serialNum, value: element gameObject
        private Dictionary<int, GameObject> _elements = new Dictionary<int, GameObject>();
        // key: layoutHashCode, value: normalized scrollPos
        private Dictionary<int, float> _persistentScrollPos = new Dictionary<int, float>();
        // key: layoutHashCode, value: 
        private Dictionary<int, int> _cachedLayoutHashcode = new Dictionary<int, int>();
        // key: layoutElement, value: reset default layout options action
        private Dictionary<LayoutElement, Action> _resetDefaultLayoutOptionsAction = new Dictionary<LayoutElement, Action>();

        public bool CanvasEnabled
        {
            get => _rootCanvas.enabled;
            set => _rootCanvas.enabled = value;
        }

        public void OnImuguiBegin()
        {
            _cachedLayoutHashcode.Clear();
        }

        public void OnImuguiEnd()
        {
            // restore scroll pos
            using var itor = _cachedLayoutHashcode.GetEnumerator();
            while (itor.MoveNext())
            {
                var layoutHashCode = itor.Current.Key;
                var elementSerialNum = itor.Current.Value;
                if (!_elements.TryGetValue(elementSerialNum, out var instance)) throw new ImuguiException();
                var scrollView = instance.GetComponent<ScrollRect>();
                if (_persistentScrollPos.ContainsKey(layoutHashCode))
                {
                    scrollView.verticalNormalizedPosition = _persistentScrollPos[layoutHashCode];
                }
                scrollView.onValueChanged.RemoveAllListeners();
                scrollView.onValueChanged.AddListener(value =>
                {
                    _persistentScrollPos[layoutHashCode] = value.y;
                });
            }
            foreach (var s in _persistentScrollPos.Where(
                         kv => !_cachedLayoutHashcode.ContainsKey(kv.Key)).ToList()) {
                _persistentScrollPos.Remove(s.Key);
            }
        }

        public void Clear()
        {
            foreach (Transform child in _rootTrans)
            {
                Object.Destroy(child.gameObject);
            }
            _elements.Clear();
        }
        private void Init()
        {
            // todo size and anchor
        }

        public void SyncTrans(Transform trans)
        {
            _rootTrans.position = trans.position;
            _rootTrans.rotation = trans.rotation;
        }

        #region add ui component
        public LayoutElement AddLabel(int serialNum, RectTransform parent, string content, string style)
        {
            var prefabPath = string.IsNullOrEmpty(style) ? ImuguiComponent.Instance.Skin.Label : style;
            var instance = Instantiate(Resources.Load(prefabPath)) as GameObject;
            _elements[serialNum] = instance;
            instance.transform.SetParent(parent, false);
            instance.transform.localScale = Vector3.one;
            return SetLabel(instance, content);
        }

        public LayoutElement AddButton(int serialNum, RectTransform parent, string content, UnityAction callback, string style)
        {
            var prefabPath = string.IsNullOrEmpty(style) ? ImuguiComponent.Instance.Skin.Button : style;
            var instance = Instantiate(Resources.Load(prefabPath)) as GameObject;
            instance.transform.SetParent(parent, false);
            instance.transform.localScale = Vector3.one;
            _elements[serialNum] = instance;
            return SetButton(instance, content, callback);
        }
        
        public LayoutElement AddToggle(int serialNum, RectTransform parent, string content, bool isOn, UnityAction<bool> callback, string style)
        {
            var prefabPath = string.IsNullOrEmpty(style) ? ImuguiComponent.Instance.Skin.Toggle : style;
            var instance = Instantiate(Resources.Load(prefabPath)) as GameObject;
            instance.transform.SetParent(parent, false);
            instance.transform.localScale = Vector3.one;
            _elements[serialNum] = instance;
            return SetToggle(instance, content, isOn, callback);
        }

        public LayoutElement AddSpace(int serialNum, RectTransform parent, float width)
        {
            var instance = Instantiate(Resources.Load(ImuguiComponent.Instance.Skin.Space)) as GameObject;
            instance.transform.SetParent(parent, false);
            instance.transform.localScale = Vector3.one;
            _elements[serialNum] = instance;
            return SetSpace(instance, width);
        }
        
        public LayoutElement AddVerticalSpace(int serialNum, RectTransform parent, float height)
        {
            var instance = Instantiate(Resources.Load(ImuguiComponent.Instance.Skin.Space)) as GameObject;
            instance.transform.SetParent(parent, false);
            instance.transform.localScale = Vector3.one;
            _elements[serialNum] = instance;
            return SetVerticalSpace(instance, height);
        }

        public RectTransform AddScrollView(int serialNum, int layoutHashCode, RectTransform parent, float height, string style)
        {
            var prefabPath = string.IsNullOrEmpty(style) ? ImuguiComponent.Instance.Skin.ScrollView : style;
            var instance = Instantiate(Resources.Load(prefabPath)) as GameObject;
            _elements[serialNum] = instance;
            var rectTrans = instance.transform as RectTransform;
            rectTrans.SetParent(parent, false);
            rectTrans.localScale = Vector3.one;
            
            _cachedLayoutHashcode.Add(layoutHashCode, serialNum);
            return SetScrollView(instance, height);
        }
        
        public RectTransform AddHorizontalLayout(int serialNum, int layoutHashCode, RectTransform parent, float height, string style)
        {
            var prefabPath = string.IsNullOrEmpty(style) ? ImuguiComponent.Instance.Skin.HorizontalLayout : style;
            var instance = Instantiate(Resources.Load(prefabPath)) as GameObject;
            _elements[serialNum] = instance;
            var rectTrans = instance.transform as RectTransform;
            rectTrans.SetParent(parent, false);
            rectTrans.localScale = Vector3.one;
            return SetHorizontalLayout(instance, height);
        }
        
        public RectTransform AddVerticalLayout(int serialNum, int layoutHashCode, RectTransform parent, float width, string style)
        {
            var prefabPath = string.IsNullOrEmpty(style) ? ImuguiComponent.Instance.Skin.VerticalLayout : style;
            var instance = Instantiate(Resources.Load(prefabPath)) as GameObject;
            _elements[serialNum] = instance;
            var rectTrans = instance.transform as RectTransform;
            rectTrans.SetParent(parent, false);
            rectTrans.localScale = Vector3.one;
            return SetVerticalLayout(instance, width);
        }
        #endregion
        
        #region update property

        public LayoutElement UpdateButton(int serialNum, string content, UnityAction callback)
        {
            if (!_elements.TryGetValue(serialNum, out var instance)) throw new ImuguiException();
            return SetButton(instance, content, callback);
        }

        public LayoutElement UpdateLabel(int serialNum, string content)
        {
            if (!_elements.TryGetValue(serialNum, out var instance)) throw new ImuguiException();
            return SetLabel(instance, content);
        }

        public LayoutElement UpdateToggle(int serialNum, string content, bool isOn, UnityAction<bool> callback)
        {
            if (!_elements.TryGetValue(serialNum, out var instance)) throw new ImuguiException();
            return SetToggle(instance, content, isOn, callback);
        }

        public LayoutElement UpdateSpace(int serialNum, float width)
        {
            if (!_elements.TryGetValue(serialNum, out var instance)) throw new ImuguiException();
            return SetSpace(instance, width);
        }
        
        public LayoutElement UpdateVerticalSpace(int serialNum, float height)
        {
            if (!_elements.TryGetValue(serialNum, out var instance)) throw new ImuguiException();
            return SetVerticalSpace(instance, height);
        }

        public RectTransform UpdateScrollView(int serialNum, float height)
        {
            if (!_elements.TryGetValue(serialNum, out var instance)) throw new ImuguiException();
            return SetScrollView(instance, height);
        }
        
        public RectTransform UpdateHorizontalLayout(int serialNum, float height)
        {
            if (!_elements.TryGetValue(serialNum, out var instance)) throw new ImuguiException();
            return SetHorizontalLayout(instance, height);
        }
        
        public RectTransform UpdateVerticalLayout(int serialNum, float width)
        {
            if (!_elements.TryGetValue(serialNum, out var instance)) throw new ImuguiException();
            return SetVerticalLayout(instance, width);
        }
        #endregion

        private LayoutElement SetLabel(GameObject instance, string content)
        {
            var label = instance.GetComponent<TextMeshProUGUI>();
            label.text = content;
            return instance.GetComponent<LayoutElement>();
        }
        
        private LayoutElement SetButton(GameObject instance, string content, UnityAction callback)
        {
            var label = instance.GetComponentInChildren<TextMeshProUGUI>();
            if (null != label) label.text = content;
            var button = instance.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(callback);
            return instance.GetComponent<LayoutElement>();
        }
        
        private LayoutElement SetToggle(GameObject instance, string content, bool isOn, UnityAction<bool> callback)
        {
            var label = instance.GetComponentInChildren<TextMeshProUGUI>();
            if (null != label) label.text = content;
            var toggle = instance.GetComponent<Toggle>();
            toggle.isOn = isOn;
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener(callback);
            return instance.GetComponent<LayoutElement>();
        }

        private LayoutElement SetSpace(GameObject instance, float width)
        {
            var layoutElement = instance.GetComponent<LayoutElement>();
            layoutElement.minWidth = width;
            layoutElement.preferredWidth = width;
            layoutElement.flexibleWidth = 0f;
            var rectTrans = instance.transform as RectTransform;
            rectTrans.sizeDelta = new Vector2(width, rectTrans.sizeDelta.y);
            return layoutElement;
        }

        private LayoutElement SetVerticalSpace(GameObject instance, float height)
        {
            var layoutElement = instance.GetComponent<LayoutElement>();
            layoutElement.minHeight = height;
            layoutElement.preferredHeight = height;
            layoutElement.flexibleHeight = 0f;
            var rectTrans = instance.transform as RectTransform;
            rectTrans.sizeDelta = new Vector2(rectTrans.sizeDelta.x, height);
            return layoutElement;
        }
        private RectTransform SetScrollView(GameObject instance, float height)
        {
            var rectTrans = instance.transform as RectTransform;
            var scrollView = instance.GetComponent<ScrollRect>();
            var layoutElement = instance.GetComponent<LayoutElement>();
            // flexible height or not
            if (0 < height)
            {
                layoutElement.flexibleHeight = 0f;
                layoutElement.minHeight = height;
                layoutElement.preferredHeight = height;
                rectTrans.sizeDelta = new Vector2(rectTrans.sizeDelta.x, height);
            }
            else
            {
                layoutElement.flexibleHeight = 1f;
            }
            return scrollView.content;
        }

        private RectTransform SetHorizontalLayout(GameObject instance, float height)
        {
            var rectTrans = instance.transform as RectTransform;
            var layoutElement = instance.GetComponent<LayoutElement>();
            // flexible height or not
            if (0 < height)
            {
                layoutElement.flexibleHeight = 0f;
                layoutElement.minHeight = height;
                layoutElement.preferredHeight = height;
                rectTrans.sizeDelta = new Vector2(rectTrans.sizeDelta.x, height);
            }
            else
            {
                layoutElement.flexibleHeight = 1f;
            }
            return rectTrans;
        }
        
        private RectTransform SetVerticalLayout(GameObject instance, float width)
        {
            var rectTrans = instance.transform as RectTransform;
            var layoutElement = instance.GetComponent<LayoutElement>();
            // flexible height or not
            if (0 < width)
            {
                layoutElement.flexibleHeight = 0f;
                layoutElement.minWidth = width;
                layoutElement.preferredWidth = width;
                rectTrans.sizeDelta = new Vector2(width, rectTrans.sizeDelta.x);
            }
            else
            {
                layoutElement.flexibleWidth = 1f;
            }
            return rectTrans;
        }
        
        #region layout option

        public void ApplyLayoutOption(LayoutElement layoutElement, params GUILayoutOption[] options)
        {
            _resetDefaultLayoutOptionsAction.TryGetValue(layoutElement, out var resetDefaultLayoutOptionsAction);
            if (!options.Any())
            {
                if (null == resetDefaultLayoutOptionsAction) return;
                resetDefaultLayoutOptionsAction.Invoke();
                _resetDefaultLayoutOptionsAction.Remove(layoutElement);
                return;
            }
            var initResetAction = resetDefaultLayoutOptionsAction == null;
            var rectTrans = layoutElement.transform as RectTransform;
            foreach (var option in options)
            {
                switch (option.type)
                {
                    case GUILayoutOption.Type.fixedWidth:
                        layoutElement.minWidth = layoutElement.preferredWidth = (float) option.value;
                        rectTrans.sizeDelta = new Vector2(layoutElement.minWidth, rectTrans.sizeDelta.y);
                        break;
                    case GUILayoutOption.Type.minWidth:
                        layoutElement.minWidth = (float) option.value;
                        break;
                    case GUILayoutOption.Type.maxWidth:
                        layoutElement.preferredWidth = (float) option.value;
                        break;
                    case GUILayoutOption.Type.fixedHeight:

                        if (initResetAction)
                        {
                            var defaultMinHeight = layoutElement.minHeight;
                            resetDefaultLayoutOptionsAction += () =>
                            {
                                layoutElement.minHeight = defaultMinHeight;
                                rectTrans.sizeDelta = new Vector2(rectTrans.sizeDelta.x, defaultMinHeight);
                            };
                        }
                        layoutElement.minHeight = layoutElement.preferredHeight = (float) option.value;
                        rectTrans.sizeDelta = new Vector2(rectTrans.sizeDelta.x, layoutElement.minHeight);
                        break;
                    case GUILayoutOption.Type.minHeight:
                        layoutElement.minHeight = (float) option.value;
                        break;
                    case GUILayoutOption.Type.maxHeight:
                        layoutElement.preferredHeight = (float) option.value;
                        break;
                }
            }
            if (initResetAction) _resetDefaultLayoutOptionsAction[layoutElement] = resetDefaultLayoutOptionsAction;
        }
        #endregion
        
        public void Destroy()
        {
            Object.Destroy(gameObject);
        }
    }
}