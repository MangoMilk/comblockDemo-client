using System; 
using System.Collections.Generic;

using UnityEngine;

using KBEngine;
using GameCore;
using GameLogic.Components;


/// <summary>
/// 在当前空间场景里的World脚本
/// </summary>
public class World : UnityComponent
{
	public delegate void OnEntityHandler(EntityUnityComponent entity);

	class ResNames
	{
		/// <summary>
		/// 所有entity
		/// </summary>
		public const string Entity = "prefab_entity";
		/// <summary>
		/// 自己的avatar
		/// </summary>
		public const string Avatar_Player = "prefab_avatar_player";
		public const string Terrain = "prefab_terrain";
	}
	static World _instance;
	public static World Instance
	{
		get { return _instance; }
	}

	private UnityEngine.GameObject _terrain = null;
	private UnityEngine.GameObject _player = null;
	EntityUnityComponent _playerEntity;


	Dictionary<Int32, BaseSynchronizer> _entitySynchronizerDict = new Dictionary<int, BaseSynchronizer>();
    Dictionary<int, EntityUnityComponent> _entityDict = new Dictionary<int, EntityUnityComponent>();
    List<EntityUnityComponent> _entityList = new List<EntityUnityComponent>();

	#region 事件

	/// <summary>
	/// 在World已经Started时激发。
	/// </summary>
	public event Action OnStarted;
	/// <summary>
	/// 有entity进入世界，包括自己
	/// </summary>
	public event OnEntityHandler OnEntityEnter;
	/// <summary>
	/// 
	/// </summary>
	public event Action<int> OnEntityLeave;
	/// <summary>
	/// 自己进入世界，有些UI需要等待自己的完成。可在该事件中处理
	/// </summary>
	public event OnEntityHandler OnMyselfEnter;

	#endregion

	protected override void Awake()
	{
		base.Awake();
		if (_instance != null)
		{
			throw new Exception("World::已经存在instance！！");
		}
		_instance = this;
	}
	protected override void Start()
	{
		base.Start();
		InstallEvents();
		LoadTerrain("");

		if (this.OnStarted != null)
			this.OnStarted();
        
		Debug.Log("World started!");
    }

	

    protected override void OnDestroy()
	{
        UninstallEvents();

		foreach (var c in _entityList)
		{
			GameObject.Destroy(c.gameObject);
		}
		_entityDict.Clear();
		_entityDict.Clear();
		_entitySynchronizerDict.Clear();

		_instance = null;

		OnStarted = null;
		OnEntityEnter = null;
		OnEntityLeave = null;
		OnMyselfEnter = null;

		if (_terrain != null)
		{
			GameObject.Destroy(_terrain);
			_terrain = null;
		}

		base.OnDestroy();
	}

	protected override void Update()
	{
		base.Update();

		//if (Input.GetMouseButton (0))     
		//{   
		//	// 射线选择，攻击
		//	if(Camera.main)
		//	{
		//		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);   
		//		RaycastHit hit;   

		//		if (Physics.Raycast(ray, out hit))     
		//		{   
		//			Debug.DrawLine (ray.origin, hit.point); 
		//			UnityEngine.GameObject gameObj = hit.collider.gameObject;
		//			if(gameObj.name.IndexOf("terrain") == -1)
		//			{
		//				string[] s = gameObj.name.Split(new char[]{'_' });

		//				if(s.Length > 0)
		//				{
		//					int targetEntityID = Convert.ToInt32(s[s.Length - 1]);
		//					KBEngine.Event.fireIn("useTargetSkill", (Int32)1, (Int32)targetEntityID);	
		//				}	
		//			}
		//		}  
		//	}
		//}    
	}


    public List<EntityUnityComponent> EntityList
    {
        get { return _entityList; }
    }
	public EntityUnityComponent FindEntity(int entityId)
	{
		EntityUnityComponent r = null;
		if (_entityDict.TryGetValue(entityId, out r))
			return r;
		return null;
	}
	/// <summary>
	/// 自己的实体是否已经进入
	/// </summary>
	public bool MyselfEntered { get { return _player != null; } }
	public EntityUnityComponent Myself { get { return _playerEntity; } }

