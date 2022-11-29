using System.Collections.Generic;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace WanderingCloud
{
	public class PlayerInventory : MonoBehaviour
	{
		[field: SerializeField, ReadOnly] public bool isHoldingCloud { get; private set; }
		[SerializeField] private float holdDuration = 0.5f;
		public UnityEvent onEatingCloud;
		private Coroutine eating = null;

		[Button()]
		public void CloudContact()
		{
			if(eating is null) eating = StartCoroutine(EatingCloud());
		}

		private IEnumerator EatingCloud()
		{
			isHoldingCloud = true;
			yield return new  WaitForSeconds(holdDuration);
			onEatingCloud?.Invoke();
			isHoldingCloud = false;
			eating = null;
		}

	}
}
