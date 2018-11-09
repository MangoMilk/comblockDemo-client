using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
	/// <summary>
	/// Unity中GameObject的接口，给予KBE内部的实体实现类进行继承。
	/// </summary>
	public interface IGameObject
	{
		void SetRenderObj(GameObject gameObject);
		string Name { get; }

	}
	public interface IComponent<T> where T: Component
	{
		void SetComponent(T unityComponent);
	}
	
}
