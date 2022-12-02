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
		public UnityEvent onConsumeCloud;
		[SerializeField] private int pelletStock = 0;
		[SerializeField] private int pelletPerConsum = 3;
		[SerializeField] private int maxPelletStock = 5;
		private Coroutine consuming = null;

		[Button()]
		public void CloudContact()
		{
			if(consuming is null) consuming = StartCoroutine(EatingCloud());
		}
		public void ReceivedCloud()
		{
			pelletStock = Mathf.Clamp(pelletStock + pelletPerConsum, 0, maxPelletStock);
		}
		private IEnumerator EatingCloud()
		{
			isHoldingCloud = true;
			yield return new  WaitForSeconds(holdDuration);
			
			pelletStock = Mathf.Clamp(pelletStock + pelletPerConsum, 0, maxPelletStock);
			onConsumeCloud?.Invoke();
			
			isHoldingCloud = false;
			consuming = null;
		}
		

	}
}
