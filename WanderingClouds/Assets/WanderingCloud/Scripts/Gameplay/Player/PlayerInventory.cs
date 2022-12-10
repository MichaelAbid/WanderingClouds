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
		public UnityEvent onLootCloud;
		public UnityEvent onLoseCloud;
		public UnityEvent onConsumeCloud;
		[SerializeField] public int pelletStock = 0;
		[SerializeField] private int pelletPerConsum = 3;
		[SerializeField] private int maxPelletStock = 5;
		private Coroutine consuming = null;

		public bool haveCloud => pelletStock > 0;

		[Button()]
		public void CloudContact()
		{
			if (consuming is null) consuming = StartCoroutine(EatingCloud());
		}
		public void ReceivedCloud()
		{
			pelletStock = Mathf.Clamp(pelletStock + pelletPerConsum, 0, maxPelletStock);
		}
		private IEnumerator EatingCloud()
		{
			if (pelletStock <= 0) onLootCloud?.Invoke();
			isHoldingCloud = true;
			yield return new WaitForSeconds(holdDuration);

			ReceivedCloud();
			onConsumeCloud?.Invoke();
			
			isHoldingCloud = false;
			consuming = null;
		}
		
		public bool AddPullet()
		{
			if (pelletStock < maxPelletStock)
			{
				if (pelletStock <= 0) onLootCloud?.Invoke();
				pelletStock++;
				return true;
			}
			return false;
		}
		public bool RemovePullet()
		{
			if (pelletStock > 0)
			{
				pelletStock--;
				if (pelletStock <= 0) onLoseCloud?.Invoke();
				return true;
			}
			return false;
		}

	}
}
