using UnityEngine;

namespace imugui.runtime
{
        
    public class ImuguiBehaviour : MonoBehaviour, IImuguiWindow
    {
        protected static ImuguiComponent Imu => ImuguiComponent.Instance;
        private bool _inited;
        private bool _enabled;
        protected virtual void Init()
        {
            ImuguiComponent.Instance.AddWindow(this, transform.lossyScale);
            _inited = true;
        }

        protected virtual void OnDisable()
        {
            _enabled = false;
            ImuguiComponent.Instance.DisableWindow(this);
        }

        protected virtual void OnDestroy()
        {
            ImuguiComponent.Instance.RemoveWindow(this);
        }

        protected virtual void Update()
        {
            if (!_inited) Init();
            if (!_enabled)
            {
                ImuguiComponent.Instance.EnableWindow(this);
                _enabled = true;
            }
            ImuguiComponent.Instance.SyncTrans(this, transform);
        }

        protected virtual void OnPreRender()
        {
            if (!_inited) return;
            if (!_enabled) return;
            ImuguiComponent.Instance.SyncTrans(this, transform);
        }

        public virtual void OnImu()
        {
        
        }
    }
}
