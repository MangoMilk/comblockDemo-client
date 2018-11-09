using System;
using System.Collections.Generic;
using GameCore.UI;
using UnityEngine;
using UnityEngine.UI;
using KBEngine;
using GameLogic;
using GameLogic.Events;

namespace MMODEMO.UI
{
	class UIEnterGame : UIPanelBase
	{
		class Components
		{
			public const string Btn_Enter = "btn_enter";
			
		}



		Button _btn_enterGame;
		
		protected override void InitComponent()
		{
			base.InitComponent();
			
			
			_btn_enterGame = GetComponentInChildrenByName<Button>(Components.Btn_Enter);
			_btn_enterGame.interactable = true;
		}
		

		protected override void OnOpen(object openParam = null)
		{
			base.OnOpen(openParam);

			AddBtnEvt(_btn_enterGame, OnEnterGameClick);

			KBEngine.Event.registerOut(AccountEvents_Out.EventNames.OnEnterGameSuccess, this, "_OnEnterGameSuccess");
			KBEngine.Event.registerOut(AccountEvents_Out.EventNames.OnEnterGameFailed, this, "_OnEnterGameFailed");

		}

		

		protected override void OnClose(object closeParam = null)
		{
			RemoveBtnEvt(_btn_enterGame, OnEnterGameClick);

			KBEngine.Event.deregisterOut(this);
			
			base.OnClose(closeParam);
		}
		
		
		

		#region Account中fireOut的事件处理

		public void _OnEnterGameFailed(sbyte errorCode)
		{
			Debug.LogErrorFormat("EnterGame 失败,errorCode={0}", errorCode);
			_btn_enterGame.interactable = true;

		}

		public void _OnEnterGameSuccess()
		{
			Debug.Log("EnterGame 成功，开始进入世界，等待...");
			this.Close();
			LogicSceneMgr.Instance.Load(LogicSceneMgr.SceneType.WORLD);
		}

		
		

		#endregion

		
		void OnEnterGameClick()
		{
			
			KBEngine.Event.fireIn(AccountEvents_In.EventNames.enterGame);
			
			_btn_enterGame.interactable = false;
			Debug.Log("点击EnterGame，等待中...");
		}
		
		
	}
}
