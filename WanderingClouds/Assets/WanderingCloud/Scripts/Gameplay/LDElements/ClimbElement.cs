using System;
using UnityEditor;
using UnityEngine;
using WanderingCloud.Controller;
using DG.Tweening;

namespace WanderingCloud
{
	[RequireComponent(typeof(BoxCollider))]
	public class ClimbElement : MonoBehaviour
	{
		[SerializeField] private Vector3 endPos;
		[SerializeField] private float climbDuration;
		[SerializeField] private bool isForGiro;

		private void Awake()
		{
			this.GetComponent<Collider>().isTrigger = true;
		}

		public void Climb(Pawn pawn)
		{
			if (pawn.isGyro == isForGiro)
			{
				Debug.Log("climb");
				pawn.transform.DOMove(endPos, climbDuration);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.GetComponentInParent<PlayerBrain>())
			{
				var interact = other.GetComponentInParent<PlayerInteraction>();
				interact.onInteractBegin.AddListener(Climb);
			}
		}
		private void OnTriggerExit(Collider other)
		{
			if (other.GetComponentInParent<PlayerBrain>())
			{
				var interact = other.GetComponentInParent<PlayerInteraction>();
				interact.onInteractBegin.RemoveListener(Climb);
			}
		}

		private void OnDrawGizmosSelected()
		{
			endPos = Handles.DoPositionHandle(endPos, Quaternion.identity);
		}
	}
}