    void InstallEvents()
    {
        // in world
        //KBEngine.Event.registerOut("addSpaceGeometryMapping", this, "addSpaceGeometryMapping");
        //KBEngine.Event.registerOut("onEnterWorld", this, "onEnterWorld");
        //KBEngine.Event.registerOut("onLeaveWorld", this, "onLeaveWorld");
        //KBEngine.Event.registerOut("set_position", this, "set_position");
        //KBEngine.Event.registerOut("set_direction", this, "set_direction");
        //KBEngine.Event.registerOut("updatePosition", this, "updatePosition");
        //KBEngine.Event.registerOut("onControlled", this, "onControlled");

        EngineEvents.OnAddSpaceGeometryMapping += EngineEvents_OnAddSpaceGeometryMapping;
        EngineEvents.OnEnterWorld += onEnterWorld;
        EngineEvents.OnLeaveWorld += onLeaveWorld;
        EngineEvents.OnSetPosition += EngineEvents_OnSetPosition;
        EngineEvents.OnSetDirection += EngineEvents_OnSetDirection;
        EngineEvents.OnUpdatePosition += EngineEvents_OnUpdatePosition;
        //EngineEvents.OnControlled += EngineEvents_OnControlled;

        
    }
    void UninstallEvents()
    {
        //KBEngine.Event.deregisterOut(this);
        EngineEvents.OnAddSpaceGeometryMapping -= EngineEvents_OnAddSpaceGeometryMapping;
        EngineEvents.OnEnterWorld -= onEnterWorld;
        EngineEvents.OnLeaveWorld -= onLeaveWorld;
        EngineEvents.OnSetPosition -= EngineEvents_OnSetPosition;
        EngineEvents.OnSetDirection -= EngineEvents_OnSetDirection;
        EngineEvents.OnUpdatePosition -= EngineEvents_OnUpdatePosition;
        //EngineEvents.OnControlled -= EngineEvents_OnControlled;
    }
    
    void LoadTerrain(string resPath)
    {
        Debug.Log("loading terrain res(" + resPath + ")...");
        if (_terrain == null)
        {
            ResourceManager.Instance.GetResource(this, ResNames.Terrain, this.OnTerrainResLoaded);
        }
    }
	void FinishMyselfLoadTask()
	{
		//_playerController.Enable();
		if (OnMyselfEnter != null)
			OnMyselfEnter(_playerEntity);
	}
    void CreateEntity(Entity entity)
	{
		//todo: 增加一个Entity创建工厂
		string resName = null;
		if (entity.isPlayer())
		{
			resName = ResNames.Avatar_Player;
			Debug.Log("自己的实体创建中...");
		}
		else
		{
			resName = ResNames.Entity;
		}
		
		ResourceManager.Instance.GetResource(this, resName, this.OnEntityResLoaded, entity);
	}
	void OnEntityResLoaded(string resName, string resPath, UnityEngine.Object resObject, System.Object userData)
	{
		IGameObject go = userData as IGameObject;
		Entity entity = userData as Entity;
		if (go == null || entity == null)
			throw new Exception("World::OnResLoaded: userData cannot transfer to IGameObject or Entity");
		if (KBEngineApp.app.findEntity(entity.id) == null)
		{
			Debug.LogWarningFormat("World::OnEntityResLoaded:发现entity已经不存在，则不需要加载资源了");
			return;
		}
		float y = entity.position.y;
		//if (entity.isOnGround)
		//	y = 1.3f;
		Vector3 pos = new Vector3(entity.position.x, y, entity.position.z);
		Quaternion rotation = Quaternion.Euler(EngineUtils.EngineDirectionToUnity(entity.direction));
		GameObject renderObj = GameObject.Instantiate(resObject, pos, rotation) as GameObject;
		renderObj.name = entity.className + "_" + entity.id;
		//该方法会由Entity的实现类去完成Unity相关组件的添加和初始化
		go.SetRenderObj(renderObj);
		var synchronizer = renderObj.GetComponent<BaseSynchronizer>();
		if (synchronizer != null)
		{
			synchronizer.SetDestPosition(entity.position);
			synchronizer.SetDestDirection(EngineUtils.EngineDirectionToUnity(entity.direction));
			synchronizer.SpaceID = KBEngineApp.app.spaceID;
			_entitySynchronizerDict.Add(entity.id, synchronizer);
		}
		

		
		EntityUnityComponent component = null;
		if (entity.isPlayer())
		{//如果是玩家自己，需要先disable一下，等待地形完成加载

			_playerEntity = renderObj.GetComponent<EntityUnityComponent>();
		}
		component = renderObj.GetComponent<EntityUnityComponent>();

		if (component == null)
		{
			Debug.LogErrorFormat("Entity资源加载后，竟然没有EntityUnityComponent！,entityId={0}, className={1}", entity.id, entity.className);
			return;
		}
		_entityList.Add(component);
		_entityDict.Add(entity.id, component);

		if (OnEntityEnter != null)
			OnEntityEnter(component);

		if (entity.isPlayer())
		{
			if (_terrain != null)
			{
				FinishMyselfLoadTask();
			}
		}

	}


