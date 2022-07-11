using UnityEngine;

namespace imugui.runtime
{
        
    public class ImuguiBehaviour : MonoBehaviour, IImuguiWindow
    {
        protected static ImuguiComponent Imu => ImuguiComponent.Instance;
        protected virtual void Awake()
        {
            ImuguiComponent.Instance.AddWindow(this);
        }

        protected virtual void OnEnable()
        {
            ImuguiComponent.Instance.EnableWindow(this);
        }

        protected virtual void OnDisable()
        {
            ImuguiComponent.Instance.DisableWindow(this);
        }

        protected virtual void OnDestroy()
        {
            ImuguiComponent.Instance.RemoveWindow(this);
        }

        protected virtual void Update()
        {
            ImuguiComponent.Instance.SyncTrans(this, transform);
        }

        public virtual void OnImu()
        {
        
        }
    }
}
