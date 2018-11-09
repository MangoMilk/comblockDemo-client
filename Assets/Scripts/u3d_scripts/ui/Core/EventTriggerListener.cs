using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameCore.UI
{
	/// <summary>
	/// UGUI下对EventTrigger的封装
	/// </summary>
	public class EventTriggerListener : UnityEngine.EventSystems.EventTrigger
	{
		public delegate void VoidDelegate(EventTriggerListener sender, GameObject go);
		public event VoidDelegate OnClick;

		public static EventTriggerListener Get(GameObject go)
		{
			EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
			if (listener == null)
				listener = go.AddComponent<EventTriggerListener>();
			return listener;
		}

		PointerEventData _currentEventData;

		public PointerEventData CurrentEventData { get { return _currentEventData; } }


		public override void OnPointerClick(PointerEventData eventData)
		{
			base.OnPointerClick(eventData);
			_currentEventData = eventData;
			if (OnClick != null)
				OnClick(this, gameObject);
		}

	}
}