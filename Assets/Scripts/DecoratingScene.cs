using GGMatch3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class DecoratingScene : MonoBehaviour
{
	[Serializable]
	public class GroupDefinition
	{
		[Serializable]
		public class AnimationDef
		{
			public string groupName;

			public string animationName;
		}

		public int groupIndex;

		public string title;

		public List<string> toSayAfterGroupCompletes = new List<string>();

		public AnimationDef playAfterFinish = new AnimationDef();
	}

	public enum ImagesFolderName
	{
		Folder_2048,
		Folder_1024
	}

	public struct RoomProgressState
	{
		public int completed;

		public int total;

		public bool isPassed => completed >= total;

		public float progress => Mathf.InverseLerp(0f, total, completed);

		public float Progress(int removeCompleted)
		{
			return Mathf.InverseLerp(0f, total, completed - removeCompleted);
		}
	}

	private sealed class _003CDoZoomOutAnimation_003Ed__78 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public DecoratingScene _003C_003E4__this;

		private ConfirmPurchasePanel.Settings _003Csettings_003E5__2;

		private float _003Ctime_003E5__3;

		private Vector3 _003CstartScale_003E5__4;

		private Vector3 _003CstartPos_003E5__5;

		private Vector3 _003CendPos_003E5__6;

		private Vector3 _003CendScale_003E5__7;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CDoZoomOutAnimation_003Ed__78(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = _003C_003E1__state;
			DecoratingScene decoratingScene = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003Csettings_003E5__2 = Match3Settings.instance.confirmPurchasePanelSettings;
				_003Ctime_003E5__3 = 0f;
				_003CstartScale_003E5__4 = decoratingScene.rootTransformScale;
				_003CstartPos_003E5__5 = decoratingScene.rootTransformOffset;
				_003CendPos_003E5__6 = Vector3.zero;
				_003CendScale_003E5__7 = Vector3.one;
				break;
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			if (_003Ctime_003E5__3 <= _003Csettings_003E5__2.zoomOutDuration)
			{
				_003Ctime_003E5__3 += Time.deltaTime;
				float num2 = Mathf.InverseLerp(0f, _003Csettings_003E5__2.zoomOutDuration, _003Ctime_003E5__3);
				if (_003Csettings_003E5__2.outCurve != null)
				{
					num2 = _003Csettings_003E5__2.outCurve.Evaluate(num2);
				}
				Vector3 rootTransformOffset = Vector3.LerpUnclamped(_003CstartPos_003E5__5, _003CendPos_003E5__6, num2);
				Vector3 vector2 = decoratingScene.rootTransformScale = Vector3.LerpUnclamped(_003CstartScale_003E5__4, _003CendScale_003E5__7, num2);
				decoratingScene.rootTransformOffset = rootTransformOffset;
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			}
			return false;
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	private sealed class _003CDoZoomInAnimation_003Ed__79 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public VisualObjectBehaviour visualObjectBehaviour;

		public DecoratingScene _003C_003E4__this;

		private ConfirmPurchasePanel.Settings _003Csettings_003E5__2;

		private float _003Ctime_003E5__3;

		private Vector3 _003CstartScale_003E5__4;

		private Vector3 _003CstartPos_003E5__5;

		private Vector3 _003CendPos_003E5__6;

		private Vector3 _003CendScale_003E5__7;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CDoZoomInAnimation_003Ed__79(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = _003C_003E1__state;
			DecoratingScene decoratingScene = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
			{
				_003C_003E1__state = -1;
				Vector3 iconHandlePosition = visualObjectBehaviour.iconHandlePosition;
				_003Csettings_003E5__2 = Match3Settings.instance.confirmPurchasePanelSettings;
				Vector3 a = decoratingScene.PSDToWorldPoint(iconHandlePosition);
				_003Ctime_003E5__3 = 0f;
				_003CstartScale_003E5__4 = decoratingScene.rootTransformScale;
				_003CstartPos_003E5__5 = decoratingScene.rootTransformOffset;
				_003CendPos_003E5__6 = -a;
				_003CendPos_003E5__6 = Vector3.Lerp(Vector3.zero, _003CendPos_003E5__6, _003Csettings_003E5__2.moveTowardsFactor);
				_003CendScale_003E5__7 = new Vector3(_003Csettings_003E5__2.zoomInFactor, _003Csettings_003E5__2.zoomInFactor, 1f);
				_003CendScale_003E5__7 = Vector3.Lerp(Vector3.one, _003CendScale_003E5__7, _003Csettings_003E5__2.moveTowardsFactor);
				break;
			}
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			if (_003Ctime_003E5__3 <= _003Csettings_003E5__2.zoomInDuration)
			{
				_003Ctime_003E5__3 += Time.deltaTime;
				float num2 = Mathf.InverseLerp(0f, _003Csettings_003E5__2.zoomInDuration, _003Ctime_003E5__3);
				if (_003Csettings_003E5__2.curve != null)
				{
					num2 = _003Csettings_003E5__2.curve.Evaluate(num2);
				}
				Vector3 rootTransformOffset = Vector3.LerpUnclamped(_003CstartPos_003E5__5, _003CendPos_003E5__6, num2);
				Vector3 vector2 = decoratingScene.rootTransformScale = Vector3.LerpUnclamped(_003CstartScale_003E5__4, _003CendScale_003E5__7, num2);
				decoratingScene.rootTransformOffset = rootTransformOffset;
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			}
			return false;
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	[SerializeField]
	private List<GroupDefinition> groupDefinitions = new List<GroupDefinition>();

	[SerializeField]
	public List<VisualObjectBehaviour> visualObjectBehaviours = new List<VisualObjectBehaviour>();

	private List<VisualObjectBehaviour> activeBehaviours = new List<VisualObjectBehaviour>();

	public string sceneFolder;

	public string hierarchyJSONFile;

	public string hitboxJSONFile;

	public string metadataJSONFile;

	public string handlesJSONFile;

	public string dashLineJSONFile;

	[SerializeField]
	public bool shouldCreateCharacterScene;

	[SerializeField]
	public Transform rootTransform;

	[SerializeField]
	public RoomCharacterScene characterScene;

	[SerializeField]
	public CharacterAnimationPlayer animationPlayer;

	[SerializeField]
	public RoomSceneRenderObject roomSceneRender;

	[NonSerialized]
	public DecorateRoomScreen roomScreen;

	private Vector3 rootTransformOffset_ = Vector3.zero;

	private Vector3 rootTransformOffsetAcceleration_ = Vector3.zero;

	[SerializeField]
	public Transform offsetTransform;

	[SerializeField]
	public GraphicsSceneConfig config;

	public ImagesFolderName imagesFolder;

	public Color backgroundColor = Color.black;

	public bool showCollisions;

	private IEnumerator animationEnum;

	private IEnumerator characterAnimationEnum;

	public int ownedItemsCount
	{
		get
		{
			int num = 0;
			for (int i = 0; i < visualObjectBehaviours.Count; i++)
			{
				if (visualObjectBehaviours[i].visualObject.isOwned)
				{
					num++;
				}
			}
			return num;
		}
	}

	public DecoratingSceneConfig.AnimationSequence runningAnimation
	{
		get
		{
			if (animationPlayer == null)
			{
				return null;
			}
			return animationPlayer.lastAnimation;
		}
	}

	public bool isCharacterAvailable => animationPlayer != null;

	public Vector3 psdTransformationScale => psdTransformationTransform.localScale;

	public Vector3 rootTransformScale
	{
		get
		{
			return rootTransform.localScale;
		}
		set
		{
			rootTransform.localScale = value;
		}
	}

	public Vector3 rootTransformOffset
	{
		get
		{
			return rootTransformOffset_;
		}
		set
		{
			rootTransformOffset_ = value;
			SetRootTransform();
		}
	}

	public Vector3 rootTransformOffsetAcceleration
	{
		get
		{
			return rootTransformOffsetAcceleration_;
		}
		set
		{
			rootTransformOffsetAcceleration_ = value;
			SetRootTransform();
		}
	}

	private Transform psdTransformationTransform => offsetTransform;

	public string roomName => base.name;

	public RoomsBackend.RoomAccessor roomBackend => SingletonInit<RoomsBackend>.instance.GetRoom(roomName);

	public GroupDefinition GetGroupForIndex(int index)
	{
		for (int i = 0; i < groupDefinitions.Count; i++)
		{
			GroupDefinition groupDefinition = groupDefinitions[i];
			if (groupDefinition.groupIndex == index)
			{
				return groupDefinition;
			}
		}
		return null;
	}

	public GroupDefinition CurrentGroup()
	{
		for (int i = 0; i < groupDefinitions.Count; i++)
		{
			GroupDefinition groupDefinition = groupDefinitions[i];
			if (!IsAllElementsPickedUpInGroup(groupDefinition.groupIndex))
			{
				return groupDefinition;
			}
		}
		return null;
	}

	public bool IsAllElementsPickedUpInGroup(int index)
	{
		for (int i = 0; i < visualObjectBehaviours.Count; i++)
		{
			VisualObjectBehaviour visualObjectBehaviour = visualObjectBehaviours[i];
			if (visualObjectBehaviour.visualObject.sceneObjectInfo.groupIndex == index && !visualObjectBehaviour.visualObject.isOwned)
			{
				return false;
			}
		}
		return true;
	}

	public Vector3 CharacterBubblePosition(CharacterAvatar avatar)
	{
		if (avatar == null)
		{
			return Vector3.zero;
		}
		Vector3 bubblePosition = avatar.bubblePosition;
		Vector3 vector = characterScene.WorldToScreenPoint(bubblePosition);
		return PSDToWorldPoint(new Vector2(vector.x, vector.y - (float)config.height));
	}

	public List<DecoratingSceneConfig.AnimationSequence> GetAvailableSequences()
	{
		if (animationPlayer == null)
		{
			return null;
		}
		DecoratingSceneConfig.AnimationSequenceGroup animationSequenceGroup = ScriptableObjectSingleton<DecoratingSceneConfig>.instance.GetAnimationSequenceGroup(base.name);
		if (animationSequenceGroup == null)
		{
			UnityEngine.Debug.Log("NO GROUP " + base.name);
			return null;
		}
		return animationSequenceGroup.AvailableSequences(this);
	}

	public void StopCharacterAnimation()
	{
		if (!(animationPlayer == null))
		{
			animationPlayer.Stop();
			animationPlayer.HideAvatars();
		}
	}

	public ChangeAnimationArguments AnimationForVisualBehaviour(VisualObjectBehaviour behaviour)
	{
		if (animationPlayer == null || behaviour == null)
		{
			return ChangeAnimationArguments.NoAnimation;
		}
		return ChangeAnimationArguments.Create(base.name, behaviour.visualObject.name);
	}

	public void PlayCharacterAnimation(ChangeAnimationArguments arguments)
	{
		if (animationPlayer == null)
		{
			if (arguments.onComplete != null)
			{
				arguments.onComplete();
			}
		}
		else
		{
			animationPlayer.Play(arguments);
		}
	}

	public void SetCharacterAlpha(float alpha)
	{
		if (!(roomSceneRender == null))
		{
			roomScreen.SetSpeachBubbleAlpha(alpha);
			roomSceneRender.SetAlpha(alpha);
		}
	}

	public void SetCharacterAnimationAlpha(float alpha)
	{
		if (!(roomSceneRender == null))
		{
			roomSceneRender.animationAlpha = alpha;
			roomScreen.SetSpeachBubbleAlpha(alpha);
		}
	}

	public void AnimateCharacterAlphaTo(float alpha)
	{
		if (!(roomSceneRender == null))
		{
			roomSceneRender.AnimateAlphaTo(alpha, 0.25f, roomScreen);
		}
	}

	private void SetRootTransform()
	{
		rootTransform.localPosition = rootTransformOffsetAcceleration_ + rootTransformOffset_;
	}

	public GraphicsSceneConfig CreateConfig()
	{
		GraphicsSceneConfig graphicsSceneConfig = new GraphicsSceneConfig();
		graphicsSceneConfig.width = config.width;
		graphicsSceneConfig.height = config.height;
		graphicsSceneConfig.objects.Clear();
		for (int i = 0; i < visualObjectBehaviours.Count; i++)
		{
			VisualObjectBehaviour visualObjectBehaviour = visualObjectBehaviours[i];
			if (!(visualObjectBehaviour == null))
			{
				GraphicsSceneConfig.VisualObject item = JsonUtility.FromJson<GraphicsSceneConfig.VisualObject>(JsonUtility.ToJson(visualObjectBehaviour.visualObject));
				graphicsSceneConfig.objects.Add(item);
			}
		}
		return graphicsSceneConfig;
	}

	public void ShowAll()
	{
		for (int i = 0; i < visualObjectBehaviours.Count; i++)
		{
			VisualObjectBehaviour visualObjectBehaviour = visualObjectBehaviours[i];
			if (visualObjectBehaviour.isPlayerControlledObject)
			{
				visualObjectBehaviour.ShowVariationBehaviour(UnityEngine.Random.RandomRange(0, 3));
			}
		}
	}

	public void ShowGroup(int groupIndex)
	{
		for (int i = 0; i < visualObjectBehaviours.Count; i++)
		{
			VisualObjectBehaviour visualObjectBehaviour = visualObjectBehaviours[i];
			if (visualObjectBehaviour.isPlayerControlledObject)
			{
				if (visualObjectBehaviour.visualObject.sceneObjectInfo.groupIndex == groupIndex)
				{
					visualObjectBehaviour.ShowVariationBehaviour(0);
				}
				else
				{
					visualObjectBehaviour.Hide();
				}
			}
		}
	}

	public void DestroyCharacterScene()
	{
		if (roomSceneRender != null)
		{
			UnityEngine.Object.DestroyImmediate(roomSceneRender.gameObject, allowDestroyingAssets: true);
			roomSceneRender = null;
		}
		if (!(characterScene == null))
		{
			GameObject gameObject = characterScene.gameObject;
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(gameObject);
			}
			characterScene = null;
		}
	}

	private void DestroyAllObjectBehaviours()
	{
		List<VisualObjectBehaviour> list = new List<VisualObjectBehaviour>();
		list.AddRange(visualObjectBehaviours);
		foreach (Transform item in offsetTransform)
		{
			VisualObjectBehaviour component = item.GetComponent<VisualObjectBehaviour>();
			if (component == null)
			{
				return;
			}
			if (!list.Contains(component))
			{
				list.Add(component);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			VisualObjectBehaviour visualObjectBehaviour = list[i];
			if (!(visualObjectBehaviour == null))
			{
				visualObjectBehaviour.DestroySelf();
			}
		}
		visualObjectBehaviours.Clear();
	}

	private VisualObjectBehaviour CreateVisualObjectBehaviour(GraphicsSceneConfig.VisualObject vo)
	{
		GameObject gameObject = new GameObject();
		gameObject.transform.parent = offsetTransform;
		gameObject.name = vo.name;
		gameObject.transform.localPosition = Vector3.zero;
		VisualObjectBehaviour visualObjectBehaviour = gameObject.AddComponent<VisualObjectBehaviour>();
		visualObjectBehaviour.Init(vo);
		return visualObjectBehaviour;
	}

	public void CreateSceneBehaviours(GraphicsSceneConfig config)
	{
		DestroyAllObjectBehaviours();
		this.config = config;
		if (rootTransform == null)
		{
			rootTransform = new GameObject("root").transform;
			rootTransform.parent = base.transform;
			rootTransform.localPosition = Vector3.zero;
			rootTransform.localScale = Vector3.one;
			rootTransform.localRotation = Quaternion.identity;
		}
		if (offsetTransform == null)
		{
			offsetTransform = new GameObject("offset").transform;
			offsetTransform.parent = rootTransform;
			offsetTransform.localPosition = Vector3.zero;
			offsetTransform.localScale = Vector3.one;
			offsetTransform.localRotation = Quaternion.identity;
		}
		for (int i = 0; i < config.objects.Count; i++)
		{
			GraphicsSceneConfig.VisualObject vo = config.objects[i];
			VisualObjectBehaviour visualObjectBehaviour = CreateVisualObjectBehaviour(vo);
			if (visualObjectBehaviour.isPlayerControlledObject)
			{
				visualObjectBehaviours.Add(visualObjectBehaviour);
			}
		}
	}

	public RoomProgressState GetRoomProgressState()
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < visualObjectBehaviours.Count; i++)
		{
			VisualObjectBehaviour visualObjectBehaviour = visualObjectBehaviours[i];
			if (visualObjectBehaviour.isPlayerControlledObject)
			{
				num2++;
				if (visualObjectBehaviour.visualObject.isOwned)
				{
					num++;
				}
			}
		}
		RoomProgressState result = default(RoomProgressState);
		result.completed = num;
		result.total = num2;
		return result;
	}

	public VisualObjectBehaviour GetBehaviour(string name)
	{
		for (int i = 0; i < visualObjectBehaviours.Count; i++)
		{
			VisualObjectBehaviour visualObjectBehaviour = visualObjectBehaviours[i];
			if (visualObjectBehaviour.name.ToLower() == name)
			{
				return visualObjectBehaviour;
			}
		}
		return null;
	}

	public Vector3 PSDToWorldPoint(Vector2 point)
	{
		return psdTransformationTransform.TransformPoint(new Vector3(point.x, point.y, 0f));
	}

	public Vector3 WorldToPSDPoint(Vector2 point)
	{
		return psdTransformationTransform.InverseTransformPoint(new Vector3(point.x, point.y, 0f));
	}

	public void ResetZoomIn()
	{
		rootTransform.localPosition = Vector3.zero;
		rootTransform.localScale = Vector3.one;
	}

	public void InitRuntimeData()
	{
		string name = base.name;
		DecoratingSceneConfig.RoomConfig roomConfig = ScriptableObjectSingleton<DecoratingSceneConfig>.instance.GetRoomConfig(name);
		for (int i = 0; i < visualObjectBehaviours.Count; i++)
		{
			visualObjectBehaviours[i].InitRuntimeData(roomConfig);
		}
	}

	public void Init(Camera mainCamera, float additionalMargin, DecorateRoomScreen roomScreen)
	{
		this.roomScreen = roomScreen;
		if (animationPlayer != null)
		{
			animationPlayer.Init(this);
		}
		rootTransform.localPosition = Vector3.zero;
		rootTransform.localScale = Vector3.one;
		ScaleToFitInCamera(mainCamera, additionalMargin);
		Color color = backgroundColor;
		color.a = 1f;
		mainCamera.backgroundColor = color;
		RoomsBackend.RoomAccessor room = SingletonInit<RoomsBackend>.instance.GetRoom(base.name);
		for (int i = 0; i < visualObjectBehaviours.Count; i++)
		{
			visualObjectBehaviours[i].Init(room);
		}
		InitRuntimeData();
	}

	public void ZoomIn(VisualObjectBehaviour visualObjectBehaviour)
	{
		animationEnum = DoZoomInAnimation(visualObjectBehaviour);
	}

	public void ZoomOut()
	{
		animationEnum = null;
		if (!(rootTransformScale == Vector3.one) || !(rootTransformOffset == Vector3.zero))
		{
			animationEnum = DoZoomOutAnimation();
		}
	}

	private IEnumerator DoZoomOutAnimation()
	{
		return new _003CDoZoomOutAnimation_003Ed__78(0)
		{
			_003C_003E4__this = this
		};
	}

	private IEnumerator DoZoomInAnimation(VisualObjectBehaviour visualObjectBehaviour)
	{
		return new _003CDoZoomInAnimation_003Ed__79(0)
		{
			_003C_003E4__this = this,
			visualObjectBehaviour = visualObjectBehaviour
		};
	}

	private void ScaleToFitInCamera(Camera mainCamera, float additionalMargin)
	{
		Vector2 visibleScenePercent = SceneObjectsDB.instance.maxMargins.visibleScenePercent;
		Vector2 marginsOffset = SceneObjectsDB.instance.maxMargins.marginsOffset;
		int width = config.width;
		int height = config.height;
		Vector3 vector = mainCamera.ViewportToWorldPoint(Vector3.zero);
		Vector3 vector2 = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));
		float num = vector2.x - vector.x;
		float num2 = vector2.y - vector.y;
		Mathf.Min(visibleScenePercent.x, visibleScenePercent.y);
		float num3 = Mathf.Max(num / ((float)width * visibleScenePercent.x - additionalMargin * 2f), num2 / ((float)height * visibleScenePercent.y - additionalMargin * 2f));
		Transform psdTransformationTransform = this.psdTransformationTransform;
		psdTransformationTransform.localScale = new Vector3(num3, num3, 1f);
		psdTransformationTransform.position = new Vector3((float)(-width) + marginsOffset.x, (float)height - marginsOffset.y) * num3 * 0.5f;
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			if (characterAnimationEnum != null && !characterAnimationEnum.MoveNext())
			{
				characterAnimationEnum = null;
			}
			if (animationEnum != null && !animationEnum.MoveNext())
			{
				animationEnum = null;
			}
		}
	}
}
