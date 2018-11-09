using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameCore;
using GameCore.UI;
using GameLogic;
namespace MMODEMO.UI
{
	/// <summary>
	/// 所有面板的枚举罗列
	/// </summary>
	public enum UIPanelType
	{
		LOGON,
		CREATE_ACCOUNT,
		ENTER_GAME,
		MAIN
	}
	/// <summary>
	/// 管理UI的管理器
	/// </summary>
	public class UIManager
	{
		static UIManager _instance;
		public static UIManager Instance
		{
			get
			{
				if (_instance == null)
					_instance = new UIManager();
				return _instance;
			}
		}
		static T EnsureComponent<T>(GameObject target) where T : Component
		{
			T temp = target.GetComponent<T>();
			if (temp == null)
			{
				target.AddComponent<T>();
			}

			return temp;
		}
		/// <summary>
		/// UI系统下的实例化
		/// </summary>
		/// <param name="prefab"></param>
		/// <param name="parent"></param>
		public static GameObject Instantiate(GameObject prefab, Transform parent)
		{
			GameObject instance = GameObject.Instantiate(prefab, parent);
			Transform trans = instance.transform;
			trans.localPosition = Vector3.zero;
			trans.localRotation = Quaternion.identity;
			trans.localScale = Vector3.one;
			GameObjectUtils.SetLayerRecursively(trans, LayerManager.GetLayerInt(LayerManager.Layer.Internal_UI));
			return instance;
		}
		/// <summary>
		/// 内置资源，不会销毁，直到UIManager销毁
		/// </summary>
		public class InternalResNames
		{
			public const string defaultIcon = "ui_icon_default";
		}
		class ResNames
		{
			
			public const string Logon = "ui_logon";
			public const string CreateAccount = "ui_create_account";
			public const string Main = "ui_main";
		
			public const string EnterGame = "ui_enter_game";
			
		}
		class PanelData
		{
			public string resName;
			public GameObject panelResObject;
			public UIPanelBase instance;
			public UIPanelType type;
			public bool isLoadingResource = false;
			public bool IsReady { get { return this.panelResObject != null; } }
			public string componentName;

			bool _inited = false;

			Component _componentOnPrefabGo;
			bool _componentWasOnPrefab = false;
			/// <summary>
			/// 确保panel上的组件
			/// </summary>
			/// <returns></returns>
			public bool EnsureComponent()
			{
				if (_inited)
					return true;
				Type t = Type.GetType(componentName);
				if (t == null)
				{
					Debug.LogErrorFormat("UIManager+PanelData::EnsureComponent:找不到名为{0}的组件类型", componentName);
					return false;
				}
				if (!t.IsSubclassOf(typeof(UIPanelBase)))
				{
					Debug.LogErrorFormat("UIManager+PanelData::EnsureComponent:名为{0}的组件类型不是一个UIPanelBase类型", componentName);
					return false;
				}
				_componentOnPrefabGo = panelResObject.GetComponent(t);

				if (_componentOnPrefabGo == null)
				{
					_componentWasOnPrefab = false;
					_componentOnPrefabGo = panelResObject.AddComponent(t);
				}
				else
					_componentWasOnPrefab = true;



				_inited = true;
				return true;
			}
			public void Clear()
			{
#if UNITY_EDITOR
				if (!_componentWasOnPrefab && _inited)
				{
					if (_componentOnPrefabGo != null)
					{
						UnityEngine.Debug.LogFormat("UIManager+PanelData::Clear:component on prefab destroyed.Component={0}, go={1}, resName={2}",
							_componentOnPrefabGo.GetType().ToString(), _componentOnPrefabGo.name, resName);
						UnityEngine.Object.DestroyImmediate(_componentOnPrefabGo, true);
						_componentOnPrefabGo = null;

					}
				}
#endif
			}
		}
		abstract class CachedParamBase
		{
			
			public CachedParamBase(string resName,UIPanelType panelType, System.Object uiParam)
			{
				this.ResName = resName;
				this.PanelType = panelType;
				this.UIParam = uiParam;
			}

			public string ResName { get; private set; }
			public UIPanelType PanelType { get; private set; }
			public System.Object UIParam { get; private set; }
			public abstract void Execute();
		}
		class OpenParam : CachedParamBase
		{
			public OpenParam(string resName, UIPanelType panelType, System.Object uiParam)
				: base(resName, panelType, uiParam)
			{ }
			public override void Execute()
			{
				_instance.OpenPanel(PanelType, UIParam);
			}
		}
		class CloseParam : CachedParamBase
		{
			public CloseParam(string resName, UIPanelType panelType, System.Object uiParam)
				: base(resName, panelType, uiParam)
			{

			}
			public override void Execute()
			{
				_instance.ClosePanel(PanelType, UIParam);
			}
		}

