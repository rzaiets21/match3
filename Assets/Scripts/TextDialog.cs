using GGMatch3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextDialog : MonoBehaviour
{
	public struct MessageArguments
	{
		public string message;

		public bool showProgress;

		public float fromProgress;

		public float toProgress;
	}

	private sealed class _003CDoProgressBar_003Ed__13 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public TextDialog _003C_003E4__this;

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
		public _003CDoProgressBar_003Ed__13(int _003C_003E1__state)
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
			TextDialog textDialog = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				GGUtil.Show(textDialog.progressContainer);
				textDialog.progressBar.fillAmount = textDialog.messageArguments.fromProgress;
				GGUtil.ChangeText(textDialog.progressTextLabel, $"{GGFormat.FormatPercent(textDialog.messageArguments.fromProgress)}%");
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				_003Ctime_003E5__2 = 0f;
				goto IL_00c0;
			case 2:
				_003C_003E1__state = -1;
				goto IL_00c0;
			case 3:
				{
					_003C_003E1__state = -1;
					break;
				}
				IL_00c0:
				if (_003Ctime_003E5__2 < textDialog.introDuration)
				{
					_003Ctime_003E5__2 += Time.deltaTime;
					_003C_003E2__current = null;
					_003C_003E1__state = 2;
					return true;
				}
				_003Ctime_003E5__2 = 0f;
				break;
			}
			if (_003Ctime_003E5__2 < textDialog.progressBarDuration)
			{
				_003Ctime_003E5__2 += Time.deltaTime;
				float t = Mathf.InverseLerp(0f, textDialog.progressBarDuration, _003Ctime_003E5__2);
				float num2 = Mathf.Lerp(textDialog.messageArguments.fromProgress, textDialog.messageArguments.toProgress, t);
				GGUtil.ChangeText(textDialog.progressTextLabel, $"{GGFormat.FormatPercent(num2)}%");
				textDialog.progressBar.fillAmount = num2;
				_003C_003E2__current = null;
				_003C_003E1__state = 3;
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
	private TextMeshProUGUI textLabel;

	[SerializeField]
	private float introDuration = 0.5f;

	[SerializeField]
	private float time;

	[SerializeField]
	private Transform progressContainer;

	[SerializeField]
	private Image progressBar;

	[SerializeField]
	private float progressBarDuration = 1f;

	[SerializeField]
	private TextMeshProUGUI progressTextLabel;

	private Action onComplete;

	private IEnumerator animation;

	private MessageArguments messageArguments;


	public void ShowOkFirst(MessageArguments messageArguments, Action onComplete)
	{
		this.messageArguments = messageArguments;
		string message = messageArguments.message;
		GGUtil.ChangeText(textLabel, message);
		this.onComplete = onComplete;
		time = 0f;
		// NavigationManager.instance.Push(base.gameObject, isModal: true);
		animation = null;
		if (!messageArguments.showProgress)
		{
			GGUtil.Hide(progressContainer);
		}
		else
		{
			animation = DoProgressBar();
			animation.MoveNext();
		}
		// GGSoundSystem.Play(GGSoundSystem.SFXType.FlyIn);
	}
	
	public void ShowOk(MessageArguments messageArguments, Action onComplete)
	{
		this.messageArguments = messageArguments;
		string message = messageArguments.message;
		GGUtil.ChangeText(textLabel, message);
		this.onComplete = onComplete;
		time = 0f;
		NavigationManager.instance.Push(base.gameObject, isModal: true);
		animation = null;
		if (!messageArguments.showProgress)
		{
			GGUtil.Hide(progressContainer);
		}
		else
		{
			animation = DoProgressBar();
			animation.MoveNext();
		}
		GGSoundSystem.Play(GGSoundSystem.SFXType.FlyIn);
	}

	public void ButtonCallback_OnClick()
	{
		if (!(time <= introDuration))
		{
			GGSoundSystem.Play(GGSoundSystem.SFXType.ButtonPress);
			NavigationManager.instance.Pop();
			if (onComplete != null)
			{
				onComplete();
			}
		}
	}

	private IEnumerator DoProgressBar()
	{
		return new _003CDoProgressBar_003Ed__13(0)
		{
			_003C_003E4__this = this
		};
	}

	private void Update()
	{
		time += Time.deltaTime;
		if (animation != null)
		{
			animation.MoveNext();
		}
	}
}
