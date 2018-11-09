//该文件描述引擎内置的事件代理

using UnityEngine;
using System;
using System.Collections;

namespace KBEngine
{
    #region 通用代理
    /// <summary>
    /// 通用的错误处理器
    /// </summary>
    /// <param name="errorCode">错误代码，可用</param>
    public delegate void OnEngineErrorHanlder(UInt16 errorCode);
    public delegate void OnEntityHandler(Entity entity);
	public delegate void OnDBIDHandler(UInt64 dbid);
    #endregion

    #region 一般的
    public delegate void OnKickedHandler(UInt16 failedCode);
    
    public delegate void OnConnectionStateHandler(bool success);
    #endregion

    #region login

    /// <summary>
    /// 创建账户的结果代理
    /// </summary>
    /// <param name="retCode">返回的code，不等于0时说明发生错误</param>
    /// <param name="datas">服务器传回的数据</param>
    public delegate void OnCreateAccountResultHandler(UInt16 retCode, byte[] datas);

    
    /// <summary>
    /// 客户端版本和服务器版本不符合
    /// </summary>
    /// <param name="verInfo">客户端版本信息</param>
    /// <param name="serverVerInfo">服务器版本信息</param>
    public delegate void OnVersionNotMatchHandler(string verInfo, string serverVerInfo);
 
    
    #endregion

    #region cell
    //public delegate void OnEnterWorldHandler(Entity entity);
    //public delegate void OnLeaveWorldHandler(Entity entity);
    public delegate void OnAddSpaceGeometryMappingHandler(string resPath);
    public delegate void OnControlledHandler(Entity entity, bool isControlled);
    #endregion
}