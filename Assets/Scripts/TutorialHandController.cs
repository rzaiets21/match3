using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class TutorialHandController : MonoBehaviour
{
	[Serializable]
	public class Settings
	{
		public float scaleHandFrom = 2f;

		public float scaleHandTo = 1f;

		public float alphaHandFrom;

		public float alphaHandTo = 1f;

		public float scaleInDuration = 0.5f;

		public AnimationCurve scaleInCurve;

		public float delayAfterScale;

		public float moveDuration = 2f;

		public AnimationCurve moveCurve;

		public float waitOnDestination;

		public float fromAlphaMove = 1f;

		public float toAlphaMove = 1f;
	}

	public struct InitArguments
	{
		public bool isInit;
		
		public Vector3 startLocalPosition;

		public Vector3 endLocalPosition;

		public bool repeat;

		public Settings settings;
	}

	private sealed class _003CDoAnimation_003Ed__12 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public TutorialHandController _003C_003E4__this;

		public InitArguments initArguments;

		private IEnumerator _003CanimEnum_003E5__2;

		private float _003Ctime_003E5__3;

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
		public _003CDoAnimation_003Ed__12(int _003C_003E1__state)
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
			TutorialHandController tutorialHandController = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				goto IL_0031;
			case 1:
				_003C_003E1__state = -1;
				goto IL_00a6;
			case 2:
				_003C_003E1__state = -1;
				goto IL_00e9;
			case 3:
				_003C_003E1__state = -1;
				goto IL_0154;
			case 4:
				{
					_003C_003E1__state = -1;
					goto IL_0197;
				}
				IL_0031:
				GGUtil.Hide(tutorialHandController.widgetsToHide);
				tutorialHandController.handContainer.localPosition = initArguments.startLocalPosition;
				_003CanimEnum_003E5__2 = null;
				_003Ctime_003E5__3 = 0f;
				GGUtil.Show(tutorialHandController.handContainer);
				if (tutorialHandController.settngs.scaleInDuration > 0f)
				{
					_003CanimEnum_003E5__2 = tutorialHandController.ScaleIn();
					goto IL_00a6;
				}
				goto IL_00fc;
				IL_0197:
				if (_003Ctime_003E5__3 < tutorialHandController.settngs.waitOnDestination)
				{
					_003Ctime_003E5__3 += Time.deltaTime;
					_003C_003E2__current = null;
					_003C_003E1__state = 4;
					return true;
				}
				if (!initArguments.repeat)
				{
					tutorialHandController.Hide();
					return false;
				}
				_003CanimEnum_003E5__2 = null;
				goto IL_0031;
				IL_00a6:
				if (_003CanimEnum_003E5__2.MoveNext())
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				_003Ctime_003E5__3 = 0f;
				goto IL_00e9;
				IL_0154:
				if (_003CanimEnum_003E5__2.MoveNext())
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 3;
					return true;
				}
				goto IL_0161;
				IL_00e9:
				if (_003Ctime_003E5__3 < tutorialHandController.settngs.delayAfterScale)
				{
					_003Ctime_003E5__3 += Time.deltaTime;
					_003C_003E2__current = null;
					_003C_003E1__state = 2;
					return true;
				}
				goto IL_00fc;
				IL_0161:
				_003Ctime_003E5__3 = 0f;
				goto IL_0197;
				IL_00fc:
				tutorialHandController.trail.Clear();
				GGUtil.Show(tutorialHandController.trail);
				if (initArguments.startLocalPosition != initArguments.endLocalPosition)
				{
					_003CanimEnum_003E5__2 = tutorialHandController.DoMove();
					goto IL_0154;
				}
				goto IL_0161;
			}
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

	private sealed class _003CDoMove_003Ed__13 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public TutorialHandController _003C_003E4__this;

		private Vector3 _003CfromPosition_003E5__2;

		private Vector3 _003CtoPosition_003E5__3;

		private float _003Cduration_003E5__4;

		private float _003Ctime_003E5__5;

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
		public _003CDoMove_003Ed__13(int _003C_003E1__state)
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
			TutorialHandController tutorialHandController = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
			{
				_003C_003E1__state = -1;
				Settings settng = tutorialHandController.settngs;
				_003CfromPosition_003E5__2 = tutorialHandController.initArguments.startLocalPosition;
				_003CtoPosition_003E5__3 = tutorialHandController.initArguments.endLocalPosition;
				Vector3 localPosition = Vector3.one * tutorialHandController.settngs.scaleHandTo;
				tutorialHandController.handContainer.localPosition = localPosition;
				_003Cduration_003E5__4 = tutorialHandController.settngs.moveDuration;
				_003Ctime_003E5__5 = 0f;
				break;
			}
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			if (_003Ctime_003E5__5 <= _003Cduration_003E5__4)
			{
				_003Ctime_003E5__5 += Time.deltaTime;
				float num2 = Mathf.InverseLerp(0f, _003Cduration_003E5__4, _003Ctime_003E5__5);
				float t = tutorialHandController.settngs.moveCurve.Evaluate(num2);
				Vector3 localPosition2 = Vector3.LerpUnclamped(_003CfromPosition_003E5__2, _003CtoPosition_003E5__3, t);
				float alpha = Mathf.Lerp(tutorialHandController.settngs.fromAlphaMove, tutorialHandController.settngs.toAlphaMove, num2);
				GGUtil.SetAlpha(tutorialHandController.handAlpha, alpha);
				tutorialHandController.handContainer.localPosition = localPosition2;
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

	private sealed class _003CScaleIn_003Ed__14 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public TutorialHandController _003C_003E4__this;

		private Vector3 _003CfromScale_003E5__2;

		private Vector3 _003CtoScale_003E5__3;

		private float _003Ctime_003E5__4;

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
		public _003CScaleIn_003Ed__14(int _003C_003E1__state)
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
			TutorialHandController tutorialHandController = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
			{
				_003C_003E1__state = -1;
				Settings settng = tutorialHandController.settngs;
				_003CfromScale_003E5__2 = Vector3.one * tutorialHandController.settngs.scaleHandFrom;
				_003CtoScale_003E5__3 = Vector3.one * tutorialHandController.settngs.scaleHandTo;
				_003Ctime_003E5__4 = 0f;
				break;
			}
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			if (_003Ctime_003E5__4 <= tutorialHandController.settngs.scaleInDuration)
			{
				_003Ctime_003E5__4 += Time.deltaTime;
				float num2 = Mathf.InverseLerp(0f, tutorialHandController.settngs.scaleInDuration, _003Ctime_003E5__4);
				float t = tutorialHandController.settngs.scaleInCurve.Evaluate(num2);
				Vector3 localScale = Vector3.LerpUnclamped(_003CfromScale_003E5__2, _003CtoScale_003E5__3, t);
				float alpha = Mathf.Lerp(tutorialHandController.settngs.alphaHandFrom, tutorialHandController.settngs.alphaHandTo, num2);
				GGUtil.SetAlpha(tutorialHandController.handAlpha, alpha);
				tutorialHandController.handContainer.localScale = localScale;
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
	private List<Transform> widgetsToHide = new List<Transform>();

	[SerializeField]
	private Transform handContainer;

	[SerializeField]
	private TrailRenderer trail;

	[SerializeField]
	private CanvasGroup handAlpha;

	private InitArguments initArguments;

	private IEnumerator animation;

	public bool Shown { get; private set; }
	
	private Settings settngs => initArguments.settings;

	public void Show(InitArguments initArguments)
	{
		base.gameObject.SetActive(value: true);
		this.initArguments = initArguments;
		animation = DoAnimation(initArguments);
		animation.MoveNext();
		Shown = true;
	}

	public void Hide()
	{
		animation = null;
		Shown = false;
		base.gameObject.SetActive(value: false);
	}

	private IEnumerator DoAnimation(InitArguments initArguments)
	{
		return new _003CDoAnimation_003Ed__12(0)
		{
			_003C_003E4__this = this,
			initArguments = initArguments
		};
	}

	private IEnumerator DoMove()
	{
		return new _003CDoMove_003Ed__13(0)
		{
			_003C_003E4__this = this
		};
	}

	private IEnumerator ScaleIn()
	{
		return new _003CScaleIn_003Ed__14(0)
		{
			_003C_003E4__this = this
		};
	}

	private void Update()
	{
		if (animation != null)
		{
			animation.MoveNext();
		}
	}
}
