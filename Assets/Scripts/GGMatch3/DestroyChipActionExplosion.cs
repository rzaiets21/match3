using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace GGMatch3
{
	public class DestroyChipActionExplosion : BoardAction
	{
		[Serializable]
		public class Settings
		{
			public bool useParticles;

			public float fromScale = 2f;

			public float duration;

			public float distance = 10f;

			public AnimationCurve animationCurve;

			public bool useScaleCurve;

			public AnimationCurve scaleCurve;

			public bool holdGravityOnChip;

			public float holdGravityDuration;

			public float lightIntensity = 0.6f;

			public SpriteSortingSettings sorting = new SpriteSortingSettings();
		}

		private sealed class _003CDoAnimation_003Ed__13 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public DestroyChipActionExplosion _003C_003E4__this;

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
			public _003CDoAnimation_003Ed__13(int _003C_003E1__state)
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
				DestroyChipActionExplosion destroyChipActionExplosion = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003Ctime_003E5__2 = 0f;
					_003Cduration_003E5__3 = destroyChipActionExplosion.settings.duration;
					_003Ccurve_003E5__4 = destroyChipActionExplosion.settings.animationCurve;
					_003Cboard_003E5__5 = destroyChipActionExplosion.slot.game.board;
					_003CchipTransform_003E5__6 = destroyChipActionExplosion.chip.GetComponentBehaviour<TransformBehaviour>();
					_003CholdGravityDuration_003E5__7 = destroyChipActionExplosion.settings.holdGravityDuration;
					break;
				case 1:
					_003C_003E1__state = -1;
					break;
				}
				if (_003Ctime_003E5__2 < _003Cduration_003E5__3)
				{
					_003Ctime_003E5__2 += _003Cboard_003E5__5.currentDeltaTime;
					if (destroyChipActionExplosion.chipLock != null && _003Ctime_003E5__2 >= _003CholdGravityDuration_003E5__7)
					{
						destroyChipActionExplosion.chipLock.UnlockAll();
						destroyChipActionExplosion.chipLock = null;
					}
					float num2 = Mathf.InverseLerp(0f, _003Cduration_003E5__3, _003Ctime_003E5__2);
					float num3 = num2;
					if (_003Ccurve_003E5__4 != null)
					{
						num3 = _003Ccurve_003E5__4.Evaluate(num2);
					}
					float t = num3;
					if (destroyChipActionExplosion.settings.useScaleCurve)
					{
						t = destroyChipActionExplosion.settings.scaleCurve.Evaluate(num2);
					}
					float num4 = Mathf.LerpUnclamped(destroyChipActionExplosion.settings.fromScale, 0f, t);
					if (_003CchipTransform_003E5__6 != null)
					{
						_003CchipTransform_003E5__6.scalerTransform.localScale = new Vector3(num4, num4, 1f);
						_003CchipTransform_003E5__6.localPosition = Vector3.LerpUnclamped(destroyChipActionExplosion.startPosition, destroyChipActionExplosion.endPosition, num3);
					}
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				if (destroyChipActionExplosion.chipLock != null)
				{
					destroyChipActionExplosion.chipLock.UnlockAll();
				}
				destroyChipActionExplosion.chip.RemoveFromGame();
				destroyChipActionExplosion.isAlive = false;
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

		private IntVector2 bombOriginPostion;

		private Vector3 direction;

		private Vector3 startPosition;

		private Vector3 endPosition;

		public Settings settings => Match3Settings.instance.destroyChipActionExplosionSettings;

		public void Init(Chip chip, Slot slot, IntVector2 bombOriginPostion, SlotDestroyParams slotDestroyParams)
		{
			this.bombOriginPostion = bombOriginPostion;
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
			Vector3 a = chip.lastConnectedSlot.localPositionOfCenter;
			if (componentBehaviour != null)
			{
				componentBehaviour.SetSortingLayer(settings.sorting);
				a = componentBehaviour.localPosition;
			}
			direction = a - slot.game.LocalPositionOfCenter(bombOriginPostion);
			direction.z = 0f;
			direction.Normalize();
			startPosition = a;
			endPosition = startPosition + direction * settings.distance;
			Match3Game game = slot.game;
			if (settings.useParticles)
			{
				if (slotDestroyParams.bombType == ChipType.DiscoBall)
				{
					game.particles.CreateParticles(slot, Match3Particles.PositionType.OnDestroyChipDiscoBomb);
				}
				else
				{
					game.particles.CreateParticles(chip, Match3Particles.PositionType.OnDestroyChipExplosion, chip.chipType, chip.itemColor);
				}
			}
		}

		public override void OnStart(ActionManager manager)
		{
			base.OnStart(manager);
		}

		private IEnumerator DoAnimation()
		{
			return new _003CDoAnimation_003Ed__13(0)
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
