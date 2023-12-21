using GGMatch3;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomCharacterScene : MonoBehaviour
{
	[SerializeField]
	private DecoratingScene scene;

	[SerializeField]
	private Transform rootTransform;

	[SerializeField]
	private Transform offset;

	[SerializeField]
	private Transform spaceRoot;

	[SerializeField]
	public Camera worldCamera;

	[SerializeField]
	private float distanceFromCamera = 20f;

	[SerializeField]
	public Material geoMaterial;

	[SerializeField]
	private SpriteSortingSettings geoSortingLayer = new SpriteSortingSettings();

	[SerializeField]
	public Material spriteMaterial;

	[SerializeField]
	private LayerMask layer;

	[SerializeField]
	private List<CharacterVisualObjectBehaviour> visualObjectBehaviours = new List<CharacterVisualObjectBehaviour>();

	private List<Transform> transformsToRemove = new List<Transform>();

	private CharacterVisualObjectBehaviour GetVisualObjectBehaviourByName(string name)
	{
		for (int i = 0; i < visualObjectBehaviours.Count; i++)
		{
			CharacterVisualObjectBehaviour characterVisualObjectBehaviour = visualObjectBehaviours[i];
			if (characterVisualObjectBehaviour.name == name)
			{
				return characterVisualObjectBehaviour;
			}
		}
		return null;
	}

	private void DestroyAll()
	{
		Transform transform = offset;
		if (transform == null)
		{
			return;
		}
		transformsToRemove.Clear();
		foreach (Transform item in transform)
		{
			transformsToRemove.Add(item);
		}
		for (int i = 0; i < transformsToRemove.Count; i++)
		{
			Transform transform2 = transformsToRemove[i];
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(transform2.gameObject);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(transform2.gameObject);
			}
		}
		visualObjectBehaviours.Clear();
	}

	private CharacterVisualObjectBehaviour CreateVisualObjectBehaviour(VisualObjectBehaviour vo)
	{
		GameObject gameObject = new GameObject();
		gameObject.transform.parent = offset;
		gameObject.name = vo.name;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.layer = offset.gameObject.layer;
		CharacterVisualObjectBehaviour characterVisualObjectBehaviour = gameObject.AddComponent<CharacterVisualObjectBehaviour>();
		characterVisualObjectBehaviour.Init(vo, this);
		return characterVisualObjectBehaviour;
	}

	public void Init(DecoratingScene3DSetup scene3dSetup, DecoratingScene decoratingScene, RenderTexture renderTexture)
	{
		InitGameObjects();
		scene = decoratingScene;
		GameObject gameObject = UnityEngine.Object.Instantiate(scene3dSetup.FindMainCamera().gameObject, spaceRoot);
		worldCamera = gameObject.GetComponent<Camera>();
		worldCamera.targetTexture = renderTexture;
		worldCamera.cullingMask = layer.value;
		Init();
		List<DecoratingScene3DSetup.VisualObject> visualObject = scene3dSetup.visualObjectList;
		int num = 1;
		for (int i = 0; i < visualObjectBehaviours.Count; i++)
		{
			CharacterVisualObjectBehaviour characterVisualObjectBehaviour = visualObjectBehaviours[i];
			DecoratingScene3DSetup.VisualObject forName = scene3dSetup.GetForName(characterVisualObjectBehaviour.visualObjectBeh.visualObject.name);
			if (forName != null)
			{
				Transform transform = forName.rootTransform;
				if (!(transform == null) && !(forName.collisionRoot == null))
				{
					GameObject gameObject2 = UnityEngine.Object.Instantiate(transform.gameObject, spaceRoot);
					Transform transform2 = gameObject2.transform;
					GGUtil.CopyWorldTransform(transform, transform2);
					CharacterVisualObjectSceneItem characterVisualObjectSceneItem = gameObject2.AddComponent<CharacterVisualObjectSceneItem>();
					characterVisualObjectSceneItem.Init(forName, geoMaterial, geoSortingLayer);
					characterVisualObjectSceneItem.stencilIndex = num;
					num++;
					characterVisualObjectBehaviour.sceneItem = characterVisualObjectSceneItem;
					characterVisualObjectBehaviour.Hide();
				}
			}
		}
		GGUtil.SetLayerRecursively(base.gameObject, layer);
	}

	private void InitGameObjects()
	{
		if (rootTransform == null)
		{
			GameObject gameObject = new GameObject("Root");
			rootTransform = gameObject.transform;
			rootTransform.parent = base.transform;
			rootTransform.localPosition = Vector3.zero;
			rootTransform.localScale = Vector3.one;
			rootTransform.localRotation = Quaternion.identity;
		}
		if (offset == null)
		{
			GameObject gameObject2 = new GameObject("Offset");
			offset = gameObject2.transform;
			offset.parent = rootTransform;
			offset.localPosition = Vector3.zero;
			offset.localScale = Vector3.one;
			offset.localRotation = Quaternion.identity;
		}
		if (spaceRoot == null)
		{
			GameObject gameObject3 = new GameObject("SpaceRoot");
			spaceRoot = gameObject3.transform;
			spaceRoot.parent = base.transform;
			spaceRoot.localPosition = Vector3.zero;
			spaceRoot.localScale = Vector3.one;
			spaceRoot.localRotation = Quaternion.identity;
		}
	}

	public Vector3 WorldToScreenPoint(Vector3 worldPoint)
	{
		return worldCamera.WorldToScreenPoint(worldPoint);
	}

	public void Init()
	{
		InitGameObjects();
		rootTransform.position = Vector3.zero;
		rootTransform.rotation = Quaternion.identity;
		rootTransform.localScale = Vector3.one;
		offset.localPosition = Vector3.left * scene.config.width * 0.5f + Vector3.up * scene.config.height * 0.5f;
		DestroyAll();
		List<VisualObjectBehaviour> list = scene.visualObjectBehaviours;
		for (int i = 0; i < list.Count; i++)
		{
			VisualObjectBehaviour visualObjectBehaviour = list[i];
			CharacterVisualObjectBehaviour item = visualObjectBehaviour.characterBehaviour = CreateVisualObjectBehaviour(visualObjectBehaviour);
			visualObjectBehaviours.Add(item);
		}
		Transform transform = worldCamera.transform;
		rootTransform.rotation = transform.rotation;
		Vector3[] array = new Vector3[4];
		worldCamera.CalculateFrustumCorners(new Rect(0f, 0f, 1f, 1f), distanceFromCamera, Camera.MonoOrStereoscopicEye.Mono, array);
		Vector3 a = worldCamera.transform.TransformVector(array[0]);
		Vector3 b = worldCamera.transform.TransformVector(array[1]);
		Vector3 b2 = worldCamera.transform.TransformVector(array[2]);
		Vector3 b3 = worldCamera.transform.TransformVector(array[3]);
		Vector3 position = (a + b + b2 + b3) * 0.25f;
		float num = worldCamera.sensorSize.x / worldCamera.sensorSize.y;
		int height = worldCamera.targetTexture.height;
		float num2 = (float)height * num;
		int width = scene.config.width;
		int height2 = scene.config.height;
		float aspect = worldCamera.aspect;
		float num3 = 2f * distanceFromCamera * Mathf.Tan(worldCamera.fieldOfView * 0.5f * ((float)Math.PI / 180f));
		float num4 = num3 * num;
		Vector3 vector = transform.InverseTransformPoint(position);
		vector.z = distanceFromCamera;
		float num5 = num3;
		float num6 = num4;
		float num7 = Mathf.Max(num5 / (float)scene.config.height, num6 / (float)scene.config.width);
		rootTransform.position = transform.position + transform.forward * distanceFromCamera + transform.right * num6 * worldCamera.lensShift.x + transform.up * num5 * worldCamera.lensShift.y;
		UnityEngine.Debug.Log("IMAGE WIDTH " + num2 + " resWidth " + width + " imageHeight " + height + " resolutionHeight " + height2);
		float num8 = num7 * (float)width / num2;
		float y = num8;
		rootTransform.localScale = new Vector3(num8, y, 1f);
	}
}
