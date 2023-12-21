using GGMatch3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class WinScreenCoins : MonoBehaviour
{
	private sealed class _003CDoMoveCoins_003Ed__5 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public int count;

		public WinScreenCoins _003C_003E4__this;

		public long startCoins;

		public RectTransform destination;

		public long endCoins;

		private CurrencyDisplay _003CcurrencyDisplay_003E5__2;

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
		public _003CDoMoveCoins_003Ed__5(int _003C_003E1__state)
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
			WinScreenCoins winScreenCoins = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
			{
				_003C_003E1__state = -1;
				if (count == 0)
				{
					return false;
				}
				CurrencyPanel currencyPanel = winScreenCoins.winScreen.currencyPanel;
				_003CcurrencyDisplay_003E5__2 = currencyPanel.DisplayForCurrency(CurrencyType.coins);
				_003CcurrencyDisplay_003E5__2.DisplayCount(startCoins);
				Vector3 destinationLocalPosition = winScreenCoins.coinsPool.parent.InverseTransformPoint(destination.position);
				destinationLocalPosition.z = 0f;
				winScreenCoins.coinsPool.Clear();
				for (int i = 0; i < count; i++)
				{
					GGUtil.Hide(winScreenCoins.coinsPool.Next());
				}
				winScreenCoins.enumList.Clear();
				WinScreen.Settings settings = winScreenCoins.settings;
				List<GameObject> usedObjects = winScreenCoins.coinsPool.usedObjects;
				float num2 = 0f;
				float num3 = Mathf.Max(0f, settings.starTravelDuration - settings.coinTravelDuration) / (float)count;
				for (int num4 = usedObjects.Count - 1; num4 >= 0; num4--)
				{
					GameObject coinGameObject = usedObjects[num4];
					float num5 = Mathf.InverseLerp(usedObjects.Count - 1, 0f, num4);
					long coinCount = (long)((float)(endCoins - startCoins) * num5 + (float)startCoins);
					winScreenCoins.enumList.Add(winScreenCoins.DoMoveCoin(coinGameObject, destinationLocalPosition, num2, coinCount));
					num2 += num3;
				}
				break;
			}
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			if (winScreenCoins.enumList.Update())
			{
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			}
			_003CcurrencyDisplay_003E5__2.DisplayCount(endCoins);
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

	private sealed class _003CDoMoveCoin_003Ed__9 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public WinScreenCoins _003C_003E4__this;

		public float delay;

		public GameObject coinGameObject;

		public Vector3 destinationLocalPosition;

		public long coinCount;

		private CurrencyDisplay _003CcurrencyDisplay_003E5__2;

		private float _003Ctime_003E5__3;

		private Transform _003CcoinTransform_003E5__4;

		private Vector3 _003CstartLocalPosition_003E5__5;

		private Vector3 _003CendLocalPosition_003E5__6;

		private int _003CstartScale_003E5__7;

		private float _003CendScale_003E5__8;

		private float _003Cduration_003E5__9;

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
		public _003CDoMoveCoin_003Ed__9(int _003C_003E1__state)
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
			WinScreenCoins winScreenCoins = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
			{
				_003C_003E1__state = -1;
				CurrencyPanel currencyPanel = winScreenCoins.winScreen.currencyPanel;
				_003CcurrencyDisplay_003E5__2 = currencyPanel.DisplayForCurrency(CurrencyType.coins);
				_003Ctime_003E5__3 = 0f;
				goto IL_0078;
			}
			case 1:
				_003C_003E1__state = -1;
				goto IL_0078;
			case 2:
				{
					_003C_003E1__state = -1;
					break;
				}
				IL_0078:
				if (_003Ctime_003E5__3 < delay)
				{
					_003Ctime_003E5__3 += Time.deltaTime;
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				_003CcoinTransform_003E5__4 = coinGameObject.transform;
				_003CcoinTransform_003E5__4.localPosition = Vector3.zero;
				_003CcoinTransform_003E5__4.localScale = Vector3.one;
				GGUtil.Show(coinGameObject);
				_003CstartLocalPosition_003E5__5 = Vector3.zero;
				_003CendLocalPosition_003E5__6 = destinationLocalPosition;
				_003CstartScale_003E5__7 = 1;
				_003CendScale_003E5__8 = winScreenCoins.settings.coinEndScale;
				_003Cduration_003E5__9 = winScreenCoins.settings.coinTravelDuration;
				_003Ctime_003E5__3 = 0f;
				break;
			}
			if (_003Ctime_003E5__3 <= _003Cduration_003E5__9)
			{
				_003Ctime_003E5__3 += Time.deltaTime;
				float t = Mathf.InverseLerp(0f, _003Cduration_003E5__9, _003Ctime_003E5__3);
				float d = Mathf.Lerp(_003CstartScale_003E5__7, _003CendScale_003E5__8, t);
				Vector3 localPosition = Vector3.Lerp(_003CstartLocalPosition_003E5__5, _003CendLocalPosition_003E5__6, t);
				_003CcoinTransform_003E5__4.localPosition = localPosition;
				_003CcoinTransform_003E5__4.localScale = Vector3.one * d;
				_003C_003E2__current = null;
				_003C_003E1__state = 2;
				return true;
			}
			GGSoundSystem.Play(GGSoundSystem.SFXType.RecieveCoin);
			_003CcurrencyDisplay_003E5__2.ShowShineParticle();
			_003CcurrencyDisplay_003E5__2.DisplayCount(coinCount);
			GGUtil.Hide(coinGameObject);
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
	private ComponentPool coinsPool = new ComponentPool();

	[SerializeField]
	private RectTransform coinImage;

	[SerializeField]
	private TextMeshProUGUI coinsLabel;

	private WinScreen winScreen;

	private EnumeratorsList enumList = new EnumeratorsList();

	private WinScreen.Settings settings => Match3Settings.instance.winScreenSettings;

	public void Init(long wonCoins, WinScreen winScreen)
	{
		this.winScreen = winScreen;
		GGUtil.SetActive(this, wonCoins > 0);
		coinsPool.Clear();
		coinsPool.HideNotUsed();
		GGUtil.ChangeText(coinsLabel, wonCoins);
		GGUtil.Show(coinImage);
	}

	public IEnumerator DoMoveCoins(int count, RectTransform destination, long startCoins, long endCoins)
	{
		return new _003CDoMoveCoins_003Ed__5(0)
		{
			_003C_003E4__this = this,
			count = count,
			destination = destination,
			startCoins = startCoins,
			endCoins = endCoins
		};
	}

	private IEnumerator DoMoveCoin(GameObject coinGameObject, Vector3 destinationLocalPosition, float delay, long coinCount)
	{
		return new _003CDoMoveCoin_003Ed__9(0)
		{
			_003C_003E4__this = this,
			coinGameObject = coinGameObject,
			destinationLocalPosition = destinationLocalPosition,
			delay = delay,
			coinCount = coinCount
		};
	}
}
