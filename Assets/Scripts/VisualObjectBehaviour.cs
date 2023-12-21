using JSONData;
using System;
using System.Collections.Generic;
using UnityEngine;

public class VisualObjectBehaviour : MonoBehaviour
{
	public VisualObjectVariation defaultVariation;

	public List<VisualObjectVariation> variations = new List<VisualObjectVariation>();

	public List<VisualObjectVariation> allVariations = new List<VisualObjectVariation>();

	public GraphicsSceneConfig.VisualObject visualObject;

	[SerializeField]
	public CharacterVisualObjectBehaviour characterBehaviour;

	private DecoratingSceneConfig.VisualObjectOverride visualObjectOverride;

	private bool isMarkersCreated;

	[NonSerialized]
	private Transform markersTransform;

	public bool isPlayerControlledObject => variations.Count > 0;

	public bool hasDefaultVariation => defaultVariation != null;

	public Vector3 iconHandlePosition
	{
		get
		{
			Vector3 vector = visualObject.iconHandlePosition;
			if (visualObjectOverride != null)
			{
				vector += visualObjectOverride.iconHandlePositionOffset;
			}
			return vector;
		}
	}

	public Vector3 iconHandleScale
	{
		get
		{
			if (visualObjectOverride == null)
			{
				return Vector3.one;
			}
			return visualObjectOverride.iconHandlePositionScale;
		}
	}

	public Quaternion iconHandleRotation
	{
		get
		{
			if (visualObjectOverride == null)
			{
				return Quaternion.identity;
			}
			return Quaternion.Euler(visualObjectOverride.iconHandleRotation);
		}
	}

	public VisualObjectVariation activeVariation => variations[visualObject.ownedVariationIndex];

	public void SetMarkersActive(bool active)
	{
		GGUtil.SetActive(markersTransform, active);
	}

	public void InitMarkers(GameObject markerPrefab)
	{
		if (!isPlayerControlledObject || isMarkersCreated)
		{
			return;
		}
		isMarkersCreated = true;
		markersTransform = new GameObject("markers").transform;
		markersTransform.parent = base.transform;
		markersTransform.localPosition = Vector3.zero;
		markersTransform.localScale = Vector3.one;
		List<ShapeGraphShape> dashLines = visualObject.dashLines;
		if (dashLines == null || dashLines.Count == 0)
		{
			GGUtil.SetActive(markersTransform, active: false);
			return;
		}
		List<Vector2> list = new List<Vector2>();
		int depthForMarkerLines = visualObject.depthForMarkerLines;
		for (int i = 0; i < dashLines.Count; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(markerPrefab);
			gameObject.transform.parent = markersTransform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localRotation = Quaternion.identity;
			DistortedSprite component = gameObject.GetComponent<DistortedSprite>();
			GGUtil.SetActive(component, active: true);
			component.sortingSettings.sortingOrder = depthForMarkerLines;
			ShapeGraphShape shapeGraphShape = dashLines[i];
			List<Vector2> list2 = list;
			list2.Clear();
			Transform transform = component.transform;
			for (int j = 0; j < shapeGraphShape.points.Count; j++)
			{
				Vector2 item = shapeGraphShape.points[j];
				list2.Add(item);
			}
			if (shapeGraphShape.GetOrientation() == ShapeGraphShape.Orientation.CW)
			{
				component.bl = list2[0];
				component.tl = list2[1];
				component.tr = list2[2];
				component.br = list2[3];
			}
			else
			{
				component.bl = list2[3];
				component.tl = list2[2];
				component.tr = list2[1];
				component.br = list2[0];
			}
			component.CreateGeometry();
			GGUtil.SetActive(gameObject, active: true);
		}
	}

	private VisualObjectVariation CreateVariation(GraphicsSceneConfig.Variation variation)
	{
		GameObject gameObject = new GameObject();
		gameObject.transform.parent = base.transform;
		gameObject.name = variation.name;
		gameObject.transform.localPosition = Vector3.zero;
		VisualObjectVariation visualObjectVariation = gameObject.AddComponent<VisualObjectVariation>();
		visualObjectVariation.Init(this, variation);
		return visualObjectVariation;
	}

	public void InitRuntimeData(DecoratingSceneConfig.RoomConfig roomConfig)
	{
		visualObjectOverride = null;
		if (roomConfig == null)
		{
			return;
		}
		SceneObjectsDB.SceneObjectInfo sceneObjectInfo = visualObject.sceneObjectInfo;
		if (sceneObjectInfo.isVisualObjectOverriden)
		{
			visualObjectOverride = sceneObjectInfo.objectOverride;
		}
		if (Application.isEditor)
		{
			DecoratingSceneConfig.VisualObjectOverride objectOverride = roomConfig.GetObjectOverride(visualObject.name);
			if (objectOverride != null && !objectOverride.isSettingSaved)
			{
				visualObjectOverride = objectOverride;
			}
		}
	}

	public void Init(RoomsBackend.RoomAccessor roomAccessor)
	{
		visualObject.Init(roomAccessor);
	}

	public void Init(GraphicsSceneConfig.VisualObject visualObject)
	{
		this.visualObject = visualObject;
		for (int i = 0; i < visualObject.variations.Count; i++)
		{
			GraphicsSceneConfig.Variation variation = visualObject.variations[i];
			VisualObjectVariation item = CreateVariation(variation);
			variations.Add(item);
			allVariations.Add(item);
		}
		if (visualObject.hasDefaultVariation)
		{
			defaultVariation = CreateVariation(visualObject.defaultVariation);
			allVariations.Add(defaultVariation);
		}
	}

	public void DestroySelf()
	{
		for (int i = 0; i < allVariations.Count; i++)
		{
			VisualObjectVariation visualObjectVariation = allVariations[i];
			if (!(visualObjectVariation == null))
			{
				visualObjectVariation.DestroySelf();
			}
		}
		variations.Clear();
		Destroy(base.gameObject);
	}

	public void SetVisualState()
	{
		bool isOwned = visualObject.isOwned;
		if (defaultVariation != null && !isOwned)
		{
			ShowVariation(defaultVariation);
		}
		else if (!isOwned)
		{
			Hide();
		}
		else
		{
			ShowVariationBehaviour(visualObject.ownedVariationIndex);
		}
	}

	public void ShowVariationBehaviour(int variationIndex)
	{
		VisualObjectVariation variation = null;
		if (variationIndex >= 0 && variationIndex < variations.Count)
		{
			variation = variations[variationIndex];
		}
		ShowVariation(variation);
	}

	private void ShowVariation(VisualObjectVariation variation)
	{
		int variationIndex = -1;
		for (int i = 0; i < allVariations.Count; i++)
		{
			VisualObjectVariation visualObjectVariation = allVariations[i];
			bool flag = visualObjectVariation == variation;
			if (flag)
			{
				variationIndex = i;
			}
			visualObjectVariation.SetActive(flag);
		}
		if (characterBehaviour != null)
		{
			characterBehaviour.ShowGlobalVariation(variationIndex);
		}
	}

	public void Hide()
	{
		ShowVariation(null);
	}

	public static void Destroy(GameObject obj)
	{
		if (!Application.isPlaying)
		{
			UnityEngine.Object.DestroyImmediate(obj);
		}
		else
		{
			UnityEngine.Object.Destroy(obj);
		}
	}
}
