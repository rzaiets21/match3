using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace GGMatch3
{
	public class DestroyFromGravityAction : BoardAction
	{
		[Serializable]
		public class Settings
		{
			public float fromScale = 2f;

			public float toScale = 1f;

			public float fromAlpha = 1f;

			public float toAlpha = 1f;

			public float duration;

			public AnimationCurve animationCurve;

			public bool holdGravityOnChip;

			public float holdGravityDuration;

			public float lightIntensity;

			public bool activateParticles;
		}

		private sealed class _003CDoAnimation_003Ed__9 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public DestroyFromGravityAction _003C_003E4__this;

			private float _003Ctime_003E5__2;

			private float _003Cduration_003E5__3;

			private AnimationCurve _003Ccurve_003E5__4;

			private Match3Board _003Cboard_003E5__5;

			private TransformBehaviour _003CchipTransform_003E5__6;

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
				DestroyFromGravityAction destroyFromGravityAction = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003Ctime_003E5__2 = 0f;
					_003Cduration_003E5__3 = destroyFromGravityAction.settings.duration;
					_003Ccurve_003E5__4 = destroyFromGravityAction.settings.animationCurve;
					_003Cboard_003E5__5 = destroyFromGravityAction.slot.game.board;
					_003CchipTransform_003E5__6 = destroyFromGravityAction.chip.GetComponentBehaviour<TransformBehaviour>();
					_003CholdGravityDuration_003E5__7 = destroyFromGravityAction.settings.holdGravityDuration;
					break;
				case 1:
					_003C_003E1__state = -1;
					break;
				}
				if (_003Ctime_003E5__2 < _003Cduration_003E5__3)
				{
					_003Ctime_003E5__2 += _003Cboard_003E5__5.currentDeltaTime;
					if (destroyFromGravityAction.chipLock != null && _003Ctime_003E5__2 >= _003CholdGravityDuration_003E5__7)
					{
						destroyFromGravityAction.chipLock.UnlockAll();
						destroyFromGravityAction.chipLock = null;
					}
					float num2 = Mathf.InverseLerp(0f, _003Cduration_003E5__3, _003Ctime_003E5__2);
					if (_003Ccurve_003E5__4 != null)
					{
						num2 = _003Ccurve_003E5__4.Evaluate(num2);
					}
					float num3 = Mathf.LerpUnclamped(destroyFromGravityAction.settings.fromScale, destroyFromGravityAction.settings.toScale, num2);
					float alpha = Mathf.Lerp(destroyFromGravityAction.settings.fromAlpha, destroyFromGravityAction.settings.toAlpha, num2);
					if (_003CchipTransform_003E5__6 != null)
					{
						_003CchipTransform_003E5__6.localScale = new Vector3(num3, num3, 1f);
						_003CchipTransform_003E5__6.SetAlpha(alpha);
					}
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				if (destroyFromGravityAction.chipLock != null)
				{
					destroyFromGravityAction.chipLock.UnlockAll();
				}
				destroyFromGravityAction.chip.RemoveFromGame();
				destroyFromGravityAction.isAlive = false;
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

		public Settings settings => Match3Settings.instance.destroyFromGravityAction;

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
			if (settings.activateParticles)
			{
				slot.game.particles.CreateParticles(chip, Match3Particles.PositionType.OnDestroyChip, chip.chipType, chip.itemColor);
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
