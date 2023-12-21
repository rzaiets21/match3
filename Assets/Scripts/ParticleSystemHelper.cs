using UnityEngine;

public class ParticleSystemHelper
{
	public static void SetEmmisionActive(ParticleSystem ps, bool active)
	{
		if (!(ps == null))
		{
			var emissionModule = ps.emission;
			emissionModule.enabled = active;
		}
	}

	public static void SetEmmisionActiveRecursive(ParticleSystem ps, bool active)
	{
		if (!(ps == null))
		{
			SetEmmisionActive(ps, active);
			Transform transform = ps.transform;
			for (int i = 0; i < transform.childCount; i++)
			{
				SetEmmisionActiveRecursive(transform.GetChild(i).GetComponent<ParticleSystem>(), active);
			}
		}
	}
}
