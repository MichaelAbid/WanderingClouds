using UnityEngine;
using System.Linq;

/// <summary>
/// Abstract class for making reload-proof singletons out of ScriptableObjects
/// Returns the asset created on the editor, or null if there is none
/// Based on https://www.youtube.com/watch?v=VBA1QCoEAX4
/// </summary>
/// <typeparam name="T">Singleton type</typeparam>

public abstract class SingletonSCO<T> : ScriptableObject where T : ScriptableObject
{
	static T instance = null;
	public static T Instance
	{
		get
		{
			if (!instance)
				instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
			return instance;
		}
	}
}
