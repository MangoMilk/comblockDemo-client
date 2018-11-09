using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MMODEMO.UI;
using GameCore;
namespace GameLogic
{
	/// <summary>
	/// 逻辑层的场景管理器，管理场景切换时的资源
	/// </summary>
	public class LogicSceneMgr
	{
		static LogicSceneMgr _instance;
		public static LogicSceneMgr Instance
		{
			get
			{
				if (_instance == null)
					_instance = new LogicSceneMgr();
				return _instance;
			}
		}
		public enum SceneType
		{
			__UNKNOWN__,
			/// <summary>
			/// 刚开启的场景
			/// </summary>
			__STARTUP__,
			LOGON,
			ENTERGAME,
		
			WORLD
		}
		/// <summary>
		/// 内置的场景名常量
		/// </summary>
		class Scenes
		{
			public const string login = "login";
			public const string enterGame = "enterGame";
			public const string world = "world";
		}
		Dictionary<SceneType, string> _dic = new Dictionary<SceneType, string>();
		SceneType _currentScene;
		private LogicSceneMgr()
		{
			Debug.LogFormat("test:scenePathAt0={0}", SceneUtility.GetScenePathByBuildIndex(0));
		}
		public SceneType Current { get { return _currentScene; } }
		public void Init()
		{
			_currentScene = SceneType.__STARTUP__;
			_dic.Add(SceneType.LOGON, Scenes.login);
			_dic.Add(SceneType.ENTERGAME, Scenes.enterGame);
			
			_dic.Add(SceneType.WORLD, Scenes.world);

			SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
			SceneManager.sceneLoaded += SceneManager_sceneLoaded;
		}

		private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
		{
			string name = arg0.name;
			Debug.LogFormat("scene loaded, name={0}", name);
			SceneType scene = Find(name);
			if (scene == SceneType.__UNKNOWN__)
			{
				Debug.LogErrorFormat("场景({0})加载完毕，但是该场景未被管理!", name);
				return;
			}
			_currentScene = scene;
			if (name == Scenes.login)
			{
				UIManager.Instance.OpenPanel(UIPanelType.LOGON);
			}
			else if (name == Scenes.enterGame)
			{
				UIManager.Instance.OpenPanel(UIPanelType.ENTER_GAME);
			}
			else if (name == Scenes.world)
			{
				OnEnterWorld();
			}
		}

		private void SceneManager_sceneUnloaded(Scene arg0)
		{
			string name = arg0.name;
			Debug.LogFormat("scene unloaded, name={0}", name);
			if (name == Scenes.login)
			{
				UIManager.Instance.DestroyPanel(UIPanelType.LOGON);
				UIManager.Instance.DestroyPanel(UIPanelType.CREATE_ACCOUNT);
			}
			else if (name == Scenes.enterGame)
			{
				UIManager.Instance.DestroyPanel(UIPanelType.ENTER_GAME);
			}
			else if (name == Scenes.world)
			{
				UIManager.Instance.DestroyPanel(UIPanelType.MAIN);
				//归还资源
				ResourceManager.Instance.ReturnResource(this, "prefab_world");
			}
		}

		public void Load(SceneType scene)
		{
            if (scene == SceneType.WORLD)
            {//如果加载世界，则需要暂停事件系统，等世界场景加载完毕后resume，否则这个过程中会丢失事件。
                KBEngine.Event.pause();
            }
			SceneManager.LoadScene(_dic[scene]);
		}

		SceneType Find(string sceneName)
		{
			foreach (var pair in _dic)
			{
				if (pair.Value == sceneName)
					return pair.Key;
			}
			return SceneType.__UNKNOWN__;
		}
		void OnEnterWorld()
		{
			//请求资源
			ResourceManager.Instance.GetResource(this, "prefab_world", OnWorldEntryResLoaded);
		}
		void OnWorldEntryResLoaded(string resName, string resPath, UnityEngine.Object resObject, System.Object userData)
		{
			GameObject instance = GameObject.Instantiate(resObject) as GameObject;
			Transform trans = instance.transform;
			trans.localPosition = Vector3.zero;
			trans.localRotation = Quaternion.identity;
			trans.localScale = Vector3.one;
			//增加逻辑组件
			World world = instance.AddComponent<World>();
			world.OnStarted += World_OnStarted;
			//初始化UI
			UIManager.Instance.OpenPanel(UIPanelType.MAIN);
		}

		private void World_OnStarted()
		{
			World.Instance.OnStarted -= World_OnStarted;
			//恢复事件系统，把之前累计的事件激发出来
			KBEngine.Event.resume();
		}
	}
}
