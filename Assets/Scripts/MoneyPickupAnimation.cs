using GGMatch3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MoneyPickupAnimation : MonoBehaviour
{
	[Serializable]
	public class Settings
	{
		public float inAnimationDuration = 1f;

		public float totalDelayForInAnimationIndexes = 0.5f;

		public AnimationCurve inScaleCurve;

		public float startScale;

		public Vector3 randomRange;

		public float travelDuration = 0.75f;

		public float totalDelayForIndexes = 1f;

		public AnimationCurve travelCurve;

		public float travelEndScale;

		public AnimationCurve travelScaleCurve;

		public float bobDuration = 1f;

		public float bobScale = 1.1f;

		public AnimationCurve bobCurve;

		public int numberOfCoins = 10;

		public int numberOfStars = 10;
	}

	public struct ShowParams
	{
		public int numberOfCoins;

		public int numberOfStars;

		public RectTransform starDestinationTransform;

		public RectTransform coinDestinationTransform;

		public Action onComplete;
	}

	public struct ElementDefinition
	{
		public MoneyPickupAnimationMoney.Style style;

		public int count;
	}

	private sealed class _003CDoInAnimation_003Ed__16 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public MoneyPickupAnimation _003C_003E4__this;

		private Settings _003Csettings_003E5__2;

		private float _003Ctime_003E5__3;

		private float _003CdelayPerIndex_003E5__4;

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
		public _003CDoInAnimation_003Ed__16(int _003C_003E1__state)
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
			MoneyPickupAnimation moneyPickupAnimation = _003C_003E4__this;
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
				_003Csettings_003E5__2 = moneyPickupAnimation.settings;
				_003Ctime_003E5__3 = 0f;
				_003CdelayPerIndex_003E5__4 = _003Csettings_003E5__2.totalDelayForInAnimationIndexes / (float)moneyPickupAnimation.moneyElements.Count;
			}
			_003Ctime_003E5__3 += Time.deltaTime;
			bool flag = false;
			for (int i = 0; i < moneyPickupAnimation.moneyElements.Count; i++)
			{
				MoneyPickupAnimationMoney moneyPickupAnimationMoney = moneyPickupAnimation.moneyElements[i];
				GGUtil.SetActive(moneyPickupAnimationMoney, active: true);
				float num2 = _003Ctime_003E5__3 - (float)moneyPickupAnimationMoney.index * _003CdelayPerIndex_003E5__4;
				float num3 = Mathf.InverseLerp(0f, _003Csettings_003E5__2.inAnimationDuration, num2);
				if (_003Csettings_003E5__2.inScaleCurve != null)
				{
					num3 = _003Csettings_003E5__2.inScaleCurve.Evaluate(num3);
				}
				Vector3 localScale = Vector3.LerpUnclamped(new Vector3(0f, 0f, 1f), Vector3.one, num3);
				if (num2 < _003Csettings_003E5__2.inAnimationDuration)
				{
					flag = true;
				}
				if (num2 >= _003Csettings_003E5__2.inAnimationDuration)
				{
					num2 -= _003Csettings_003E5__2.inAnimationDuration;
					num3 = Mathf.PingPong(num2, _003Csettings_003E5__2.bobDuration);
					if (_003Csettings_003E5__2.bobCurve != null)
					{
						num3 = _003Csettings_003E5__2.bobCurve.Evaluate(num3);
					}
					localScale = Vector3.LerpUnclamped(Vector3.one, new Vector3(_003Csettings_003E5__2.bobScale, _003Csettings_003E5__2.bobScale, 1f), num3);
				}
				moneyPickupAnimationMoney.transform.localScale = localScale;
			}
			if (!flag)
			{
				moneyPickupAnimation.isInAnimationComplete = true;
			}
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

	private sealed class _003CDoTravelToAnimation_003Ed__17 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public MoneyPickupAnimation _003C_003E4__this;

		private Vector3 _003ClocalEndPositionStar_003E5__2;

		private Vector3 _003ClocalEndPositionCoin_003E5__3;

		private Settings _003Csettings_003E5__4;

		private float _003Ctime_003E5__5;

		private float _003CdelayPerIndex_003E5__6;

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
		public _003CDoTravelToAnimation_003Ed__17(int _003C_003E1__state)
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
			MoneyPickupAnimation moneyPickupAnimation = _003C_003E4__this;
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
				_003ClocalEndPositionStar_003E5__2 = moneyPickupAnimation.coinPool.parent.InverseTransformPoint(moneyPickupAnimation.showParams.starDestinationTransform.position);
				_003ClocalEndPositionCoin_003E5__3 = moneyPickupAnimation.coinPool.parent.InverseTransformPoint(moneyPickupAnimation.showParams.coinDestinationTransform.position);
				_003Csettings_003E5__4 = moneyPickupAnimation.settings;
				_003Ctime_003E5__5 = 0f;
				_003CdelayPerIndex_003E5__6 = _003Csettings_003E5__4.totalDelayForIndexes / (float)moneyPickupAnimation.moneyElements.Count;
				for (int i = 0; i < moneyPickupAnimation.moneyElements.Count; i++)
				{
					MoneyPickupAnimationMoney moneyPickupAnimationMoney = moneyPickupAnimation.moneyElements[i];
					moneyPickupAnimationMoney.startTravelScale = moneyPickupAnimationMoney.transform.localScale;
				}
			}
			_003Ctime_003E5__5 += Time.deltaTime;
			bool flag = false;
			for (int j = 0; j < moneyPickupAnimation.moneyElements.Count; j++)
			{
				MoneyPickupAnimationMoney moneyPickupAnimationMoney2 = moneyPickupAnimation.moneyElements[j];
				int num2 = moneyPickupAnimation.moneyElements.Count - 1 - moneyPickupAnimationMoney2.index;
				num2 = moneyPickupAnimationMoney2.index;
				float num3 = _003Ctime_003E5__5 - (float)num2 * _003CdelayPerIndex_003E5__6;
				float num4 = Mathf.InverseLerp(0f, _003Csettings_003E5__4.travelDuration, num3);
				Vector3 zero = Vector3.zero;
				zero = ((moneyPickupAnimationMoney2.style != 0) ? _003ClocalEndPositionStar_003E5__2 : _003ClocalEndPositionCoin_003E5__3);
				float num5 = num4;
				if (_003Csettings_003E5__4.travelCurve != null)
				{
					num5 = _003Csettings_003E5__4.travelCurve.Evaluate(num5);
				}
				Vector3 localPosition = Vector3.LerpUnclamped(moneyPickupAnimationMoney2.startLocalPosition, zero, num5);
				if (num3 < _003Csettings_003E5__4.travelDuration)
				{
					flag = true;
				}
				float num6 = num4;
				if (_003Csettings_003E5__4.travelScaleCurve != null)
				{
					num6 = _003Csettings_003E5__4.travelScaleCurve.Evaluate(num6);
				}
				Vector3 localScale = Vector3.LerpUnclamped(moneyPickupAnimationMoney2.startTravelScale, new Vector3(_003Csettings_003E5__4.travelEndScale, _003Csettings_003E5__4.travelEndScale, 0f), num6);
				moneyPickupAnimationMoney2.transform.localPosition = localPosition;
				moneyPickupAnimationMoney2.transform.localScale = localScale;
			}
			if (flag)
			{
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			}
			GGUtil.SetActive(moneyPickupAnimation, active: false);
			if (moneyPickupAnimation.showParams.onComplete != null)
			{
				moneyPickupAnimation.showParams.onComplete();
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
	private ComponentPool coinPool = new ComponentPool();

	[SerializeField]
	private RectTransform coinSpawnOrigin;

	[SerializeField]
	private float elementWith = 200f;

	private ShowParams showParams;

	private List<MoneyPickupAnimationMoney> moneyElements = new List<MoneyPickupAnimationMoney>();

	private bool isInAnimationComplete;

	private bool isInTranslateAnimation;

	private IEnumerator animationEnumerator;

	private List<ElementDefinition> elements = new List<ElementDefinition>();

	public Settings settings => Match3Settings.instance.moneyPickupAnimationSettings;

	public void Show(ShowParams showParams)
	{
		this.showParams = showParams;
		isInAnimationComplete = false;
		isInTranslateAnimation = false;
		coinPool.Clear();
		moneyElements.Clear();
		elements.Clear();
		if (showParams.numberOfStars > 0)
		{
			ElementDefinition item = default(ElementDefinition);
			item.style = MoneyPickupAnimationMoney.Style.Star;
			item.count = showParams.numberOfStars;
			elements.Add(item);
		}
		if (showParams.numberOfCoins > 0)
		{
			ElementDefinition item2 = default(ElementDefinition);
			item2.style = MoneyPickupAnimationMoney.Style.Coin;
			item2.count = showParams.numberOfCoins;
			elements.Add(item2);
		}
		float num = elementWith * (float)elements.Count;
		for (int i = 0; i < elements.Count; i++)
		{
			ElementDefinition elementDefinition = elements[i];
			MoneyPickupAnimationMoney moneyPickupAnimationMoney = coinPool.Next<MoneyPickupAnimationMoney>();
			GGUtil.SetActive(moneyPickupAnimationMoney, active: true);
			Vector3 startLocalPosition = new Vector3((0f - num) * 0.5f + ((float)i + 0.5f) * elementWith, 0f, 0f);
			moneyPickupAnimationMoney.Init(i, startLocalPosition, elementDefinition.style, elementDefinition.count);
			moneyElements.Add(moneyPickupAnimationMoney);
			GGUtil.SetActive(moneyPickupAnimationMoney, active: false);
		}
		coinPool.HideNotUsed();
		GGUtil.SetActive(this, active: true);
		animationEnumerator = DoInAnimation();
	}

	public void TravelToAnimation()
	{
		animationEnumerator = DoTravelToAnimation();
	}

	private IEnumerator DoInAnimation()
	{
		return new _003CDoInAnimation_003Ed__16(0)
		{
			_003C_003E4__this = this
		};
	}

	private IEnumerator DoTravelToAnimation()
	{
		return new _003CDoTravelToAnimation_003Ed__17(0)
		{
			_003C_003E4__this = this
		};
	}

	public void Callback_OnClick()
	{
		if (isInAnimationComplete && !isInTranslateAnimation)
		{
			isInTranslateAnimation = true;
			TravelToAnimation();
		}
	}

	private void Update()
	{
		if (animationEnumerator != null && !animationEnumerator.MoveNext())
		{
			animationEnumerator = null;
		}
	}
}
