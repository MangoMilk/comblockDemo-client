using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using KBEngine;
using MMODEMO.UI;
using GameLogic;

public class clientapp : KBEMain 
{
	Thread _unityThread;
	object _queueMethodLock = new object();
	List<IInvoker> _tmpQueueMethod = new List<IInvoker>();
	List<IInvoker> _queuedMethod = new List<IInvoker>();
	static clientapp _instance;
	public static clientapp Instance
	{
		get { return _instance; }
	}
	/// <summary>
	/// 是否为多线程
	/// </summary>
	public bool Multithread { get { return KBEngineApp.app.getInitArgs().isMultiThreads; } }
	
	#region 辅助类

	interface IInvoker
	{
		void Invoke();
	}
	class InvokeInfor0Param : IInvoker
	{
		Action handler;

		public InvokeInfor0Param(Action callBack)
		{
			handler = callBack;
		}

		public void Invoke()
		{
			handler();
		}
	}
	class InvokeInfoDelayParam : IInvoker
	{
		Action handler;
		float delay;
		MonoBehaviour mono;
		public InvokeInfoDelayParam(MonoBehaviour mono, Action callback, float delay)
		{
			this.handler = callback;
			this.delay = delay;
			this.mono = mono;
		}
		public void Invoke()
		{
			if (this.handler == null)
				return;
			mono.StartCoroutine(waitToCallback());
		}
		System.Collections.IEnumerator waitToCallback()
		{
			yield return new WaitForSeconds(this.delay);
			this.handler();
		}
	}

	#endregion

	public override void installEvents()
    {
        base.installEvents();
        EngineEvents.InstallEvents();
        if (!LogicEvents.Check())
        {
			Dbg.ERROR_MSG("clientapp::installEvents:逻辑事件检查失败，可能有重名！");
            Destroy(this);
            return;
        }
        
        GameCore.ResourceManager.Instance.Init();
		LogicSceneMgr.Instance.Init();
    }

	public override void initKBEngine()
	{
		if (_instance != null)
			throw new Exception("clientapp的实例已存在！！");
		_instance = this;
		_unityThread = Thread.CurrentThread;
		
		base.initKBEngine();
		
		UIManager.Instance.Init();
		
		//============程序入口=============//
		LogicSceneMgr.Instance.Load(LogicSceneMgr.SceneType.LOGON);
	}
	private void Update()
    {
        GameCore.ResourceManager.Instance.Update();
    }
	private void OnApplicationQuit()
	{
		UIManager.Instance.Deinit();
	}
	public override void KBEUpdate()
	{
		base.KBEUpdate();
		DoQueueMethod();
	}
	/// <summary>
	/// 回到unity主线程进行调用
	/// </summary>
	/// <param name="handler"></param>
	public void Invoke(Action handler)
	{
		if (Thread.CurrentThread == _unityThread)
		{
			handler();
			return;
		}
		InvokeInfor0Param invokeInfo = new InvokeInfor0Param(handler);
		lock (_queueMethodLock)
		{
			_queuedMethod.Add(invokeInfo);
		}
	}
	/// <summary>
	/// 回到unity主线程进行调用,可延迟delay秒
	/// </summary>
	/// <param name="handler"></param>
	/// <param name="delay"></param>
	public void Invoke(Action handler, float delay)
	{
		if (delay <= 0)
		{
			Invoke(handler);
			return;
		}
		InvokeInfoDelayParam invokeInfo = new InvokeInfoDelayParam(this, handler, delay);
		if (Thread.CurrentThread == _unityThread)
		{
			invokeInfo.Invoke();
			return;
		}
		
		lock (_queueMethodLock)
		{
			_queuedMethod.Add(invokeInfo);
		}
	}
	void DoQueueMethod()
	{
		lock (_queueMethodLock)
		{
			if (_queuedMethod.Count > 0)
			{
				_tmpQueueMethod.AddRange(_queuedMethod);
				_queuedMethod.Clear();
			}
		}

		if (_tmpQueueMethod.Count > 0)
		{
			for (int i = 0; i < _tmpQueueMethod.Count; i++)
			{
				IInvoker invoker = _tmpQueueMethod[i];
				invoker.Invoke();
			}
			_tmpQueueMethod.Clear();
		}
	}
}
