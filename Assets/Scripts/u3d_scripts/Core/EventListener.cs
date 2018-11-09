using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
	/// <summary>
	/// 泛型的事件监听器，常用于回调函数，泛型类型是一个代理
	/// </summary>
	/// <typeparam name="Delegate"></typeparam>
	public class EventListener<Delegate>
	{
		protected Delegate m_callback;
		protected object m_userData;

		public EventListener()
		{ }
		public EventListener(Delegate callback, object userData)
		{
			m_callback = callback;
			m_userData = userData;
		}
		public Delegate Callback
		{
			get { return m_callback; }
			set { m_callback = value; }
		}
		public object UserData
		{
			get { return m_userData; }
			set { m_userData = value; }
		}
		//public Delegate GetCallback()
		//{
		//    return m_callback;
		//}
		//public object GetUserData()
		//{
		//    return m_userData;
		//}

		public override bool Equals(object obj)
		{
			EventListener<Delegate> listener = obj as EventListener<Delegate>;
			if (listener == null)
				return base.Equals(obj);
			return (m_callback.Equals(listener.m_callback) && (m_userData == listener.m_userData));
		}
		public override int GetHashCode()
		{
			int num = 0x17;
			if (m_callback != null)
			{
				num = (num * 0x11) + m_callback.GetHashCode();
			}
			if (m_userData != null)
			{
				num = (num * 0x11) + m_userData.GetHashCode();
			}
			return num;
		}

	}
}
