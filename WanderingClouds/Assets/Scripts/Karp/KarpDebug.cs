using System.Collections.Generic;
using System.Collections;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;

namespace WanderingCloud
{
	public class KarpDebug : MonoBehaviour
	{

	#region Variables
	public ReadOnlyArray<InputUser> value;
	[SerializeField] int userCount;
	#endregion

	#region UnityMethods
	private void OnEnable() {}
	private void OnDisable() {}

	private void Update()
	{
		value = InputUser.all;
		userCount = value.Count;
	}
	#endregion
	

	}
}
