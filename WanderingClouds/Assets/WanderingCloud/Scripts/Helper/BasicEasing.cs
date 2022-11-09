using UnityEngine;

/// <summary>
/// Source https://easings.net/
/// </summary>
public static class BasicEasing
{
	public static float InOutSine(float t)
	{
		return -(Mathf.Cos(Mathf.PI * t) - 1.0f) * 0.5f;
	}

	public static float InOutCubic(float t)
	{
		return t < 0.5 ? 
			4 * t * t * t : 
			1 - Mathf.Pow(-2 * t + 2, 3) * 0.5f;
	}

	public static float InOutCirc(float t)
	{
		return t < 0.5 ?
			(1 - Mathf.Sqrt(1 - Mathf.Pow(2 * t, 2))) * 0.5f :
			(Mathf.Sqrt(1 - Mathf.Pow(-2 * t + 2, 2)) + 1) * 0.5f;
	}

	public static float OutElastic(float t)
	{
		const float c = Mathf.PI/2;

		return t == 0 ? 0 : 
			   t == 1 ? 1 : 
			   Mathf.Pow(2, -15 * t) * Mathf.Sin((t * 20 - 2.5f) * c) + 1;
	}

/// <summary>
/// Avec anticipation
/// </summary>
	// https://easings.net/fr#easeInOutBack
	const float c1 = 1.70158f;
	const float c2 = c1 * 1.525f;
	public static float InOutBack(float t)
	{
		return t < 0.5 ?
		(Mathf.Pow(2 * t, 2) * ((c2 + 1) * 2 * t - c2)) * 0.5f :
		(Mathf.Pow(2 * t - 2, 2) * ((c2 + 1) * (t * 2 - 2) + c2) + 2) * 0.5f;
	}
}
