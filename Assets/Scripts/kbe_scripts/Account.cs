using System; 
using System.Collections; 
using System.Collections.Generic;
using System.Linq;
using Event = KBEngine.Event;
using UnityEngine;
using GameCore;
using GameLogic.Events;
using GameLogic.Components;
using GameLogic.EntityComponents;

namespace KBEngine
{
	/// <summary>
	/// 服务端Account实体的Client实现
	/// </summary>
    public class Account : AccountBase, IGameObject
	{
        public Account() : base()
        {
		}

		public string Name {
			get
			{
				return base.id.ToString();
			}
		}

		public override void __init__()
		{//实体被初始化时会调用该函数
		 
			// 注册事件
			Event.registerIn(AccountEvents_In.EventNames.enterGame, this, "_EnterGame");
			Event.registerIn(AccountEvents_In.EventNames.helloToFirstEntity, this, "_HelloToFirstEntity");

			// 触发登陆成功事件
			// 当Account entity创建时，我们从业务的角度上设计了一个登录成功的事件
			Event.fireOut(AccountEvents_Out.EventNames.OnLoginSuccessfully, KBEngineApp.app.entity_uuid, this);
        }
		
		public override void onDestroy()
        {//实体被销毁时会调用该函数
			Event.deregisterIn(this);
		}

		public void SetRenderObj(GameObject gameObject)
		{
			base.renderObj = gameObject;

			gameObject.AddComponent<AccountUnity>();

			BaseSynchronizer synchronizer = null;
			if (isPlayer())
			{
				synchronizer = gameObject.AddComponent<BaseSynchronizer>();
				//unity组件的初始化
				synchronizer.SetTarget(this);
			}
		}

		#region 外部的fireIn事件处理

		public void _EnterGame()
		{
			Dbg.DEBUG_MSG("Account::enterGame");
			this.baseEntityCall.enterGame();
		}
		public void _HelloToFirstEntity(string content)
		{
			Dbg.DEBUG_MSG("Account::HelloToFirstEntity, content="+content);
			FirstEntityBase firstEntity = null;
			foreach (var pair in KBEngineApp.app.entities)
			{
				if (pair.Value.className == "FirstEntity")
				{
					firstEntity = pair.Value as FirstEntityBase;
				}
			}
			if (firstEntity == null)
			{
				Dbg.ERROR_MSG("Account::HelloToFirstEntity, firstEntity can not find");
				return;
			}
			firstEntity.cellEntityCall.hello(content);
		}

		#endregion

		#region 自定义事件的回调

		public override void onEnterGameFailed(sbyte errorCode)
        {
            Dbg.ERROR_MSG("Account::onEnterGameFailed: errorCode=" + errorCode);
			Event.fireOut(AccountEvents_Out.EventNames.OnEnterGameFailed, errorCode);
		}
        public override void onEnterGameSuccess()
        {
            Dbg.DEBUG_MSG("Account::onEnterGameSuccess!");
			Event.fireOut(AccountEvents_Out.EventNames.OnEnterGameSuccess);
		}

		public override void onFirstEntityHello(string content)
		{
			Dbg.DEBUG_MSG("Account::onFirstEntityHello:content=\r\n" + content);
			Event.fireOut(AccountEvents_Out.EventNames.OnFirstEntityHello, content);
		}

		#endregion

	}
}