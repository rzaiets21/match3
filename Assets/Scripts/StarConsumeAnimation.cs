using GGMatch3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class StarConsumeAnimation : MonoBehaviour
{
	[Serializable]
	public class Settings
	{
		public float moveStartScale;

		public float moveEndScale;

		public float moveDuration;

		public float animationDelayDuration;

		public float moveEndAlpha;

		public AnimationCurve moveCurve;

		public float endScale;

		public float scaleDuration;

		public float whiteoutAlphaEnd = 1f;

		public AnimationCurve scaleCurve;
	}

	public struct InitParams
	{
		public DecorateRoomScreen screen;

		public DecorateRoomSceneVisualItem visualItem;

		public Action onEnd;
	}

	private sealed class _003CDoAnimation_003Ed__9 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public StarConsumeAnimation _003C_003E4__this;

		private float _003Ctime_003E5__2;

		private Settings _003Csettings_003E5__3;

		private Vector3 _003CstartLocalPos_003E5__4;

		private Vector3 _003CendLocalPos_003E5__5;

		private Vector3 _003CstartScale_003E5__6;

		private float _003CstartAlpha_003E5__7;

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
		public _003CDoAnimation_003Ed__9(int _003C_003E1__state)
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
			StarConsumeAnimation starConsumeAnimation = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
			{
				_003C_003E1__state = -1;
				_003Ctime_003E5__2 = 0f;
				_003Csettings_003E5__3 = starConsumeAnimation.settings;
				DecoratingScene scene = starConsumeAnimation.initParams.screen.scene;
				starConsumeAnimation.star.Init();
				starConsumeAnimation.initParams.visualItem.visualObjectBehaviour.activeVariation.ScaleAnimation(_003Csettings_003E5__3.animationDelayDuration, !starConsumeAnimation.initParams.visualItem.visualObjectBehaviour.visualObject.hasDefaultVariation);
				_003CstartLocalPos_003E5__4 = starConsumeAnimation.transform.InverseTransformPoint(starConsumeAnimation.originStarTransform.position);
				_003CendLocalPos_003E5__5 = starConsumeAnimation.transform.InverseTransformPoint(scene.PSDToWorldPoint(starConsumeAnimation.initParams.visualItem.visualObjectBehaviour.iconHandlePosition));
				_003CstartLocalPos_003E5__4.z = (_003CendLocalPos_003E5__5.z = 0f);
				goto IL_01ff;
			}
			case 1:
				_003C_003E1__state = -1;
				goto IL_01ff;
			case 2:
				{
					_003C_003E1__state = -1;
					break;
				}
				IL_01ff:
				if (_003Ctime_003E5__2 <= _003Csettings_003E5__3.moveDuration)
				{
					_003Ctime_003E5__2 += Time.deltaTime;
					float time = Mathf.InverseLerp(0f, _003Csettings_003E5__3.moveDuration, _003Ctime_003E5__2);
					float t = _003Csettings_003E5__3.moveCurve.Evaluate(time);
					Vector3 localScale = Vector3.LerpUnclamped(Vector3.one * _003Csettings_003E5__3.moveStartScale, Vector3.one * _003Csettings_003E5__3.moveEndScale, t);
					Vector3 localPosition = Vector3.LerpUnclamped(_003CstartLocalPos_003E5__4, _003CendLocalPos_003E5__5, t);
					float alpha = Mathf.Lerp(1f, _003Csettings_003E5__3.moveEndAlpha, t);
					GGUtil.SetAlpha(starConsumeAnimation.star.mainGroup, alpha);
					starConsumeAnimation.star.transform.localPosition = localPosition;
					starConsumeAnimation.star.transform.localScale = localScale;
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				starConsumeAnimation.star.transform.localPosition = _003CendLocalPos_003E5__5;
				starConsumeAnimation.star.transform.localScale = Vector3.one * _003Csettings_003E5__3.moveEndScale;
				GGUtil.SetAlpha(starConsumeAnimation.star.mainGroup, _003Csettings_003E5__3.moveEndAlpha);
				_003CstartScale_003E5__6 = starConsumeAnimation.star.transform.localScale;
				GGUtil.SetAlpha(starConsumeAnimation.star.whiteOutImage, 0f);
				GGUtil.SetActive(starConsumeAnimation.star.whiteOutImage, active: true);
				_003CstartAlpha_003E5__7 = starConsumeAnimation.star.mainGroup.alpha;
				_003Ctime_003E5__2 = 0f;
				break;
			}
			if (_003Ctime_003E5__2 <= _003Csettings_003E5__3.scaleDuration)
			{
				_003Ctime_003E5__2 += Time.deltaTime;
				float time2 = Mathf.InverseLerp(0f, _003Csettings_003E5__3.scaleDuration, _003Ctime_003E5__2);
				float t2 = _003Csettings_003E5__3.scaleCurve.Evaluate(time2);
				Vector3 localScale2 = Vector3.LerpUnclamped(_003CstartScale_003E5__6, _003Csettings_003E5__3.endScale * Vector3.one, t2);
				starConsumeAnimation.star.transform.localScale = localScale2;
				float alpha2 = Mathf.Lerp(_003CstartAlpha_003E5__7, 0f, t2);
				GGUtil.SetAlpha(starConsumeAnimation.star.mainGroup, alpha2);
				float alpha3 = Mathf.Lerp(0f, _003Csettings_003E5__3.whiteoutAlphaEnd, t2);
				GGUtil.SetAlpha(starConsumeAnimation.star.whiteOutImage, alpha3);
				_003C_003E2__current = null;
				_003C_003E1__state = 2;
				return true;
			}
			GGUtil.Hide(starConsumeAnimation);
			if (starConsumeAnimation.initParams.onEnd != null)
			{
				starConsumeAnimation.initParams.onEnd();
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
	private StarConsumeAnimationStar star;

	[SerializeField]
	private RectTransform originStarTransform;

	private InitParams initParams;

	private IEnumerator animationEnumerator;

	public Settings settings => Match3Settings.instance.starConsumeSettings;

	public void Show(InitParams initParams)
	{
		this.initParams = initParams;
		GGUtil.Show(this);
		animationEnumerator = DoAnimation();
		animationEnumerator.MoveNext();
	}

	private IEnumerator DoAnimation()
	{
		return new _003CDoAnimation_003Ed__9(0)
		{
			_003C_003E4__this = this
		};
	}

	private void Update()
	{
		if (animationEnumerator != null && !animationEnumerator.MoveNext())
		{
			animationEnumerator = null;
		}
	}
}
