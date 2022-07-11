namespace imugui.runtime
{
        
    public interface IImuguiWindow
    {
        /// <summary>
        /// 有如下限制：
        /// 1、因为这个方法每一帧会被imugui系统掉用多次（不同用途），所以需要保证该方法在同一帧中每次调用执行路径一致，否则会造成UI绘制或布局错误
        /// 2、对于Button、Toggle等需要赋值回调方法的组件，最好不要用复杂逻辑来动态修改回调方法，否则会造成修改回调方法不起作用的bug
        /// 
        /// </summary>
        void OnImu();
    }
}
