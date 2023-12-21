using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace GGMatch3
{
	public class DestroyChipActionGrow : BoardAction
	{
		[Serializable]
		public class Settings
		{
			public SpriteSortingSettings sortingLayer = new SpriteSortingSettings();

			public float fromScale = 2f;

			public float toScale;

			public float fromAlpha = 1f;

			public float toAlpha;

			public float duration;

			public AnimationCurve animationCurve;

			public bool holdGravityOnChip;

			public float holdGravityDuration;

			public float lightIntensity = 0.6f;
		}

		private sealed class _003CDoAnimation_003Ed__9 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public DestroyChipActionGrow _003C_003E4__this;

			private float _003Ctime_003E5__2;

			private float _003Cduration_003E5__3;

			private AnimationCurve _003Ccurve_003E5__4;

			private Match3Board _003Cboard_003E5__5;

			private TransformBehaviour _003CchipBeh_003E5__6;

			private float _003CholdGravityDuration_003E5__7;

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
				DestroyChipActionGrow destroyChipActionGrow = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003Ctime_003E5__2 = 0f;
					_003Cduration_003E5__3 = destroyChipActionGrow.settings.duration;
					_003Ccurve_003E5__4 = destroyChipActionGrow.settings.animationCurve;
					_003Cboard_003E5__5 = destroyChipActionGrow.slot.game.board;
					_003CchipBeh_003E5__6 = destroyChipActionGrow.chip.GetComponentBehaviour<TransformBehaviour>();
					_003CholdGravityDuration_003E5__7 = destroyChipActionGrow.settings.holdGravityDuration;
					break;
				case 1:
					_003C_003E1__state = -1;
					break;
				}
				if (_003Ctime_003E5__2 < _003Cduration_003E5__3)
				{
					_003Ctime_003E5__2 += _003Cboard_003E5__5.currentDeltaTime;
					if (destroyChipActionGrow.chipLock != null && _003Ctime_003E5__2 >= _003CholdGravityDuration_003E5__7)
					{
						destroyChipActionGrow.chipLock.UnlockAll();
						destroyChipActionGrow.chipLock = null;
					}
					float num2 = Mathf.InverseLerp(0f, _003Cduration_003E5__3, _003Ctime_003E5__2);
					if (_003Ccurve_003E5__4 != null)
					{
						num2 = _003Ccurve_003E5__4.Evaluate(num2);
					}
					float num3 = Mathf.LerpUnclamped(destroyChipActionGrow.settings.fromScale, destroyChipActionGrow.settings.toScale, num2);
					float alpha = Mathf.Lerp(destroyChipActionGrow.settings.fromAlpha, destroyChipActionGrow.settings.toAlpha, num2);
					if (_003CchipBeh_003E5__6 != null)
					{
						_003CchipBeh_003E5__6.localScale = new Vector3(num3, num3, 1f);
						_003CchipBeh_003E5__6.SetAlpha(alpha);
					}
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				if (destroyChipActionGrow.chipLock != null)
				{
					destroyChipActionGrow.chipLock.UnlockAll();
				}
				destroyChipActionGrow.chip.RemoveFromGame();
				destroyChipActionGrow.isAlive = false;
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

		private IEnumerator animation;

		private Chip chip;

		private Slot slot;

		private Lock chipLock;

		public Settings settings => Match3Settings.instance.destroyActionGrowSettings;

		public void Init(Chip chip, Slot slot)
		{
			this.chip = chip;
			this.slot = slot;
			if (settings.holdGravityOnChip)
			{
				chipLock = lockContainer.NewLock();
				chipLock.isSlotGravitySuspended = true;
				chipLock.LockSlot(slot);
			}
			slot.light.AddLight(settings.lightIntensity);
			TransformBehaviour componentBehaviour = chip.GetComponentBehaviour<TransformBehaviour>();
			if (componentBehaviour != null)
			{
				componentBehaviour.SetSortingLayer(settings.sortingLayer);
			}
		}

		public override void OnStart(ActionManager manager)
		{
			base.OnStart(manager);
		}

		private IEnumerator DoAnimation()
		{
			return new _003CDoAnimation_003Ed__9(0)
			{
				_003C_003E4__this = this
			};
		}

		public override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			if (isAlive)
			{
				if (animation == null)
				{
					animation = DoAnimation();
				}
				animation.MoveNext();
			}
		}
	}
}
