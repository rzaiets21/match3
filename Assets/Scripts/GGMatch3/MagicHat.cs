using System;
using UnityEngine;

namespace GGMatch3
{
	public class MagicHat : SlotComponent
	{
		[Serializable]
		public class Settings
		{
			public float durationForBunnyUp;

			public Vector3 bunnyOffsetIn;

			public Vector3 bunnyOffsetOut;

			public Vector3 bunnyScaleIn;

			public Vector3 bunnyScaleOut;

			public AnimationCurve positionCurve;

			public AnimationCurve scaleCurve;

			public float delayUp;
		}

		private MagicHatBehaviour hatBehaviour;

		private float timeSinceBunnyCollected = 100f;

		public override int sortingOrder => 10;

		private Settings settings => Match3Settings.instance.magicHatSettingsBomb;

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

		public void Init(MagicHatBehaviour hatBehaviour)
		{
			this.hatBehaviour = hatBehaviour;
			UpdateBunny();
		}

		public override bool IsAvailableForDiscoBombSuspended(bool replaceWithBombs)
		{
			return true;
		}

		public override bool IsCompatibleWithPickupGoal(Match3Goals.ChipTypeDef chipTypeDef)
		{
			return chipTypeDef.chipType == ChipType.BunnyChip;
		}

		public override SlotDestroyResolution OnDestroyNeighbourSlotComponent(Slot slotBeingDestroyed, SlotDestroyParams destroyParams)
		{
			SlotDestroyResolution result = default(SlotDestroyResolution);
			if (destroyParams.isHitByBomb && !destroyParams.isBombAllowingNeighbourDestroy)
			{
				return result;
			}
			result.isDestroyed = true;
			result.stopPropagation = true;
			CreateBunny(slotBeingDestroyed, destroyParams);
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
			result.stopPropagation = true;
			result.isNeigbourDestroySuspendedForThisChipOnly = true;
			CreateBunny(slot, destroyParams);
			return result;
		}

		private void CreateBunny(Slot slotBeingDestroyed, SlotDestroyParams destroyParams)
		{
			Slot lastConnectedSlot = base.lastConnectedSlot;
			Match3Game game = base.lastConnectedSlot.game;
			Chip chip = game.CreateCharacterInSlot(lastConnectedSlot, ChipType.BunnyChip, 0);
			chip.RemoveFromSlot();
			Match3Goals.ChipTypeDef chipTypeDef = Match3Goals.ChipTypeDef.Create(chip);
			Match3Goals.GoalBase activeGoal = game.goals.GetActiveGoal(chipTypeDef);
			CollectGoalAction collectGoalAction = new CollectGoalAction();
			CollectGoalAction.CollectGoalParams collectParams = default(CollectGoalAction.CollectGoalParams);
			collectParams.chip = chip;
			collectParams.chipSlot = lastConnectedSlot;
			collectParams.game = game;
			collectParams.goal = activeGoal;
			collectParams.isMagicHat = true;
			collectParams.explosionCentre = lastConnectedSlot.position + IntVector2.down;
			collectParams.destroyParams = destroyParams;
			collectGoalAction.Init(collectParams);
			game.board.actionManager.AddAction(collectGoalAction);
			game.particles.CreateParticles(lastConnectedSlot.localPositionOfCenter, Match3Particles.PositionType.MagicHatCreate, ChipType.MagicHat, ItemColor.Unknown);
			timeSinceBunnyCollected = 0f;
			game.Play(GGSoundSystem.SFXType.BunnyOutOfHat);
		}

		private void UpdateBunny()
		{
			if (!(hatBehaviour == null))
			{
				Settings settings = this.settings;
				float time = Mathf.InverseLerp(0f, settings.durationForBunnyUp, timeSinceBunnyCollected - settings.delayUp);
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
			timeSinceBunnyCollected += deltaTime;
			UpdateBunny();
		}
	}
}
