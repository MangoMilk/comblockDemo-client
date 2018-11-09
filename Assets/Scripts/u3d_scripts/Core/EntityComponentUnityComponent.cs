using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
	/// <summary>
	/// KBEngine.EntityComponent对应的unity组件基类
	/// </summary>
	public abstract class EntityComponentUnityComponent : UnityComponent
	{
	    protected KBEngine.EntityComponent _entityComponent;
		
		public void SetTarget(KBEngine.EntityComponent entityComponent)
		{
			_entityComponent = entityComponent;
			OnTargetSet(_entityComponent);
		}
		protected virtual void OnTargetSet(KBEngine.EntityComponent entityComponent)
		{ }
		protected override void OnDestroy()
		{
			_entityComponent = null;

			base.OnDestroy();
		}
	}
}
