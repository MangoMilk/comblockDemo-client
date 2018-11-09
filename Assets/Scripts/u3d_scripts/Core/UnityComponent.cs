using UnityEngine;
using System;
using System.Collections;

namespace GameCore
{
	/// <summary>
	/// 组件基类，unity monobehavior的辅助类，内部提供了一些辅助的方法
	/// </summary>
	public class UnityComponent : MonoBehaviour
	{
		protected Transform _trans;

	    public Transform Trans
		{
			get
			{
				if (_trans == null)
					_trans = this.transform;
				return _trans;
			}
		}
		protected virtual void Awake()
		{
			_trans = this.transform;
		}

		protected virtual void Start()
		{

		}

		

		protected virtual void OnEnable()
		{
			
		}

		protected virtual void OnDisable()
		{
			
		}
		protected virtual void Update()
		{

		}
		protected virtual void FixedUpdate()
		{ }
		protected virtual void OnDestroy()
		{ }
		

		#region common tools

		protected T CreateTemplateInstance<T>(T template, Transform parent = null) where T : UnityComponent
		{
			if (template == null)
				return null;
			if (parent == null)
				parent = Trans;
			T result = GameObject.Instantiate<T>(template, parent);
			result.gameObject.SetActive(true);
			return result;
		}
		protected T CreateTemplateInstance<T>(GameObject templateGo, Transform parent = null) where T : UnityComponent
		{
			if (templateGo == null)
				return null;
			T template = templateGo.GetComponent<T>();
			if (template == null)
				template = templateGo.AddComponent<T>();
			return CreateTemplateInstance(template, parent);
		}
		protected T CreateTemplateInstance<T>(Transform templateTrans, Transform parent = null) where T : UnityComponent
		{
			if (templateTrans == null)
				return null;
			T template = templateTrans.GetComponent<T>();
			if (template == null)
				template = templateTrans.gameObject.AddComponent<T>();
			return CreateTemplateInstance(template, parent);
		}

		protected T GetComponentInChildrenByName<T>(string gameObjectName, Transform parent = null) where T : Component
		{
			GameObject go = GetGameObjectInChildren(gameObjectName, parent);
			if (go == null)
				return null;
			T comp = go.GetComponent<T>();
			if (comp == null)
				Debug.LogErrorFormat("go({0})下没有找到名为{1}的组件{2}", this.gameObject.name, gameObjectName, typeof(T).ToString());
			return comp;
		}

		/// <summary>
		/// Find a GameObject called gameobjectname in mine Transform or others
		/// </summary>
		/// <param name="gameObjectName"></param>
		/// <param name="parent">，</param>
		/// <returns> </returns>
		public GameObject GetGameObjectInChildren(string gameObjectName, Transform parent = null)
		{
			if (parent == null)
			{
				parent = transform;
			}
			GameObject result = _GetGameObjectInChildren(gameObjectName, parent);
			if (result == null)
				Debug.LogErrorFormat("go({0}[{1}])下没有找到名为{2}的GameObject", parent.name, GetType().ToString(), gameObjectName);
			return result;
		}
		private GameObject _GetGameObjectInChildren(string gameObjectName, Transform parent)
		{
			foreach (Transform child in parent)
			{
				if (child.name == gameObjectName)
				{
					return child.gameObject;
				}
				GameObject go = _GetGameObjectInChildren(gameObjectName, child);

				if (go != null)
				{
					return go;
				}
			}
			return null;
		}


		

		#endregion
	}

}
