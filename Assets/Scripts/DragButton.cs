using GGMatch3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
{
	[Serializable]
	public class Settings
	{
		public float bobDuration;

		public float bobDisplace;

		public AnimationCurve animationCurve;

		public float pressScale = 1.3f;
	}

	private sealed class _003CDoAnimate_003Ed__16 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public DragButton _003C_003E4__this;

		private float _003Ctime_003E5__2;

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
		public _003CDoAnimate_003Ed__16(int _003C_003E1__state)
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
			DragButton dragButton = _003C_003E4__this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				_003C_003E1__state = -1;
			}
			else
			{
				_003C_003E1__state = -1;
				_003Ctime_003E5__2 = 0f;
			}
			_003Ctime_003E5__2 += Time.deltaTime;
			float time = Mathf.PingPong(_003Ctime_003E5__2, dragButton.settings.bobDuration);
			time = dragButton.settings.animationCurve.Evaluate(time);
			Vector3 normalized = dragButton.transform.InverseTransformPoint(dragButton.panel.dragTarget.transform.position).normalized;
			float d = Mathf.LerpUnclamped(0f, dragButton.settings.bobDisplace, time);
			dragButton.offsetTransform.localPosition = normalized * d;
			dragButton.arrowOffsetTransform.localPosition = Vector3.up * d;
			_003C_003E2__current = null;
			_003C_003E1__state = 1;
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

	private Camera uiCamera_;

	[SerializeField]
	private TrailRenderer trailRenderer;

	[SerializeField]
	private RectTransform offsetTransform;

	[SerializeField]
	private RectTransform arrowOffsetTransform;

	private ConfirmPurchasePanel panel;

	private bool dragging;

	private IEnumerator animationEnum;

	private bool isDrag;

	private Camera uiCamera
	{
		get
		{
			if (uiCamera_ == null)
			{
				uiCamera_ = NavigationManager.instance.GetCamera();
			}
			return uiCamera_;
		}
	}

	public Settings settings => Match3Settings.instance.dragButtonSettings;

	public void Init(ConfirmPurchasePanel panel, Sprite sprite)
	{
		base.transform.localPosition = Vector3.zero;
		this.panel = panel;
		trailRenderer.sortingLayerName = "UI";
		trailRenderer.Clear();
		dragging = false;
		ResetOffsetTransform();
		base.transform.localScale = Vector3.one;
	}

	private void ResetOffsetTransform()
	{
		offsetTransform.localPosition = Vector3.zero;
		arrowOffsetTransform.localPosition = Vector3.zero;
		animationEnum = DoAnimate();
	}

	public void StopAnimation()
	{
		offsetTransform.localPosition = Vector3.zero;
		arrowOffsetTransform.localPosition = Vector3.zero;
		animationEnum = null;
	}

	private IEnumerator DoAnimate()
	{
		return new _003CDoAnimate_003Ed__16(0)
		{
			_003C_003E4__this = this
		};
	}

	public void OnDragStart(BaseEventData data)
	{
		panel.OnDragStart();
		trailRenderer.Clear();
		dragging = true;
		base.transform.localScale = Vector3.one * settings.pressScale;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		isDrag = false;
		base.transform.localScale = Vector3.one * settings.pressScale;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		base.transform.localScale = Vector3.one;
		if (!isDrag)
		{
			panel.OnButtonClick();
		}
	}

	public void OnDrag(BaseEventData data)
	{
		isDrag = true;
		PointerEventData pointerEventData = data as PointerEventData;
		if (!(pointerEventData.pointerDrag != base.gameObject))
		{
			base.transform.position = uiCamera.ScreenToWorldPoint(pointerEventData.position);
			panel.OnDrag();
		}
	}

	public void OnDragEnd(BaseEventData data)
	{
		if (!((data as PointerEventData).pointerDrag != base.gameObject))
		{
			base.transform.localPosition = Vector3.zero;
			panel.OnDragEnd();
			dragging = false;
			ResetOffsetTransform();
			base.transform.localScale = Vector3.one;
		}
	}

	private void Update()
	{
		if (!dragging && animationEnum != null)
		{
			animationEnum.MoveNext();
		}
	}
}
