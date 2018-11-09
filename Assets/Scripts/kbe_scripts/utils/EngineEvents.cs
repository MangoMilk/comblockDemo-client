using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
namespace KBEngine
{
    /// <summary>
    /// 引擎事件管理，会管理引擎内部会激发出来的所有事件。
    /// 
    /// 引擎内部激发的事件会转由unity主线程进行处理，所以是线程安全的
    /// 
    /// <code>
    /// void Init()
    /// {
    ///     EngineEvents.InstallEvents();
    ///     EngineEvents.OnKiced += onkicked;
    /// }
    /// 
    /// void onKicked(UINT16 failedCode)
    /// {
    ///     do something...
    /// }
    /// </code>
    /// 
    /// </summary>
    public class EngineEvents
    {
        static InternalClass _class;
        class EventNames
        {
            public const string onKicked = "onKicked";
            public const string onDisconnected = "onDisconnected";
            public const string onConnectionState = "onConnectionState";

            //logon相关
            public const string onCreateAccountResult = "onCreateAccountResult";
            public const string onLoginFailed = "onLoginFailed";
            public const string onVersionNotMatch = "onVersionNotMatch";
            public const string onScriptVersionNotMatch = "onScriptVersionNotMatch";

            public const string onLoginBaseapp = "onLoginBaseapp";
            public const string onLoginBaseappFailed = "onLoginBaseappFailed";

            public const string onReloginBaseapp = "onReloginBaseapp";
            public const string onReloginBaseappSuccessfully = "onReloginBaseappSuccessfully";
            public const string onReloginBaseappFailed = "onReloginBaseappFailed";


            public const string onEnterWorld = "onEnterWorld";
            public const string onLeaveWorld = "onLeaveWorld";
            public const string onEnterSpace = "onEnterSpace";
            public const string onLeaveSpace = "onLeaveSpace";
            public const string set_position = "set_position";
            public const string set_direction = "set_direction";
            public const string updatePosition = "updatePosition";

            public const string addSpaceGeometryMapping = "addSpaceGeometryMapping";
            public const string onControlled = "onControlled";
        }
        /// <summary>
        /// 被踢
        /// </summary>
        public static event OnKickedHandler OnKicked;
        /// <summary>
        /// 丢失连接
        /// </summary>
        public static event System.Action OnDisconnected;
        /// <summary>
        /// 连接状态改变，连接成功则success=true
        /// </summary>
        public static event OnConnectionStateHandler OnConnectionState;

        #region login

        public static event OnCreateAccountResultHandler OnCreateAccountResult;
        public static event OnEngineErrorHanlder OnLoginFailed;
        /// <summary>
        /// 客户端版本和服务器版本不符合
        /// </summary>
        public static event OnVersionNotMatchHandler OnVersionNotMatch;
        /// <summary>
        /// 脚本版本不匹配
        /// </summary>
        public static event OnVersionNotMatchHandler OnScriptVersionNotMatch;
        /// <summary>
        /// 开始登陆Baseapp
        /// </summary>
        public static event System.Action OnLoginBaseapp;
        /// <summary>
        /// 登录到baseapp（网关）失败
        /// </summary>
        public static event OnEngineErrorHanlder OnLoginBaseappFailed;

        /// <summary>
        /// 开始重登陆Baseapp
        /// </summary>
        public static event System.Action OnRelogonBaseapp;
        public static event System.Action OnReloginBaseappSuccessfully;
        public static event OnEngineErrorHanlder OnReloginBaseappFailed;

        #endregion


        /// <summary>
        /// 有entity进入世界,如果是玩家自己，则第一次在客户端上生成时，会激发；如果是其他玩家，则进入玩家自己的VIEW时会激发
        /// </summary>
        public static event OnEntityHandler OnEnterWorld;
        public static event OnEntityHandler OnLeaveWorld;
		/// <summary>
		/// 玩家自己的entity进入到另一个空间时激发。只有玩家自己时会激发该事件，其他玩家只会激发OnEnterWorld。
		/// </summary>
		public static event OnEntityHandler OnEnterSpace;
        public static event OnEntityHandler OnLeaveSpace;

        public static event OnEntityHandler OnSetPosition;
        public static event OnEntityHandler OnSetDirection;
        public static event OnEntityHandler OnUpdatePosition;
        public static event OnAddSpaceGeometryMappingHandler OnAddSpaceGeometryMapping;
        public static event OnControlledHandler OnControlled;

        
        static EngineEvents()
        {
            _class = new InternalClass();
        }

