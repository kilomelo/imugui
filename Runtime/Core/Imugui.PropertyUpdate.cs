using UnityEngine.Events;

namespace imugui.runtime
{

    public partial class Imugui
    {
        private void UpdateLabel(int serialNum, string content, params GUILayoutOption[] options)
        {
            if (null == _drawingHandler.DrawingView) throw new ImuguiException();
            var layoutElement = _drawingHandler.DrawingView.UpdateLabel(serialNum, content);
            _drawingHandler.DrawingView.ApplyLayoutOption(layoutElement, options);
        }
        
        private void UpdateButton(int serialNum, string content, UnityAction callback, params GUILayoutOption[] options)
        {
            if (null == _drawingHandler.DrawingView) throw new ImuguiException();
            var layoutElement = _drawingHandler.DrawingView.UpdateButton(serialNum, content, callback);
            _drawingHandler.DrawingView.ApplyLayoutOption(layoutElement, options);
        }
        
        private void UpdateToggle(int serialNum, string content, bool isOn, UnityAction<bool> callback, params GUILayoutOption[] options)
        {
            if (null == _drawingHandler.DrawingView) throw new ImuguiException();
            var layoutElement = _drawingHandler.DrawingView.UpdateToggle(serialNum, content, isOn, callback);
            _drawingHandler.DrawingView.ApplyLayoutOption(layoutElement, options);
        }
        
        private void UpdateSpace(int serialNum, float width)
        {
            if (null == _drawingHandler.DrawingView) throw new ImuguiException();
            _drawingHandler.DrawingView.UpdateSpace(serialNum, width);
        }
        
        private void UpdateVerticalSpace(int serialNum, float height)
        {
            if (null == _drawingHandler.DrawingView) throw new ImuguiException();
            _drawingHandler.DrawingView.UpdateVerticalSpace(serialNum, height);
        }
        
        private void UpdateScrollView(int serialNum, float height)
        {
            if (null == _drawingHandler.DrawingView) throw new ImuguiException();
            _drawingHandler.DrawingView.UpdateScrollView(serialNum, height);
        }
        
        private void UpdateHorizontalLayout(int serialNum, float height)
        {
            if (null == _drawingHandler.DrawingView) throw new ImuguiException();
            _drawingHandler.DrawingView.UpdateHorizontalLayout(serialNum, height);
        }
        
        private void UpdateVerticalLayout(int serialNum, float width)
        {
            if (null == _drawingHandler.DrawingView) throw new ImuguiException();
            _drawingHandler.DrawingView.UpdateVerticalLayout(serialNum, width);
        }
    }
}