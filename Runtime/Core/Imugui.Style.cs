using System.Collections.Generic;
using System.Linq;

namespace imugui.runtime
{
    public partial class Imugui
    {
        public static class DefaultStyle
        {
            public const string Canvas = "ImuguiSkin/Default/ImuguiCanvas";
            public const string Label = "ImuguiSkin/Default/Text";
            public const string Button = "ImuguiSkin/Default/Button";
            public const string Toggle = "ImuguiSkin/Default/Toggle";
            public const string InputField = "";
            public const string ScrollView = "ImuguiSkin/Default/ScrollView";
            public const string HorizontalLayout = "ImuguiSkin/Default/HorizontalLayout";
            public const string VerticalLayout = "ImuguiSkin/Default/VerticalLayout";
            public const string Space = "ImuguiSkin/Default/Space";
        }

        public struct ImuguiSkin
        {
            public string Canvas;
            public string Label;
            public string Button;
            public string Toggle;
            public string ScrollView;
            public string InputField;
            public string HorizontalLayout;
            public string VerticalLayout;
            public string Space;
        }

        private const int SkinStackCapacityLimit = 16;
        private Stack<ImuguiSkin> _skinStack = new Stack<ImuguiSkin>();
        private ImuguiSkin _currentSkin;

        public Imugui()
        {
            _currentSkin.Canvas = DefaultStyle.Canvas;
            _currentSkin.Label = DefaultStyle.Label;
            _currentSkin.Button = DefaultStyle.Button;
            _currentSkin.Toggle = DefaultStyle.Toggle;
            _currentSkin.ScrollView = DefaultStyle.ScrollView;
            _currentSkin.HorizontalLayout = DefaultStyle.HorizontalLayout;
            _currentSkin.VerticalLayout = DefaultStyle.VerticalLayout;
            _currentSkin.InputField = DefaultStyle.InputField;
            _currentSkin.Space = DefaultStyle.Space;
        }

        public ImuguiSkin CurrentSkin => _currentSkin;

        public void PushSkin(ImuguiSkin skin)
        {
            if (_skinStack.Count() >= SkinStackCapacityLimit)
                throw new ImuguiException();
            _skinStack.Push(_currentSkin);
            _currentSkin = skin;
        }
        
        public void PopSkin()
        {
            if (!_skinStack.Any())
                throw new ImuguiException();
            _currentSkin = _skinStack.Pop();
        }
    }
}