        /// <summary>
        /// 注册引擎内部的事件
        /// </summary>
        public static void InstallEvents()
        {
            KBEngine.Event.registerOut(EventNames.onKicked, _class, "_onKicked");
            KBEngine.Event.registerOut(EventNames.onDisconnected, _class, "_onDisconnected");
            KBEngine.Event.registerOut(EventNames.onConnectionState, _class, "_onConnectionState");

            //logon相关
            KBEngine.Event.registerOut(EventNames.onCreateAccountResult, _class, "_onCreateAccountResult");
            KBEngine.Event.registerOut(EventNames.onLoginFailed, _class, "_onLoginFailed");
            KBEngine.Event.registerOut(EventNames.onVersionNotMatch, _class, "_onVersionNotMatch");
            KBEngine.Event.registerOut(EventNames.onScriptVersionNotMatch, _class, "_onScriptVersionNotMatch");

            KBEngine.Event.registerOut(EventNames.onLoginBaseapp, _class, "_onLoginBaseapp");
            KBEngine.Event.registerOut(EventNames.onLoginBaseappFailed, _class, "_onLoginBaseappFailed");

            KBEngine.Event.registerOut(EventNames.onReloginBaseapp, _class, "_onReloginBaseapp");
            KBEngine.Event.registerOut(EventNames.onReloginBaseappSuccessfully, _class, "_onReloginBaseappSuccessfully");
            KBEngine.Event.registerOut(EventNames.onReloginBaseappFailed, _class, "_onReloginBaseappFailed");




            KBEngine.Event.registerOut(EventNames.onEnterWorld, _class, "_onEnterWorld");
            KBEngine.Event.registerOut(EventNames.onLeaveWorld, _class, "_onLeaveWorld");
            KBEngine.Event.registerOut(EventNames.onEnterSpace, _class, "_onEnterSpace");
            KBEngine.Event.registerOut(EventNames.onLeaveSpace, _class, "_onLeaveSpace");
            KBEngine.Event.registerOut(EventNames.set_position, _class, "_set_position");
            KBEngine.Event.registerOut(EventNames.set_direction, _class, "_set_direction");
            KBEngine.Event.registerOut(EventNames.updatePosition, _class, "_updatePosition");
            KBEngine.Event.registerOut(EventNames.addSpaceGeometryMapping, _class, "_addSpaceGeometryMapping");
            KBEngine.Event.registerOut(EventNames.onControlled, _class, "_onControlled");
        }
        public static void Clear()
        {
            KBEngine.Event.deregisterOut(_class);
        }
        public static List<string> GetEngineEventNames()
        {
            List<string> result = new List<string>();
            Type t = typeof(EventNames);
            System.Reflection.FieldInfo[] infos = t.GetFields(System.Reflection.BindingFlags.Public| System.Reflection.BindingFlags.Static);
            foreach (var info in infos)
            {
                string v = (string)info.GetValue(null);
                //Debug.LogFormat("EngineEventNames:{0}", v);
                result.Add(v);
            }
            return result;
        }

        class InternalClass
        {
            public InternalClass()
            {
                
            }
                
            public void _onKicked(UInt16 failedCode)
            {
                if (OnKicked != null)
                {
                    OnKicked(failedCode);
                }
            }
            public void _onDisconnected()
            {
                if (OnDisconnected != null)
                    OnDisconnected();
            }
            public void _onConnectionState(bool success)
            {
                if (OnConnectionState != null)
                    OnConnectionState(success);
            }

            //=====logon相关======
            public void _onCreateAccountResult(UInt16 retCode, byte[] datas)
            {
                if (OnCreateAccountResult != null)
                    OnCreateAccountResult(retCode, datas);
            }
            public void _onLoginFailed(UInt16 failedcode)
            {
                if (OnLoginFailed != null)
                    OnLoginFailed(failedcode);
            }
            public void _onVersionNotMatch(string verInfo, string serverVerInfo)
            {
                if (OnVersionNotMatch != null)
                    OnVersionNotMatch(verInfo, serverVerInfo);
            }
            public void _onScriptVersionNotMatch(string verInfo, string serverVerInfo)
            {
                if (OnScriptVersionNotMatch != null)
                    OnScriptVersionNotMatch(verInfo, serverVerInfo);
            }

            public void _onLoginBaseapp()
            {
                if (OnLoginBaseapp != null)
                    OnLoginBaseapp();
            }
            public void _onLoginBaseappFailed(UInt16 failedCode)
            {
                if (OnLoginBaseappFailed != null)
                    OnLoginBaseappFailed(failedCode);
            }

            
            public void _onReloginBaseapp()
            {
                if (OnRelogonBaseapp != null)
                    OnRelogonBaseapp();
            }
            public void _onReloginBaseappSuccessfully()
            {
                if (OnReloginBaseappSuccessfully != null)
                    OnReloginBaseappSuccessfully();
            }
            public void _onReloginBaseappFailed(UInt16 failedcode)
            {
                if (OnReloginBaseappFailed != null)
                    OnReloginBaseappFailed(failedcode);
            }


            public void _onEnterWorld(KBEngine.Entity entity)
            {
                if (OnEnterWorld != null)
                    OnEnterWorld(entity);
            }
            public void _onLeaveWorld(KBEngine.Entity entity)
            {
                if (OnLeaveWorld != null)
                    OnLeaveWorld(entity);
            }
            public void _onEnterSpace(KBEngine.Entity entity)
            {
                if (OnEnterSpace != null)
                    OnEnterSpace(entity);
            }
            public void _onLeaveSpace(KBEngine.Entity entity)
            {
                if (OnLeaveSpace != null)
                    OnLeaveSpace(entity);
            }
            public void _set_position(KBEngine.Entity entity)
            {
                if (OnSetPosition != null)
                    OnSetPosition(entity);
            }
            public void _set_direction(KBEngine.Entity entity)
            {
                if (OnSetDirection != null)
                    OnSetDirection(entity);
            }
            public void _updatePosition(KBEngine.Entity entity)
            {
                if (OnUpdatePosition != null)
                    OnUpdatePosition(entity);
            }
            public void _addSpaceGeometryMapping(string respath)
            {
                if (OnAddSpaceGeometryMapping != null)
                    OnAddSpaceGeometryMapping(respath);
            }
            public void _onControlled(Entity entity, bool isControlled)
            {
                if (OnControlled != null)
                    OnControlled(entity, isControlled);
            }
        }
    }
}