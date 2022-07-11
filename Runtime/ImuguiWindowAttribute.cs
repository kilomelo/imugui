using System;
using UnityEngine;

namespace imugui.runtime
{
    
    [AttributeUsage(AttributeTargets.Class)]
    public class ImuguiWindowAttribute : Attribute
    {
        public Vector2 Offset
        {
            get;
        }

        public Vector2 Size
        {
            get;
        }
        
        public Imugui.EAnchorMode AnchorMode
        {
            get;
        }
        
        public float Scale
        {
            get;
        }

        public string Style
        {
            get;
        }

        public ImuguiWindowAttribute(Imugui.EAnchorMode anchorMode, float offsetX, float offsetY,
            float width, float height, float scale = Imugui.ImuguiWindowDefaultScale, string style = null)
        {
            AnchorMode = anchorMode;
            Offset = new Vector2(offsetX, offsetY);
            Size = new Vector2(width, height);
            Scale = scale;
            Style = style;
        }
    }
}