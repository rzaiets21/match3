using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace GGMatch3
{
	public class HammerHitAction : BoardAction
	{
		public struct InitArguments
		{
			public Match3Game game;

			public PowerupPlacementHandler.PlacementCompleteArguments completeArguments;
		}

		private sealed class _003CDoAnimation_003Ed__4 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public HammerHitAction _003C_003E4__this;

			private Match3Game _003Cgame_003E5__2;

			private HammerAnimationBehaviour _003ChammerBehaviour_003E5__3;

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
			public _003CDoAnimation_003Ed__4(int _003C_003E1__state)
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
				HammerHitAction hammerHitAction = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003Cgame_003E5__2 = hammerHitAction.initArguments.game;
					_003Cgame_003E5__2.Play(GGSoundSystem.SFXType.HammerStart);
					_003Cgame_003E5__2.board.powerupAnimationsInProgress++;
					_003ChammerBehaviour_003E5__3 = _003Cgame_003E5__2.CreateHammerAnimationBehaviour();
					if (_003ChammerBehaviour_003E5__3 != null)
					{
						_003ChammerBehaviour_003E5__3.Init(hammerHitAction.initArguments.completeArguments.initArguments.powerup.type);
						Slot targetSlot = hammerHitAction.initArguments.completeArguments.targetSlot;
						_003ChammerBehaviour_003E5__3.transform.localPosition = targetSlot.localPositionOfCenter;
					}
					goto IL_00ca;
				case 1:
					_003C_003E1__state = -1;
					goto IL_00ca;
				case 2:
					{
						_003C_003E1__state = -1;
						break;
					}
					IL_00ca:
					if (!(_003ChammerBehaviour_003E5__3 == null) && !(_003ChammerBehaviour_003E5__3.animationTime >= _003ChammerBehaviour_003E5__3.timeWhenHammerHit) && !(_003ChammerBehaviour_003E5__3.animationNormalizedTime >= 1f))
					{
						_003C_003E2__current = null;
						_003C_003E1__state = 1;
						return true;
					}
					hammerHitAction.ActivatePowerup();
					_003Cgame_003E5__2.board.powerupAnimationsInProgress--;
					break;
				}
				if (!(_003ChammerBehaviour_003E5__3 == null) && !(_003ChammerBehaviour_003E5__3.animationNormalizedTime >= 1f))
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 2;
					return true;
				}
				if (_003ChammerBehaviour_003E5__3 != null)
				{
					_003ChammerBehaviour_003E5__3.RemoveFromGame();
				}
				hammerHitAction.isAlive = false;
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

		private InitArguments initArguments;

		private IEnumerator animationAction;

		public void Init(InitArguments initArguments)
		{
			this.initArguments = initArguments;
		}

		private IEnumerator DoAnimation()
		{
			return new _003CDoAnimation_003Ed__4(0)
			{
				_003C_003E4__this = this
			};
		}

		private void ActivatePowerup()
		{
			Match3Game game = initArguments.game;
			PowerupPlacementHandler.PlacementCompleteArguments completeArguments = initArguments.completeArguments;
			PowerupsDB.PowerupDefinition powerup = completeArguments.initArguments.powerup;
			Slot targetSlot = completeArguments.targetSlot;
			if (powerup.type == PowerupType.Hammer)
			{
				SlotDestroyParams slotDestroyParams = new SlotDestroyParams();
				slotDestroyParams.isNeigbourDestroySuspended = true;
				slotDestroyParams.isHavingCarpet = (game.board.carpet.isCarpetPossible && !targetSlot.isBlockingCarpetSpread);
				targetSlot.OnDestroySlot(slotDestroyParams);
				game.particles.CreateParticles(targetSlot, Match3Particles.PositionType.OnHammerHit);
				game.Play(GGSoundSystem.SFXType.HammerHit);
			}
			else if (powerup.type == PowerupType.PowerHammer)
			{
				FlyCrossRocketAction flyCrossRocketAction = new FlyCrossRocketAction();
				FlyCrossRocketAction.FlyParams flyParams = default(FlyCrossRocketAction.FlyParams);
				flyParams.game = game;
				flyParams.prelockAll = true;
				flyParams.originPosition = targetSlot.position;
				flyParams.rows = 1;
				flyParams.columns = 1;
				flyParams.useDelayBetweenRowsAndColumns = false;
				flyCrossRocketAction.Init(flyParams);
				game.board.actionManager.AddAction(flyCrossRocketAction);
				game.particles.CreateParticles(targetSlot, Match3Particles.PositionType.OnHammerPowerHit);
				game.Play(GGSoundSystem.SFXType.PowerHammerHit);
			}
		}

		public override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			if (animationAction == null)
			{
				animationAction = DoAnimation();
			}
			animationAction.MoveNext();
		}
	}
}
