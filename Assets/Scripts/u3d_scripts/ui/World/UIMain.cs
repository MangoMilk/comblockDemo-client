using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using KBEngine;
using GameCore;
using GameCore.UI;
using GameLogic.Events;

namespace MMODEMO.UI
{
	/// <summary>
	/// 进入世界后的主界面
	/// </summary>
	class UIMain : UIPanelBase
	{
		class Components
		{
			public const string btn_hello = "_btn_helloToFirstEntity";
		}
		
		Button _btn_hello;

		protected override void InitComponent()
		{
			base.InitComponent();

			_btn_hello = GetComponentInChildrenByName<Button>(Components.btn_hello);
			_btn_hello.gameObject.SetActive(false);
		}
		protected override void OnDestroy()
		{
			
			base.OnDestroy();
		}

		protected override void OnOpen(object openParam = null)
		{
			base.OnOpen(openParam);


			AddBtnEvt(_btn_hello, OnHelloBtnClick);

			var world = World.Instance;
			if (world.MyselfEntered)
			{
				OnMyselfEntered(world.Myself);
			}
			else
			{
				world.OnMyselfEnter += OnMyselfEntered;
			}
		}
		protected override void OnClose(object closeParam = null)
		{
			RemoveBtnEvt(_btn_hello, OnHelloBtnClick);

			var world = World.Instance;
			world.OnMyselfEnter -= OnMyselfEntered;

			base.OnClose(closeParam);
		}
		void OnMyselfEntered(EntityUnityComponent entity)
		{
			_btn_hello.gameObject.SetActive(true);
		}

		void OnHelloBtnClick()
		{
			KBEngine.Event.fireIn(AccountEvents_In.EventNames.helloToFirstEntity, "Hi, I`m your master.");
		}
		
		
	}
}