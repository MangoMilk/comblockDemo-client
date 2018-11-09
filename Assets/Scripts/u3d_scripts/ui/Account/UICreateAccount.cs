using System;
using System.Collections.Generic;
using GameCore.UI;
using UnityEngine;
using UnityEngine.UI;
using KBEngine;
namespace MMODEMO.UI
{
	class UICreateAccount : UIPanelBase
	{
		class Components
		{
			public const string Account_Input = "_account_InputField";
			public const string Password_Input = "_password_InputField";
			public const string Btn_Login = "btn_create";
			public const string Btn_Back = "btn_back";
		}

		InputField _accountInput, _passwordInput;
		Button _btn_create,_btn_back;

		protected override void InitComponent()
		{
			base.InitComponent();
			_accountInput = GetComponentInChildrenByName<InputField>(Components.Account_Input);
			_passwordInput = GetComponentInChildrenByName<InputField>(Components.Password_Input);
			_btn_create = GetComponentInChildrenByName<Button>(Components.Btn_Login);
			_btn_back = GetComponentInChildrenByName<Button>(Components.Btn_Back);
		}
		protected override void OnOpen(object openParam = null)
		{
			base.OnOpen(openParam);

			AddBtnEvt(_btn_create, OnCreateClick);
			AddBtnEvt(_btn_back, OnBackClick);

			EngineEvents.OnCreateAccountResult += EngineEvents_OnCreateAccountResult;
			EngineEvents.OnConnectionState += EngineEvents_OnConnectionState;
		}
		protected override void OnClose(object closeParam = null)
		{
			RemoveBtnEvt(_btn_create, OnCreateClick);
			RemoveBtnEvt(_btn_back, OnBackClick);

			EngineEvents.OnCreateAccountResult -= EngineEvents_OnCreateAccountResult;
			EngineEvents.OnConnectionState -= EngineEvents_OnConnectionState;

			base.OnClose(closeParam);
		}

		void OnCreateClick()
		{
			string account = _accountInput.text;
			string password = _passwordInput.text;
			Debug.LogFormat("UICreateAccount::OnCreateClick:account={0},password={1}", account, password);
			if (password.Length < 6 && account.Length < 6)
			{
				Debug.LogError("UICreateAccount::OnCreateClick:account or password is error, length < 6!(账号或者密码错误，长度必须大于5!)");
				return;
			}
			EngineUtils.CreateAccount(account, password, System.Text.Encoding.UTF8.GetBytes("kbengine_unity3d_demo"));
		}
		void OnBackClick()
		{
			this.Close();
			UIManager.Instance.OpenPanel(UIPanelType.LOGON);
		}


		private void EngineEvents_OnCreateAccountResult(ushort retCode, byte[] datas)
		{
			if (retCode != 0)
			{
				Debug.LogErrorFormat("UICreateAccount::OnCreateAccountResult:注册账号错误, error={0}", KBEngineApp.app.serverErr(retCode));
				return;
			}
			Debug.Log("UICreateAccount::创建账户成功");
			this.Close();
			UIManager.Instance.OpenPanel(UIPanelType.LOGON, _accountInput.text);
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
	}
}
