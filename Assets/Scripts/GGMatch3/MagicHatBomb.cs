using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class MagicHatBomb : SlotComponent
	{
		private MagicHatBehaviour hatBehaviour;

		private float timeSinceMissleLaunched = 100f;

		private int lastBombLaunchedMove = -1;

		private float timeSinceUp;

		private int bombsFired;

		private bool hasBombsCount = true;

		private int bombsCount = 3;
		public int BombsCount => bombsCount;

		private ChipType chipType = ChipType.MagicHatSeekingMissle;
		public ChipType ChipType => chipType;

		public override int sortingOrder => 10;

		private MagicHat.Settings settings => Match3Settings.instance.magicHatSettingsBomb;

		public override bool isSlotMatchingSuspended => true;

		public override bool isAttachGrowingElementSuspended => true;

		public override bool isPlaceBubbleSuspended => true;

		public override bool isMoveIntoSlotSuspended => true;

		public override bool isSlotGravitySuspended => true;

		public override bool isSlotSwapSuspended => true;

		public override bool isPreventingGravity => true;

		public override bool isCreatePowerupWithThisSlotSuspended => true;

		public override bool isMovingWithConveyor => true;

		public override bool isDestroyedByMatchingNextTo => true;

		private bool isUp
		{
			get
			{
				if (timeSinceMissleLaunched >= settings.delayUp)
				{
					return true;
				}
				if (lastConnectedSlot != null)
				{
					if (lastConnectedSlot.game.board.lastSettledMove <= lastBombLaunchedMove)
					{
						return false;
					}
					return true;
				}
				return false;
			}
		}

		public void Init(MagicHatBehaviour hatBehaviour, int itemCount, ChipType chipType)
		{
			this.hatBehaviour = hatBehaviour;
			this.chipType = chipType;
			if (itemCount <= 0)
			{
				hasBombsCount = false;
			}
			bombsCount = itemCount;
			if (hatBehaviour != null)
			{
				hatBehaviour.Init(chipType);
				hatBehaviour.SetCountActive(hasBombsCount);
				hatBehaviour.SetCount(bombsCount - bombsFired);
			}
			UpdateRocket();
		}

		public override bool IsAvailableForDiscoBombSuspended(bool replaceWithBombs)
		{
			return true;
		}

		public override bool IsCompatibleWithPickupGoal(Match3Goals.ChipTypeDef chipTypeDef)
		{
			return false;
		}

		public override SlotDestroyResolution OnDestroyNeighbourSlotComponent(Slot slotBeingDestroyed, SlotDestroyParams destroyParams)
		{
			SlotDestroyResolution result = default(SlotDestroyResolution);
			if (destroyParams.isHitByBomb && !destroyParams.isBombAllowingNeighbourDestroy)
			{
				return result;
			}
			if (!isUp)
			{
				return result;
			}
			result.isDestroyed = true;
			result.stopPropagation = true;
			FireRocket(slotBeingDestroyed, destroyParams);
			return result;
		}

		public override SlotDestroyResolution OnDestroySlotComponent(SlotDestroyParams destroyParams)
		{
			SlotDestroyResolution result = default(SlotDestroyResolution);
			result.isDestroyed = true;
			if (destroyParams.isFromTap)
			{
				result.stopPropagation = true;
				return result;
			}
			if (!isUp)
			{
				return result;
			}
			if (destroyParams.isHitByBomb && destroyParams.bombType == ChipType.SeekingMissle)
			{
				return result;
			}
			result.stopPropagation = true;
			result.isNeigbourDestroySuspendedForThisChipOnly = true;
			FireRocket(slot, destroyParams);
			return result;
		}

		private void FireRocket(Slot slotBeingDestroyed, SlotDestroyParams destroyParams)
		{
			Slot lastConnectedSlot = base.lastConnectedSlot;
			Match3Game game = base.lastConnectedSlot.game;
			List<Slot> list = game.goals.BestSlotsForSeekingMissle(game, lastConnectedSlot);
			if (list == null && list.Count <= 0)
			{
				return;
			}
			bombsFired++;
			lastBombLaunchedMove = game.board.lastSettledMove;
			if (chipType == ChipType.MagicHatSeekingMissle)
			{
				SeekingMissileAction.Parameters parameters = new SeekingMissileAction.Parameters();
				parameters.doCrossExplosion = false;
				parameters.game = game;
				parameters.startSlot = lastConnectedSlot;
				SeekingMissileAction seekingMissileAction = new SeekingMissileAction();
				seekingMissileAction.Init(parameters);
				game.board.actionManager.AddAction(seekingMissileAction);
			}
			else if (chipType == ChipType.MagicHatBomb || chipType == ChipType.MagicHatRocket)
			{
				ChipType powerup = ChipType.Bomb;
				if (chipType == ChipType.MagicHatRocket)
				{
					powerup = ((game.RandomRange(0, 100) > 50) ? ChipType.HorizontalRocket : ChipType.VerticalRocket);
				}
				Slot slotThatCanBeReplacedWithPowerup = PlacePowerupAction.GetSlotThatCanBeReplacedWithPowerup(game, powerup);
				if (slotThatCanBeReplacedWithPowerup != null)
				{
					AnimateCarryPiece animateCarryPiece = new AnimateCarryPiece();
					AnimateCarryPiece.InitArguments initArguments = default(AnimateCarryPiece.InitArguments);
					initArguments.chipType = powerup;
					initArguments.destinationSlot = slotThatCanBeReplacedWithPowerup;
					initArguments.originPosition = lastConnectedSlot.position;
					initArguments.game = game;
					animateCarryPiece.Init(initArguments);
					game.board.actionManager.AddAction(animateCarryPiece);
				}
			}
			game.particles.CreateParticles(lastConnectedSlot.localPositionOfCenter, Match3Particles.PositionType.MagicHatCreate, ChipType.MagicHat, ItemColor.Unknown);
			timeSinceMissleLaunched = 0f;
			timeSinceUp = 0f;
			game.Play(GGSoundSystem.SFXType.BunnyOutOfHat);
			if (hatBehaviour != null)
			{
				hatBehaviour.SetCount(bombsCount - bombsFired);
			}
			if (hasBombsCount && bombsFired >= bombsCount)
			{
				RemoveFromGame();
			}
		}

		private void UpdateRocket()
		{
			if (!(hatBehaviour == null))
			{
				MagicHat.Settings settings = this.settings;
				float time = Mathf.InverseLerp(0f, settings.durationForBunnyUp, timeSinceUp);
				float t = settings.positionCurve.Evaluate(time);
				float t2 = settings.scaleCurve.Evaluate(time);
				Vector3 bunnyOffset = Vector3.LerpUnclamped(settings.bunnyOffsetIn, settings.bunnyOffsetOut, t);
				Vector3 bunnyScale = Vector3.LerpUnclamped(settings.bunnyScaleIn, settings.bunnyScaleOut, t2);
				hatBehaviour.bunnyOffset = bunnyOffset;
				hatBehaviour.bunnyScale = bunnyScale;
			}
		}

		public override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			timeSinceMissleLaunched += deltaTime;
			if (isUp)
			{
				timeSinceUp += deltaTime;
			}
			else
			{
				timeSinceUp = 0f;
			}
			UpdateRocket();
		}
	}
}
