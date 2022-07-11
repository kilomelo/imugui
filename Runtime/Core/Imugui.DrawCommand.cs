using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace imugui.runtime
{
    public partial class Imugui
    {
        // limit the total capacity of single window
        private const int WindowStructureLengthLimit = 4096;
        private readonly Dictionary<IImuguiWindow, int> _lastDrawCommandStructureHash = new Dictionary<IImuguiWindow, int>();
        private readonly Dictionary<IImuguiWindow, Dictionary<int, int>> _lastDrawCommandPropertyHash = new Dictionary<IImuguiWindow, Dictionary<int, int>>();
        private enum EDrawPhase
        {
            CalculateHash,
            RecreateUgui,
            UpdateProperty,
        }

        private EDrawPhase _currentPhase;
        private StringBuilder _windowStructureTemp = new StringBuilder();
        private IImuguiWindow _currentWindow;
        private List<int> _dirtyElements = new List<int>();
        private int _uiElementSerialNumGenerator;
        
        #region public draw commands, called by ImuguiComponent
        public void Label(string content, string style, params GUILayoutOption[] options)
        {
            _uiElementSerialNumGenerator++;
            switch (_currentPhase)
            {
                case EDrawPhase.CalculateHash:
                    var elementPropertyHash = GetOptionArrayHashCode(options) + content.GetHashCode();
                    CheckDirtyElement(elementPropertyHash);
                    AppendWindowStructure(string.IsNullOrEmpty(style) ? "Lbl" : $"Lbl{style.GetHashCode():x8}");
                    return;
                case EDrawPhase.UpdateProperty:
                    if (_dirtyElements.Contains(_uiElementSerialNumGenerator))
                        UpdateLabel(_uiElementSerialNumGenerator, content, options);
                    break;
                case EDrawPhase.RecreateUgui:
                    DrawLabel(_uiElementSerialNumGenerator, content, style, options);
                    break;
            }
        }

        public void Button(string content, UnityAction callback, string style, params GUILayoutOption[] options)
        {
            _uiElementSerialNumGenerator++;
            switch (_currentPhase)
            {
                case EDrawPhase.CalculateHash:
                    var elementPropertyHash = GetOptionArrayHashCode(options) + content.GetHashCode();// + callback.GetHashCode();
                    CheckDirtyElement(elementPropertyHash);
                    AppendWindowStructure(string.IsNullOrEmpty(style) ? "Btn" : $"Btn{style.GetHashCode():x8}");
                    break;
                case EDrawPhase.UpdateProperty:
                    if (_dirtyElements.Contains(_uiElementSerialNumGenerator))
                        UpdateButton(_uiElementSerialNumGenerator, content, callback, options);
                    break;
                case EDrawPhase.RecreateUgui:
                    DrawButton(_uiElementSerialNumGenerator, content, callback, style, options);
                    break;
            }
        }
        
        public void Toggle(string content, bool isOn, UnityAction<bool> callback, string style, params GUILayoutOption[] options)
        {
            _uiElementSerialNumGenerator++;
            switch (_currentPhase)
            {
                case EDrawPhase.CalculateHash:
                    var elementPropertyHash = GetOptionArrayHashCode(options) + content.GetHashCode() + isOn.GetHashCode();// + callback.GetHashCode();
                    CheckDirtyElement(elementPropertyHash);
                    AppendWindowStructure(string.IsNullOrEmpty(style) ? "Tgl" : $"Tgl{style.GetHashCode():x8}");
                    break;
                case EDrawPhase.UpdateProperty:
                    if (_dirtyElements.Contains(_uiElementSerialNumGenerator))
                        UpdateToggle(_uiElementSerialNumGenerator, content, isOn, callback, options);
                    break;
                case EDrawPhase.RecreateUgui:
                    DrawToggle(_uiElementSerialNumGenerator, content, isOn, callback, style, options);
                    break;
            }
        }
        
        public void Space(float width)
        {
            _uiElementSerialNumGenerator++;
            switch (_currentPhase)
            {
                case EDrawPhase.CalculateHash:
                    var elementPropertyHash = width.GetHashCode();
                    CheckDirtyElement(elementPropertyHash);
                    AppendWindowStructure("Spc");
                    break;
                case EDrawPhase.UpdateProperty:
                    if (_dirtyElements.Contains(_uiElementSerialNumGenerator))
                        UpdateSpace(_uiElementSerialNumGenerator, width);
                    break;
                case EDrawPhase.RecreateUgui:
                    DrawSpace(_uiElementSerialNumGenerator, width);
                    break;
            }
        }
        
        public void VerticalSpace(float height)
        {
            _uiElementSerialNumGenerator++;
            switch (_currentPhase)
            {
                case EDrawPhase.CalculateHash:
                    var elementPropertyHash = height.GetHashCode();
                    
                    AppendWindowStructure("VSp");
                    break;
                case EDrawPhase.UpdateProperty:
                    if (_dirtyElements.Contains(_uiElementSerialNumGenerator))
                        UpdateVerticalSpace(_uiElementSerialNumGenerator, height);
                    break;
                case EDrawPhase.RecreateUgui:
                    DrawVerticalSpace(_uiElementSerialNumGenerator, height);
                    break;
            }
        }
        
        public void BeginScrollView(float height, string style)
        {
            _uiElementSerialNumGenerator++;
            switch (_currentPhase)
            {
                case EDrawPhase.CalculateHash:
                    var elementPropertyHash = height.GetHashCode();
                    CheckDirtyElement(elementPropertyHash);
                    AppendWindowStructure(string.IsNullOrEmpty(style) ? "Src" : $"Src{style.GetHashCode():x8}");
                    break;
                case EDrawPhase.UpdateProperty:
                    if (_dirtyElements.Contains(_uiElementSerialNumGenerator))
                        UpdateScrollView(_uiElementSerialNumGenerator, height);
                    break;
                case EDrawPhase.RecreateUgui:
                    BeginScrollViewInternal(_uiElementSerialNumGenerator, height, style);
                    break;
            }
        }

        public void EndScrollView()
        {
            switch (_currentPhase)
            {
                case EDrawPhase.CalculateHash:
                    AppendWindowStructure("ScrEnd");
                    return;
                case EDrawPhase.RecreateUgui:
                    EndLayoutInternal();
                    break;
            }
        }
        
        public void BeginHorizontalLayout(float height, string style)
        {
            _uiElementSerialNumGenerator++;
            switch (_currentPhase)
            {
                case EDrawPhase.CalculateHash:
                    var elementPropertyHash = height.GetHashCode();
                    CheckDirtyElement(elementPropertyHash);
                    AppendWindowStructure(string.IsNullOrEmpty(style) ? "Hor" : $"Hor{style.GetHashCode():x8}");
                    break;
                case EDrawPhase.UpdateProperty:
                    if (_dirtyElements.Contains(_uiElementSerialNumGenerator))
                        UpdateHorizontalLayout(_uiElementSerialNumGenerator, height);
                    break;
                case EDrawPhase.RecreateUgui:
                    BeginHorizontalLayoutInternal(_uiElementSerialNumGenerator, height, style);
                    break;
            }
        }

        public void EndHorizontalLayout()
        {
            switch (_currentPhase)
            {
                case EDrawPhase.CalculateHash:
                    AppendWindowStructure("HorEnd");
                    return;
                case EDrawPhase.RecreateUgui:
                    EndLayoutInternal();
                    break;
            }
        }
        
        public void BeginVerticalLayout(float width, string style)
        {
            _uiElementSerialNumGenerator++;
            switch (_currentPhase)
            {
                case EDrawPhase.CalculateHash:
                    var elementPropertyHash = width.GetHashCode();
                    CheckDirtyElement(elementPropertyHash);
                    AppendWindowStructure(string.IsNullOrEmpty(style) ? "Ver" : $"Ver{style.GetHashCode():x8}");
                    break;
                case EDrawPhase.UpdateProperty:
                    if (_dirtyElements.Contains(_uiElementSerialNumGenerator))
                        UpdateVerticalLayout(_uiElementSerialNumGenerator, width);
                    break;
                case EDrawPhase.RecreateUgui:
                    BeginVerticalLayoutInternal(_uiElementSerialNumGenerator, width, style);
                    break;
            }
        }

        public void EndVerticalLayout()
        {
            switch (_currentPhase)
            {
                case EDrawPhase.CalculateHash:
                    AppendWindowStructure("VerEnd");
                    return;
                case EDrawPhase.RecreateUgui:
                    EndLayoutInternal();
                    break;
            }
        }
        #endregion

        private void AppendWindowStructure(string structureName)
        {
            _windowStructureTemp.Append(structureName);
            if (_windowStructureTemp.Length > WindowStructureLengthLimit)
            {
                Debug.LogError(_windowStructureTemp.ToString());
                throw new ImuguiException();
            }
        }

        private void UpdateDrawCommand()
        {
            using var itor = AddedWindows.GetEnumerator();
            while (itor.MoveNext())
            {
                _currentPhase = EDrawPhase.CalculateHash;
                _uiElementSerialNumGenerator = 0;
                _windowStructureTemp.Clear();
                _currentWindow = itor.Current.Key;
                if (!_lastDrawCommandPropertyHash.TryGetValue(_currentWindow, out var detailHashDic))
                {
                    detailHashDic = new Dictionary<int, int>();
                    _lastDrawCommandPropertyHash.Add(_currentWindow, detailHashDic);
                }
                _dirtyElements.Clear();
                _lastDrawCommandStructureHash.TryGetValue(_currentWindow, out var lastHash);
                // loop 0, generate structure hash and check element hash
                _currentWindow.OnImu();
                
                var newHash = _windowStructureTemp.ToString().GetHashCode();
                // if this window is dirty, redraw it
                if (lastHash != newHash)
                {
                    _currentPhase = EDrawPhase.RecreateUgui;
                    detailHashDic.Clear();
                    _uiElementSerialNumGenerator = 0;
                    ClearUguiWindow(_currentWindow);
                    BeginImugui(_currentWindow);
                    // loop 1.0, recreate structure and element
                    _currentWindow.OnImu();
                    EndImugui(_currentWindow);
                    _lastDrawCommandStructureHash[_currentWindow] = newHash;
                    Debug.Log($"Imugui recreate [ {_currentWindow} ], element number: [ {_uiElementSerialNumGenerator} ], structureTempStrLen: [ {_windowStructureTemp.Length} ]");
                }
                else if (_dirtyElements.Any())
                {
                    _currentPhase = EDrawPhase.UpdateProperty;
                    _uiElementSerialNumGenerator = 0;
                    BeginImugui(_currentWindow);
                    // loop 1.1, update element property
                    _currentWindow.OnImu();
                    EndImugui(_currentWindow);
                    Debug.Log($"Imugui refresh [ {_currentWindow} ], dirty element number: [ {_dirtyElements.Count()} ]");
                }
            }
        }
        
        private int GetOptionArrayHashCode(GUILayoutOption[] options)
        {
            var hashCode = 0;
            foreach (var option in options)
            {
                hashCode += option.type.GetHashCode() + option.value.GetHashCode();
            }

            return hashCode;
        }

        private void CheckDirtyElement(int elementPropertyHash)
        {
            if (_lastDrawCommandPropertyHash[_currentWindow]
                    .TryGetValue(_uiElementSerialNumGenerator, out var lastElementPropertyHash) &&
                lastElementPropertyHash != elementPropertyHash)
            {
                _lastDrawCommandPropertyHash[_currentWindow][_uiElementSerialNumGenerator] =
                    elementPropertyHash;
                _dirtyElements.Add(_uiElementSerialNumGenerator);
            }
            else
            {
                _lastDrawCommandPropertyHash[_currentWindow][_uiElementSerialNumGenerator] = elementPropertyHash;
            }
        }
    }
}