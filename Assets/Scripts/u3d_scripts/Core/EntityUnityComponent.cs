using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
	/// <summary>
	/// entity的unity组件的基类
	/// </summary>
	public abstract class EntityUnityComponent : UnityComponent
	{
	    KBEngine.Entity _entity;
		public int EntityId { get { return _entity.id; } }
		public bool IsPlayer { get { return _entity.isPlayer(); } }
		public KBEngine.Entity KBEEntity { get { return _entity; } }
		public void SetTarget(KBEngine.Entity entity)
		{
			_entity = entity;
			OnTargetSet(_entity);
		}
		protected virtual void OnTargetSet(KBEngine.Entity entity)
		{ }
		protected override void OnDestroy()
		{
			_entity = null;

			base.OnDestroy();
		}
	}
}
