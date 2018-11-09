using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore.UI
{
    /// <summary>
    /// UI面板的基类
    /// </summary>
    public class UIPanelBase : UIComponentBase
    {
        protected override void Awake()
        {
            base.Awake();
            
        }
		public void Open(System.Object openParam = null)
		{
			this.Visible = true;
			OnOpen(openParam);
		}
		public void Close(System.Object closeParam = null)
		{
			this.Visible = false;
			OnClose(closeParam);
		}
		protected virtual void OnOpen(System.Object openParam = null)
		{ }
		protected virtual void OnClose(System.Object closeParam = null)
		{ }
        public virtual bool Visible
        {
            protected set
            {
                gameObject.SetActive(value);
            }
            get
            {
                return gameObject.activeInHierarchy;
            }
        }
        
    }
}
