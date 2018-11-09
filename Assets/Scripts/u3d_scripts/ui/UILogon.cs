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
	class UILogon : UIPanelBase
	{
		class Components
		{
			public const string Account_Input = "_account_InputField";
			public const string Password_Input = "_password_InputField";
			public const string Btn_Login = "btn_login";
			public const string Btn_Create = "btn_create";
		}

		InputField _accountInput, _passwordInput;
		Button _btn_login, _btn_create;
		protected override void Awake()
		{
			base.Awake();

		}
		protected override void InitComponent()
		{
			base.InitComponent();
			_accountInput = GetComponentInChildrenByName<InputField>(Components.Account_Input);
			_passwordInput = GetComponentInChildrenByName<InputField>(Components.Password_Input);
			_btn_login = GetComponentInChildrenByName<Button>(Components.Btn_Login);
			_btn_create = GetComponentInChildrenByName<Button>(Components.Btn_Create);
		}

		protected override void OnOpen(object openParam = null)
		{
			base.OnOpen(openParam);
			if (openParam != null)
			{
				string account = openParam as string;
				if (!string.IsNullOrEmpty(account))
				{
					_accountInput.text = account;
				}

			}
			else
			{//尝试从账号记录中获取
				string accountName = PlayerPrefs.GetString(GameConfig.PrefsKey.AccountName, "");
				if (!string.IsNullOrEmpty(accountName))
				{
					_accountInput.text = accountName;
				}
				string password = PlayerPrefs.GetString(GameConfig.PrefsKey.Password, "");
				if (!string.IsNullOrEmpty(password))
				{
					_passwordInput.text = password;
				}
			}

			AddBtnEvt(_btn_login, OnLoginClick);
			AddBtnEvt(_btn_create, OnCreateClick);

			EngineEvents.OnConnectionState += EngineEvents_OnConnectionState;
			EngineEvents.OnLoginFailed += EngineEvents_OnLoginFailed;
			EngineEvents.OnScriptVersionNotMatch += EngineEvents_OnScriptVersionNotMatch;
			EngineEvents.OnVersionNotMatch += EngineEvents_OnVersionNotMatch;

			EngineEvents.OnKicked += EngineEvents_OnKicked;
			EngineEvents.OnDisconnected += EngineEvents_OnDisconnected;

			EngineEvents.OnLoginBaseapp += EngineEvents_OnLoginBaseapp;
			EngineEvents.OnLoginBaseappFailed += EngineEvents_OnLoginBaseappFailed;

			EngineEvents.OnRelogonBaseapp += EngineEvents_OnRelogonBaseapp;
			EngineEvents.OnReloginBaseappSuccessfully += EngineEvents_OnReloginBaseappSuccessfully;
			EngineEvents.OnReloginBaseappFailed += EngineEvents_OnReloginBaseappFailed;

			
			KBEngine.Event.registerOut(AccountEvents_Out.EventNames.OnLoginSuccessfully, this, "_OnLoginSuccessfully");
			

		}
		protected override void OnClose(object closeParam = null)
		{
			RemoveBtnEvt(_btn_login, OnLoginClick);
			RemoveBtnEvt(_btn_create, OnCreateClick);

			EngineEvents.OnConnectionState -= EngineEvents_OnConnectionState;
			EngineEvents.OnLoginFailed -= EngineEvents_OnLoginFailed;
			EngineEvents.OnScriptVersionNotMatch -= EngineEvents_OnScriptVersionNotMatch;
			EngineEvents.OnVersionNotMatch -= EngineEvents_OnVersionNotMatch;

			EngineEvents.OnKicked -= EngineEvents_OnKicked;
			EngineEvents.OnDisconnected -= EngineEvents_OnDisconnected;

			EngineEvents.OnLoginBaseapp -= EngineEvents_OnLoginBaseapp;
			EngineEvents.OnLoginBaseappFailed -= EngineEvents_OnLoginBaseappFailed;

			EngineEvents.OnRelogonBaseapp -= EngineEvents_OnRelogonBaseapp;
			EngineEvents.OnReloginBaseappSuccessfully -= EngineEvents_OnReloginBaseappSuccessfully;
			EngineEvents.OnReloginBaseappFailed -= EngineEvents_OnReloginBaseappFailed;

			KBEngine.Event.deregisterOut(this);
			

			base.OnClose(closeParam);

		}
		#region 处理fireOut的事件

		public void _OnLoginSuccessfully(ulong rndUUID, KBEngine.Account account)
		{
			Debug.LogFormat("登录成功！！");
#if UNITY_EDITOR
			//编辑器下，为了快速测试，就保持了一下账号密码
			PlayerPrefs.SetString(GameConfig.PrefsKey.AccountName, _accountInput.text);
			PlayerPrefs.SetString(GameConfig.PrefsKey.Password, _passwordInput.text);
#endif
			LogicSceneMgr.Instance.Load(LogicSceneMgr.SceneType.ENTERGAME);
		}

		#endregion
		private void EngineEvents_OnDisconnected()
		{
			Debug.LogError("disconnected!!");
		}

		private void EngineEvents_OnKicked(ushort errorCode)
		{
			Debug.LogErrorFormat("kick, disconnect!, reason={0}", KBEngineApp.app.serverErr(errorCode));
		}
		private void EngineEvents_OnReloginBaseappFailed(ushort errorCode)
		{
			Debug.LogErrorFormat("relogin Baseapp is failed(重登陆网关失败), err={0}", KBEngineApp.app.serverErr(errorCode));
		}

		private void EngineEvents_OnReloginBaseappSuccessfully()
		{
			Debug.Log("ReloginBaseapp 成功！");
		}

		private void EngineEvents_OnRelogonBaseapp()
		{
			Debug.Log("正在重新登录Baseapp，请稍后...");
		}

		private void EngineEvents_OnLoginBaseappFailed(ushort errorCode)
		{
			Debug.LogErrorFormat("login Baseapp is failed(登陆网关失败), err={0}", KBEngineApp.app.serverErr(errorCode));
		}

		

		private void EngineEvents_OnLoginBaseapp()
		{
			Debug.Log("正在登录Baseapp，请稍后...");
		}

		private void EngineEvents_OnVersionNotMatch(string verInfo, string serverVerInfo)
		{
			Debug.LogErrorFormat("version not match(curr={0}, server={1})", verInfo, serverVerInfo);
		}

		private void EngineEvents_OnScriptVersionNotMatch(string verInfo, string serverVerInfo)
		{
			Debug.LogErrorFormat("script version not match(curr={0}, server={1})", verInfo, serverVerInfo);
		}

		private void EngineEvents_OnLoginFailed(ushort errorCode)
		{
			if (errorCode == 20)
			{
				Debug.LogErrorFormat("login is failed(登陆失败), err={0}, {1}", KBEngineApp.app.serverErr(errorCode), System.Text.Encoding.ASCII.GetString(KBEngineApp.app.serverdatas()));
			}
			else
			{
				Debug.LogErrorFormat("login is failed(登陆失败), err={0}", KBEngineApp.app.serverErr(errorCode));
			}
		}

		

		private void EngineEvents_OnConnectionState(bool success)
		{
			if (!success)
			{
				KBEngineArgs args = KBEngineApp.app.getInitArgs();
				Debug.LogErrorFormat("连接服务器({0}:{1})，发生错误", args.ip, args.port);
			}
			else
				Debug.Log("连接服务器成功，登录中...");
			
		}

		

		void OnLoginClick()
		{
			
			string account = _accountInput.text;
			string password = _passwordInput.text;
			Debug.LogFormat("UILogon::OnLoginClick:account={0},password={1}", account, password);
			if (password.Length <6 && account.Length <6)
			{
				Debug.LogError("UILogon::OnLoginClick:account or password is error, length < 6!(账号或者密码错误，长度必须大于5!)");
				return;
			}
			EngineUtils.Login(account, password, System.Text.Encoding.UTF8.GetBytes("kbengine_unity3d_demo"));
		}
		void OnCreateClick()
		{
			this.Close();
			UIManager.Instance.OpenPanel(UIPanelType.CREATE_ACCOUNT);
		}
	}
}
