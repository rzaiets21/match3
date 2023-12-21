using System.Collections.Generic;
using UnityEngine;

public class PlayerInputFollower : MonoBehaviour
{
	[SerializeField]
	private List<ParticleSystem> particles = new List<ParticleSystem>();

	[SerializeField]
	private List<TrailRenderer> trails = new List<TrailRenderer>();

	public void SetActive(bool active)
	{
		for (int i = 0; i < particles.Count; i++)
		{
			ParticleSystem particleSystem = particles[i];
			if (!(particleSystem == null))
			{
				GGUtil.SetActive(particleSystem.transform, active);
			}
		}
		for (int j = 0; j < trails.Count; j++)
		{
			TrailRenderer trailRenderer = trails[j];
			if (!(trailRenderer == null))
			{
				GGUtil.SetActive(trailRenderer, active);
			}
		}
	}

	public void Clear()
	{
		for (int i = 0; i < particles.Count; i++)
		{
			ParticleSystem particleSystem = particles[i];
			if (!(particleSystem == null))
			{
				particleSystem.Clear();
			}
		}
		for (int j = 0; j < trails.Count; j++)
		{
			TrailRenderer trailRenderer = trails[j];
			if (!(trailRenderer == null))
			{
				trailRenderer.Clear();
			}
		}
	}
}
