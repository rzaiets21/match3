using GGMatch3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Extensions;
using ITSoft;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class DecorateRoomScreen : UILayer, Match3GameListener
{
	public enum UIItemName
	{
		PlayButton,
		SettingsButton,
		CoinsBar,
		HeartsBar
	}

	[Serializable]
	public class UIItemSetup
	{
		public UIItemName name;

		public Transform widget;
	}

	[Serializable]
	public class Accelerometer
	{
		private Vector3 _003CcurrentPosition_003Ek__BackingField;

		public float lowPassFilter = 0.5f;

		public Vector3 currentPosition
		{
			get
			{
				return _003CcurrentPosition_003Ek__BackingField;
			}
			protected set
			{
				_003CcurrentPosition_003Ek__BackingField = value;
			}
		}

		public Vector3 ClampPosition(Vector3 position)
		{
			position.x = Mathf.Clamp(position.x, -1f, 1f);
			position.y = Mathf.Clamp(position.y, -1f, 1f);
			position.z = Mathf.Clamp(position.z, -1f, 1f);
			return position;
		}

		public void Init()
		{
			currentPosition = ClampPosition(Input.acceleration);
		}

		public void Update()
		{
			Vector3 b = ClampPosition(Input.acceleration);
			currentPosition = Vector3.Lerp(currentPosition, b, lowPassFilter * Time.deltaTime);
		}
	}

	public class Room
	{
		public string name;

		public RoomsDB.Room loadedRoom;

		public Scene scene;

		public DecoratingScene sceneBehaviour;

		public Room(string name, DecoratingScene sceneBehaviour)
		{
			this.name = name;
			this.sceneBehaviour = sceneBehaviour;
		}
	}

	private sealed class _003CDoLoadScene_003Ed__56 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public DecorateRoomScreen _003C_003E4__this;

		public RoomsDB.Room room;

		private InAppPurchaseConfirmScreen _003CinAppPurchaseConfirm_003E5__2;

		private RoomsDB.LoadRoomRequest _003CroomRequest_003E5__3;

		private IEnumerator _003CupdateEnum_003E5__4;

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
		public _003CDoLoadScene_003Ed__56(int _003C_003E1__state)
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
			DecorateRoomScreen decorateRoomScreen = _003C_003E4__this;
			DecoratingScene sceneBehaviour;
			switch (num)
			{
			default:
				return false;
			case 0:
			{
				_003C_003E1__state = -1;
				_003CinAppPurchaseConfirm_003E5__2 = NavigationManager.instance.GetObject<InAppPurchaseConfirmScreen>();
				if (_003CinAppPurchaseConfirm_003E5__2 != null)
				{
					_003CinAppPurchaseConfirm_003E5__2.SuspendShow();
				}
				InAppBackend instance2 = BehaviourSingletonInit<InAppBackend>.instance;
				GGUtil.SetActive(decorateRoomScreen.widgetsToHide, active: false);
				decorateRoomScreen.loadingSceneStyle.Apply();
				_003CroomRequest_003E5__3 = new RoomsDB.LoadRoomRequest(room);
				RoomsDB instance = ScriptableObjectSingleton<RoomsDB>.instance;
				_003CupdateEnum_003E5__4 = instance.LoadRoom(_003CroomRequest_003E5__3);
				GGUtil.SetFill(decorateRoomScreen.progressBarSprite, 0f);
				goto IL_00db;
			}
			case 1:
				_003C_003E1__state = -1;
				goto IL_00db;
			case 2:
				{
					_003C_003E1__state = -1;
					GGUtil.SetActive(decorateRoomScreen.widgetsToHide, active: false);
					decorateRoomScreen.roomLoadedStyle.Apply();
					if (_003CinAppPurchaseConfirm_003E5__2 != null)
					{
						_003CinAppPurchaseConfirm_003E5__2.ResumeShow();
					}
					decorateRoomScreen.InitScene(decorateRoomScreen.scene, isFirstTime: true);
					return false;
				}
				IL_00db:
				if (_003CupdateEnum_003E5__4.MoveNext())
				{
					float progress = _003CroomRequest_003E5__3.progress;
					GGUtil.SetFill(decorateRoomScreen.progressBarSprite, progress);
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				sceneBehaviour = room.sceneBehaviour;
				if (sceneBehaviour == null)
				{
					decorateRoomScreen.InitRetry();
					return false;
				}
				decorateRoomScreen.activeRoom = new Room(room.name, sceneBehaviour);
				decorateRoomScreen.activeRoom.loadedRoom = room;
				GGUtil.SetActive(sceneBehaviour, active: true);
				decorateRoomScreen.scene.Init(NavigationManager.instance.GetCamera(), decorateRoomScreen.marginsPsdSize, decorateRoomScreen);
				_003C_003E2__current = null;
				_003C_003E1__state = 2;
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

	private sealed class _003C_003Ec__DisplayClass58_0
	{
		public DecorateRoomScreen _003C_003E4__this;

		public DecoratingScene scene;

		public bool isFirstTime;
	}

	private sealed class _003C_003Ec__DisplayClass58_1
	{
		public NavigationManager nav;

		public _003C_003Ec__DisplayClass58_0 CS_0024_003C_003E8__locals1;

		internal void _003CInitScene_003Eb__0(bool success)
		{
			if (!success)
			{
				Application.Quit();
				return;
			}
			GGPlayerSettings.instance.Model.acceptedTermsOfService = true;
			GGPlayerSettings.instance.Save();
			nav.Pop();
			CS_0024_003C_003E8__locals1._003C_003E4__this.InitScene(CS_0024_003C_003E8__locals1.scene, CS_0024_003C_003E8__locals1.isFirstTime);
		}
	}

	private sealed class _003C_003Ec__DisplayClass59_0
	{
		public bool isComplete;

		internal void _003CDoShowCharacterAnimation_003Eb__0()
		{
			isComplete = true;
		}
	}

	private sealed class _003CDoShowCharacterAnimation_003Ed__59 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public List<ChangeAnimationArguments> animationParamsList;

		public DecorateRoomScreen _003C_003E4__this;

		private _003C_003Ec__DisplayClass59_0 _003C_003E8__1;

		public Action onComplete;

		private bool _003CwideBarsShown_003E5__2;

		private int _003Ci_003E5__3;

		private bool _003ChasMoreAnimations_003E5__4;

		private ChangeAnimationArguments _003CanimationParams_003E5__5;

		private float _003Ctime_003E5__6;

		private float _003Cduration_003E5__7;

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
		public _003CDoShowCharacterAnimation_003Ed__59(int _003C_003E1__state)
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
			DecorateRoomScreen decorateRoomScreen = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003CwideBarsShown_003E5__2 = false;
				_003Ci_003E5__3 = 0;
				goto IL_01f7;
			case 1:
				_003C_003E1__state = -1;
				goto IL_014e;
			case 2:
				{
					_003C_003E1__state = -1;
					goto IL_01c6;
				}
				IL_01f7:
				if (_003Ci_003E5__3 < animationParamsList.Count)
				{
					_003C_003E8__1 = new _003C_003Ec__DisplayClass59_0();
					bool flag = _003Ci_003E5__3 == animationParamsList.Count - 1;
					_003ChasMoreAnimations_003E5__4 = !flag;
					_003CanimationParams_003E5__5 = animationParamsList[_003Ci_003E5__3];
					if (_003CanimationParams_003E5__5.isAnimationAvailable)
					{
						if (_003CanimationParams_003E5__5.showWideBars && !_003CwideBarsShown_003E5__2)
						{
							decorateRoomScreen.aspectBars.AnimateShow();
							_003CwideBarsShown_003E5__2 = true;
						}
						if (_003CwideBarsShown_003E5__2 && !_003CanimationParams_003E5__5.showWideBars)
						{
							decorateRoomScreen.aspectBars.AnimateHide();
							_003CwideBarsShown_003E5__2 = false;
						}
						decorateRoomScreen.scene.AnimateCharacterAlphaTo(1f);
						decorateRoomScreen.scene.SetCharacterAnimationAlpha(1f);
						_003C_003E8__1.isComplete = false;
						_003CanimationParams_003E5__5.onComplete = _003C_003E8__1._003CDoShowCharacterAnimation_003Eb__0;
						decorateRoomScreen.scene.PlayCharacterAnimation(_003CanimationParams_003E5__5);
						goto IL_014e;
					}
					goto IL_01e7;
				}
				if (_003CwideBarsShown_003E5__2)
				{
					decorateRoomScreen.aspectBars.AnimateHide();
					_003CwideBarsShown_003E5__2 = false;
				}
				if (onComplete != null)
				{
					onComplete();
				}
				else
				{
					decorateRoomScreen.InitScene(decorateRoomScreen.scene, isFirstTime: false);
				}
				return false;
				IL_01d4:
				_003C_003E8__1 = null;
				_003CanimationParams_003E5__5 = default(ChangeAnimationArguments);
				goto IL_01e7;
				IL_014e:
				if (!_003C_003E8__1.isComplete)
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				if (_003ChasMoreAnimations_003E5__4 || !_003CanimationParams_003E5__5.animation.leaveAfterInit)
				{
					decorateRoomScreen.scene.AnimateCharacterAlphaTo(0f);
					_003Ctime_003E5__6 = 0f;
					_003Cduration_003E5__7 = 0.25f;
					goto IL_01c6;
				}
				goto IL_01d4;
				IL_01c6:
				if (_003Ctime_003E5__6 <= _003Cduration_003E5__7)
				{
					_003Ctime_003E5__6 += Time.deltaTime;
					_003C_003E2__current = null;
					_003C_003E1__state = 2;
					return true;
				}
				goto IL_01d4;
				IL_01e7:
				_003Ci_003E5__3++;
				goto IL_01f7;
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

	private sealed class _003C_003Ec__DisplayClass60_0
	{
		public bool isComplete;

		internal void _003CDoShowCharacterAnimation_003Eb__0()
		{
			isComplete = true;
		}
	}

	private sealed class _003CDoShowCharacterAnimation_003Ed__60 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public ChangeAnimationArguments animationParams;

		public DecorateRoomScreen _003C_003E4__this;

		private _003C_003Ec__DisplayClass60_0 _003C_003E8__1;

		public Action onComplete;

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
		public _003CDoShowCharacterAnimation_003Ed__60(int _003C_003E1__state)
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
			DecorateRoomScreen decorateRoomScreen = _003C_003E4__this;
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
				if (!animationParams.isAnimationAvailable)
				{
					goto IL_00e4;
				}
				_003C_003E8__1 = new _003C_003Ec__DisplayClass60_0();
				decorateRoomScreen.scene.AnimateCharacterAlphaTo(1f);
				decorateRoomScreen.scene.SetCharacterAnimationAlpha(1f);
				_003C_003E8__1.isComplete = false;
				animationParams.onComplete = _003C_003E8__1._003CDoShowCharacterAnimation_003Eb__0;
				decorateRoomScreen.scene.PlayCharacterAnimation(animationParams);
			}
			if (!_003C_003E8__1.isComplete)
			{
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			}
			if (!animationParams.animation.leaveAfterInit)
			{
				decorateRoomScreen.scene.AnimateCharacterAlphaTo(0f);
			}
			_003C_003E8__1 = null;
			goto IL_00e4;
			IL_00e4:
			if (onComplete != null)
			{
				onComplete();
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

	private sealed class _003CDoShowMessageEnumerator_003Ed__61 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public ProgressMessageList progressMessages;

		public DecorateRoomScreen _003C_003E4__this;

		public Action onComplete;

		private List<string> _003Cmessages_003E5__2;

		private IEnumerator _003Cenumerator_003E5__3;

		private TextDialog.MessageArguments _003CmessageArguments_003E5__4;

		private int _003Ci_003E5__5;

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
		public _003CDoShowMessageEnumerator_003Ed__61(int _003C_003E1__state)
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
			DecorateRoomScreen decorateRoomScreen = _003C_003E4__this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				_003C_003E1__state = -1;
				goto IL_011e;
			}
			_003C_003E1__state = -1;
			_003Cmessages_003E5__2 = progressMessages.messageList;
			decorateRoomScreen.HideSelectionUI();
			_003Cenumerator_003E5__3 = null;
			_003CmessageArguments_003E5__4 = default(TextDialog.MessageArguments);
			if (progressMessages.progressChange != null)
			{
				_003CmessageArguments_003E5__4.showProgress = true;
				_003CmessageArguments_003E5__4.fromProgress = progressMessages.progressChange.fromProgress;
				_003CmessageArguments_003E5__4.toProgress = progressMessages.progressChange.toProgress;
			}
			if (_003Cmessages_003E5__2 != null)
			{
				_003Ci_003E5__5 = 0;
				goto IL_013b;
			}
			goto IL_0151;
			IL_0151:
			if (onComplete != null)
			{
				onComplete();
			}
			else
			{
				decorateRoomScreen.InitScene(decorateRoomScreen.scene, isFirstTime: false);
			}
			return false;
			IL_013b:
			if (_003Ci_003E5__5 < _003Cmessages_003E5__2.Count)
			{
				string message = _003Cmessages_003E5__2[_003Ci_003E5__5];
				if (_003Ci_003E5__5 == _003Cmessages_003E5__2.Count - 1)
				{
					_003CmessageArguments_003E5__4.message = message;
					_003Cenumerator_003E5__3 = decorateRoomScreen.DoShowMessageEnumerator(_003CmessageArguments_003E5__4);
				}
				else
				{
					_003Cenumerator_003E5__3 = decorateRoomScreen.DoShowMessageEnumerator(message);
				}
				goto IL_011e;
			}
			goto IL_0151;
			IL_011e:
			if (_003Cenumerator_003E5__3.MoveNext())
			{
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			}
			_003Ci_003E5__5++;
			goto IL_013b;
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

	[Serializable]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Action _003C_003E9__62_0;

		internal void _003CDoShowMessageEnumerator_003Eb__62_0()
		{
		}
	}

	private sealed class _003CDoShowMessageEnumerator_003Ed__62 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public DecorateRoomScreen _003C_003E4__this;

		public List<string> messages;

		public List<ChangeAnimationArguments> animationParamsList;

		public Action onComplete;

		private IEnumerator _003Cenumerator_003E5__2;

		private int _003Ci_003E5__3;

		private IEnumerator _003CanimationParamEnum_003E5__4;

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
		public _003CDoShowMessageEnumerator_003Ed__62(int _003C_003E1__state)
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
			DecorateRoomScreen decorateRoomScreen = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				decorateRoomScreen.HideSelectionUI();
				_003Cenumerator_003E5__2 = null;
				if (messages != null)
				{
					_003Ci_003E5__3 = 0;
					goto IL_009c;
				}
				goto IL_00af;
			case 1:
				_003C_003E1__state = -1;
				goto IL_007f;
			case 2:
				{
					_003C_003E1__state = -1;
					goto IL_0101;
				}
				IL_009c:
				if (_003Ci_003E5__3 < messages.Count)
				{
					string message = messages[_003Ci_003E5__3];
					_003Cenumerator_003E5__2 = decorateRoomScreen.DoShowMessageEnumerator(message);
					goto IL_007f;
				}
				goto IL_00af;
				IL_00af:
				if (animationParamsList == null)
				{
					break;
				}
				_003CanimationParamEnum_003E5__4 = decorateRoomScreen.DoShowCharacterAnimation(animationParamsList, _003C_003Ec._003C_003E9._003CDoShowMessageEnumerator_003Eb__62_0);
				goto IL_0101;
				IL_007f:
				if (_003Cenumerator_003E5__2.MoveNext())
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				_003Ci_003E5__3++;
				goto IL_009c;
				IL_0101:
				if (_003CanimationParamEnum_003E5__4.MoveNext())
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 2;
					return true;
				}
				_003CanimationParamEnum_003E5__4 = null;
				break;
			}
			if (onComplete != null)
			{
				onComplete();
			}
			else
			{
				decorateRoomScreen.InitScene(decorateRoomScreen.scene, isFirstTime: false);
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

	private sealed class _003C_003Ec__DisplayClass64_0
	{
		public bool canContiue;

		internal void _003CDoShowMessageEnumerator_003Eb__0()
		{
			canContiue = true;
		}
	}

	private sealed class _003CDoShowMessageEnumerator_003Ed__64 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public DecorateRoomScreen _003C_003E4__this;

		public TextDialog.MessageArguments messageArguments;

		private _003C_003Ec__DisplayClass64_0 _003C_003E8__1;

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
		public _003CDoShowMessageEnumerator_003Ed__64(int _003C_003E1__state)
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
			DecorateRoomScreen decorateRoomScreen = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
			{
				_003C_003E1__state = -1;
				_003C_003E8__1 = new _003C_003Ec__DisplayClass64_0();
				decorateRoomScreen.visibilityHelper.Clear();
				decorateRoomScreen.visibilityHelper.SaveIsVisible(decorateRoomScreen.widgetsToHide);
				GGUtil.SetActive(decorateRoomScreen.widgetsToHide, active: false);
				messageArguments.message = messageArguments.message.Replace("\\n", "\n");
				TextDialog @object = NavigationManager.instance.GetObject<TextDialog>();
				_003C_003E8__1.canContiue = false;
				@object.ShowOk(messageArguments, _003C_003E8__1._003CDoShowMessageEnumerator_003Eb__0);
				break;
			}
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			if (!_003C_003E8__1.canContiue)
			{
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			}
			decorateRoomScreen.visibilityHelper.Complete();
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

	private sealed class _003C_003Ec__DisplayClass69_0
	{
		public NavigationManager nav;

		public RoomsDB.Room loadedRoom;

		public GiftBoxScreen giftBoxScreen;

		public GiftBoxScreen.ShowArguments arguments;

		internal void _003COnCompleteRoom_003Eb__0()
		{
			nav.Pop(activateNextScreen: false);
			ScrollableSelectRoomScreen.ChangeRoomArguments changeRoomArguments = new ScrollableSelectRoomScreen.ChangeRoomArguments();
			changeRoomArguments.passedRoom = loadedRoom;
			changeRoomArguments.passedRoom.isPassed = true;
			changeRoomArguments.unlockedRoom = ScriptableObjectSingleton<RoomsDB>.instance.NextRoom(loadedRoom);
			nav.GetObject<ScrollableSelectRoomScreen>().Show(changeRoomArguments);
		}

		internal void _003COnCompleteRoom_003Eb__1()
		{
			nav.Pop();
			giftBoxScreen.Show(arguments);
		}
	}

	private sealed class _003C_003Ec__DisplayClass82_0
	{
		public WinScreen.InitArguments winScreenArguments;

		public WinScreen winScreen;

		internal void _003COnGameComplete_003Eb__0()
		{
			GGPlayerSettings.instance.walletManager.AddCurrency(CurrencyType.coins, (int)winScreenArguments.coinsWon);
			GGPlayerSettings.instance.walletManager.AddCurrency(CurrencyType.coins, 20);
			// winScreen.Show(winScreenArguments);
			GGSoundSystem.Play(GGSoundSystem.MusicType.MainMenuMusic);
		}
	}

	private sealed class _003C_003Ec__DisplayClass93_0
	{
		public NavigationManager nav;

		internal void _003COnGoBack_003Eb__0(bool success)
		{
			if (success)
			{
				nav.Pop();
			}
			else
			{
				Application.Quit();
			}
		}
	}
	
	[SerializeField] private RoomProgressBar roomProgressBar;

	[SerializeField]
	private List<Transform> levelDifficultyWidgets = new List<Transform>();

	[SerializeField]
	private VisualStyleSet normalDifficultySlyle = new VisualStyleSet();

	[SerializeField]
	private VisualStyleSet hardDifficultySlyle = new VisualStyleSet();

	[SerializeField]
	private VisualStyleSet nightmareDifficultySlyle = new VisualStyleSet();

	[SerializeField]
	private bool alwaysPlaySceneCompleteAnimation;

	[SerializeField]
	public TutorialHandController tutorialHand;

	[SerializeField]
	private ComponentPool speachBubblePool = new ComponentPool();

	[SerializeField]
	private CanvasGroup bubbleGroup;

	[SerializeField]
	private GroupFooter groupFooter;

	[SerializeField]
	private WideAspectBars aspectBars;

	[SerializeField]
	private StarConsumeAnimation starConsumeAnimation;

	[SerializeField]
	private List<UIItemSetup> uiItemSetups = new List<UIItemSetup>();

	[SerializeField]
	private float marginsPsdSize = 30f;

	[SerializeField]
	private bool useAccelerometer;

	[SerializeField]
	private ItemSelectionButton itemSelect;

	[SerializeField]
	private GameObject confettiParticle;

	[SerializeField]
	private VisualObjectParticles visualObjectParticles;

	private VisibilityHelper visibilityHelper = new VisibilityHelper();

	[SerializeField]
	private Accelerometer accelerometer = new Accelerometer();

	[SerializeField]
	public bool noCoinsForPurchase;

	[SerializeField]
	private TextMeshProUGUI levelCountLabel;

	[SerializeField]
	private ComponentPool visualItemsPool = new ComponentPool();

	[SerializeField]
	private List<RectTransform> widgetsToHide = new List<RectTransform>();

	[SerializeField]
	private List<RectTransform> controlWidgets = new List<RectTransform>();

	[SerializeField]
	private Image progressBarSprite;

	[SerializeField]
	private VisualStyleSet loadingSceneStyle = new VisualStyleSet();

	[SerializeField]
	private VisualStyleSet retryLoadingStyle = new VisualStyleSet();

	[SerializeField]
	private VisualStyleSet roomLoadedStyle = new VisualStyleSet();

	[SerializeField]
	private VariationPanel variationPanel;

	[SerializeField]
	private ConfirmPurchasePanel confirmPurchasePanel;

	[SerializeField]
	private MoneyPickupAnimation moneyPickupAnimation;

	[SerializeField]
	public RectTransform coinRect;

	[SerializeField]
	public RectTransform starRect;

	[SerializeField]
	public CurrencyPanel currencyPanel;

	[NonSerialized]
	private List<DecorateRoomSceneVisualItem> uiVisualItems = new List<DecorateRoomSceneVisualItem>();

	[SerializeField]
	private ScreenMessagePanel messagePanel;

	private IEnumerator updateEnumerator;

	private IEnumerator animationEnumerator;

	public Room activeRoom;

	private int wonCoinsCount;

	public DecoratingScene scene
	{
		get
		{
			if (activeRoom == null)
			{
				return null;
			}
			return activeRoom.sceneBehaviour;
		}
	}

	private bool isRoomLoaded
	{
		get
		{
			if (activeRoom != null && activeRoom.sceneBehaviour != null)
			{
				return activeRoom.loadedRoom == ScriptableObjectSingleton<RoomsDB>.instance.ActiveRoom;
			}
			return false;
		}
	}

	public void SetSpeachBubbleAlpha(float alpha)
	{
		bubbleGroup.alpha = alpha;
	}

	public CharacterSpeachBubble GetSpeachBubble(CharacterAvatar avatar)
	{
		string name = avatar.name;
		List<GameObject> usedObjects = speachBubblePool.usedObjects;
		CharacterSpeachBubble characterSpeachBubble = null;
		for (int i = 0; i < usedObjects.Count; i++)
		{
			CharacterSpeachBubble component = usedObjects[i].GetComponent<CharacterSpeachBubble>();
			if (component.characterName == name)
			{
				characterSpeachBubble = component;
				break;
			}
		}
		if (characterSpeachBubble == null)
		{
			characterSpeachBubble = speachBubblePool.Next<CharacterSpeachBubble>();
			GGUtil.SetActive(characterSpeachBubble, active: false);
		}
		characterSpeachBubble.characterName = name;
		characterSpeachBubble.avatar = avatar;
		return characterSpeachBubble;
	}

	private void ShowConfettiParticle()
	{
		GGUtil.SetActive(UnityEngine.Object.Instantiate(confettiParticle, base.transform), active: true);
	}

	private void OnEnable()
	{
		Init();
		accelerometer.Init();

		//StartGame(new Match3GameParams());
	}

	private void OnDisable()
	{
		if (isRoomLoaded)
		{
			GGUtil.SetActive(scene, active: false);
		}
	}

	private void OnApplicationFocus(bool pause)
	{
		if (updateEnumerator == null && !pause)
		{
			GGSnapshotCloudSync.CallOnFocus(pause);
			if (GGSnapshotCloudSync.syncNeeded)
			{
				Init();
			}
		}
	}

	public void Init()
	{
		NavigationManager instance = NavigationManager.instance;
		if (GGSnapshotCloudSync.syncNeeded)
		{
			instance.GetObject<SyncGameScreen>().SynchronizeNow();
		}
		else
		{
			FinishInit();
		}
	}

	private void FinishInit()
	{
		Match3StagesDB.Stage currentStage = Match3StagesDB.instance.currentStage;
		itemSelect.Init(this);
		GGUtil.ChangeText(levelCountLabel, currentStage.index + 1);
		if (isRoomLoaded)
		{
			GGUtil.SetActive(scene, active: true);
			GGUtil.SetActive(widgetsToHide, active: false);
			roomLoadedStyle.Apply();
			InitScene(scene, isFirstTime: true);
			InAppBackend instance = BehaviourSingletonInit<InAppBackend>.instance;
			roomProgressBar.InitProgressbar(scene.GetRoomProgressState().progress);
		}
		else
		{
			RoomsDB.Room room = ScriptableObjectSingleton<RoomsDB>.instance.ActiveRoom;
			LoadScene(room);
		}
		

	}

	private void LoadScene(RoomsDB.Room room)
	{
		GGUtil.SetActive(widgetsToHide, active: false);
		loadingSceneStyle.Apply();
		updateEnumerator = DoLoadScene(room);
	}

	private IEnumerator DoLoadScene(RoomsDB.Room room)
	{
		return new _003CDoLoadScene_003Ed__56(0)
		{
			_003C_003E4__this = this,
			room = room
		};
	}

	private void InitRetry()
	{
		GGUtil.SetActive(widgetsToHide, active: false);
		retryLoadingStyle.Apply();
		speachBubblePool.Clear();
		speachBubblePool.HideNotUsed();
		visualItemsPool.Clear();
		visualItemsPool.HideNotUsed();
		uiVisualItems.Clear();
		GGUtil.SetActive(controlWidgets, active: true);
		currencyPanel.Show();
		Match3StagesDB.Stage currentStage = Match3StagesDB.instance.currentStage;
		for (int i = 0; i < uiItemSetups.Count; i++)
		{
			UIItemSetup uIItemSetup = uiItemSetups[i];
			bool active = !currentStage.hideUIElements;
			if (Application.isEditor && uIItemSetup.name == UIItemName.SettingsButton)
			{
				active = true;
			}
			GGUtil.SetActive(uIItemSetup.widget, active);
		}
		GGUtil.Hide(levelDifficultyWidgets);
		if (currentStage.difficulty == Match3StagesDB.Stage.Difficulty.Normal)
		{
			normalDifficultySlyle.Apply();
		}
		else if (currentStage.difficulty == Match3StagesDB.Stage.Difficulty.Hard)
		{
			hardDifficultySlyle.Apply();
		}
		else if (currentStage.difficulty == Match3StagesDB.Stage.Difficulty.Nightmare)
		{
			nightmareDifficultySlyle.Apply();
		}
	}

	private void InitScene(DecoratingScene scene, bool isFirstTime)
	{
		_003C_003Ec__DisplayClass58_0 _003C_003Ec__DisplayClass58_ = new _003C_003Ec__DisplayClass58_0();
		_003C_003Ec__DisplayClass58_._003C_003E4__this = this;
		_003C_003Ec__DisplayClass58_.scene = scene;
		_003C_003Ec__DisplayClass58_.isFirstTime = isFirstTime;
		if (_003C_003Ec__DisplayClass58_.scene == null)
		{
			InitRetry();
			return;
		}
		if (Application.isEditor)
		{
			_003C_003Ec__DisplayClass58_.scene.InitRuntimeData();
		}
		speachBubblePool.Clear();
		speachBubblePool.HideNotUsed();
		groupFooter.Init(_003C_003Ec__DisplayClass58_.scene);
		if (_003C_003Ec__DisplayClass58_.isFirstTime)
		{
			_003C_003Ec__DisplayClass58_.scene.SetCharacterAlpha(0f);
			_003C_003Ec__DisplayClass58_.scene.StopCharacterAnimation();
		}
		ChangeAnimationArguments noAnimation = ChangeAnimationArguments.NoAnimation;
		List<DecoratingSceneConfig.AnimationSequence> availableSequences = _003C_003Ec__DisplayClass58_.scene.GetAvailableSequences();
		DecoratingSceneConfig.AnimationSequence animationSequence = null;
		if (availableSequences != null && availableSequences.Count > 0)
		{
			animationSequence = availableSequences[0];
		}
		if (animationSequence != null)
		{
			noAnimation.animation = animationSequence;
		}
		_003C_003Ec__DisplayClass58_.scene.ZoomOut();
		visualItemsPool.Clear();
		List<VisualObjectBehaviour> visualObjectBehaviours = _003C_003Ec__DisplayClass58_.scene.visualObjectBehaviours;
		uiVisualItems.Clear();
		int num = 0;
		float a = 0f;
		for (int i = 0; i < visualObjectBehaviours.Count; i++)
		{
			VisualObjectBehaviour visualObjectBehaviour = visualObjectBehaviours[i];
			GraphicsSceneConfig.VisualObject visualObject = visualObjectBehaviour.visualObject;
			if (_003C_003Ec__DisplayClass58_.isFirstTime && visualObject.isOwned && !visualObject.hasDefaultVariation)
			{
				a = Mathf.Max(a, visualObjectBehaviour.iconHandlePosition.x / (float)_003C_003Ec__DisplayClass58_.scene.config.width * 0.2f);
			}
		}
		a = 0f;
		DecorateRoomSceneVisualItem decorateRoomSceneVisualItem = null;
		for (int j = 0; j < visualObjectBehaviours.Count; j++)
		{
			VisualObjectBehaviour visualObjectBehaviour2 = visualObjectBehaviours[j];
			GraphicsSceneConfig.VisualObject visualObject2 = visualObjectBehaviour2.visualObject;
			DecorateRoomSceneVisualItem decorateRoomSceneVisualItem2 = visualItemsPool.Next<DecorateRoomSceneVisualItem>();
			GGUtil.SetActive(decorateRoomSceneVisualItem2, active: true);
			decorateRoomSceneVisualItem2.Init(visualObjectBehaviour2, this, num, a);
			bool num2 = visualObject2.IsUnlocked(_003C_003Ec__DisplayClass58_.scene) && !visualObject2.isOwned;
			if (num2)
			{
				num++;
			}
			uiVisualItems.Add(decorateRoomSceneVisualItem2);
			if (num2 && visualObject2.sceneObjectInfo.autoSelect)
			{
				decorateRoomSceneVisualItem = decorateRoomSceneVisualItem2;
			}
		}
		visualItemsPool.HideNotUsed();
		GGUtil.SetActive(controlWidgets, active: true);
		currencyPanel.Show();
		Match3StagesDB.Stage currentStage = Match3StagesDB.instance.currentStage;
		for (int k = 0; k < uiItemSetups.Count; k++)
		{
			UIItemSetup uIItemSetup = uiItemSetups[k];
			bool active = !currentStage.hideUIElements;
			if (Application.isEditor && uIItemSetup.name == UIItemName.SettingsButton)
			{
				active = true;
			}
			GGUtil.SetActive(uIItemSetup.widget, active);
		}
		GGUtil.Hide(levelDifficultyWidgets);
		if (currentStage.difficulty == Match3StagesDB.Stage.Difficulty.Normal)
		{
			normalDifficultySlyle.Apply();
		}
		else if (currentStage.difficulty == Match3StagesDB.Stage.Difficulty.Hard)
		{
			hardDifficultySlyle.Apply();
		}
		else if (currentStage.difficulty == Match3StagesDB.Stage.Difficulty.Nightmare)
		{
			nightmareDifficultySlyle.Apply();
		}
		if (!GGPlayerSettings.instance.Model.acceptedTermsOfService)
		{
			_003C_003Ec__DisplayClass58_1 _003C_003Ec__DisplayClass58_2 = new _003C_003Ec__DisplayClass58_1();
			_003C_003Ec__DisplayClass58_2.CS_0024_003C_003E8__locals1 = _003C_003Ec__DisplayClass58_;
			_003C_003Ec__DisplayClass58_2.nav = NavigationManager.instance;
			_003C_003Ec__DisplayClass58_2.nav.GetObject<TermsOfServiceDialog>().Show(_003C_003Ec__DisplayClass58_2._003CInitScene_003Eb__0);
		}
		else if (_003C_003Ec__DisplayClass58_.isFirstTime && currentStage.startMessages.Count > 0 && !currentStage.isIntroMessageShown)
		{
			_003C_003Ec__DisplayClass58_.scene.StopCharacterAnimation();
			animationEnumerator = DoShowMessageEnumerator(currentStage.startMessages, null);
			animationEnumerator.MoveNext();
			currentStage.isIntroMessageShown = true;
		}
		else if (decorateRoomSceneVisualItem != null)
		{
			VisualItemCallback_OnBuyItemPressed(decorateRoomSceneVisualItem);
		}
		else if (noAnimation.isAnimationAvailable)
		{
			if (_003C_003Ec__DisplayClass58_.scene.runningAnimation != noAnimation.animation)
			{
				animationEnumerator = DoShowCharacterAnimation(noAnimation);
				animationEnumerator.MoveNext();
			}
			else
			{
				_003C_003Ec__DisplayClass58_.scene.AnimateCharacterAlphaTo(1f);
			}
		}
	}

	private IEnumerator DoShowCharacterAnimation(List<ChangeAnimationArguments> animationParamsList, Action onComplete = null)
	{
		return new _003CDoShowCharacterAnimation_003Ed__59(0)
		{
			_003C_003E4__this = this,
			animationParamsList = animationParamsList,
			onComplete = onComplete
		};
	}

	private IEnumerator DoShowCharacterAnimation(ChangeAnimationArguments animationParams, Action onComplete = null)
	{
		return new _003CDoShowCharacterAnimation_003Ed__60(0)
		{
			_003C_003E4__this = this,
			animationParams = animationParams,
			onComplete = onComplete
		};
	}

	private IEnumerator DoShowMessageEnumerator(ProgressMessageList progressMessages, Action onComplete = null)
	{
		return new _003CDoShowMessageEnumerator_003Ed__61(0)
		{
			_003C_003E4__this = this,
			progressMessages = progressMessages,
			onComplete = onComplete
		};
	}

	private IEnumerator DoShowMessageEnumerator(List<string> messages, List<ChangeAnimationArguments> animationParamsList, Action onComplete = null)
	{
		return new _003CDoShowMessageEnumerator_003Ed__62(0)
		{
			_003C_003E4__this = this,
			messages = messages,
			animationParamsList = animationParamsList,
			onComplete = onComplete
		};
	}

	private IEnumerator DoShowMessageEnumerator(string message)
	{
		TextDialog.MessageArguments messageArguments = default(TextDialog.MessageArguments);
		messageArguments.message = message;
		return DoShowMessageEnumerator(messageArguments);
	}

	private IEnumerator DoShowMessageEnumerator(TextDialog.MessageArguments messageArguments)
	{
		return new _003CDoShowMessageEnumerator_003Ed__64(0)
		{
			_003C_003E4__this = this,
			messageArguments = messageArguments
		};
	}

	private void HideSelectionUI()
	{
		for (int i = 0; i < uiVisualItems.Count; i++)
		{
			DecorateRoomSceneVisualItem decorateRoomSceneVisualItem = uiVisualItems[i];
			GGUtil.SetActive(decorateRoomSceneVisualItem, active: false);
			decorateRoomSceneVisualItem.visualObjectBehaviour.SetMarkersActive(active: false);
		}
	}

	private void ShowStarConsumeAnimation(DecorateRoomSceneVisualItem visualItem, Action onEnd)
	{
		StarConsumeAnimation.InitParams initParams = default(StarConsumeAnimation.InitParams);
		initParams.screen = this;
		initParams.visualItem = visualItem;
		initParams.onEnd = onEnd;
		starConsumeAnimation.Show(initParams);
	}

	public void ButtonCallback_OnRetry()
	{
		Init();
	}

	public void VariationPanelCallback_OnClosed(VariationPanel variationPanel)
	{
		GraphicsSceneConfig.VisualObject visualObject = variationPanel.uiItem.visualObjectBehaviour.visualObject;
		List<string> list = visualObject.sceneObjectInfo.toSayAfterOpen;
		bool isPurchased = variationPanel.initParams.isPurchased;
		DecorateRoomSceneVisualItem uiItem = variationPanel.uiItem;
		VisualObjectBehaviour visualObjectBehaviour = uiItem.visualObjectBehaviour;
		scene.AnimationForVisualBehaviour(visualObjectBehaviour);
		if (isPurchased || variationPanel.isVariationChanged)
		{
			visualObjectParticles.CreateParticles(VisualObjectParticles.PositionType.BuySuccess, scene.rootTransform.gameObject, uiItem.visualObjectBehaviour);
			GGSoundSystem.Play(GGSoundSystem.SFXType.ButtonConfirm);
		}
		DecoratingScene.RoomProgressState roomProgressState = scene.GetRoomProgressState();
		RoomsBackend.RoomAccessor roomBackend = scene.roomBackend;
		bool isPassed = roomBackend.isPassed;
		bool flag = false;
		if (roomProgressState.isPassed && !roomBackend.isPassed)
		{
			roomBackend.isPassed = true;
			flag = true;
		}
		if (isPurchased)
		{
			new List<ChangeAnimationArguments>();
			DecoratingScene.GroupDefinition groupForIndex = scene.GetGroupForIndex(visualObject.sceneObjectInfo.groupIndex);
			if (groupForIndex != null && scene.IsAllElementsPickedUpInGroup(visualObject.sceneObjectInfo.groupIndex) && groupForIndex.toSayAfterGroupCompletes.Count > 0)
			{
				list = groupForIndex.toSayAfterGroupCompletes;
			}
			if (list.Count == 0)
			{
				list.Add("Great job!");
			}
			ProgressMessageList progressMessageList = new ProgressMessageList();
			progressMessageList.messageList = list;
			if (!isPassed)
			{
				progressMessageList.progressChange = new ProgressMessageList.ProgressChange();
				progressMessageList.progressChange.fromProgress = roomProgressState.Progress(1);
				progressMessageList.progressChange.toProgress = roomProgressState.progress;
			}
			Action onComplete = null;
			if (flag || (Application.isEditor && alwaysPlaySceneCompleteAnimation))
			{
				onComplete = OnCompleteRoom;
			}
			IEnumerator enumerator = null;
			if (list.Count > 0)
			{
				enumerator = DoShowMessageEnumerator(progressMessageList, onComplete);
			}
			if (enumerator != null)
			{
				animationEnumerator = enumerator;
				animationEnumerator.MoveNext();
				ShowConfettiParticle();
				return;
			}
		}
		InitScene(scene, isFirstTime: false);
	}

	[ContextMenu("Complete room")]
	private void OnCompleteRoom()
	{
		_003C_003Ec__DisplayClass69_0 _003C_003Ec__DisplayClass69_ = new _003C_003Ec__DisplayClass69_0();
		_003C_003Ec__DisplayClass69_.nav = NavigationManager.instance;
		_003C_003Ec__DisplayClass69_.giftBoxScreen = _003C_003Ec__DisplayClass69_.nav.GetObject<GiftBoxScreen>();
		_003C_003Ec__DisplayClass69_.loadedRoom = activeRoom.loadedRoom;
		_003C_003Ec__DisplayClass69_.arguments = default(GiftBoxScreen.ShowArguments);
		_003C_003Ec__DisplayClass69_.arguments.title = "Room Complete";
		_003C_003Ec__DisplayClass69_.arguments.giftsDefinition = _003C_003Ec__DisplayClass69_.loadedRoom.giftDefinition;
		_003C_003Ec__DisplayClass69_.arguments.onComplete = _003C_003Ec__DisplayClass69_._003COnCompleteRoom_003Eb__0;
		_003C_003Ec__DisplayClass69_.nav.GetObject<WellDoneScreen>().Show(new WellDoneScreen.InitArguments
		{
			mainText = "Room Complete",
			onComplete = _003C_003Ec__DisplayClass69_._003COnCompleteRoom_003Eb__1
		});
	}

	private void ShowVariations(DecorateRoomSceneVisualItem uiItem, VariationPanel.InitParams initParams)
	{
		HideSelectionUI();
		scene.ZoomIn(uiItem.visualObjectBehaviour);
		List<VisualObjectBehaviour> visualObjectBehaviours = scene.visualObjectBehaviours;
		uiVisualItems.Clear();
		for (int i = 0; i < visualObjectBehaviours.Count; i++)
		{
			VisualObjectBehaviour visualObjectBehaviour = visualObjectBehaviours[i];
			visualObjectBehaviour.SetVisualState();
			visualObjectBehaviour.SetMarkersActive(active: false);
		}
		uiItem.HideButton();
		uiItem.ShowMarkers();
		GGUtil.SetActive(controlWidgets, active: false);
		variationPanel.Show(this, uiItem, initParams);
		if (initParams.isPurchased)
		{
			ShowStarConsumeAnimation(uiItem, null);
		}
		scene.AnimateCharacterAlphaTo(0f);
		bool hideCharacterWhenSelectingVariation = uiItem.visualObjectBehaviour.visualObject.sceneObjectInfo.hideCharacterWhenSelectingVariations;
		visualObjectParticles.CreateParticles(VisualObjectParticles.PositionType.ChangeSuccess, scene.rootTransform.gameObject, uiItem.visualObjectBehaviour);
	}

	public void VisualItemCallback_OnBuyItemPressed(DecorateRoomSceneVisualItem uiItem)
	{
		HideSelectionUI();
		GGUtil.SetActive(uiItem, active: true);
		uiItem.ShowMarkers();
		uiItem.HideButton();
		GGUtil.SetActive(confirmPurchasePanel, active: true);
		confirmPurchasePanel.Show(uiItem, this);
		scene.ZoomIn(uiItem.visualObjectBehaviour);
		GGUtil.SetActive(controlWidgets, active: false);
		scene.AnimateCharacterAlphaTo(0f);
		GGSoundSystem.Play(GGSoundSystem.SFXType.FlyIn);
	}

	private void UpdateBubblePosition()
	{
		if (scene == null)
		{
			return;
		}
		List<GameObject> usedObjects = speachBubblePool.usedObjects;
		for (int i = 0; i < usedObjects.Count; i++)
		{
			CharacterSpeachBubble component = usedObjects[i].GetComponent<CharacterSpeachBubble>();
			if (component.avatar == null)
			{
				GGUtil.SetActive(component, active: false);
			}
			Vector3 localPosition = component.transform.parent.InverseTransformPoint(scene.CharacterBubblePosition(component.avatar));
			component.transform.localPosition = localPosition;
		}
	}

	private void Update()
	{
		if (useAccelerometer)
		{
			accelerometer.Update();
		}
		UpdateBubblePosition();
		if (scene != null && useAccelerometer)
		{
			Vector3 rootTransformOffsetAcceleration = new Vector3(accelerometer.currentPosition.x * marginsPsdSize, (0f - accelerometer.currentPosition.y) * marginsPsdSize, 0f);
			rootTransformOffsetAcceleration.x *= scene.psdTransformationScale.x;
			rootTransformOffsetAcceleration.y *= scene.psdTransformationScale.y;
			scene.rootTransformOffsetAcceleration = rootTransformOffsetAcceleration;
		}
		if (updateEnumerator != null && !updateEnumerator.MoveNext())
		{
			roomProgressBar.InitProgressbar(scene.GetRoomProgressState().progress);
			updateEnumerator = null;
		}
		if (animationEnumerator != null && !animationEnumerator.MoveNext())
		{
			animationEnumerator = null;
		}
	}

	public void ButtonCallback_OnSceneClick()
	{
		Vector3 position = UnityEngine.Input.mousePosition;
		if (UnityEngine.Input.touchCount > 0)
		{
			position = UnityEngine.Input.GetTouch(0).position;
		}
		Vector3 v = NavigationManager.instance.GetCamera().ScreenToWorldPoint(position);
		Vector3 v2 = TransformWorldToPSDPoint(v);
		DecorateRoomSceneVisualItem decorateRoomSceneVisualItem = null;
		int num = 0;
		for (int i = 0; i < uiVisualItems.Count; i++)
		{
			DecorateRoomSceneVisualItem decorateRoomSceneVisualItem2 = uiVisualItems[i];
			if (!decorateRoomSceneVisualItem2.visualObjectBehaviour.visualObject.isOwned)
			{
				continue;
			}
			GraphicsSceneConfig.VisualObject.HitResult hitResult = decorateRoomSceneVisualItem2.visualObjectBehaviour.visualObject.GetHitResult(v2);
			if (hitResult.isHit)
			{
				int hitDepth = hitResult.hitDepth;
				if (!(decorateRoomSceneVisualItem != null) || num <= hitDepth)
				{
					decorateRoomSceneVisualItem = decorateRoomSceneVisualItem2;
					num = hitDepth;
				}
			}
		}
		if (decorateRoomSceneVisualItem == null)
		{
			GGSoundSystem.Play(GGSoundSystem.SFXType.CancelPress);
			return;
		}
		VariationPanel.InitParams initParams = default(VariationPanel.InitParams);
		GGSoundSystem.Play(GGSoundSystem.SFXType.ButtonPress);
		ShowVariations(decorateRoomSceneVisualItem, initParams);
	}

	public void StartGame()
	{
		var disableLives = true;
		if(!disableLives)
		{
			if (!BehaviourSingleton<EnergyManager>.instance.HasEnergyForOneLife())
			{
				// OutOfLivesDialog @object = NavigationManager.instance.GetObject<OutOfLivesDialog>();
				// LivesPriceConfig.PriceConfig priceForLevelOrDefault =
				// 	ScriptableObjectSingleton<LivesPriceConfig>.instance.GetPriceForLevelOrDefault(
				// 		initParams.levelIndex);
				// @object.Show(priceForLevelOrDefault, OnAllLifesRefilled, OnFirstLifeRefilled, Init);
				// return;
			}
		}
		GameScreen object2 = NavigationManager.instance.GetObject<GameScreen>();
		// Match3StagesDB.Stage currentStage = Match3StagesDB.instance.currentStage;
		// var initParams = new Match3GameParams();
		// initParams.level = Resources.Load<LevelSettings>("LevelSettingss").LevelDefinition;
		// Match3StagesDB.Stage currentStage = Match3StagesDB.instance[-2000];
		Match3StagesDB.Stage currentStage = Match3StagesDB.instance.currentStage;
		//
		//currentStage.CompleteStage();
		var initParams = currentStage.GetInitParams(this);
		object2.Show(initParams);
		GGSoundSystem.Play(GGSoundSystem.MusicType.GameMusic);
	}

	public void OnAllLifesRefilled()
	{
		NavigationManager.instance.GetObject<OutOfLivesDialog>().Hide();
		ShowPregameDialog();
	}

	public void OnFirstLifeRefilled()
	{
		NavigationManager.instance.GetObject<OutOfLivesDialog>().Hide();
		ShowPregameDialog();
	}

	public void ButtonCallback_PlayButtonClick()
	{
		StartGame();
		//ShowPregameDialog();
	}

	public void ShowPregameDialog()
	{
		PreGameDialog @object = NavigationManager.instance.GetObject<PreGameDialog>();
		HideSelectionUI();
		if (scene != null)
		{
			List<VisualObjectBehaviour> visualObjectBehaviours = scene.visualObjectBehaviours;
			uiVisualItems.Clear();
			for (int i = 0; i < visualObjectBehaviours.Count; i++)
			{
				visualObjectBehaviours[i].SetMarkersActive(active: false);
			}
		}
		GGUtil.SetActive(widgetsToHide, active: false);
		Match3StagesDB.Stage currentStage = Match3StagesDB.instance.currentStage;
		@object.Show(currentStage, this, Init);
	}

	public void OnGameStarted(GameStartedParams startedParams)
	{
		//BehaviourSingleton<EnergyManager>.instance.SpendLifeIfNotFreeEnergy();
		List<BoosterConfig> activeBoosters = startedParams.game.initParams.activeBoosters;
		PlayerInventory instance = PlayerInventory.instance;
		for (int i = 0; i < activeBoosters.Count; i++)
		{
			BoosterConfig boosterConfig = activeBoosters[i];
			instance.SetOwned(boosterConfig.boosterType, instance.OwnedCount(boosterConfig.boosterType) - 1);
			instance.SetUsedCount(boosterConfig.boosterType, instance.UsedCount(boosterConfig.boosterType) + 1);
		}
	}

	public void OnGameComplete(GameCompleteParams completeParams)
	{
		Match3Game game = completeParams.game;
		Match3StagesDB.Stage stage = game.initParams.stage;
		bool flag = false;
		if (stage != null)
		{
			flag = stage.isPassed;
			stage.OnGameComplete(completeParams);
		}
		
		currencyPanel.SetLabels();
		GGSoundSystem.Play(GGSoundSystem.MusicType.MainMenuMusic);
		GameScreen gameScreen = game.gameScreen;
		NavigationManager.instance.Pop();
		gameScreen.ShowGame(false);
		return;
		
		if (completeParams.isWin)
		{
			_003C_003Ec__DisplayClass82_0 _003C_003Ec__DisplayClass82_ = new _003C_003Ec__DisplayClass82_0();
			NavigationManager instance = NavigationManager.instance;
			_003C_003Ec__DisplayClass82_.winScreen = instance.GetObject<WinScreen>();
			WalletManager walletManager = GGPlayerSettings.instance.walletManager;
			_003C_003Ec__DisplayClass82_.winScreenArguments = new WinScreen.InitArguments();
			_003C_003Ec__DisplayClass82_.winScreenArguments.previousCoins = walletManager.CurrencyCount(CurrencyType.coins);
			_003C_003Ec__DisplayClass82_.winScreenArguments.baseStageWonCoins = 50L;
			_003C_003Ec__DisplayClass82_.winScreenArguments.previousStars = walletManager.CurrencyCount(CurrencyType.diamonds);
			_003C_003Ec__DisplayClass82_.winScreenArguments.currentStars = _003C_003Ec__DisplayClass82_.winScreenArguments.previousStars + 1;
			_003C_003Ec__DisplayClass82_.winScreenArguments.onMiddle = OnWinScreenAnimationMiddle;
			_003C_003Ec__DisplayClass82_.winScreenArguments.game = game;
			_003C_003Ec__DisplayClass82_.winScreenArguments.decorateRoomScreen = this;
			_003C_003Ec__DisplayClass82_.winScreenArguments.starRect = starRect;
			_003C_003Ec__DisplayClass82_.winScreenArguments.coinRect = coinRect;
			_003C_003Ec__DisplayClass82_.winScreenArguments.currencyPanel = currencyPanel;
			game.StartWinAnimation(_003C_003Ec__DisplayClass82_.winScreenArguments, _003C_003Ec__DisplayClass82_._003COnGameComplete_003Eb__0);
			wonCoinsCount = stage.coinsCount;
			if (!flag)
			{
				GGPlayerSettings.instance.walletManager.AddCurrency(CurrencyType.diamonds, moneyPickupAnimation.settings.numberOfStars);
			}
			BehaviourSingleton<EnergyManager>.instance.AddLifeIfNotFreeEnergy();
		}
		else
		{
			GGSoundSystem.Play(GGSoundSystem.MusicType.MainMenuMusic);
			if (!game.hasPlayedAnyMoves)
			{
				BehaviourSingleton<EnergyManager>.instance.AddLifeIfNotFreeEnergy();
			}
			GameScreen gameScreen2 = game.gameScreen;
			NavigationManager.instance.Pop();
			gameScreen2.DestroyCreatedGameObjects();
			if (game.hasPlayedAnyMoves)
			{
				ShowPregameDialog();
			}
		}
	}

	private void OnWinScreenAnimationMiddle()
	{
		NavigationManager instance = NavigationManager.instance;
		instance.GetObject<GameScreen>().DestroyCreatedGameObjects();
		GiftsDefinitionDB.GiftDefinition currentGift = ScriptableObjectSingleton<GiftsDefinitionDB>.instance.currentGift;
		bool num = currentGift?.isAvailableToCollect ?? false;
		RateCallerSettings instance2 = ScriptableObjectSingleton<RateCallerSettings>.instance;
		if (num)
		{
			instance.Pop(activateNextScreen: false);
			GiftBoxScreen @object = instance.GetObject<GiftBoxScreen>();
			GiftBoxScreen.ShowArguments showArguments = new GiftBoxScreen.ShowArguments
			{
				giftsDefinition = currentGift.gifts,
				title = ""
			};
			currentGift.ClaimGifts();
			@object.Show(showArguments);
		}
		else if (instance2.ShouldShow(Match3StagesDB.instance.passedStages))
		{
			instance.Pop();
			RatingScreen object2 = NavigationManager.instance.GetObject<RatingScreen>();
			NavigationManager.instance.Push(object2.gameObject, isModal: true);
			instance2.OnDialogShow();
		}
		else
		{
			instance.Pop();
			InitScene(scene, isFirstTime: true);
			currencyPanel.SetLabels();
		}
	}

	private void OnCoinsGiven()
	{
		InitScene(scene, isFirstTime: true);
		currencyPanel.SetLabels();
	}

	public Vector3 TransformPSDToWorldPoint(Vector2 point)
	{
		return scene.PSDToWorldPoint(point);
	}

	public Vector3 TransformWorldToPSDPoint(Vector2 point)
	{
		return scene.WorldToPSDPoint(point);
	}

	public void ConfirmPurchasePanelCallback_OnConfirm(DecorateRoomSceneVisualItem uiItem)
	{
		SingleCurrencyPrice price = uiItem.visualObjectBehaviour.visualObject.sceneObjectInfo.price;
		WalletManager walletManager = GGPlayerSettings.instance.walletManager;
		GGUtil.SetActive(confirmPurchasePanel, active: false);
		#if !UNITY_EDITOR
		UnityEngine.Debug.LogFormat("Price is {0} {1}", price.cost, price.currency);
		if ((!Application.isEditor || !noCoinsForPurchase) && !walletManager.CanBuyItemWithPrice(price))
		{
			ShowPregameDialog();
			return;
		}
#endif
		uiItem.visualObjectBehaviour.visualObject.isOwned = true;
		VariationPanel.InitParams initParams = default(VariationPanel.InitParams);
		initParams.isPurchased = true;
		ShowVariations(uiItem, initParams);
		walletManager.BuyItem(price);
		currencyPanel.SetLabels();
		Analytics.RoomItemBoughtEvent roomItemBoughtEvent = new Analytics.RoomItemBoughtEvent();
		roomItemBoughtEvent.price = price;
		roomItemBoughtEvent.screen = this;
		roomItemBoughtEvent.visualObject = uiItem.visualObjectBehaviour.visualObject;
		int ownedVariationIndex = uiItem.visualObjectBehaviour.visualObject.ownedVariationIndex;
		roomItemBoughtEvent.variation = uiItem.visualObjectBehaviour.visualObject.variations[ownedVariationIndex];
		roomItemBoughtEvent.numberOfItemsOwned = scene.ownedItemsCount;
		roomItemBoughtEvent.Send();
		GGSoundSystem.Play(GGSoundSystem.SFXType.FlyIn);
		var designNumber = PlayerPrefs.GetInt("designNumber", 0);
		designNumber++;
		AnalyticsManager.Log($"design_complete_{designNumber}");
		PlayerPrefs.SetInt("designNumber", designNumber);
	}

	public void ConfirmPurchasePanelCallback_OnClosed()
	{
		InitScene(scene, isFirstTime: false);
	}

	public void ButtonCallback_OnLivesClicked()
	{
		GGSoundSystem.Play(GGSoundSystem.SFXType.ButtonPress);
		if (!BehaviourSingleton<EnergyManager>.instance.isFreeEnergy && BehaviourSingleton<EnergyManager>.instance.ownedPlayCoins != EnergyControlConfig.instance.totalCoin)
		{
			GGSoundSystem.Play(GGSoundSystem.SFXType.ButtonPress);
			OutOfLivesDialog @object = NavigationManager.instance.GetObject<OutOfLivesDialog>();
			int index = Match3StagesDB.instance.currentStage.index;
			LivesPriceConfig.PriceConfig priceForLevelOrDefault = ScriptableObjectSingleton<LivesPriceConfig>.instance.GetPriceForLevelOrDefault(index);
			@object.Show(priceForLevelOrDefault, null, null, () =>
			{
				if (!gameObject.activeSelf)
				{
					Debug.LogError("In not start screen");
					return;
				}
				
				Init();
			});
		}
	}

	public void ButtonCallback_OnSettingsButtonPress()
	{
		NavigationManager instance = NavigationManager.instance;
		if (ConfigBase.instance.debug)
		{
			SettingsScreen @object = instance.GetObject<SettingsScreen>();
			instance.Push(@object);
		}
		else
		{
			InGameSettingsScreen object2 = instance.GetObject<InGameSettingsScreen>();
			instance.Push(object2);
		}
	}

	public override void OnGoBack(NavigationManager nav)
	{
		_003C_003Ec__DisplayClass93_0 _003C_003Ec__DisplayClass93_ = new _003C_003Ec__DisplayClass93_0();
		_003C_003Ec__DisplayClass93_.nav = nav;
		base.OnGoBack(_003C_003Ec__DisplayClass93_.nav);
		Dialog.instance.Show("Exit Game?", "Stay", "Exit", _003C_003Ec__DisplayClass93_._003COnGoBack_003Eb__0);
	}
}
