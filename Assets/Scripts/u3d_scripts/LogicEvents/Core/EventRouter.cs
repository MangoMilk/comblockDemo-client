using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLogic.Events.Core
{
    
    /// <summary>
    /// 事件中转器，把引擎sdk和客户端之间的事件进行强类型关联，并避免事件名的冲突
    /// </summary>
    public abstract class EventRouter
    {
        
        protected EventRouter()
        {
           
        }
		

		public virtual bool Check(List<string> existEvents)
		{
			List<string> myEventNames = GetEventNames();
			for (int i = 0; i < myEventNames.Count; i++)
			{
				string name = myEventNames[i];
				if (existEvents.Contains(name))
				{
					UnityEngine.Debug.LogErrorFormat("在类{0}中存在名为{1}的事件名，不可重复！", this.GetType().Name, name);
					return false;
				}
				existEvents.Add(name);
			}
			return true;
		}
        
        protected abstract List<string> GetEventNames();
        
        
		/// <summary>
		/// 从类的常量中获取事件名的值
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		protected static List<string> GetEventsFromConstVaraibles(Type t)
		{
			List<string> result = new List<string>();
			System.Reflection.FieldInfo[] infos = t.GetFields(System.Reflection.BindingFlags.Public| System.Reflection.BindingFlags.Static);
			string className = t.FullName;
			//UnityEngine.Debug.LogFormat("GetEventsFromConstVaraibles:class={0}", className);
			foreach (var info in infos)
			{
				
				string v = (string)info.GetValue(null);
				//UnityEngine.Debug.LogFormat("GetEventsFromConstVaraibles:value={0}", v);
				result.Add(v);
			}
			return result;
		}
    }
}