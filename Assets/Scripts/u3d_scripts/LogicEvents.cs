using UnityEngine;
using System;
using KBEngine;
using System.Collections.Generic;
using GameLogic.Events;


public delegate void OnErrorHandler(sbyte errorCode);


/// <summary>
/// 自定义的逻辑事件管理，方便统一管理和分发。
/// 因fireOut是在unity的主线程下分发，所以在unity中的操作是线程安全的。
/// </summary>
public class LogicEvents
{
	static AccountEvents_In _AccountEvents_In;
	static AccountEvents_Out _AccountEvents_Out;


	public static AccountEvents_In AccountEvents_In { get { return _AccountEvents_In; } }
	public static AccountEvents_Out AccountEvents_Out { get { return _AccountEvents_Out; } }


    static LogicEvents()
    {

		_AccountEvents_In = new AccountEvents_In();
		_AccountEvents_Out = new AccountEvents_Out();
    }
    /// <summary>
    /// 检查是否与引擎内置的事件名冲突
    /// </summary>
    /// <returns></returns>
    public static bool Check()
    {
        List<string> events = EngineEvents.GetEngineEventNames();
		//Debug.Log("LogicEvents::Check:" + events.Count);
		if (!_AccountEvents_In.Check(events))
			return false;
		if (!_AccountEvents_Out.Check(events))
			return false;
       
		//Debug.Log("LogicEvents::Check:" + events.Count);
		return true;
    }
    
	
   
}