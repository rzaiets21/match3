using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace GGMatch3
{
	public class DestroyChipAction : BoardAction
	{
		[Serializable]
		public class Settings
		{
			public bool useChipAnimation;

			public float fromScale = 2f;

			public float toScale = 1f;

			public float duration;

			public AnimationCurve animationCurve;

			public float fromAlpha = 1f;

			public float toAlpha = 1f;

			public bool holdGravityOnChip;

			public float holdGravityDuration;

			public float lightIntensity = 0.6f;

			public float brightness = 1f;

			public bool useRocketDestroySettings;
		}

		public struct InitArguments
		{
			public Chip chip;

			public Slot slot;

			public SlotDestroyParams destroyParams;

			public bool isFromRocket
			{
				get
				{
					if (destroyParams == null)
					{
						return false;
					}
					if (destroyParams.isHitByBomb)
					{
						if (destroyParams.bombType != ChipType.HorizontalRocket)
						{
							return destroyParams.bombType == ChipType.VerticalRocket;
						}
						return true;
					}
					return false;
				}
			}
		}

		private sealed class _003CDoAnimation_003Ed__11 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public DestroyChipAction _003C_003E4__this;

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
			public _003CDoAnimation_003Ed__11(int _003C_003E1__state)
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
				DestroyChipAction destroyChipAction = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
				{
					_003C_003E1__state = -1;
					_003Ctime_003E5__2 = 0f;
					_003Cduration_003E5__3 = destroyChipAction.settings.duration;
					_003Ccurve_003E5__4 = destroyChipAction.settings.animationCurve;
					_003Cboard_003E5__5 = destroyChipAction.slot.game.board;
					_003CchipTransform_003E5__6 = destroyChipAction.chip.GetComponentBehaviour<TransformBehaviour>();
					Match3Game game = destroyChipAction.slot.game;
					_003CholdGravityDuration_003E5__7 = destroyChipAction.settings.holdGravityDuration;
					ChipBehaviour componentBehaviour = destroyChipAction.chip.GetComponentBehaviour<ChipBehaviour>();
					GameObject gameObject = null;
					gameObject = ((!destroyChipAction.initArguments.isFromRocket) ? game.particles.CreateParticles(destroyChipAction.chip, Match3Particles.PositionType.OnDestroyChip, destroyChipAction.chip.chipType, destroyChipAction.chip.itemColor) : game.particles.CreateParticles(destroyChipAction.chip, Match3Particles.PositionType.OnDestroyChipRocket, destroyChipAction.chip.chipType, destroyChipAction.chip.itemColor));
					if (componentBehaviour != null)
					{
						componentBehaviour.SetBrightness(destroyChipAction.settings.brightness);
						if (destroyChipAction.settings.useChipAnimation)
						{
							componentBehaviour.StartChipDestroyAnimation(gameObject);
						}
					}
					break;
				}
				case 1:
					_003C_003E1__state = -1;
					break;
				}
				if (_003Ctime_003E5__2 < _003Cduration_003E5__3)
				{
					_003Ctime_003E5__2 += _003Cboard_003E5__5.currentDeltaTime;
					if (destroyChipAction.chipLock != null && _003Ctime_003E5__2 >= _003CholdGravityDuration_003E5__7)
					{
						destroyChipAction.chipLock.UnlockAll();
						destroyChipAction.chipLock = null;
					}
					float num2 = Mathf.InverseLerp(0f, _003Cduration_003E5__3, _003Ctime_003E5__2);
					if (_003Ccurve_003E5__4 != null)
					{
						num2 = _003Ccurve_003E5__4.Evaluate(num2);
					}
					float num3 = Mathf.LerpUnclamped(destroyChipAction.settings.fromScale, destroyChipAction.settings.toScale, num2);
					float alpha = Mathf.Lerp(destroyChipAction.settings.fromAlpha, destroyChipAction.settings.toAlpha, num2);
					if (_003CchipTransform_003E5__6 != null)
					{
						_003CchipTransform_003E5__6.localScale = new Vector3(num3, num3, 1f);
						_003CchipTransform_003E5__6.SetAlpha(alpha);
					}
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				if (destroyChipAction.chipLock != null)
				{
					destroyChipAction.chipLock.UnlockAll();
				}
				destroyChipAction.chip.RemoveFromGame();
				destroyChipAction.isAlive = false;
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

		private InitArguments initArguments;

		public Settings settings
		{
			get
			{
				Settings settings = Match3Settings.instance.destroyActionSettings;
				if (settings.useRocketDestroySettings && initArguments.isFromRocket)
				{
					settings = Match3Settings.instance.destroyActionSettingsRocket;
				}
				return settings;
			}
		}

		public void Init(InitArguments initArguments)
		{
			this.initArguments = initArguments;
			chip = initArguments.chip;
			slot = initArguments.slot;
			if (settings.holdGravityOnChip)
			{
				chipLock = lockContainer.NewLock();
				chipLock.isSlotGravitySuspended = true;
				chipLock.LockSlot(slot);
			}
			slot.light.AddLight(settings.lightIntensity);
			Match3Game game = slot.game;
		}

		public override void OnStart(ActionManager manager)
		{
			base.OnStart(manager);
		}

		private IEnumerator DoAnimation()
		{
			return new _003CDoAnimation_003Ed__11(0)
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