		Transform _rootCanvasTrans;
		Canvas _rootCanvas;
		UnityEngine.EventSystems.EventSystem _eventSystem;
		ResourceManager _resManager;
		
		Dictionary<UIPanelType, PanelData> _panelDict = new Dictionary<UIPanelType, PanelData>();
		Dictionary<string, PanelData> _resNameDict = new Dictionary<string, PanelData>();
		Dictionary<string, List<CachedParamBase>> _paramCacheDict = new Dictionary<string, List<CachedParamBase>>();

		Dictionary<string, UnityEngine.Object> _internalResourceDict = new Dictionary<string, UnityEngine.Object>();

		private UIManager()
		{

		}
		public void Init()
		{
			Canvas canvas = UnityEngine.Object.FindObjectOfType<Canvas>();
			if (canvas == null)
				throw new Exception("UIManager::Init:失败，未找到Canvas节点");
			_rootCanvas = canvas;
			_rootCanvasTrans = canvas.transform;
			GameObject.DontDestroyOnLoad(_rootCanvas.gameObject);

			UnityEngine.EventSystems.EventSystem eventSystem = UnityEngine.Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
			if (eventSystem == null)
				throw new Exception("UIManager::Init:失败，未找到EventSystem节点");
			_eventSystem = eventSystem;
			GameObject.DontDestroyOnLoad(_eventSystem.gameObject);

			_resManager = ResourceManager.Instance;
			InitPanelData();
			LoadDefaultResource();

		}
		public void Deinit()
		{
			UnityEngine.Debug.Log("UIManager::Deinit:");
			foreach (var panelData in _panelDict.Values)
			{
				panelData.Clear();
			}
		}
		public UnityEngine.Object GetInternalResource(string resName)
		{
			UnityEngine.Object result = null;
			if (_internalResourceDict.TryGetValue(resName, out result))
				return result;
			return null;
		}
		public Canvas RootCanvas { get { return _rootCanvas; } }
		public void OpenPanel(UIPanelType panelType, System.Object openParam = null)
		{
			PanelData data = null;
			if (!_panelDict.TryGetValue(panelType, out data))
			{
				throw new Exception("UIManager::OpenPanel:can not find paneltype=" + panelType.ToString());
			}
			if (!data.IsReady)
			{
				List<CachedParamBase> cachedParamsList = null;
				if (!_paramCacheDict.TryGetValue(data.resName, out cachedParamsList))
					cachedParamsList = new List<CachedParamBase>();
				cachedParamsList.Add(new OpenParam(data.resName, panelType, openParam));
				_paramCacheDict[data.resName] = cachedParamsList;

				if (!data.isLoadingResource)
				{//只加载一次
					data.isLoadingResource = true;
					_resManager.GetResource(this, data.resName, OnUIResourceLoaded);
				}
				return;
			}
			if (data.instance == null)
			{
				data.instance = CreateUI(data);
			}
			data.instance.Open(openParam);
		}
		public void ClosePanel(UIPanelType panelType, System.Object closeParam = null)
		{
			PanelData data = null;
			if (!_panelDict.TryGetValue(panelType, out data))
			{
				throw new Exception("UIManager::ClosePanel:can not find paneltype=" + panelType.ToString());
			}
			
			if (!data.IsReady)
			{
				//如果资源不在加载中，就没有缓存close的意义
				if (data.isLoadingResource)
				{
					List<CachedParamBase> cachedParamsList = null;
					if (!_paramCacheDict.TryGetValue(data.resName, out cachedParamsList))
					{
						cachedParamsList = new List<CachedParamBase>();
					}
					cachedParamsList.Add(new CloseParam(data.resName, panelType, closeParam));
					_paramCacheDict[data.resName] = cachedParamsList;
				}
				
				return;
			}
			if (data.instance == null)
				return;
			data.instance.Close(closeParam);
		}
		public void DestroyPanel(UIPanelType panelType)
		{
			PanelData data = null;
			if (!_panelDict.TryGetValue(panelType, out data))
			{
				throw new Exception("UIManager::DestroyPanel:can not find paneltype=" + panelType.ToString());
			}
			if (data.instance != null)
			{
				data.instance.Close();
				GameObject.Destroy(data.instance.gameObject);
				data.instance = null;
			}
			if (data.panelResObject != null)
			{
				//这里不进行destroy prefab，由资源管理器负责，此处仅仅取消索引即可
				//GameObject.Destroy(data.panelResObject.gameObject);
				data.panelResObject = null;
			}
			_resManager.ReturnResource(this, data.resName);
		}
		UIPanelBase CreateUI(PanelData panelData)
		{
			//先尝试找到Component类型，接着直接在资源对象上附上组件，这样以后在实例化时，无需再装配组件。
			if (!panelData.EnsureComponent())
			{//确认组件类型是否正确
				return null;
			}
			GameObject prefab = panelData.panelResObject;
			//Type t = Type.GetType(panelData.componentName);
			//if (t == null)
			//{
			//	Debug.LogErrorFormat("UIManager::CreateUI:找不到名为{0}的组件类型", panelData.componentName);
			//	return null;
			//}
			//if (!t.IsSubclassOf(typeof(UIPanelBase)))
			//{
			//	Debug.LogErrorFormat("UIManager::CreateUI:名为{0}的组件类型不是一个UIPanelBase类型", panelData.componentName);
			//	return null;
			//}
			//if (prefab.GetComponent(t) == null)
			//	prefab.AddComponent(t);


			GameObject instance = GameObject.Instantiate(prefab, _rootCanvasTrans);
			Transform trans = instance.transform;
			trans.localPosition = Vector3.zero;
			trans.localRotation = Quaternion.identity;
			trans.localScale = Vector3.one;
			GameObjectUtils.SetLayerRecursively(trans, LayerManager.GetLayerInt(LayerManager.Layer.Internal_UI));

			UIPanelBase panel = instance.GetComponent<UIPanelBase>();
			if (panel == null)
			{
				Debug.LogErrorFormat("UIManager::CreateUI:名为{0}的组件类型不是一个UIPanelBase类型", panelData.componentName);
				return null;
			}
			return panel;
		}
		void OnUIResourceLoaded(string resName, string resPath, UnityEngine.Object resObject,System.Object userData)
		{
			List<CachedParamBase> cachedParamsList = null;
			if (!_paramCacheDict.TryGetValue(resName, out cachedParamsList))
				return;
			if (cachedParamsList.Count > 0)
			{
				UIPanelType panelType = cachedParamsList[0].PanelType;
				PanelData data = _panelDict[panelType];
				data.isLoadingResource = false;
				data.panelResObject = resObject as GameObject;
				if(data.panelResObject == null)
				{
					throw new Exception("UIManager::OnUIResourceLoaded:UI的资源对象应该是一个GameObject，但是无法转换");
				}

				foreach (var cache in cachedParamsList)
				{
					cache.Execute();
				}
			}

			_paramCacheDict.Remove(resName);
		}
		void LoadDefaultResource()
		{
			_resManager.GetResource(this, InternalResNames.defaultIcon, OnDefaultUIResLoaded);
		}
		void OnDefaultUIResLoaded(string resName, string resPath, UnityEngine.Object resObject, System.Object userData)
		{
			//Debug.LogFormat("UIManager::OnDefaultUIResLoaded:resName={0}, resPath={1}, objectName={2}, type={3}", resName, resPath, resObject.name, resObject.GetType());
			_internalResourceDict.Add(resName, resObject);
			
		}
		/// <summary>
		/// 初始化面板的数据，以后替换成从数据表进行读取
		/// </summary>
		void InitPanelData()
		{
			PanelData data = null;
			//logon
			data = new PanelData()
			{
				resName = ResNames.Logon,
				type = UIPanelType.LOGON,
				componentName = "MMODEMO.UI.UILogon"
			};
			_panelDict.Add(data.type, data);
			//MAIN
			data = new PanelData()
			{
				resName = ResNames.Main,
				type = UIPanelType.MAIN,
				componentName = "MMODEMO.UI.UIMain"
			};
			_panelDict.Add(data.type, data);
			//UICreateAccount
			data = new PanelData()
			{
				resName = ResNames.CreateAccount,
				type = UIPanelType.CREATE_ACCOUNT,
				componentName = "MMODEMO.UI.UICreateAccount"
			};
			_panelDict.Add(data.type, data);
			//UIEnterGame
			data = new PanelData()
			{
				resName = ResNames.EnterGame,
				type = UIPanelType.ENTER_GAME,
				componentName = "MMODEMO.UI.UIEnterGame"
			};
			_panelDict.Add(data.type, data);
			
		}
	}
}
