using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class Chip : SlotComponent
	{
		public int chipTag;

		public bool hasGrowingElement;

		private TransformBehaviour growingElementGraphics;

		private ChipPhysics physics = new ChipPhysics();

		private TeleporterAnimation teleportAnimation = new TeleporterAnimation();

		public ChipType chipType;

		public ItemColor itemColor;

		public int itemLevel;

		public ChipJumpBehaviour jumpBehaviour;

		private WobbleAnimation wobbleAnimation = new WobbleAnimation();

		public int carriesCoins;

		private bool _003CisFeatherShown_003Ek__BackingField;

		public override int sortingOrder => 10;

		public override bool isPlaceBubbleSuspended
		{
			get
			{
				if (chipType != 0)
				{
					return chipType != ChipType.Bomb;
				}
				return false;
			}
		}

		public override bool isPreventingReplaceByOtherChips => chipType != ChipType.Chip;

		public int moreMovesCount
		{
			get
			{
				if (chipType != ChipType.MoreMovesChip)
				{
					return 0;
				}
				return itemLevel + 1;
			}
		}

		public bool canBeTappedToActivate
		{
			get
			{
				if (chipType != ChipType.HorizontalRocket && chipType != ChipType.VerticalRocket && chipType != ChipType.Bomb)
				{
					return chipType == ChipType.SeekingMissle;
				}
				return true;
			}
		}

		public bool isRocket
		{
			get
			{
				if (chipType != ChipType.HorizontalRocket)
				{
					return chipType == ChipType.VerticalRocket;
				}
				return true;
			}
		}

		public bool isPowerup
		{
			get
			{
				if (chipType != ChipType.HorizontalRocket && chipType != ChipType.VerticalRocket && chipType != ChipType.Bomb && chipType != ChipType.SeekingMissle)
				{
					return chipType == ChipType.DiscoBall;
				}
				return true;
			}
		}

		public override bool isAttachGrowingElementSuspended
		{
			get
			{
				if (isPreventingReplaceByOtherChips)
				{
					return true;
				}
				if (hasGrowingElement)
				{
					return true;
				}
				if (canFormColorMatches)
				{
					return false;
				}
				return true;
			}
		}

		public bool isPickupElement
		{
			get
			{
				if (chipType != ChipType.BunnyChip && chipType != ChipType.CookiePickup)
				{
					return chipType == ChipType.MoreMovesChip;
				}
				return true;
			}
		}

		public bool isStoppingRocket => chipType == ChipType.CookiePickup;

		public bool isFallingPickupElement => chipType == ChipType.FallingGingerbreadMan;

		public bool canFormColorMatches
		{
			get
			{
				if (chipType != 0)
				{
					return chipType == ChipType.MonsterChip;
				}
				return true;
			}
		}

		public override long lastMoveFrameIndex => Math.Max(physics.lastMoveFrameIndex, teleportAnimation.lastMoveFrame);

		public override float lastMoveTime => Mathf.Max(physics.lastMoveTime, teleportAnimation.lastMoveTime);

		public override bool isSlotSwapSuspended
		{
			get
			{
				if (!physics.isActive)
				{
					return teleportAnimation.isActive;
				}
				return true;
			}
		}

		public override bool isSlotMatchingSuspended
		{
			get
			{
				if (!physics.isActive)
				{
					return teleportAnimation.isActive;
				}
				return true;
			}
		}

		public override bool isMoving
		{
			get
			{
				if (!physics.isActive)
				{
					return teleportAnimation.isActive;
				}
				return true;
			}
		}

		public override bool isMoveByConveyorSuspended
		{
			get
			{
				if (!physics.isActive)
				{
					return teleportAnimation.isActive;
				}
				return true;
			}
		}

		public override bool isDestroyedByMatchingNextTo => isPickupElement;

		public override bool isSlotGravitySuspended
		{
			get
			{
				if (!physics.isActive)
				{
					return teleportAnimation.isActive;
				}
				return true;
			}
		}

		public override bool isMovingWithConveyor => true;

		public bool canBeDestroyed
		{
			get
			{
				if (isFallingPickupElement)
				{
					return false;
				}
				for (int i = 0; i < slotComponentLocks.Count; i++)
				{
					if (slotComponentLocks[i].isDestroySuspended)
					{
						return false;
					}
				}
				return true;
			}
		}

		public override bool canReactWithBomb => true;

		public override bool isMovedByGravity => true;

		public override bool isPreventingOtherChipsToFallIntoSlot => true;

		private MonsterElements.MonsterElementPieces pieceThatWillBeFedThisPiece
		{
			get
			{
				Match3Goals.ChipTypeDef chipTypeDef = Match3Goals.ChipTypeDef.Create(this);
				if (lastConnectedSlot == null)
				{
					return null;
				}
				return lastConnectedSlot.game.board.monsterElements.GetPieceThatNeedsFeeding(chipTypeDef);
			}
		}

		public bool isPartOfActiveGoal
		{
			get
			{
				if (lastConnectedSlot == null)
				{
					return false;
				}
				Match3Game game = lastConnectedSlot.game;
				Match3Goals.ChipTypeDef chipTypeDef = Match3Goals.ChipTypeDef.Create(this);
				if (!hasGrowingElement && pieceThatWillBeFedThisPiece != null)
				{
					return true;
				}
				return game.goals.GetActiveGoal(chipTypeDef) != null;
			}
		}

		public bool isFeatherShown => _003CisFeatherShown_003Ek__BackingField;

		public void DestroyGrowingElement()
		{
			if (hasGrowingElement)
			{
				if (growingElementGraphics != null)
				{
					UnityEngine.Object.Destroy(growingElementGraphics.gameObject);
					growingElementGraphics = null;
				}
				hasGrowingElement = false;
			}
		}

		public void AttachGrowingElement(TransformBehaviour growingElementGraphics)
		{
			hasGrowingElement = true;
			this.growingElementGraphics = growingElementGraphics;
			if (!(growingElementGraphics == null))
			{
				GetComponentBehaviour<TransformBehaviour>().AddChild(growingElementGraphics);
				growingElementGraphics.localScale = Vector3.one;
				growingElementGraphics.localPosition = Vector3.zero;
			}
		}

		public override void AddToGoalsAtStart(Match3Goals goals)
		{
			Match3Goals.ChipTypeDef chipTypeDef = Match3Goals.ChipTypeDef.Create(this);
			goals.GetChipTypeCounter(chipTypeDef).countAtStart++;
		}

		public void Init(ChipType chipType, ItemColor itemColor)
		{
			this.chipType = chipType;
			this.itemColor = itemColor;
			if (isPickupElement || isFallingPickupElement)
			{
				jumpBehaviour = new ChipJumpBehaviour();
				jumpBehaviour.Init(Match3Settings.instance.chipJumpSettings);
			}
			if (isPowerup)
			{
				jumpBehaviour = new ChipJumpBehaviour();
				if (chipType == ChipType.HorizontalRocket)
				{
					jumpBehaviour.Init(Match3Settings.instance.horizontalRocketJumpSettings);
				}
				if (chipType == ChipType.VerticalRocket)
				{
					jumpBehaviour.Init(Match3Settings.instance.verticalRocketJumpSettings);
				}
				if (chipType == ChipType.Bomb)
				{
					jumpBehaviour.Init(Match3Settings.instance.bombJumpSettings);
				}
				if (chipType == ChipType.DiscoBall)
				{
					jumpBehaviour.Init(Match3Settings.instance.discoBallJumpSettings);
				}
				if (chipType == ChipType.SeekingMissle)
				{
					jumpBehaviour.Init(Match3Settings.instance.seekingMissleJumpSettings);
				}
			}
		}

		public void SetTransformToMove(Transform t)
		{
		}

		public override bool IsCompatibleWithPickupGoal(Match3Goals.ChipTypeDef chipTypeDef)
		{
			if (hasGrowingElement)
			{
				Match3Goals.ChipTypeDef b = default(Match3Goals.ChipTypeDef);
				b.chipType = ChipType.GrowingElementPiece;
				b.itemColor = itemColor;
				if (chipTypeDef.IsEqual(b))
				{
					return true;
				}
			}
			return chipTypeDef.IsEqual(Match3Goals.ChipTypeDef.Create(this));
		}

		public override void OnMovedBySlotGravity(Slot fromSlot, Slot toSlot, MoveContentsToSlotParams moveParams)
		{
			base.OnMovedBySlotGravity(fromSlot, toSlot, moveParams);
			long currentFrameIndex = fromSlot.game.board.currentFrameIndex;
			Match3Board board = fromSlot.game.board;
			bool flag = true;
			float num = 0f;
			if (currentFrameIndex - teleportAnimation.lastMoveFrame <= 1)
			{
				num = Mathf.Min(teleportAnimation.currentSpeed, Match3Settings.instance.pipeSettings.maxContinueVelocity);
				flag = false;
			}
			if (currentFrameIndex - physics.lastMoveFrameIndex <= 1)
			{
				num = physics.speed;
				flag = false;
			}
			if (moveParams.isFromPortal)
			{
				IntVector2 intVector = toSlot.gravity.forceDirections[0];
				TeleporterAnimation.MoveParams mp = default(TeleporterAnimation.MoveParams);
				mp.chip = this;
				mp.game = lastConnectedSlot.game;
				mp.positionToMoveFrom = fromSlot.position;
				mp.directionToMoveFrom = fromSlot.gravity.forceDirections[0];
				mp.entrancePipe = fromSlot.entrancePipe;
				mp.exitPipe = toSlot.exitPipe;
				mp.positionToMoveTo = toSlot.position;
				mp.directionToMoveTo = toSlot.gravity.forceDirections[0];
				mp.initialSpeed = num;
				mp.currentFrameIndex = currentFrameIndex;
				mp.currentTime = board.currentTime;
				teleportAnimation.StartMove(mp);
			}
			else
			{
				physics.speed = num;
				physics.StartMove(fromSlot.localPositionOfCenter, toSlot.localPositionOfCenter, currentFrameIndex, board.currentTime);
			}
			if (flag)
			{
				SlotStartMoveParams startMoveParams = default(SlotStartMoveParams);
				startMoveParams.fromSlot = fromSlot;
				startMoveParams.toSlot = toSlot;
				startMoveParams.slotComponent = this;
				slot.OnSlotComponentMadeAStartMove(startMoveParams);
			}
			slot.OnSlotComponentMadeATransformChange(this);
		}

		public override void Wobble(WobbleAnimation.Settings settings)
		{
			if (settings != null)
			{
				wobbleAnimation.Init(settings, GetComponentBehaviour<TransformBehaviour>());
			}
		}

		public override void OnCreatedBySlot(Slot toSlot)
		{
			Match3Board board = toSlot.game.board;
			IntVector2 intVector = new IntVector2(0, 0);
			List<IntVector2> forceDirections = toSlot.gravity.forceDirections;
			for (int i = 0; i < forceDirections.Count; i++)
			{
				IntVector2 b = forceDirections[i];
				intVector += b;
			}
			physics.StartMove(toSlot.game.LocalPositionOfCenter(toSlot.position - intVector), toSlot.localPositionOfCenter, board.currentFrameIndex, board.currentTime);
			slot.OnSlotComponentMadeATransformChange(this);
		}

		public override void OnUpdate(float deltaTime)
		{
			if (slot == null)
			{
				return;
			}
			UpdateFeatherShow();
			if (jumpBehaviour != null)
			{
				jumpBehaviour.Update(this, deltaTime);
			}
			TransformBehaviour componentBehaviour = GetComponentBehaviour<TransformBehaviour>();
			if (componentBehaviour != null)
			{
				componentBehaviour.SetPartOfGoalActive(isPartOfActiveGoal);
			}
			wobbleAnimation.Update(deltaTime);
			TransformBehaviour componentBehaviour2 = GetComponentBehaviour<TransformBehaviour>();
			if (componentBehaviour2 != null)
			{
				componentBehaviour2.slotOffsetPosition = slot.offsetPosition;
				Vector3 offsetScale = slot.offsetScale;
				float num = 1f;
				GeneralSettings generalSettings = Match3Settings.instance.generalSettings;
				num = (isPowerup ? generalSettings.bombScaleMult : ((!isPickupElement) ? generalSettings.chipScaleMult : generalSettings.pickupScaleMult));
				offsetScale = Vector3.Scale(offsetScale, new Vector3(num, num, 1f));
				if (slot.isSlotGravitySuspendedByComponentOtherThan(this))
				{
					offsetScale = Vector3.Scale(offsetScale, new Vector3(generalSettings.trapScaleMult, generalSettings.trapScaleMult, 1f));
				}
				componentBehaviour2.slotLocalScale = offsetScale;
			}
			UpdateParams updateParams = default(UpdateParams);
			updateParams.deltaTime = deltaTime;
			while (true)
			{
				UpdateResult updateResult = default(UpdateResult);
				if (teleportAnimation.isActive)
				{
					updateParams.udpateIteration = 0f;
					do
					{
						updateResult = teleportAnimation.OnUpdate(updateParams);
						updateParams.udpateIteration += 1f;
						if (updateResult.wasTraveling && !teleportAnimation.isActive)
						{
							slot.ApplySlotGravity();
						}
					}
					while (!(updateResult.leftOverDeltaTime <= 0f) && teleportAnimation.isActive);
					if (updateResult.leftOverDeltaTime <= 0f)
					{
						break;
					}
					updateParams.deltaTime = updateResult.leftOverDeltaTime;
					updateParams.udpateIteration = 0f;
				}
				if (slot.isChipGravitySuspended)
				{
					return;
				}
				updateResult = physics.OnUpdate(updateParams);
				if (!physics.isActive && updateResult.wasTraveling)
				{
					slot.ApplySlotGravity();
				}
				if (updateResult.leftOverDeltaTime <= 0f)
				{
					break;
				}
				updateParams.udpateIteration += 1f;
				updateParams.deltaTime = updateResult.leftOverDeltaTime;
			}
			slot.OnSlotComponentMadeATransformChange(this);
			if (isFallingPickupElement && !slot.isSlotGravitySuspended && slot.isExitForFallingChip)
			{
				DoDestroyFallingPickupElement();
			}
		}

		public void UpdateFeatherShow()
		{
		}

		public override void OnAddedToSlot(Slot slot)
		{
			base.OnAddedToSlot(slot);
			physics.chip = this;
			TransformBehaviour componentBehaviour = GetComponentBehaviour<TransformBehaviour>();
			if (componentBehaviour != null)
			{
				componentBehaviour.SetPartOfGoalActive(isPartOfActiveGoal);
			}
		}

		public override SlotDestroyResolution OnDestroyNeighbourSlotComponent(Slot slotBeingDestroyed, SlotDestroyParams destroyParams)
		{
			SlotDestroyResolution result = default(SlotDestroyResolution);
			if (!canBeDestroyed)
			{
				return result;
			}
			if (!isDestroyedByMatchingNextTo)
			{
				return result;
			}
			if (isSlotMatchingSuspended)
			{
				return result;
			}
			if (destroyParams.isHitByBomb && !destroyParams.isBombAllowingNeighbourDestroy)
			{
				return result;
			}
			return RemoveLevelOnDestroyNeighbourSlotComponent(slotBeingDestroyed, destroyParams);
		}

		public void DestroyBomb(SlotDestroyParams destroyParams)
		{
			Slot lastConnectedSlot = base.lastConnectedSlot;
			Match3Game game = lastConnectedSlot.game;
			if (chipType == ChipType.DiscoBall)
			{
				DiscoBallDestroyAction.DiscoParams discoParams = new DiscoBallDestroyAction.DiscoParams();
				discoParams.replaceWithBombs = false;
				discoParams.InitWithItemColor(lastConnectedSlot, game, game.BestItemColorForDiscoBomb(replaceWithBombs: false), discoParams.replaceWithBombs);
				discoParams.originBomb = this;
				DiscoBallDestroyInstantAction discoBallDestroyInstantAction = new DiscoBallDestroyInstantAction();
				discoBallDestroyInstantAction.Init(discoParams);
				game.board.actionManager.AddAction(discoBallDestroyInstantAction);
			}
			else if (chipType == ChipType.SeekingMissle)
			{
				CollectPointsAction.OnChipDestroy(this, destroyParams);
				SeekingMissileAction seekingMissileAction = new SeekingMissileAction();
				SeekingMissileAction.Parameters parameters = new SeekingMissileAction.Parameters();
				parameters.game = lastConnectedSlot.game;
				parameters.startSlot = lastConnectedSlot;
				parameters.doCrossExplosion = true;
				parameters.isHavingCarpet = destroyParams.isHavingCarpet;
				seekingMissileAction.Init(parameters);
				lastConnectedSlot.game.board.actionManager.AddAction(seekingMissileAction);
				RemoveFromGame();
			}
			else if (chipType == ChipType.Bomb)
			{
				CollectPointsAction.OnChipDestroy(this, destroyParams);
				ExplosionAction explosionAction = new ExplosionAction();
				ExplosionAction.ExplosionSettings settings = default(ExplosionAction.ExplosionSettings);
				settings.position = lastConnectedSlot.position;
				settings.radius = 3;
				settings.bombChip = this;
				settings.isUsingBombAreaOfEffect = true;
				settings.isHavingCarpet = destroyParams.isHavingCarpet;
				explosionAction.Init(lastConnectedSlot.game, settings);
				lastConnectedSlot.game.board.actionManager.AddAction(explosionAction);
			}
			else if (chipType == ChipType.HorizontalRocket || chipType == ChipType.VerticalRocket)
			{
				CollectPointsAction.OnChipDestroy(this, destroyParams);
				FlyLineRocketAction flyLineRocketAction = new FlyLineRocketAction();
				FlyLineRocketAction.Params flyParams = default(FlyLineRocketAction.Params);
				flyParams.game = lastConnectedSlot.game;
				flyParams.bombChip = this;
				flyParams.position = lastConnectedSlot.position;
				flyParams.prelock = true;
				flyParams.isHavingCarpet = destroyParams.isHavingCarpet;
				flyParams.canUseScale = (destroyParams.isFromSwap || destroyParams.isFromTap);
				flyParams.rocketType = chipType;
				flyParams.swapParams = destroyParams.swapParams;
				flyLineRocketAction.Init(flyParams);
				lastConnectedSlot.game.board.actionManager.AddAction(flyLineRocketAction);
			}
		}

		public void RemoveFromGameWithPickupGoal(SlotDestroyParams destroyParams)
		{
			if (lastConnectedSlot == null)
			{
				RemoveFromGame();
				return;
			}
			Match3Game game = lastConnectedSlot.game;
			RemoveFromSlot();
			Match3Goals.GoalBase goalBase = null;
			if (hasGrowingElement)
			{
				Match3Goals.ChipTypeDef chipTypeDef = default(Match3Goals.ChipTypeDef);
				chipTypeDef.chipType = ChipType.GrowingElementPiece;
				chipTypeDef.itemColor = itemColor;
				goalBase = game.goals.GetActiveGoal(chipTypeDef);
			}
			if (goalBase == null)
			{
				Match3Goals.ChipTypeDef chipTypeDef2 = Match3Goals.ChipTypeDef.Create(this);
				goalBase = game.goals.GetActiveGoal(chipTypeDef2);
			}
			if (goalBase != null)
			{
				CollectGoalAction collectGoalAction = new CollectGoalAction();
				CollectGoalAction.CollectGoalParams collectParams = default(CollectGoalAction.CollectGoalParams);
				collectParams.chip = this;
				collectParams.chipSlot = slot;
				collectParams.game = game;
				collectParams.goal = goalBase;
				collectParams.isExplosion = false;
				collectParams.destroyParams = destroyParams;
				collectGoalAction.Init(collectParams);
				game.board.actionManager.AddAction(collectGoalAction);
			}
			else
			{
				RemoveFromGame();
			}
		}

		public override SlotDestroyResolution OnDestroySlotComponent(SlotDestroyParams destroyParams)
		{
			SlotDestroyResolution result = default(SlotDestroyResolution);
			if (!canBeDestroyed)
			{
				return result;
			}
			if (destroyParams.isHitByBomb && !canReactWithBomb)
			{
				return result;
			}
			Slot lastConnectedSlot = base.lastConnectedSlot;
			Match3Game game = lastConnectedSlot.game;
			if (isPickupElement)
			{
				return RemoveLevelOnDestroy(destroyParams);
			}
			bool isPowerup = this.isPowerup;
			lastConnectedSlot?.RemoveComponent(this);
			if (destroyParams.isFromTap)
			{
				game.OnUserMadeMove();
			}
			Match3Goals.ChipTypeDef chipTypeDef = Match3Goals.ChipTypeDef.Create(this);
			Match3Goals.GoalBase goalBase = game.goals.GetActiveGoal(chipTypeDef);
			Match3Goals.GoalBase goalBase2 = null;
			
			//game.goals.OnPickupGoal(goalBase);
			if (hasGrowingElement)
			{
				Match3Goals.ChipTypeDef chipTypeDef2 = default(Match3Goals.ChipTypeDef);
				chipTypeDef2.chipType = ChipType.GrowingElementPiece;
				chipTypeDef2.itemColor = itemColor;
				goalBase2 = game.goals.GetActiveGoal(chipTypeDef2);
			}
			if (goalBase == null)
			{
				goalBase = goalBase2;
			}
			bool flag = goalBase != null;
			MonsterElements.MonsterElementPieces pieceThatWillBeFedThisPiece = this.pieceThatWillBeFedThisPiece;
			if (!isPowerup)
			{
				CollectPointsAction.OnChipDestroy(this, destroyParams);
			}
			if (carriesCoins > 0)
			{
				float num = 0.15f;
				for (int i = 0; i < carriesCoins; i++)
				{
					CollectCoinAction.InitArguments initArguments = default(CollectCoinAction.InitArguments);
					initArguments.game = game;
					initArguments.chipSlot = lastConnectedSlot;
					initArguments.delay = num * (float)i;
					CollectCoinAction collectCoinAction = new CollectCoinAction();
					collectCoinAction.Init(initArguments);
					game.board.actionManager.AddAction(collectCoinAction);
				}
			}
			if (!hasGrowingElement && pieceThatWillBeFedThisPiece != null)
			{
				CollectGoalAction collectGoalAction = new CollectGoalAction();
				CollectGoalAction.CollectGoalParams collectParams = default(CollectGoalAction.CollectGoalParams);
				collectParams.chip = this;
				collectParams.chipSlot = lastConnectedSlot;
				collectParams.game = game;
				collectParams.monsterToFeed = pieceThatWillBeFedThisPiece;
				collectParams.isExplosion = destroyParams.isExplosion;
				collectParams.explosionCentre = destroyParams.explosionCentre;
				collectParams.destroyParams = destroyParams;
				collectGoalAction.Init(collectParams);
				game.board.actionManager.AddAction(collectGoalAction);
			}
			else if (isPowerup)
			{
				if (destroyParams.isHitByBomb && destroyParams.bombType == chipType && isRocket)
				{
					if (chipType == ChipType.HorizontalRocket)
					{
						chipType = ChipType.VerticalRocket;
					}
					else if (chipType == ChipType.VerticalRocket)
					{
						chipType = ChipType.HorizontalRocket;
					}
					SolidPieceRenderer componentBehaviour = GetComponentBehaviour<SolidPieceRenderer>();
					if (componentBehaviour != null)
					{
						componentBehaviour.Init(chipType);
					}
				}
				
				CollectGoalAction collectGoalAction2 = new CollectGoalAction();
				CollectGoalAction.CollectGoalParams collectGoalParams = default(CollectGoalAction.CollectGoalParams);
				collectGoalParams.chip = null;
				collectGoalParams.moveTransform = null;
				collectGoalParams.chipSlot = null;
				collectGoalParams.smallScale = true;
				collectGoalParams.game = game;
				collectGoalParams.goal = goalBase;
				collectGoalParams.isExplosion = false;
				
				collectGoalParams.destroyParams = destroyParams;
				collectGoalAction2.Init(collectGoalParams);
				game.board.actionManager.AddAction(collectGoalAction2);
				
				if (destroyParams.activationDelay > 0f)
				{
					DestroyAfterAction.InitArguments initArguments2 = default(DestroyAfterAction.InitArguments);
					initArguments2.chip = this;
					initArguments2.slot = lastConnectedSlot;
					initArguments2.delay = destroyParams.activationDelay;
					initArguments2.game = game;
					initArguments2.destroyParams = destroyParams;
					DestroyAfterAction destroyAfterAction = new DestroyAfterAction();
					destroyAfterAction.Init(initArguments2);
					game.board.actionManager.AddAction(destroyAfterAction);
				}
				else if (destroyParams.isHitByBomb && destroyParams.bombType != ChipType.DiscoBall && Match3Settings.instance.generalSettings.waitIfRocketHitsPowerup)
				{
					DestroyAfterAction.InitArguments initArguments3 = default(DestroyAfterAction.InitArguments);
					initArguments3.chip = this;
					initArguments3.slot = lastConnectedSlot;
					initArguments3.game = game;
					initArguments3.destroyParams = destroyParams;
					DestroyAfterAction destroyAfterAction2 = new DestroyAfterAction();
					destroyAfterAction2.Init(initArguments3);
					game.board.actionManager.AddAction(destroyAfterAction2);
				}
				else
				{
					DestroyBomb(destroyParams);
				}
			}
			else if (chipType == ChipType.DiscoBall || chipType == ChipType.SeekingMissle || chipType == ChipType.Bomb || chipType == ChipType.HorizontalRocket || chipType == ChipType.VerticalRocket)
			{
				CollectGoalAction collectGoalAction2 = new CollectGoalAction();
				CollectGoalAction.CollectGoalParams collectGoalParams = default(CollectGoalAction.CollectGoalParams);
				collectGoalParams.chip = null;
				collectGoalParams.moveTransform = null;
				collectGoalParams.chipSlot = null;
				collectGoalParams.smallScale = true;
				collectGoalParams.game = game;
				collectGoalParams.goal = goalBase;
				collectGoalParams.isExplosion = false;
				
				collectGoalParams.destroyParams = destroyParams;
				collectGoalAction2.Init(collectGoalParams);
				game.board.actionManager.AddAction(collectGoalAction2);

				if (chipType == ChipType.DiscoBall)
				{
					DiscoBallDestroyAction.DiscoParams discoParams = new DiscoBallDestroyAction.DiscoParams();
					discoParams.replaceWithBombs = false;
					discoParams.InitWithItemColor(lastConnectedSlot, game,
						game.BestItemColorForDiscoBomb(replaceWithBombs: false), discoParams.replaceWithBombs);
					discoParams.originBomb = this;
					DiscoBallDestroyInstantAction discoBallDestroyInstantAction = new DiscoBallDestroyInstantAction();
					discoBallDestroyInstantAction.Init(discoParams);
					game.board.actionManager.AddAction(discoBallDestroyInstantAction);
				}
				else if (chipType == ChipType.SeekingMissle)
				{
					SeekingMissileAction seekingMissileAction = new SeekingMissileAction();
					SeekingMissileAction.Parameters parameters = new SeekingMissileAction.Parameters();
					parameters.game = lastConnectedSlot.game;
					parameters.startSlot = lastConnectedSlot;
					parameters.doCrossExplosion = true;
					parameters.isHavingCarpet = destroyParams.isHavingCarpet;
					seekingMissileAction.Init(parameters);
					lastConnectedSlot.game.board.actionManager.AddAction(seekingMissileAction);
					RemoveFromGame();
				}
				else if (chipType == ChipType.Bomb)
				{
					ExplosionAction explosionAction = new ExplosionAction();
					ExplosionAction.ExplosionSettings settings = default(ExplosionAction.ExplosionSettings);
					settings.position = lastConnectedSlot.position;
					settings.radius = 3;
					settings.bombChip = this;
					settings.isUsingBombAreaOfEffect = true;
					settings.isHavingCarpet = destroyParams.isHavingCarpet;
					explosionAction.Init(lastConnectedSlot.game, settings);
					lastConnectedSlot.game.board.actionManager.AddAction(explosionAction);
				}
				else if (chipType == ChipType.HorizontalRocket || chipType == ChipType.VerticalRocket)
				{
					FlyLineRocketAction flyLineRocketAction = new FlyLineRocketAction();
					FlyLineRocketAction.Params flyParams = default(FlyLineRocketAction.Params);
					flyParams.game = lastConnectedSlot.game;
					flyParams.bombChip = this;
					flyParams.position = lastConnectedSlot.position;
					flyParams.prelock = true;
					flyParams.isHavingCarpet = destroyParams.isHavingCarpet;
					flyParams.canUseScale = (destroyParams.isFromSwap || destroyParams.isFromTap);
					flyParams.rocketType = chipType;
					flyParams.swapParams = destroyParams.swapParams;
					if (destroyParams.isHitByBomb && destroyParams.bombType == chipType)
					{
						if (chipType == ChipType.HorizontalRocket)
						{
							flyParams.rocketType = ChipType.VerticalRocket;
						}
						else
						{
							flyParams.rocketType = ChipType.HorizontalRocket;
						}
					}

					flyLineRocketAction.Init(flyParams);
					lastConnectedSlot.game.board.actionManager.AddAction(flyLineRocketAction);
				}
			}
			else if (flag)
			{
				if (destroyParams.isCreatingPowerupFromThisMatch)
				{
					destroyParams.AddChipForPowerupCreateAnimation(this);
				}
				else if (chipType == ChipType.Chip && isFeatherShown)
				{
					DestroyChipAction destroyChipAction = new DestroyChipAction();
					DestroyChipAction.InitArguments initArguments4 = default(DestroyChipAction.InitArguments);
					initArguments4.chip = this;
					initArguments4.slot = lastConnectedSlot;
					initArguments4.destroyParams = destroyParams;
					destroyChipAction.Init(initArguments4);
					lastConnectedSlot.game.board.actionManager.AddAction(destroyChipAction);
					CollectGoalAction collectGoalAction2 = new CollectGoalAction();
					CollectGoalAction.CollectGoalParams collectGoalParams = default(CollectGoalAction.CollectGoalParams);
					collectGoalParams.chip = null;
					collectGoalParams.moveTransform = game.CreateChipFeather(lastConnectedSlot, itemColor);
					collectGoalParams.chipSlot = lastConnectedSlot;
					collectGoalParams.smallScale = true;
					collectGoalParams.game = game;
					collectGoalParams.goal = goalBase;
					collectGoalParams.isExplosion = destroyParams.isExplosion;
					if (!collectGoalParams.isExplosion)
					{
						collectGoalParams.explosionCentre = lastConnectedSlot.position + IntVector2.down;
					}
					collectGoalParams.destroyParams = destroyParams;
					collectGoalAction2.Init(collectGoalParams);
					game.board.actionManager.AddAction(collectGoalAction2);
				}
				else
				{
					CollectGoalAction collectGoalAction3 = new CollectGoalAction();
					CollectGoalAction.CollectGoalParams collectParams2 = default(CollectGoalAction.CollectGoalParams);
					collectParams2.chip = this;
					collectParams2.chipSlot = lastConnectedSlot;
					collectParams2.game = game;
					collectParams2.goal = goalBase;
					collectParams2.isExplosion = destroyParams.isExplosion;
					collectParams2.explosionCentre = destroyParams.explosionCentre;
					collectParams2.destroyParams = destroyParams;
					collectGoalAction3.Init(collectParams2);
					game.board.actionManager.AddAction(collectGoalAction3);
				}
			}
			else if (destroyParams.isCreatingPowerupFromThisMatch)
			{
				destroyParams.AddChipForPowerupCreateAnimation(this);
			}
			else if (destroyParams.isHitByBomb && destroyParams.isExplosion)
			{
				DestroyChipActionExplosion destroyChipActionExplosion = new DestroyChipActionExplosion();
				destroyChipActionExplosion.Init(this, lastConnectedSlot, destroyParams.explosionCentre, destroyParams);
				lastConnectedSlot.game.board.actionManager.AddAction(destroyChipActionExplosion);
			}
			else if (destroyParams.isFromSwap || destroyParams.isHitByBomb)
			{
				DestroyChipAction destroyChipAction2 = new DestroyChipAction();
				DestroyChipAction.InitArguments initArguments5 = default(DestroyChipAction.InitArguments);
				initArguments5.chip = this;
				initArguments5.slot = lastConnectedSlot;
				initArguments5.destroyParams = destroyParams;
				destroyChipAction2.Init(initArguments5);
				lastConnectedSlot.game.board.actionManager.AddAction(destroyChipAction2);
			}
			else
			{
				DestroyChipAction destroyChipAction3 = new DestroyChipAction();
				DestroyChipAction.InitArguments initArguments6 = default(DestroyChipAction.InitArguments);
				initArguments6.chip = this;
				initArguments6.slot = lastConnectedSlot;
				initArguments6.destroyParams = destroyParams;
				destroyChipAction3.Init(initArguments6);
				lastConnectedSlot.game.board.actionManager.AddAction(destroyChipAction3);
			}
			if (isPowerup && (destroyParams.isFromTap || destroyParams.isFromSwap))
			{
				game.board.AddMatch();
			}
			return result;
		}

		private void DoDestroyFallingPickupElement()
		{
			Slot slot = base.slot;
			if (slot != null)
			{
				Match3Game game = slot.game;
				RemoveFromSlot();
				game.extraFallingChips.OnFallingElementPickup(this);
				Match3Goals.ChipTypeDef chipTypeDef = Match3Goals.ChipTypeDef.Create(this);
				Match3Goals.GoalBase activeGoal = game.goals.GetActiveGoal(chipTypeDef);
				CollectGoalAction collectGoalAction = new CollectGoalAction();
				CollectGoalAction.CollectGoalParams collectParams = default(CollectGoalAction.CollectGoalParams);
				collectParams.chip = this;
				collectParams.chipSlot = slot;
				collectParams.game = game;
				collectParams.goal = activeGoal;
				collectParams.isExplosion = false;
				collectGoalAction.Init(collectParams);
				game.board.actionManager.AddAction(collectGoalAction);
				game.Play(GGSoundSystem.SFXType.GingerbreadManRescue);
			}
		}

		private SlotDestroyResolution RemoveLevelOnDestroyNeighbourSlotComponent(Slot slotBeingDestroyed, SlotDestroyParams destroyParams)
		{
			SlotDestroyResolution result = default(SlotDestroyResolution);
			if (isSlotMatchingSuspended || isSlotGravitySuspended)
			{
				return result;
			}
			result.isDestroyed = true;
			DoDestroyLevel(slotBeingDestroyed, destroyParams);
			return result;
		}

		private SlotDestroyResolution RemoveLevelOnDestroy(SlotDestroyParams destroyParams)
		{
			SlotDestroyResolution result = default(SlotDestroyResolution);
			result.isDestroyed = true;
			result.stopPropagation = true;
			if (isStoppingRocket)
			{
				destroyParams.isRocketStopped = true;
			}
			DoDestroyLevel(lastConnectedSlot, destroyParams);
			return result;
		}

		private void PickupMoreMovesChip(Slot slotBeingDestroyed, SlotDestroyParams destroyParams)
		{
			Slot lastConnectedSlot = base.lastConnectedSlot;
			lastConnectedSlot?.RemoveComponent(this);
			Match3Game game = lastConnectedSlot.game;
			CollectGoalAction collectGoalAction = new CollectGoalAction();
			CollectGoalAction.CollectGoalParams collectGoalParams = default(CollectGoalAction.CollectGoalParams);
			collectGoalParams.chip = this;
			collectGoalParams.chipSlot = lastConnectedSlot;
			collectGoalParams.game = game;
			collectGoalParams.goal = null;
			collectGoalParams.collectMoreMovesCount = moreMovesCount;
			collectGoalParams.isExplosion = destroyParams.isExplosion;
			if (collectGoalParams.isExplosion)
			{
				collectGoalParams.explosionCentre = destroyParams.explosionCentre;
			}
			else if (destroyParams.matchIsland != null && destroyParams.matchIsland.allSlots.Count > 0)
			{
				collectGoalParams.explosionCentre = destroyParams.matchIsland.allSlots[0].position;
			}
			collectGoalParams.destroyParams = destroyParams;
			collectGoalAction.Init(collectGoalParams);
			game.board.actionManager.AddAction(collectGoalAction);
		}

		private void DoDestroyLevel(Slot slotBeingDestroyed, SlotDestroyParams destroyParams)
		{
			if (chipType == ChipType.MoreMovesChip)
			{
				PickupMoreMovesChip(slotBeingDestroyed, destroyParams);
				return;
			}
			itemLevel--;
			MultiLayerItemBehaviour componentBehaviour = GetComponentBehaviour<MultiLayerItemBehaviour>();
			if (componentBehaviour != null)
			{
				componentBehaviour.SetLayerIndex(itemLevel);
			}
			if (itemLevel >= 0)
			{
				return;
			}
			Slot lastConnectedSlot = base.lastConnectedSlot;
			lastConnectedSlot?.RemoveComponent(this);
			Match3Game game = lastConnectedSlot.game;
			Match3Goals.ChipTypeDef chipTypeDef = Match3Goals.ChipTypeDef.Create(this);
			Match3Goals.GoalBase activeGoal = game.goals.GetActiveGoal(chipTypeDef);
			if (activeGoal != null)
			{
				CollectGoalAction collectGoalAction = new CollectGoalAction();
				CollectGoalAction.CollectGoalParams collectGoalParams = default(CollectGoalAction.CollectGoalParams);
				collectGoalParams.chip = this;
				collectGoalParams.chipSlot = lastConnectedSlot;
				collectGoalParams.game = game;
				collectGoalParams.goal = activeGoal;
				collectGoalParams.isExplosion = destroyParams.isExplosion;
				if (collectGoalParams.isExplosion)
				{
					collectGoalParams.explosionCentre = destroyParams.explosionCentre;
				}
				else if (destroyParams.matchIsland != null && destroyParams.matchIsland.allSlots.Count > 0)
				{
					collectGoalParams.explosionCentre = destroyParams.matchIsland.allSlots[0].position;
				}
				collectGoalParams.destroyParams = destroyParams;
				collectGoalAction.Init(collectGoalParams);
				game.board.actionManager.AddAction(collectGoalAction);
			}
			else
			{
				DestroyFromGravityAction destroyFromGravityAction = new DestroyFromGravityAction();
				destroyFromGravityAction.Init(this, lastConnectedSlot);
				lastConnectedSlot.game.board.actionManager.AddAction(destroyFromGravityAction);
			}
		}
		
	}
}
