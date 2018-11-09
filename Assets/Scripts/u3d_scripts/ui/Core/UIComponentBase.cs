using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameCore.UI
{
    /// <summary>
    /// ui的基类
    /// </summary>
    public class UIComponentBase : UnityComponent
    {
		RectTransform _rectTrans;
		protected override void Awake()
		{
			base.Awake();
			InitComponent();
		}
		public new RectTransform Trans
		{
			get
			{
				if (_rectTrans == null)
					_rectTrans = base.Trans as RectTransform;
				return _rectTrans;
			}
		}

        protected virtual void OnVisible()
        {

        }

        protected virtual void OnInvisible()
        {

        }

		protected override void OnEnable()
		{
			base.OnEnable();
			if (Application.isPlaying)
			{
				OnVisible();
			}
		}

		protected override void OnDisable()
		{
			if (Application.isPlaying)
			{
				OnInvisible();
			}
			base.OnDisable();
		}
		
		protected virtual void InitComponent()
		{ }

		#region common tools

		//protected static T CreateTemplateInstance<T>(T template,Transform parent = null) where T : UIComponentBase
		//{
		//	if (template == null)
		//		return null;
		//	T result = GameObject.Instantiate<T>(template, parent);
		//	result.gameObject.SetActive(true);
		//	return result;
		//}
		//protected static T CreateTemplateInstance<T>(GameObject templateGo, Transform parent = null) where T : UIComponentBase
		//{
		//	if (templateGo == null)
		//		return null;
		//	T template = templateGo.GetComponent<T>();
		//	if (template == null)
		//		template = templateGo.AddComponent<T>();
		//	return CreateTemplateInstance(template, parent);
		//}
		//protected static T CreateTemplateInstance<T>(Transform templateTrans, Transform parent = null) where T : UIComponentBase
		//{
		//	if (templateTrans == null)
		//		return null;
		//	T template = templateTrans.GetComponent<T>();
		//	if (template == null)
		//		template = templateTrans.gameObject.AddComponent<T>();
		//	return CreateTemplateInstance(template, parent);
		//}

		

		


		protected void AddBtnEvt(GameObject btnGo, UnityAction callback)
        {
            Button btn = btnGo.GetComponent<Button>();
            if (btn == null)
            {
                Debug.LogErrorFormat("目标go({0}), 不含有Button组件，无法添加按钮事件", btnGo.name);
                return;
            }
            btn.onClick.AddListener(callback);
        }
        protected void RemoveBtnEvt(GameObject btnGo, UnityAction callback)
        {
            Button btn = btnGo.GetComponent<Button>();
            if (btn == null)
            {
                Debug.LogErrorFormat("目标go({0}), 不含有Button组件，无法移除按钮事件", btnGo.name);
                return;
            }
            btn.onClick.RemoveListener(callback);
        }
        protected void AddBtnEvt(Button btn, UnityAction callback)
        {
            btn.onClick.AddListener(callback);
        }
        protected void RemoveBtnEvt(Button btn, UnityAction callback)
        {
            btn.onClick.RemoveListener(callback);
        }

        #endregion
    }

}