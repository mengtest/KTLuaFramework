using Kernel.core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kernel.Game
{
	public class TransformComponent : IComponent
	{
		public Vector3 Position;
		public Quaternion Rotation;
		public Vector3 Scale;
		public bool PhysicsVisible;

		public Vector3 EulerAngles
		{
			get
			{
				return Rotation.eulerAngles;
			}
			set
			{
				Rotation = Quaternion.Euler(value);
			}
		}

		public Vector3 Forward
		{
			get { return Rotation * Vector3.forward; }
		}
		public Vector3 Right
		{
			get { return Rotation * Vector3.right; }
		}
		public Vector3 Up
		{
			get { return Rotation * Vector3.up; }
		}
	}
}


