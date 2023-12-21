using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UIGGParticleCreator
{
	[Serializable]
	public class ParticleSettings
	{
		public string name;

		public GameObject particlePrefab;

		public Transform parent;

		public bool keepTransform;
	}

	private List<GameObject> createdGameObjects = new List<GameObject>();

	public List<ParticleSettings> settings = new List<ParticleSettings>();

	public ParticleSettings GetSettings(string name)
	{
		for (int i = 0; i < settings.Count; i++)
		{
			ParticleSettings particleSettings = settings[i];
			if (particleSettings.name == name)
			{
				return particleSettings;
			}
		}
		return null;
	}

	public void DestroyCreatedObjects()
	{
		for (int i = 0; i < createdGameObjects.Count; i++)
		{
			GameObject gameObject = createdGameObjects[i];
			if (!(gameObject == null))
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}
		createdGameObjects.Clear();
	}

	private GameObject Create(GameObject prefab)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(prefab);
		createdGameObjects.Add(gameObject);
		return gameObject;
	}

	public void CreateAndRunParticles(string name, Transform origin)
	{
		ParticleSettings particleSettings = GetSettings(name);
		if (particleSettings != null)
		{
			CreateAndRunParticles(particleSettings.particlePrefab, particleSettings.parent, origin, particleSettings.keepTransform);
		}
	}

	public void CreateAndRunParticles(GameObject particlePrefab, Transform parent, Transform origin, bool keepTransform)
	{
		if (!(particlePrefab == null))
		{
			GameObject gameObject = Create(particlePrefab);
			Transform transform = gameObject.transform;
			transform.parent = parent;
			transform.localPosition = Vector3.zero;
			transform.localScale = Vector3.one;
			transform.localRotation = Quaternion.identity;
			if (keepTransform)
			{
				transform.localPosition = parent.InverseTransformPoint(particlePrefab.transform.position);
				transform.rotation = particlePrefab.transform.rotation;
			}
			if (origin != null)
			{
				transform.localPosition = parent.InverseTransformPoint(origin.position);
			}
			ParticleSystem component = transform.GetComponent<ParticleSystem>();
			GGUtil.SetActive(gameObject, active: true);
			component.Play();
		}
	}
}
