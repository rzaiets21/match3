using GGMatch3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ConfirmPurchasePanel : MonoBehaviour
{
	[Serializable]
	public class Settings
	{
		public AnimationCurve curve;

		public float zoomInDuration = 1f;

		public float zoomInFactor = 1.5f;

		public float moveTowardsFactor = 0.1f;

		public float zoomOutDuration = 0.5f;

		public AnimationCurve outCurve;

		public float inAnimationDuration;

		public Vector3 inAnimationZoomFrom;

		public Vector3 inAnimationZoomTo;

		public AnimationCurve inAnimationzoomAnimationCurve;

		public float selectorAnimationDuration;

		public Vector3 selectorAnimationZoomFrom;

		public float selectorAnimationAlphaFrom;

		public AnimationCurve selectorAnimationCurve;
	}

	[Serializable]
	public class NamedSprites
	{
		public string name;

		public Sprite iconSprite;

		public Sprite backgroundSprite;
	}

	private sealed class _003CDoInAnimation_003Ed__24 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public bool showTutorial;

		public ConfirmPurchasePanel _003C_003E4__this;

		private float _003Ctime_003E5__2;

		private Settings _003Csettings_003E5__3;

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
		public _003CDoInAnimation_003Ed__24(int _003C_003E1__state)
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
			ConfirmPurchasePanel confirmPurchasePanel = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				if (showTutorial)
				{
					confirmPurchasePanel.dragButton.StopAnimation();
				}
				_003Ctime_003E5__2 = 0f;
				_003Csettings_003E5__3 = confirmPurchasePanel.settings;
				break;
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			if (_003Ctime_003E5__2 <= _003Csettings_003E5__3.inAnimationDuration)
			{
				_003Ctime_003E5__2 += Time.deltaTime;
				float time = Mathf.InverseLerp(0f, _003Csettings_003E5__3.inAnimationDuration, _003Ctime_003E5__2);
				float t = _003Csettings_003E5__3.inAnimationzoomAnimationCurve.Evaluate(time);
				Vector3 localScale = Vector3.LerpUnclamped(_003Csettings_003E5__3.inAnimationZoomFrom, _003Csettings_003E5__3.inAnimationZoomTo, t);
				confirmPurchasePanel.scaleParent.transform.localScale = localScale;
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			}
			confirmPurchasePanel.ShowTutorialHandIfNeeded();
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

	private sealed class _003CDoSelectorAnimation_003Ed__26 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public ConfirmPurchasePanel _003C_003E4__this;

		private float _003Ctime_003E5__2;

		private Settings _003Csettings_003E5__3;

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
		public _003CDoSelectorAnimation_003Ed__26(int _003C_003E1__state)
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
			ConfirmPurchasePanel confirmPurchasePanel = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003Ctime_003E5__2 = 0f;
				_003Csettings_003E5__3 = confirmPurchasePanel.settings;
				break;
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			if (_003Ctime_003E5__2 <= _003Csettings_003E5__3.selectorAnimationDuration)
			{
				_003Ctime_003E5__2 += Time.deltaTime;
				float time = Mathf.InverseLerp(0f, _003Csettings_003E5__3.selectorAnimationDuration, _003Ctime_003E5__2);
				float t = _003Csettings_003E5__3.selectorAnimationCurve.Evaluate(time);
				Vector3 localScale = Vector3.LerpUnclamped(_003Csettings_003E5__3.selectorAnimationZoomFrom, Vector3.one, t);
				confirmPurchasePanel.selectorTransform.transform.localScale = localScale;
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
	private float distanceOfSourceFromTarget = 110f;

	[SerializeField]
	private Vector3 centerPositionForTarget = Vector3.zero;

	[SerializeField]
	private Vector3 falloffDistanceForTarget = Vector3.zero;

	private DecorateRoomSceneVisualItem visualItem;

	private DecorateRoomScreen screen;

	[SerializeField]
	private DragButton dragButton;

	[SerializeField]
	public DragTarget dragTarget;

	[SerializeField]
	private TextMeshProUGUI dragSourceItemNameText;

	[SerializeField]
	private TextMeshProUGUI dragSourcePriceText;

	[SerializeField]
	private RectTransform arrowTransform;

	[SerializeField]
	private RectTransform constrainRect;

	[SerializeField]
	private RectTransform dragSourceRectTransform;

	[SerializeField]
	private RectTransform scaleParent;

	[SerializeField]
	private List<NamedSprites> iconSprites = new List<NamedSprites>();

	[SerializeField]
	private RectTransform backgroundSelected;

	[SerializeField]
	private TrailRenderer trailRenderer;

	[SerializeField]
	private RectTransform selectorTransform;

	private IEnumerator inAnimation;

	private IEnumerator selectorAnimation;

	private bool showTutorialHand;

	public Settings settings => Match3Settings.instance.confirmPurchasePanelSettings;

	public void Show(DecorateRoomSceneVisualItem visualItem, DecorateRoomScreen screen)
	{
		this.visualItem = visualItem;
		this.screen = screen;
		showTutorialHand = visualItem.visualObjectBehaviour.visualObject.sceneObjectInfo.autoSelect;
		string iconSpriteName = visualItem.visualObjectBehaviour.visualObject.sceneObjectInfo.iconSpriteName;
		NamedSprites namedSprites = iconSprites[0];
		for (int i = 1; i < iconSprites.Count; i++)
		{
			if (iconSprites[i].name == iconSpriteName)
			{
				namedSprites = iconSprites[i];
				break;
			}
		}
		dragButton.Init(this, namedSprites.iconSprite);
		dragTarget.Init(this, namedSprites.backgroundSprite);
		GraphicsSceneConfig.VisualObject visualObject = visualItem.visualObjectBehaviour.visualObject;
		dragSourceItemNameText.text = visualObject.displayName;
		Debug.LogError(visualObject.sceneObjectInfo.objectName);
		Debug.LogError(visualObject.sceneObjectInfo.displayName);
		Debug.LogError(visualObject.sceneObjectInfo.price.cost);
		Debug.LogError(visualObject.sceneObjectInfo.price.currency);
		dragSourcePriceText.text = visualObject.sceneObjectInfo.price.cost.ToString();
		GGUtil.SetActive(dragSourcePriceText, visualObject.sceneObjectInfo.price.cost > 1);
		dragTarget.transform.position = screen.TransformPSDToWorldPoint(visualItem.visualObjectBehaviour.iconHandlePosition);
		GGUtil.uiUtil.RestrictRectTransform(dragTarget.transform as RectTransform, constrainRect);
		Vector3 localPosition = dragTarget.transform.localPosition;
		Vector3 zero = Vector3.zero;
		zero.x = ((localPosition.x <= centerPositionForTarget.x) ? 1 : (-1));
		zero.y = ((localPosition.y <= centerPositionForTarget.y) ? 1 : (-1));
		if (falloffDistanceForTarget.y > 0f)
		{
			zero.y *= Mathf.InverseLerp(0f, falloffDistanceForTarget.y, Mathf.Abs(centerPositionForTarget.y));
		}
		Vector3 localPosition2 = zero.normalized * distanceOfSourceFromTarget;
		dragSourceRectTransform.localPosition = localPosition2;
		GGUtil.uiUtil.RestrictRectTransform(dragSourceRectTransform, constrainRect);
		GGUtil.SetActive(backgroundSelected, active: false);
		Vector3 normalized = (dragTarget.transform.position - dragButton.transform.position).normalized;
		arrowTransform.localRotation = Quaternion.LookRotation(Vector3.forward, normalized);
		GGUtil.SetActive(arrowTransform, active: true);
		trailRenderer.enabled = false;
		scaleParent.transform.localScale = settings.inAnimationZoomFrom;
		inAnimation = DoInAnimation(showTutorialHand);
		selectorAnimation = null;
	}

	private IEnumerator DoInAnimation(bool showTutorial)
	{
		return new _003CDoInAnimation_003Ed__24(0)
		{
			_003C_003E4__this = this,
			showTutorial = showTutorial
		};
	}

	private void ShowTutorialHandIfNeeded()
	{
		if (showTutorialHand)
		{
			scaleParent.transform.localScale = Vector3.one;
			TutorialHandController.InitArguments initArguments = default(TutorialHandController.InitArguments);
			Transform transform = screen.transform;
			initArguments.endLocalPosition = transform.InverseTransformPoint(dragTarget.transform.position);
			initArguments.startLocalPosition = transform.InverseTransformPoint(dragSourceRectTransform.position);
			initArguments.settings = Match3Settings.instance.tutorialHandSettings;
			initArguments.repeat = true;
			screen.tutorialHand.Show(initArguments);
		}
	}

	private IEnumerator DoSelectorAnimation()
	{
		return new _003CDoSelectorAnimation_003Ed__26(0)
		{
			_003C_003E4__this = this
		};
	}

	private void Update()
	{
		if (!(dragTarget == null))
		{
			dragTarget.transform.position = screen.TransformPSDToWorldPoint(visualItem.visualObjectBehaviour.iconHandlePosition);
			if (inAnimation != null && !inAnimation.MoveNext())
			{
				inAnimation = null;
			}
			if (selectorAnimation != null && !selectorAnimation.MoveNext())
			{
				selectorAnimation = null;
			}
		}
	}

	public void OnPurchaseConfirmed()
	{
		showTutorialHand = false;
		screen.tutorialHand.Hide();
		screen.ConfirmPurchasePanelCallback_OnConfirm(visualItem);
	}

	public void OnBackgroundClicked()
	{
		showTutorialHand = false;
		GGUtil.SetActive(this, active: false);
		screen.tutorialHand.Hide();
		screen.ConfirmPurchasePanelCallback_OnClosed();
		GGSoundSystem.Play(GGSoundSystem.SFXType.CancelPress);
	}

	public void OnDragStart()
	{
		screen.tutorialHand.Hide();
		GGUtil.SetActive(arrowTransform, active: false);
		trailRenderer.enabled = true;
	}

	public void OnDragEnd()
	{
		ShowTutorialHandIfNeeded();
		GGUtil.SetActive(arrowTransform, active: true);
		trailRenderer.enabled = false;
		GGUtil.SetActive(backgroundSelected, active: false);
	}

	public bool IsTargetIn()
	{
		Vector3 position = dragButton.transform.position;
		return IsTargetIn(position);
	}

	public bool IsTargetIn(Vector3 dragButtonWorldPosition)
	{
		DragTarget dragTarget = this.dragTarget;
		dragButtonWorldPosition.z = dragTarget.transform.position.z;
		float magnitude = dragTarget.transform.InverseTransformPoint(dragButtonWorldPosition).magnitude;
		float num = dragTarget.GetComponent<RectTransform>().sizeDelta.x * 0.5f + dragButton.GetComponent<RectTransform>().sizeDelta.x * scaleParent.localScale.x * 0.5f;
		return magnitude <= num;
	}

	public void OnButtonClick()
	{
		showTutorialHand = true;
		ShowTutorialHandIfNeeded();
	}

	public void OnDrag()
	{
		bool activeSelf = backgroundSelected.gameObject.activeSelf;
		bool flag = IsTargetIn();
		if (!flag)
		{
			selectorAnimation = null;
		}
		GGUtil.SetActive(backgroundSelected, flag);
		if (!activeSelf && flag)
		{
			selectorAnimation = DoSelectorAnimation();
		}
	}
}
