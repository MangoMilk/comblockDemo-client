using System;
using System.Collections.Generic;

using GameLogic.Events.Core;

using KBEngine;
using Event = KBEngine.Event;
namespace GameLogic.Events
{
	

	/// <summary>
	/// Account相关，由外部向引擎内部激发的事件注册器。一般是登录界面向内部发送的事件
	/// </summary>
	public class AccountEvents_In : EventRouter
	{
		public class EventNames
		{
			public const string enterGame = "EnterGame";
			public const string helloToFirstEntity = "hello";

		}

		protected override List<string> GetEventNames()
		{
			return GetEventsFromConstVaraibles(typeof(EventNames));
		}

	}

	/// <summary>
	/// 由引擎内部向外部激发的事件注册器，一般为服务器事件
	/// </summary>
	public class AccountEvents_Out : EventRouter
	{
		public class EventNames
		{
			public const string OnLoginSuccessfully = "OnLoginSuccessfully";
			public const string OnEnterGameSuccess = "OnEnterGameSuccess";
			public const string OnEnterGameFailed = "OnEnterGameFailed";

			public const string OnFirstEntityHello = "OnFirstEntityHello";
		}
		
		public AccountEvents_Out() : base()
		{
			
		}
		

		protected override List<string> GetEventNames()
		{
			return GetEventsFromConstVaraibles(typeof(EventNames));
		}

		
	}
}