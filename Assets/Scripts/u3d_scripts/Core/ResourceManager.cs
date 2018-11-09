using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
    /// <summary>
    /// 资源类型，不同资源类型可能读取的资源目录不同
    /// </summary>
    public enum ResourceType
    {
        MUSIC,
        SPEAK,
        UI,
		UI_ICON,
        STATICDATA,
        PREFAB,
        /// <summary>
        /// 可以表示资源类型的数量
        /// </summary>
        NUM
    }
	/// <summary>
	/// 资源加载完毕的回调代理
	/// </summary>
	/// <param name="resName"></param>
	/// <param name="resPath"></param>
	/// <param name="resObject"></param>
	/// <param name="userData"></param>
    public delegate void ResDataLoadedHandler(string resName, string resPath, UnityEngine.Object resObject,System.Object userData);

    /// <summary>
    /// 资源管理器，管理unity中的资源，其中包括获取、销毁等
    /// </summary>
    public class ResourceManager
    {
        static ResourceManager _instance;
        public static ResourceManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ResourceManager();
                return _instance;
            }
        }
        static string GetRootPath(ResourceType t)
        {
            switch (t)
            {
                case ResourceType.MUSIC: return "music/";
                case ResourceType.PREFAB: return "prefab/";
                case ResourceType.SPEAK: return "speak/";
                case ResourceType.STATICDATA: return "staticdata/";
                case ResourceType.UI: return "ui/";
				case ResourceType.UI_ICON:return "ui/icons/";
            }
            return "";
        }
        ///// <summary>
        ///// 检查计数器的间隔
        ///// </summary>
        //const float CHECK_COUNTER_INTERVAL = 10;

        Dictionary<string, ResourceDataObject> _dataDict = new Dictionary<string, ResourceDataObject>();
        Dictionary<string, ResourceData> _resDict = new Dictionary<string, ResourceData>();
        List<ResourceData> _waitToLoadList = new List<ResourceData>();
        float _lastCheckCounterTime = 0;
        private ResourceManager()
        {


        }
        public void Init()
        {
            //todo:以后从资源描述表中进行初始化，现在先写死
            InitInternal();
        }
		/// <summary>
		/// 请求资源
		/// </summary>
		/// <param name="caller"></param>
		/// <param name="resName"></param>
		/// <param name="callback"></param>
		public void GetResource(System.Object caller, string resName, ResDataLoadedHandler callback, System.Object userData = null)
		{
			string key = resName.ToLower();
			ResourceDataObject resDO = null;
			if (!_dataDict.TryGetValue(key, out resDO))
			{
				throw new Exception(string.Format("名为{0}的资源不存在", resName));
				//return;
			}
			ResourceData data = null;

			if (!_resDict.TryGetValue(key, out data))
			{
				string url = GetRootPath(resDO.Type) + resDO.Path;
				data = new ResourceData(caller, key, resDO.Type, url, callback, userData);
				_waitToLoadList.Add(data);
				_resDict[key] = data;
			}
			else
			{
				data.AddCounter(caller);
				if (data.IsReady)
				{
					if (callback != null)
						callback(key, data.Url, data.Data, userData);
				}
				else
				{
					data.AddCallback(callback, userData);
				}
			}
		}
        /// <summary>
        /// 归还资源
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="resName"></param>
        public void ReturnResource(System.Object caller, string resName)
        {
            string key = resName.ToLower();
            ResourceData data = null;

            if (!_resDict.TryGetValue(key, out data))
            {
                return;
            }
            data.RemoveCounter(caller);
        }
        public void Update()
        {
            if (_waitToLoadList.Count == 0)
                return;
            while (_waitToLoadList.Count > 0)
            {
                ResourceData item = _waitToLoadList[0];
				_waitToLoadList.RemoveAt(0);
				//这里就直接使用unity resources进行加载
				UnityEngine.Object data = null;
				//根据数据类型做一些处理，相当于资源的拆解，这个是打包的逆向方法
				//todo:以后加强
				if (item.ResType == ResourceType.UI_ICON)
				{
					//UnityEngine.Sprite sprite = new UnityEngine.Sprite();
					data = UnityEngine.Resources.Load<UnityEngine.Sprite>(item.Url);
					if (data == null)
					{
						UnityEngine.Debug.LogErrorFormat("ResourceManager::Update:加载UI_ICON资源失败,resName={0},path={1}", item.Name, item.Url);
						continue;
					}
					//data = sprite;
				}
				else
				{
					data = UnityEngine.Resources.Load(item.Url);
				}

				item.SetData(data);
                

            }
        }
        /// <summary>
        /// 检查并清理未被使用的资源
        /// </summary>
        public void ClearUnused()
        {
            List<string> tmpList = new List<string>();
            foreach (ResourceData data in _resDict.Values)
            {
                data.CheckCounter();
                if (data.CounterValue == 0)
                {
                    tmpList.Add(data.Name);
                    if (data.Data != null)
                    {
                        UnityEngine.Object.Destroy(data.Data);
                        data.SetData(null);
                    }
                }
            }
            foreach (string key in tmpList)
            {
                _resDict.Remove(key);
            }
            UnityEngine.Resources.UnloadUnusedAssets();
        }
		void AddDataDict(ResourceDataObject resDO)
		{
			string lowerName = resDO.Name.ToLower();
			if (_dataDict.ContainsKey(lowerName))
				throw new Exception(string.Format("已存在名为{0}的资源", lowerName));
			_dataDict.Add(lowerName, resDO);
		}
		void InitInternal()
        {
			ResourceDataObject resDO = null;
			//ui
            resDO = new ResourceDataObject("ui_logon", ResourceType.UI, "UILogon");
            AddDataDict(resDO);
			resDO = new ResourceDataObject("ui_create_account", ResourceType.UI, "account/UICreateAccount");
			AddDataDict(resDO);
			resDO = new ResourceDataObject("ui_enter_game", ResourceType.UI, "account/UIEnterGame");
			AddDataDict(resDO);
			
			//ui-world
			resDO = new ResourceDataObject("ui_main", ResourceType.UI, "world/UIMain");
			AddDataDict(resDO);
			
			//prefab
			resDO = new ResourceDataObject("prefab_world", ResourceType.PREFAB, "world_entry");
			AddDataDict(resDO);

			resDO = new ResourceDataObject("prefab_avatar_player", ResourceType.PREFAB, "avatar_player");
            AddDataDict(resDO);
            resDO = new ResourceDataObject("prefab_entity", ResourceType.PREFAB, "entity");
            AddDataDict(resDO);
            resDO = new ResourceDataObject("prefab_terrain", ResourceType.PREFAB, "terrain");
            AddDataDict(resDO);



			InitIconRes();

		}
		void InitIconRes()
		{
			ResourceDataObject resDO = null;
			resDO = new ResourceDataObject("ui_icon_default", ResourceType.UI_ICON, "default");
			AddDataDict(resDO);
			////头像图标开始
			//resDO = new ResourceDataObject("ui_icon_head_cike", ResourceType.UI_ICON, "head/cike");
			//AddDataDict(resDO);
			////skill图标开始
			//resDO = new ResourceDataObject("ui_icon_skill_gongji", ResourceType.UI_ICON, "skill/d_gongji");
			//AddDataDict(resDO);
			////物品图标开始
			//resDO = new ResourceDataObject("ui_icon_item_equip_weapon0", ResourceType.UI_ICON, "item/equip_weapon_0");
			//AddDataDict(resDO);
			//resDO = new ResourceDataObject("ui_icon_item_equip_armor0", ResourceType.UI_ICON, "item/equip_armor_0");
			//AddDataDict(resDO);
		}

    }

}