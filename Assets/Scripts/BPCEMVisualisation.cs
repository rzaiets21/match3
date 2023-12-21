using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BPCEMVisualisation : MonoBehaviour
{
	[Serializable]
	public class Params
	{
		public Vector3 center;

		public Vector3 size;

		public Vector3 position;
	}

	public Vector3 center;

	public Vector3 size;

	public Vector3 position;

	[SerializeField]
	private bool enableGizmos;

	public List<Transform> includeOtherTransforms = new List<Transform>();

	public bool useLayerMask;

	public LayerMask includeLayers;

	public Params params1 = new Params();

	public bool useLocalValues;

	public ReflectionProbe reflectionProbe;

	public ReflectionProbe reflectionProbe1;

	public bool setParamsFromReflectionProbe;

	public bool createGameObjectsListFromChildren;

	public bool usePosition;

	public List<GameObject> gameObjects = new List<GameObject>();

	public List<Material> materialsList = new List<Material>();

	[SerializeField]
	private float mainLightFactor = 1f;

	[SerializeField]
	private IBLDescription imageBasedLighting;

	[SerializeField]
	private float diffuseLightAmmount = 1f;

	[SerializeField]
	private float specularLightAmmount = 1f;

	[SerializeField]
	private float minLuminosityInCubemap = 1f;

	[SerializeField]
	private float luminosityPower = 1f;

	[SerializeField]
	private float irradianceLuminosity = 1f;

	private const string _BBoxMin = "_BBoxMin";

	private const string _BBoxMax = "_BBoxMax";

	private const string _EnviCubeMapPos = "_EnviCubeMapPos";

	private const string _BBoxMin1 = "_BBoxMin1";

	private const string _BBoxMax1 = "_BBoxMax1";

	private const string _EnviCubeMapPos1 = "_EnviCubeMapPos1";

	private void OnDrawGizmos()
	{
		if (base.enabled && enableGizmos)
		{
			Renderer component = GetComponent<Renderer>();
			if (!(component == null) && !(component.sharedMaterial == null))
			{
				Color color = Gizmos.color;
				Gizmos.color = Color.green;
				Vector3 b = GetPosition(center);
				Vector3 vector = GetPosition(center + size) - b;
				Gizmos.DrawWireCube(b, vector);
				Gizmos.color = color;
			}
		}
	}

	private Vector3 WorldToLocal(Vector3 pos)
	{
		return base.transform.InverseTransformPoint(pos);
	}

	private Vector3 GetPositionToSet(Vector3 pos)
	{
		if (useLocalValues)
		{
			return WorldToLocal(pos);
		}
		return pos;
	}

	private Vector3 GetPosition(Vector3 pos)
	{
		if (useLocalValues)
		{
			return base.transform.TransformPoint(pos);
		}
		return pos;
	}

	private void SetParamsFromReflectionProbe()
	{
		if (!(reflectionProbe == null))
		{
			center = GetPositionToSet(reflectionProbe.bounds.center);
			size = 2f * (GetPositionToSet(reflectionProbe.bounds.max) - center);
			position = GetPositionToSet(reflectionProbe.transform.position);
			if (!(reflectionProbe1 == null))
			{
				params1.center = GetPositionToSet(reflectionProbe1.bounds.center);
				params1.size = 2f * (GetPositionToSet(reflectionProbe1.bounds.max) - params1.center);
				params1.position = GetPositionToSet(reflectionProbe1.transform.position);
			}
		}
	}

	private void CreateGameObjectsListFromChildren()
	{
		gameObjects.Clear();
		materialsList.Clear();
		CreateGameObjectsListFromChildren(base.transform, materialsList);
		for (int i = 0; i < includeOtherTransforms.Count; i++)
		{
			Transform transform = includeOtherTransforms[i];
			CreateGameObjectsListFromChildren(transform, materialsList);
		}
	}

	public void CreateGameObjectsListFromChildren(Transform transform, List<Material> materialsList)
	{
		foreach (Transform item in transform)
		{
			if (item.GetComponent<BPCEMVisualisation>() == null)
			{
				CreateGameObjectsListFromChildren(item, materialsList);
			}
			if (((1 << item.gameObject.layer) & includeLayers.value) != 0 || !useLayerMask)
			{
				Renderer component = item.gameObject.GetComponent<Renderer>();
				if (!(component == null) && !(component.sharedMaterial == null))
				{
					Material[] sharedMaterials = component.sharedMaterials;
					foreach (Material material in sharedMaterials)
					{
						if (!(material == null) && material.HasProperty("_BBoxMin") && material.HasProperty("_BBoxMax") && material.HasProperty("_EnviCubeMapPos"))
						{
							materialsList.Add(material);
						}
					}
				}
			}
		}
	}

	public void UpdateMaterials(List<Material> materialsList)
	{
		Vector3 vector = GetPosition(center);
		Vector3 a = GetPosition(center + size) - vector;
		Vector3 v = GetPosition(position);
		Vector3 vector2 = GetPosition(params1.center);
		Vector3 a2 = GetPosition(params1.center + params1.size) - vector2;
		Vector3 v2 = GetPosition(params1.position);
		if (!usePosition)
		{
			v = vector;
			v2 = vector2;
		}
		Vector3 v3 = vector - a / 2f;
		Vector3 v4 = vector + a / 2f;
		Vector3 v5 = vector2 - a2 / 2f;
		Vector3 v6 = vector2 + a2 / 2f;
		for (int i = 0; i < materialsList.Count; i++)
		{
			Material material = materialsList[i];
			if (!(material == null))
			{
				material.SetVector("_BBoxMin", v3);
				material.SetVector("_BBoxMax", v4);
				material.SetVector("_EnviCubeMapPos", v);
				if (material.HasProperty("_BBoxMax1"))
				{
					material.SetVector("_BBoxMin1", v5);
					material.SetVector("_BBoxMax1", v6);
					material.SetVector("_EnviCubeMapPos1", v2);
				}
			}
		}
	}

	public void UpdateMaterials()
	{
		if (imageBasedLighting != null)
		{
			Shader.SetGlobalFloat("_GG_DiffuseAmount_", imageBasedLighting.diffuseLightAmmount * mainLightFactor);
			Shader.SetGlobalFloat("_GG_SpecularAmount_", imageBasedLighting.specularLightAmmount * mainLightFactor);
			Shader.SetGlobalFloat("_GG_Min_Luminosity_", imageBasedLighting.minLuminosityInCubemap);
			Shader.SetGlobalFloat("_GG_Luminosity_Pow_", imageBasedLighting.luminosityPower);
			Shader.SetGlobalFloat("_GG_Irradiance_Luminosity_", imageBasedLighting.irradianceLuminosity);
			if (Application.isEditor && reflectionProbe != null)
			{
				reflectionProbe.customBakedTexture = imageBasedLighting.cubemap;
			}
		}
		else
		{
			Shader.SetGlobalFloat("_GG_DiffuseAmount_", diffuseLightAmmount * mainLightFactor);
			Shader.SetGlobalFloat("_GG_SpecularAmount_", specularLightAmmount * mainLightFactor);
			Shader.SetGlobalFloat("_GG_Min_Luminosity_", minLuminosityInCubemap);
			Shader.SetGlobalFloat("_GG_Luminosity_Pow_", luminosityPower);
			Shader.SetGlobalFloat("_GG_Irradiance_Luminosity_", irradianceLuminosity);
		}
		if (setParamsFromReflectionProbe)
		{
			SetParamsFromReflectionProbe();
		}
		Vector3 vector = GetPosition(center);
		Vector3 a = GetPosition(center + size) - vector;
		Vector3 v = GetPosition(position);
		Vector3 vector2 = GetPosition(params1.center);
		Vector3 a2 = GetPosition(params1.center + params1.size) - vector2;
		Vector3 v2 = GetPosition(params1.position);
		if (!usePosition)
		{
			v = vector;
			v2 = vector2;
		}
		Vector3 v3 = vector - a / 2f;
		Vector3 v4 = vector + a / 2f;
		Vector3 v5 = vector2 - a2 / 2f;
		Vector3 v6 = vector2 + a2 / 2f;
		if (Application.isEditor && !Application.isPlaying && createGameObjectsListFromChildren)
		{
			CreateGameObjectsListFromChildren();
		}
		List<Material> list = materialsList;
		for (int i = 0; i < list.Count; i++)
		{
			Material material = list[i];
			if (!(material == null))
			{
				material.SetVector("_BBoxMin", v3);
				material.SetVector("_BBoxMax", v4);
				material.SetVector("_EnviCubeMapPos", v);
				if (material.HasProperty("_BBoxMax1"))
				{
					material.SetVector("_BBoxMin1", v5);
					material.SetVector("_BBoxMax1", v6);
					material.SetVector("_EnviCubeMapPos1", v2);
				}
			}
		}
	}

	private void OnEnable()
	{
		UpdateMaterials();
	}

	private void Update()
	{
		if (Application.isEditor)
		{
			UpdateMaterials();
		}
	}
}