	private void EngineEvents_OnAddSpaceGeometryMapping(string resPath)
	{
        LoadTerrain(resPath);

    }
   

	private void onEnterWorld(Entity entity)
	{
		Debug.LogFormat("World::Entity(id={0},class={1}) enter world.", entity.id, entity.className);
		CreateEntity(entity);
	}
	private void onLeaveWorld(Entity entity)
	{
		Debug.LogFormat("World::Entity(id={0},class={1}) leave world.", entity.id, entity.className);
		EntityUnityComponent component = null;
        if (!_entityDict.TryGetValue(entity.id, out component))
            return;
		if (entity.renderObj == null)
			return;
		if (component == null)
		{
			//Debug.LogErrorFormat("Entity资源加载后，竟然没有EntityUnityComponent！,entityId={0}, className={1}", entity.id, entity.className);
			return;
		}
		_entitySynchronizerDict.Remove(entity.id);

		if (OnEntityLeave != null)
			OnEntityLeave(entity.id);
        _entityDict.Remove(entity.id);
        _entityList.Remove(component);
		UnityEngine.GameObject.Destroy((UnityEngine.GameObject)entity.renderObj);
		entity.renderObj = null;
	}



	private void EngineEvents_OnSetPosition(Entity entity)
	{
		if (entity.renderObj == null)
			return;
		BaseSynchronizer s = null;
		if (!_entitySynchronizerDict.TryGetValue(entity.id, out s))
			return;
		s.SetDestPosition(entity.position);
		s.position = entity.position;
		s.SpaceID = KBEngineApp.app.spaceID;
		//GameEntity gameEntity = ((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>();
		//      gameEntity.destPosition = entity.position;
		//      gameEntity.position = entity.position;
		//      gameEntity.spaceID = KBEngineApp.app.spaceID;
	}

	private void EngineEvents_OnSetDirection(Entity entity)
	{
		if (entity.renderObj == null)
			return;
		BaseSynchronizer s = null;
		if (!_entitySynchronizerDict.TryGetValue(entity.id, out s))
			return;
		//服务器上是roll, pitch, yaw，所以是y，z，x对应unity的x，y，z
		s.SetDestDirection(EngineUtils.EngineDirectionToUnity(entity.direction));
		s.SpaceID = KBEngineApp.app.spaceID;
		//GameEntity gameEntity = ((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>();
		//gameEntity.destDirection = new Vector3(entity.direction.y, entity.direction.z, entity.direction.x);
		//gameEntity.spaceID = KBEngineApp.app.spaceID;
	}

	private void EngineEvents_OnUpdatePosition(Entity entity)
	{
		if (entity.renderObj == null)
			return;
		BaseSynchronizer s = null;
		if (!_entitySynchronizerDict.TryGetValue(entity.id, out s))
			return;
		s.SetDestPosition(entity.position);
		s.SpaceID = KBEngineApp.app.spaceID;
		//GameEntity gameEntity = ((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>();
		//      gameEntity.destPosition = entity.position;
		//      gameEntity.isOnGround = entity.isOnGround;
		//      gameEntity.spaceID = KBEngineApp.app.spaceID;
	}


	void OnTerrainResLoaded(string resName, string resPath, UnityEngine.Object resObject, System.Object userData)
	{
		_terrain = GameObject.Instantiate(resObject) as GameObject;
		if (_playerEntity != null)
		{
			FinishMyselfLoadTask();
		}
	}
	

}