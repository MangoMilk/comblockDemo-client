using System;
using System.Collections.Generic;
using UnityEngine;
using GameCore;
using GameLogic.EntityComponents;
using GameLogic.Components;

namespace KBEngine
{
	/// <summary>
	/// 服务端FirstEntity实体的Client实现
	/// </summary>
	public class FirstEntity : FirstEntityBase, IGameObject
	{
		public override void __init__()
		{
			base.__init__();
		}

		public string Name
		{
			get
			{
				return base.id.ToString();
			}
		}
		public void SetRenderObj(GameObject gameObject)
		{
			base.renderObj = gameObject;

			gameObject.AddComponent<FirstEntityUnity>();

			BaseSynchronizer synchronizer = gameObject.AddComponent<BaseSynchronizer>();
			//unity组件的初始化
			synchronizer.SetTarget(this);
		}
		public override void onHello(int entityId, string content)
		{
			Dbg.DEBUG_MSG(string.Format("FirstEntity{0}::onHello:entityId={0}, content={1}", entityId, content));
		}
	}
}
