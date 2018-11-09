using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
	/// <summary>
	/// UnityLayer管理器
	/// </summary>
	public class LayerManager
	{
		/// <summary>
		/// UNITY内置的层
		/// </summary>
		public class InternalLayers
		{
			public const string Default = "Default";
			public const string TransparentFX = "TransparentFX";
			public const string IgnoreRaycast = "Ignore Raycast";
			public const string Water = "Water";
			public const string UI = "UI";
		}
		public class LayerData
		{
			private string _name;
			private int _layer;

			public LayerData(string name)
			{
				_name = name;
				_layer = LayerMask.NameToLayer(name);
			}
			/// <summary>
			/// 层名的字符串
			/// </summary>
			public string Name { get { return _name; } }
			/// <summary>
			/// 层的int值
			/// </summary>
			public int Layer { get { return _layer; } }
		}

		/// <summary>
		/// 所有自己建立的层的枚举
		/// </summary>
		public enum Layer
		{
			Internal_Default,
			Internal_TransparentFX,
			Internal_IgnoreRaycast,
			Internal_Water,
			Internal_UI,
			/// <summary>
			/// 
			/// </summary>
			Avatar,
			Floor
		}

		readonly static Dictionary<Layer, LayerData> s_dict = new Dictionary<Layer, LayerData>()
		{
			{Layer.Internal_Default, new LayerData(InternalLayers.Default)},
			{Layer.Internal_TransparentFX, new LayerData(InternalLayers.TransparentFX)},
			{Layer.Internal_IgnoreRaycast, new LayerData(InternalLayers.IgnoreRaycast)},
			{Layer.Internal_Water, new LayerData(InternalLayers.Water)},
			{Layer.Internal_UI, new LayerData(InternalLayers.UI)},
			{Layer.Avatar, new LayerData("Avatar")},
			{Layer.Floor, new LayerData("Floor")}
		};
		static LayerManager()
		{

		}
		/// <summary>
		/// 得到层数据
		/// </summary>
		/// <param name="layer"></param>
		/// <returns></returns>
		public static LayerData GetLayer(Layer layer)
		{
			LayerData d = null;
			s_dict.TryGetValue(layer, out d);
			return d;
		}
		/// <summary>
		/// 得到层名字
		/// </summary>
		/// <param name="layer"></param>
		/// <returns></returns>
		public static string GetLayerName(Layer layer)
		{
			LayerData d = GetLayer(layer);
			if (d != null)
				return d.Name;
			return null;
		}
		/// <summary>
		/// 得到层int
		/// </summary>
		/// <param name="layer"></param>
		/// <returns></returns>
		public static int GetLayerInt(Layer layer)
		{
			LayerData d = GetLayer(layer);
			if (d != null)
				return d.Layer;
			return 0;
		}
	}
}
