using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneObjectsDB : ScriptableObject
{
	[Serializable]
	public class SceneDependencies
	{
		public string name;

		public string assetsPath;

		public List<SceneObjectInfo> objectInfos = new List<SceneObjectInfo>();
	}

	[Serializable]
	public class MarginsInfo
	{
		public int totalWidth;

		public int leftMargin;

		public int rightMargin;

		public int totalHeight;

		public int bottomMargin;

		public int topMargin;

		public Vector2 visibleScenePercent => new Vector2(1f - (float)(leftMargin + rightMargin) / (float)totalWidth, 1f - (float)(topMargin + bottomMargin) / (float)totalHeight);

		public Vector2 marginsOffset => new Vector2(-leftMargin + rightMargin, -topMargin + bottomMargin);
	}

	[Serializable]
	public class SceneObjectInfo
	{
		public enum AnimationType
		{
			ScaleAnimation
		}

		public string objectName;

		public string displayName;

		public List<string> backwardDependencies = new List<string>();

		public SingleCurrencyPrice price = new SingleCurrencyPrice(1, CurrencyType.diamonds);

		public bool autoSelect;

		public List<string> toSayAfterOpen = new List<string>();

		public int groupIndex = -1;

		public bool isMarkersAbove;

		public string thumbnailNamePrefix;

		public string iconSpriteName;

		public AnimationType animationType;

		public string animationSettingsName;

		public bool usedDashedLineForIconHandlePosition;

		public bool hideCharacterWhenSelectingVariations;

		public bool isVisualObjectOverriden;

		public DecoratingSceneConfig.VisualObjectOverride objectOverride = new DecoratingSceneConfig.VisualObjectOverride();

		public string sceneName;
	}

	public List<SceneDependencies> scenes = new List<SceneDependencies>();

	public MarginsInfo maxMargins = new MarginsInfo();

	public MarginsInfo minMargins = new MarginsInfo();

	private static SceneObjectsDB _instance;

	public static SceneObjectsDB instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Resources.Load<SceneObjectsDB>("SceneObjectsDB");
			}
			return _instance;
		}
	}
}
