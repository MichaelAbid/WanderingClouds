using System;
using System.Collections.Generic;
using System.Collections;
using Codice.CM.Common.Merge;
using NaughtyAttributes;
using UnityEngine;

namespace WanderingCloud.Gameplay
{
	public class DestructibleBlocks : MonoBehaviour
	{
		private void OnTriggerEnter(Collider collider)
	{
		if (collider.GetComponent<CreatureSources>()!= null && collider.GetComponent<CreatureSources>().currentState == CloudState.DESTRUCTOR)
		{
			Destroy(this);
		}
	}
		
	}
}
