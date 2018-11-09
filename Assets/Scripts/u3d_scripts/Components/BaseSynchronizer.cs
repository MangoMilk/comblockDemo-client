using System;
using System.Collections.Generic;
using UnityEngine;
using GameCore;

namespace GameLogic.Components
{
	/// <summary>
	/// 基础的同步器组件，包括位置、方向等
	/// </summary>
	public class BaseSynchronizer : EntityUnityComponent
	{
		#region 序列化部分
		/// <summary>
		/// 同步转向时的旋转速度
		/// </summary>
		public float rotation_speed = 8f;

		#endregion
		protected Vector3 _destDirection;
		protected Vector3 _destPosition;
		private UInt32 _spaceID;
		private float _speed = 10;

		protected override void Update()
		{
			base.Update();

			Synchronize();

		}
		protected virtual void Synchronize()
		{
			if (Vector3.Distance(_trans.eulerAngles, _destDirection) > 0.0004f)
			{
				rotation = Quaternion.Slerp(rotation, Quaternion.Euler(_destDirection), this.rotation_speed * Time.deltaTime);
			}
			float dist = 0.0f;
			float deltaSpeed = (this.Speed * Time.deltaTime);
			// 如果isOnGround为true，服务端同步其他实体到客户端时为了节省流量并不同步y轴，客户端需要强制将实体贴在地面上
			// 由于这里的地面位置就是0，所以直接填入0，如果是通过navmesh不规则地表高度寻路则需要想办法得到地面位置
			if (KBEEntity.isOnGround)
			{
				Vector3 tmpPosA = new Vector3(_destPosition.x, 0f, _destPosition.z);
				tmpPosA.y = GetY(tmpPosA);
				Vector3 tmpPosB = new Vector3(position.x, 0f, position.z);
				tmpPosB.y = GetY(tmpPosB);
				dist = Vector3.Distance(tmpPosA, tmpPosB);
			}
			else
			{
				dist = Vector3.Distance(_destPosition, position);
			}
			if (dist > 0.01f)
			{
				Vector3 pos = this.position;
				Vector3 movement = _destPosition - pos;
				//movement.y = 0;
				movement.Normalize();

				movement *= deltaSpeed;

				if (dist > deltaSpeed || movement.magnitude > deltaSpeed)
					pos += movement;
				else
					pos = _destPosition;

				if (KBEEntity.isOnGround)
					pos.y = GetY(pos);

				position = pos;
			}
			else
			{
				position = _destPosition;
			}
		}
		public void SetDestDirection(Vector3 destDirection)
		{
			_destDirection = destDirection;
			Debug.LogFormat("{0}[{1}](BaseSynchronizer)::SetDestDirection:destDirection={2}", KBEEntity.className, KBEEntity.id, destDirection);
			OnDestDirectionSet(destDirection);
		}
		protected virtual void OnDestDirectionSet(Vector3 destDirection)
		{ }
		public void SetDestPosition(Vector3 destPosition)
		{
			_destPosition = destPosition;
			Debug.LogFormat("{0}[{1}](BaseSynchronizer)::SetDestPosition:destPosition={2}", KBEEntity.className, KBEEntity.id, destPosition);
			OnDestPositionSet(destPosition);
		}
		protected virtual void OnDestPositionSet(Vector3 destDirection)
		{ }
		/// <summary>
		/// 根据一个位置，得到该地形上的Y坐标
		/// </summary>
		/// <param name="position"></param>
		protected virtual float GetY(Vector3 position)
		{
			return 201.0f;
		}
		public UInt32 SpaceID
		{
			get { return _spaceID; }
			set { _spaceID = value; }
		}
		public Quaternion rotation
		{
			get { return Quaternion.Euler(_trans.eulerAngles); }
			set { _trans.eulerAngles = value.eulerAngles; }
		}
		public Vector3 position
		{
			get
			{
				return _trans.position;
			}
			set
			{
				_trans.position = position;
			}
		}

		/// <summary>
		/// 目标方向
		/// </summary>
		public Vector3 DestDirection { get { return _destDirection; } }
		/// <summary>
		/// 目标位置
		/// </summary>
		public Vector3 DestPosition { get { return _destPosition; } }
		/// <summary>
		/// 同步时，客户端表现的移动速度，该值由entity的实际速度进行设置
		/// </summary>
		public float Speed
		{
			get { return _speed; }
			set { _speed = value; }
		}

	}
}