using GGMatch3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class DecorateRoomSceneVisualItem : MonoBehaviour
{
	[Serializable]
	public class Settings
	{
		public float durationOfPop;

		public Vector3 startScale;

		public AnimationCurve popCurve;

		public float delayPerIndex = 0.05f;

		public float loopDuration;

		public Vector3 loopOffset;

		public Vector3 loopScaleOffset;

		public AnimationCurve loopCurve;
	}

	public class PointWithIndex
	{
		public Vector2 point;

		public int index;
	}

	private sealed class _003CDoAnimation_003Ed__13 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public DecorateRoomSceneVisualItem _003C_003E4__this;

		public float delay;

		private RectTransform _003CtransformToChange_003E5__2;

		private float _003Ctime_003E5__3;

		private float _003CmyDelay_003E5__4;

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
		public _003CDoAnimation_003Ed__13(int _003C_003E1__state)
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
			DecorateRoomSceneVisualItem decorateRoomSceneVisualItem = _003C_003E4__this;
			GraphicsSceneConfig.VisualObject visualObject;
			bool markersActive;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003CtransformToChange_003E5__2 = decorateRoomSceneVisualItem.animationTransform;
				decorateRoomSceneVisualItem.animationOffsetTransform.localPosition = Vector3.zero;
				_003CtransformToChange_003E5__2.localScale = decorateRoomSceneVisualItem.settings.startScale;
				_003Ctime_003E5__3 = 0f;
				if (delay > 0f)
				{
					goto IL_00a6;
				}
				goto IL_00c7;
			case 1:
				_003C_003E1__state = -1;
				goto IL_00a6;
			case 2:
				_003C_003E1__state = -1;
				goto IL_010b;
			case 3:
				_003C_003E1__state = -1;
				goto IL_01f3;
			case 4:
				{
					_003C_003E1__state = -1;
					break;
				}
				IL_00a6:
				if (_003Ctime_003E5__3 <= delay)
				{
					_003Ctime_003E5__3 += Time.deltaTime;
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				_003Ctime_003E5__3 -= delay;
				goto IL_00c7;
				IL_01f3:
				if (_003Ctime_003E5__3 <= decorateRoomSceneVisualItem.settings.durationOfPop)
				{
					_003Ctime_003E5__3 += Time.deltaTime;
					float num2 = Mathf.InverseLerp(0f, decorateRoomSceneVisualItem.settings.durationOfPop, _003Ctime_003E5__3);
					if (decorateRoomSceneVisualItem.settings.popCurve != null)
					{
						num2 = decorateRoomSceneVisualItem.settings.popCurve.Evaluate(num2);
					}
					Vector3 localScale = Vector3.LerpUnclamped(decorateRoomSceneVisualItem.settings.startScale, Vector3.one, num2);
					_003CtransformToChange_003E5__2.localScale = localScale;
					_003C_003E2__current = null;
					_003C_003E1__state = 3;
					return true;
				}
				_003Ctime_003E5__3 = 0f;
				break;
				IL_00c7:
				_003CmyDelay_003E5__4 = decorateRoomSceneVisualItem.settings.delayPerIndex * (float)decorateRoomSceneVisualItem.index;
				goto IL_010b;
				IL_010b:
				if (_003Ctime_003E5__3 <= _003CmyDelay_003E5__4)
				{
					_003Ctime_003E5__3 += Time.deltaTime;
					_003C_003E2__current = null;
					_003C_003E1__state = 2;
					return true;
				}
				_003Ctime_003E5__3 -= _003CmyDelay_003E5__4;
				visualObject = decorateRoomSceneVisualItem.visualObjectBehaviour.visualObject;
				markersActive = (!visualObject.isOwned && visualObject.IsUnlocked(decorateRoomSceneVisualItem.screen.scene));
				decorateRoomSceneVisualItem.visualObjectBehaviour.SetMarkersActive(markersActive);
				goto IL_01f3;
			}
			_003Ctime_003E5__3 += Time.deltaTime;
			float time = Mathf.PingPong(_003Ctime_003E5__3, decorateRoomSceneVisualItem.settings.loopDuration);
			time = decorateRoomSceneVisualItem.settings.loopCurve.Evaluate(time);
			Vector3 localPosition = Vector3.LerpUnclamped(Vector3.zero, decorateRoomSceneVisualItem.settings.loopOffset, time);
			Vector3 localScale2 = Vector3.LerpUnclamped(Vector3.one, decorateRoomSceneVisualItem.settings.loopScaleOffset, time);
			decorateRoomSceneVisualItem.animationTransform.localScale = localScale2;
			decorateRoomSceneVisualItem.animationOffsetTransform.transform.localPosition = localPosition;
			_003C_003E2__current = null;
			_003C_003E1__state = 4;
			return true;
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
	private ComponentPool markersPool = new ComponentPool();

	[SerializeField]
	private RectTransform buyButtonContanier;

	[SerializeField]
	private RectTransform animationTransform;

	[SerializeField]
	private RectTransform animationOffsetTransform;

	[SerializeField]
	private List<RectTransform> widgetsToHide = new List<RectTransform>();

	[NonSerialized]
	public VisualObjectBehaviour visualObjectBehaviour;

	[NonSerialized]
	private DecorateRoomScreen screen;

	private IEnumerator animationCoroutine;

	private int index;

	private List<Vector2> pointsCachedList = new List<Vector2>();

	public Settings settings => Match3Settings.instance.visualItemAnimationSettings;

	public void Init(VisualObjectBehaviour visualObjectBehaviour, DecorateRoomScreen screen, int index, float delay)
	{
		this.index = index;
		visualObjectBehaviour.InitMarkers(markersPool.prefab);
		this.visualObjectBehaviour = visualObjectBehaviour;
		this.screen = screen;
		GraphicsSceneConfig.VisualObject visualObject = visualObjectBehaviour.visualObject;
		GGUtil.SetActive(widgetsToHide, active: false);
		bool active = visualObject.IsUnlocked(screen.scene) && !visualObject.isOwned;
		GGUtil.SetActive(buyButtonContanier, active);
		SetPositionOfBuyButton();
		visualObjectBehaviour.SetVisualState();
		visualObjectBehaviour.SetMarkersActive(active: false);
		animationCoroutine = DoAnimation(delay);
		animationCoroutine.MoveNext();
	}

	private IEnumerator DoAnimation(float delay)
	{
		return new _003CDoAnimation_003Ed__13(0)
		{
			_003C_003E4__this = this,
			delay = delay
		};
	}

	private void SetPositionOfBuyButton()
	{
		Vector3 localPosition = base.transform.InverseTransformPoint(screen.TransformPSDToWorldPoint(visualObjectBehaviour.iconHandlePosition));
		localPosition.z = 0f;
		buyButtonContanier.localPosition = localPosition;
		buyButtonContanier.localScale = visualObjectBehaviour.iconHandleScale;
		buyButtonContanier.localRotation = visualObjectBehaviour.iconHandleRotation;
	}

	private void Update()
	{
		if (!(visualObjectBehaviour == null))
		{
			SetPositionOfBuyButton();
			if (animationCoroutine != null && !animationCoroutine.MoveNext())
			{
				animationCoroutine = null;
			}
		}
	}

	public void HideButton()
	{
		GGUtil.SetActive(buyButtonContanier, active: false);
	}

	public void ShowMarkers()
	{
		visualObjectBehaviour.SetMarkersActive(active: true);
	}

	public int Sort_MinX(PointWithIndex a, PointWithIndex b)
	{
		return a.point.x.CompareTo(b.point.x);
	}

	public int Sort_MinY(PointWithIndex a, PointWithIndex b)
	{
		return a.point.y.CompareTo(b.point.y);
	}

	public int Sort_Index(PointWithIndex a, PointWithIndex b)
	{
		return a.index.CompareTo(b.index);
	}

	public void ButtonCallback_OnBuyButton()
	{
		screen.VisualItemCallback_OnBuyItemPressed(this);
	}
}
