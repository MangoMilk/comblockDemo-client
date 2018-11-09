using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
    /// <summary>
    /// 资源数据对象
    /// </summary>
    public class ResourceData
    {
        List<System.Object> _userList = new List<object>();
        //ResDataLoadedHandler _callbacks;
		List<CallbackListener> _callbacks = new List<CallbackListener>();
        Dictionary<System.Object, List<CallbackListener>> _userDict = new Dictionary<object, List<CallbackListener>>();
		class CallbackListener : EventListener<ResDataLoadedHandler>
		{
			public void Fire(string resName,string resPath,UnityEngine.Object resObject)
			{
				base.m_callback(resName, resPath, resObject, base.m_userData);
			}
		}

        public ResourceData(System.Object user, string key, ResourceType type, string url, ResDataLoadedHandler callback,System.Object userData=null)
        {
            
            this.Name = key;
            this.ResType = type;
            this.Url = url;

            CallbackListener listener = new CallbackListener() { Callback = callback, UserData = userData };
            _callbacks.Add(listener);
            //_userDict.Add(user, new List<CallbackListener>() { listener });
            AddCounter(user);
        }
		internal void AddCallback(ResDataLoadedHandler callback, System.Object userData = null)
		{
			_callbacks.Add(new CallbackListener() { Callback = callback, UserData = userData });
        }

        /// <summary>
        /// 增加该资源的使用者
        /// </summary>
        /// <param name="user"></param>
        /// <param name="callback"></param>
        /// <param name="userData"></param>
        internal void AddUser(System.Object user, ResDataLoadedHandler callback, System.Object userData = null)
        {
            CallbackListener listener = new CallbackListener() { Callback = callback, UserData = userData };
            List<CallbackListener> existList = null;
            if (!_userDict.TryGetValue(user, out existList))
            {
                existList = new List<CallbackListener>();
                existList.Add(listener);

                _userDict.Add(user, existList);
                _userList.Add(user);
            }
            else
            {
                if (!existList.Contains(listener))
                {
                    existList.Add(listener);
                }
                else
                    return;//
            }

            

        }

        internal void AddCounter(System.Object user)
        {
            _userList.Add(user);
        }
        internal void RemoveCounter(System.Object user)
        {
            _userList.Remove(user);
        }
        internal void CheckCounter()
        {
            int count = CounterValue;
            for (int i = count - 1; i >= 0; i--)
            {
                System.Object user = _userList[i];
                if (!IsObjectAlive(user))
                    _userList.RemoveAt(i);

            }
        }
        internal void SetData(UnityEngine.Object data)
        {
            Data = data;
            if (data != null)
            {
                if (_callbacks.Count > 0)
                {
                    for (int i = 0; i < _callbacks.Count; i++)
                    {
                        _callbacks[i].Fire(this.Name, this.Url, data);
                    }
                    _callbacks.Clear();
                    _callbacks = null;
                }
                _userDict.Clear();
            }
           
        }
        public int CounterValue { get { return _userList.Count; } }
        public bool IsReady { get { return this.Data != null; } }
        public UnityEngine.Object Data { get; private set; }
        public string Name { get; private set; }
        public string Url { get; private set; }
        public ResourceType ResType { get; private set; }
        static bool IsObjectAlive(System.Object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is UnityEngine.Object))
                return true;
            UnityEngine.Object unityObj = (UnityEngine.Object)obj;
            return (bool)unityObj;
        }
    }
}