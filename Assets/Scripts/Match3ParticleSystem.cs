using System;
using System.Collections.Generic;
using UnityEngine;

public class Match3ParticleSystem : MonoBehaviour
{
	[SerializeField]
	private bool includeHidden;

	private List<ParticleSystem> particleSystems = new List<ParticleSystem>();

	private bool started;

	private bool initialized;

	private bool destroyed;

	public void StartParticleSystems()
	{
		Init();
		started = true;
	}

	private void OnDisable()
	{
		if (!destroyed)
			Destroy();
	}

	public List<ParticleSystem> GetAllParticleSystems()
	{
		Init();
		return particleSystems;
	}

	private void Init()
	{
		if (!initialized)
		{
			particleSystems.Clear();
			initialized = true;
			TryAddParticleSystemFromTransform(base.transform);
		}
	}

	private void TryAddParticleSystemFromTransform(Transform t)
	{
		if (t == null)
		{
			return;
		}
		ParticleSystem component = t.GetComponent<ParticleSystem>();
		if (component != null)
		{
			particleSystems.Add(component);
		}
		for (int i = 0; i < t.childCount; i++)
		{
			Transform child = t.GetChild(i);
			if (child.gameObject.activeSelf || includeHidden)
			{
				TryAddParticleSystemFromTransform(child);
			}
		}
	}

	private void Update()
	{
		if (!started)
		{
			return;
		}
		for (int i = 0; i < particleSystems.Count; i++)
		{
			if (particleSystems[i].IsAlive())
			{
				return;
			}
		}

		Destroy();
	}

	private void Destroy()
	{
		destroyed = true;
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
