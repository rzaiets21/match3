using GGMatch3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour
{
	private sealed class _003C_003Ec__DisplayClass6_0
	{
		public NavigationManager nav;

		public bool termsOfServiceDone;

		public Action<bool> _003C_003E9__0;

		internal void _003CDoLoadFirstScene_003Eb__0(bool success)
		{
			if (!success)
			{
				Application.Quit();
				return;
			}
			GGPlayerSettings.instance.Model.acceptedTermsOfService = true;
			GGPlayerSettings.instance.Save();
			nav.Pop();
			termsOfServiceDone = true;
		}
	}

	private sealed class _003CDoLoadFirstScene_003Ed__6 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public SplashScreen _003C_003E4__this;

		private _003C_003Ec__DisplayClass6_0 _003C_003E8__1;

		private AsyncOperation _003CasyncOperation_003E5__2;

		private bool _003CneedsToShowTermsOfService_003E5__3;

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
		public _003CDoLoadFirstScene_003Ed__6(int _003C_003E1__state)
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
			SplashScreen splashScreen = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003C_003E8__1 = new _003C_003Ec__DisplayClass6_0();
				GGUtil.SetFill(splashScreen.progressBarSprite, 0f);
				_003C_003E8__1.nav = NavigationManager.instance;
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				_003CasyncOperation_003E5__2 = SceneManager.LoadSceneAsync(splashScreen.sceneName);
				_003CneedsToShowTermsOfService_003E5__3 = (splashScreen.showTermsOfServiceDialog && !GGPlayerSettings.instance.Model.acceptedTermsOfService);
				_003CasyncOperation_003E5__2.allowSceneActivation = !_003CneedsToShowTermsOfService_003E5__3;
				_003C_003E8__1.termsOfServiceDone = false;
				goto IL_0194;
			case 2:
				_003C_003E1__state = -1;
				goto IL_0164;
			case 3:
				{
					_003C_003E1__state = -1;
					goto IL_0194;
				}
				IL_0171:
				_003CasyncOperation_003E5__2.allowSceneActivation = true;
				goto IL_017d;
				IL_0164:
				if (!_003C_003E8__1.termsOfServiceDone)
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 2;
					return true;
				}
				goto IL_0171;
				IL_0194:
				if (!_003CasyncOperation_003E5__2.isDone)
				{
					GGUtil.SetFill(splashScreen.progressBarSprite, _003CasyncOperation_003E5__2.progress);
					if (!_003CasyncOperation_003E5__2.allowSceneActivation && _003CasyncOperation_003E5__2.progress >= 0.9f)
					{
						if (_003CneedsToShowTermsOfService_003E5__3)
						{
							_003C_003E8__1.nav.GetObject<TermsOfServiceDialog>().Show(_003C_003E8__1._003CDoLoadFirstScene_003Eb__0);
							goto IL_0164;
						}
						goto IL_0171;
					}
					goto IL_017d;
				}
				return false;
				IL_017d:
				_003C_003E2__current = null;
				_003C_003E1__state = 3;
				return true;
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

	[SerializeField]
	private string sceneName = "MainUI";

	[SerializeField]
	private bool showTermsOfServiceDialog;

	[SerializeField]
	private Image progressBarSprite;

	private IEnumerator animationEnumerator;

	private void OnEnable()
	{
		Init();
	}

	private void Init()
	{
		animationEnumerator = DoLoadFirstScene();
		animationEnumerator.MoveNext();
	}

	private IEnumerator DoLoadFirstScene()
	{
		return new _003CDoLoadFirstScene_003Ed__6(0)
		{
			_003C_003E4__this = this
		};
	}

	private void Update()
	{
		if (animationEnumerator != null)
		{
			animationEnumerator.MoveNext();
		}
	}
}
