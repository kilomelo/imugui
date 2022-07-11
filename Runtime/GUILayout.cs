namespace imugui.runtime
{
    public sealed class GUILayoutOption
    {
        internal GUILayoutOption.Type type;
        internal object value;

        internal GUILayoutOption(GUILayoutOption.Type type, object value)
        {
            this.type = type;
            this.value = value;
        }

        internal enum Type
        {
            fixedWidth,
            fixedHeight,
            minWidth,
            maxWidth,
            minHeight,
            maxHeight,
            // stretchWidth,
            // stretchHeight,
            // alignStart,
            // alignMiddle,
            // alignEnd,
            // alignJustify,
            // equalSize,
            // spacing,
        }
    }
    
    public sealed class GUILayout
    {
        public const float DefaultLineHeight = 36f;
        public const float DefaultIndentationWidth = 40f;
        public const float DefaultPadding = 8f;
        public const float DefaultSpacing = 2f;

        /// <summary>
        ///   <para>Option passed to a control to give it an absolute width.</para>
        /// </summary>
        /// <param name="width"></param>
        public static GUILayoutOption Width(float width) =>
            new GUILayoutOption(GUILayoutOption.Type.fixedWidth, (object) width);

        /// <summary>
        ///         <para>Option passed to a control to specify a minimum width.
        /// </para>
        ///       </summary>
        /// <param name="minWidth"></param>
        public static GUILayoutOption MinWidth(float minWidth) =>
            new GUILayoutOption(GUILayoutOption.Type.minWidth, (object) minWidth);

        /// <summary>
        ///   <para>Option passed to a control to specify a maximum width.</para>
        /// </summary>
        /// <param name="maxWidth"></param>
        public static GUILayoutOption MaxWidth(float maxWidth) =>
            new GUILayoutOption(GUILayoutOption.Type.maxWidth, (object) maxWidth);

        /// <summary>
        ///   <para>Option passed to a control to give it an absolute height.</para>
        /// </summary>
        /// <param name="height"></param>
        public static GUILayoutOption Height(float height) =>
            new GUILayoutOption(GUILayoutOption.Type.fixedHeight, (object) height);

        /// <summary>
        ///   <para>Option passed to a control to specify a minimum height.</para>
        /// </summary>
        /// <param name="minHeight"></param>
        public static GUILayoutOption MinHeight(float minHeight) =>
            new GUILayoutOption(GUILayoutOption.Type.minHeight, (object) minHeight);

        /// <summary>
        ///   <para>Option passed to a control to specify a maximum height.</para>
        /// </summary>
        /// <param name="maxHeight"></param>
        public static GUILayoutOption MaxHeight(float maxHeight) =>
            new GUILayoutOption(GUILayoutOption.Type.maxHeight, (object) maxHeight);

        /// <summary>
        ///   <para>Option passed to a control to allow or disallow horizontal expansion.</para>
        /// </summary>
        /// <param name="expand"></param>
        // public static GUILayoutOption ExpandWidth(bool expand) =>
            // new GUILayoutOption(GUILayoutOption.Type.stretchWidth, (object) (expand ? 1 : 0));

        /// <summary>
        ///   <para>Option passed to a control to allow or disallow vertical expansion.</para>
        /// </summary>
        /// <param name="expand"></param>
        // public static GUILayoutOption ExpandHeight(bool expand) =>
            // new GUILayoutOption(GUILayoutOption.Type.stretchHeight, (object) (expand ? 1 : 0));
    }
}