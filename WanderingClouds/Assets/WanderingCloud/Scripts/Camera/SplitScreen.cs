using System;
using System.Collections.Generic;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace WanderingCloud
{
	[ExecuteAlways]
	public class SplitScreen : MonoBehaviour
	{

		[SerializeField] Camera camLeft;
		[SerializeField] Camera camRight;
		[SerializeField, Range(0f,1f)] public float splitValue = 0.5f;
		private float lastValue = 0.5f;

		[Button()] private void Neutral() { splitValue = 0.5f; Update(); }

		#region UnityMethods
		private void Update()
		{
			if(Math.Abs(lastValue - splitValue) < Mathf.Epsilon)return;

			camLeft.rect = new Rect(Vector2.right * 0.0f, new Vector2(splitValue,1f));
			camRight.rect = new Rect(Vector2.right * splitValue, new Vector2(1-splitValue,1f));

			lastValue = splitValue;
		}
		#endregion
	
	}
}
