using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
	/// <summary>
	/// 泛型的通用事件处理器
	/// </summary>
	/// <typeparam name="TSender">sender的泛型类型</typeparam>
	/// <typeparam name="UEventArgs">参数的泛型类型</typeparam>
	/// <param name="sender">事件发送方</param>
	/// <param name="args">对应参数</param>
	public delegate void EventHandler<TSender, UEventArgs>(TSender sender, UEventArgs args) where UEventArgs : EventArgs;
	
}
