using System;
using System.Collections.Generic;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace WanderingCloud
{
	public class WorldOffset : MonoBehaviour
	{

	[SerializeField] private Transform self;
	[SerializeField] private Transform target;
	[SerializeField] private Vector3 offset= Vector3.up;

	private void Awake()
	{
		self = transform;
	}

	private void Update()
	{
		self.position = target.position + offset;
	}
	}
}
