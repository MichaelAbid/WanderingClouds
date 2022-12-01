using System;
using System.Collections.Generic;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace WanderingCloud
{
	[RequireComponent(typeof(Collider))]
	public class Cloud : MonoBehaviour
	{
		private void Awake()
		{
			GetComponent<Collider>().isTrigger = true;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.GetComponentInParent<PlayerInventory>())
			{
				other.GetComponentInParent<PlayerInventory>().CloudContact();
			}
		}
	}
}
