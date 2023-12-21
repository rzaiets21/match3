using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GGMatch3
{
	public class Slot
	{
		public struct PositionIntegrator
		{
			public Vector3 currentPosition;

			public Vector3 prevPosition;

			public Vector3 acceleration;

			public float time;

			public void ResetAcceleration()
			{
				acceleration = Vector3.zero;
			}

			public void SetPosition(Vector3 position)
			{
				currentPosition = (prevPosition = position);
			}

			public void Update(float deltaTime, float dampingFactor, float stiffness)
			{
				float num = 0.0166666675f;
				time += deltaTime;
				while (time >= num)
				{
					time -= num;
					FixedUpdate(num, dampingFactor, stiffness);
				}
			}

			private void FixedUpdate(float fixedTime, float dampingFactor, float stiffness)
			{
				Vector3 a = currentPosition - prevPosition;
				prevPosition = currentPosition;
				currentPosition += a * (1f - dampingFactor) + acceleration * fixedTime * fixedTime;
				Vector3 a2 = currentPosition;
				float magnitude = a2.magnitude;
				Vector3 vector = stiffness * magnitude * 0.5f * a2;
				currentPosition -= vector;
			}
		}

		public enum MoveToSlotType
		{
			Gravity,
			Sandflow,
			Portal,
			Jump
		}

		public struct MoveToSlot
		{
			public MoveToSlotType type;

			public Slot slot;

			public MoveToSlot(MoveToSlotType type, Slot slot)
			{
				this.type = type;
				this.slot = slot;
			}
		}

		public class StatsToBottom
		{
			public int emptySpaces;

			public int movingChips;

			public int totalDepth;

			public Chip firstChipBelow;

			public List<Slot> pathSlots = new List<Slot>();

			public List<ItemColor> availableColors = new List<ItemColor>();

			public Slot GetPathSlot(int placesToGoDown)
			{
				return pathSlots[Mathf.Clamp(placesToGoDown, 0, pathSlots.Count - 1)];
			}

			public void Fill(Slot firstSlot)
			{
				Clear();
				GetStatsToBottom(firstSlot, this);
			}

			public void Clear()
			{
				pathSlots.Clear();
				availableColors.Clear();
				emptySpaces = 0;
				movingChips = 0;
				totalDepth = 0;
				firstChipBelow = null;
			}

			public void TryAddChip(Chip firstChip)
			{
				if (firstChipBelow == null)
				{
					firstChipBelow = firstChip;
				}
			}
		}

		public bool wasRenderedForChocolateLastFrame;

		public Match3Game game;

		public IntVector2 position;

		public PipeBehaviour entrancePipe;

		public PipeBehaviour exitPipe;

		[NonSerialized]
		public GeneratorSetup generatorSetup;

		[NonSerialized]
		public GeneratorSlotSettings generatorSlotSettings;

		private bool isMaxDistanceToEndSet;

		private int _003CmaxDistanceToEnd_003Ek__BackingField;

		public Gravity gravity;

		public bool isExitForFallingChip;

		public PositionIntegrator positionIntegrator;

		public Vector3 prevOffsetPosition;

		public Vector3 offsetPosition;

		public Vector3 offsetScale = Vector3.one;

		public List<Slot> portalDestinationSlots = new List<Slot>();

		public List<Slot> jumpOriginSlots = new List<Slot>();

		public List<Slot> jumpDestinationSlots = new List<Slot>();

		public List<Slot> incomingGravitySlots = new List<Slot>();

		public List<SlotComponent> components = new List<SlotComponent>();

		private List<Lock> slotLocks = new List<Lock>();

		private List<SetLock> setSlotLocks = new List<SetLock>();

		public bool canGenerateChip;

		private int generatedFallingElements;

		public LevelDefinition.GeneratorSettings generatorSettings;

		public LightSlotComponent backLight;

		private List<MoveToSlot> allMoveToSlots_ = new List<MoveToSlot>();

		private List<SlotComponent> componentsToRemove = new List<SlotComponent>();

		private List<Slot> neigbourSlots_ = new List<Slot>();

		private bool isNeigbourSlotsListSet;

		private List<SlotComponent> tempSlotComponentsList = new List<SlotComponent>();

		private StatsToBottom statsToBottom = new StatsToBottom();

		private List<ItemColor> selectedColors = new List<ItemColor>();

		private int maxSelectedColorCount = 10;

		[NonSerialized]
		public int createdChips;

		public bool isBackgroundPatternActive => (position.x + position.y) % 2 == 0;

		public Vector2 normalizedPositionWithinBoard => new Vector3((float)position.x / (float)game.board.size.x, (float)position.y / (float)game.board.size.y);

		public int maxDistanceToEnd
		{
			get
			{
				return _003CmaxDistanceToEnd_003Ek__BackingField;
			}
			protected set
			{
				_003CmaxDistanceToEnd_003Ek__BackingField = value;
			}
		}

		public Vector3 localPositionOfCenter => game.LocalPositionOfCenter(position);

		public int LockCount => slotLocks.Count;

		public LightSlotComponent light => GetSlotComponent<LightSlotComponent>();

		public bool isPreventingOtherChipsToFallIntoSlot
		{
			get
			{
				for (int i = 0; i < components.Count; i++)
				{
					if (components[i].isPreventingOtherChipsToFallIntoSlot)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool isEmpty
		{
			get
			{
				for (int i = 0; i < components.Count; i++)
				{
					if (components[i].isPreventingOtherChipsToFallIntoSlot)
					{
						return false;
					}
				}
				return true;
			}
		}

		public bool isDestroyedByMatchingNextTo
		{
			get
			{
				for (int i = 0; i < components.Count; i++)
				{
					if (components[i].isDestroyedByMatchingNextTo)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool isReacheableByGeneratorOrChip => IsReacheableByGeneratorOrChip(this);

		public bool isMovingElementRequired
		{
			get
			{
				for (int i = 0; i < components.Count; i++)
				{
					if (components[i].isMovingElementRequired)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool isLockedForDiscoBomb
		{
			get
			{
				for (int i = 0; i < slotLocks.Count; i++)
				{
					if (slotLocks[i].isAvailableForDiscoBombSuspended)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool isAttachGrowingElementSuspended
		{
			get
			{
				for (int i = 0; i < slotLocks.Count; i++)
				{
					if (slotLocks[i].isAttachGrowingElementSuspended)
					{
						return true;
					}
				}
				for (int j = 0; j < components.Count; j++)
				{
					if (components[j].isAttachGrowingElementSuspended)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool hasCarpet => game.board.carpet.HasCarpet(position);

		public bool isBlockingCarpetSpread
		{
			get
			{
				for (int i = 0; i < components.Count; i++)
				{
					if (components[i].isBlockingCarpetSpread)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool canCarpetSpreadFromHere
		{
			get
			{
				if (!hasCarpet)
				{
					return false;
				}
				for (int i = 0; i < components.Count; i++)
				{
					if (components[i].isBlockingCarpetSpread)
					{
						return false;
					}
				}
				return true;
			}
		}

		public bool isCreatePowerupWithThisSlotSuspended
		{
			get
			{
				for (int i = 0; i < components.Count; i++)
				{
					if (components[i].isCreatePowerupWithThisSlotSuspended)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool canBeTappedToActivate => GetSlotComponent<Chip>()?.canBeTappedToActivate ?? false;

		public bool isTapToActivateSuspended
		{
			get
			{
				if (!isSlotMatchingSuspended)
				{
					return isSlotGravitySuspended;
				}
				return true;
			}
		}

		public bool isChipGeneratorSuspended
		{
			get
			{
				for (int i = 0; i < slotLocks.Count; i++)
				{
					if (slotLocks[i].isChipGeneratorSuspended)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool isMoveByConveyorSuspended
		{
			get
			{
				if (canGenerateChip && !isSomethingMoveableByGravityInSlot && !isPreventingOtherChipsToFallIntoSlot)
				{
					return true;
				}
				for (int i = 0; i < components.Count; i++)
				{
					if (components[i].isMoveByConveyorSuspended)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool isBlockForGravity
		{
			get
			{
				for (int i = 0; i < components.Count; i++)
				{
					SlotComponent slotComponent = components[i];
					if (!(slotComponent is Chip) && slotComponent.isSlotGravitySuspended)
					{
						return true;
					}
				}
				for (int j = 0; j < slotLocks.Count; j++)
				{
					if (slotLocks[j].isSlotGravitySuspended)
					{
						return true;
					}
				}
				return false;
			}
		}

		public int totalBlockerLevelForFalling
		{
			get
			{
				int num = 0;
				for (int i = 0; i < components.Count; i++)
				{
					SlotComponent slotComponent = components[i];
					if (slotComponent.isBlockingChip)
					{
						num += slotComponent.blockerLevel;
					}
				}
				return num;
			}
		}

		public int maxBlockerLevel
		{
			get
			{
				int num = 0;
				for (int i = 0; i < components.Count; i++)
				{
					SlotComponent slotComponent = components[i];
					num = Mathf.Max(num, slotComponent.blockerLevel);
				}
				return num;
			}
		}

		public bool isBlockingBurriedElement
		{
			get
			{
				for (int i = 0; i < components.Count; i++)
				{
					if (components[i].isBlockingBurriedElement)
					{
						return true;
					}
				}
				return false;
			}
		}

		public int totalBlockerLevel
		{
			get
			{
				int num = 0;
				for (int i = 0; i < components.Count; i++)
				{
					SlotComponent slotComponent = components[i];
					num += slotComponent.blockerLevel;
				}
				return num;
			}
		}

		public bool isMoveIntoSlotSuspended
		{
			get
			{
				for (int i = 0; i < components.Count; i++)
				{
					if (components[i].isMoveIntoSlotSuspended)
					{
						return true;
					}
				}
				for (int j = 0; j < slotLocks.Count; j++)
				{
					if (slotLocks[j].isSlotGravitySuspended)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool isSlotGravitySuspendedByComponent
		{
			get
			{
				for (int i = 0; i < components.Count; i++)
				{
					if (components[i].isSlotGravitySuspended)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool isMoving
		{
			get
			{
				for (int i = 0; i < components.Count; i++)
				{
					if (components[i].isMoving)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool isSlotGravitySuspended
		{
			get
			{
				for (int i = 0; i < components.Count; i++)
				{
					if (components[i].isSlotGravitySuspended)
					{
						return true;
					}
				}
				for (int j = 0; j < slotLocks.Count; j++)
				{
					if (slotLocks[j].isSlotGravitySuspended)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool isPlaceBubbleSuspended
		{
			get
			{
				for (int i = 0; i < components.Count; i++)
				{
					if (components[i].isPlaceBubbleSuspended)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool isDestroySuspended
		{
			get
			{
				for (int i = 0; i < slotLocks.Count; i++)
				{
					if (slotLocks[i].isDestroySuspended)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool isChipGravitySuspended
		{
			get
			{
				for (int i = 0; i < slotLocks.Count; i++)
				{
					if (slotLocks[i].isChipGravitySuspended)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool isSlotSwapSuspended
		{
			get
			{
				for (int i = 0; i < components.Count; i++)
				{
					if (components[i].isSlotSwapSuspended)
					{
						return true;
					}
				}
				if (!isSlotMatchingSuspended)
				{
					return isSlotGravitySuspended;
				}
				return true;
			}
		}

		public bool isPowerupReplacementSuspended
		{
			get
			{
				for (int i = 0; i < slotLocks.Count; i++)
				{
					if (slotLocks[i].isPowerupReplacementSuspended)
					{
						return true;
					}
				}
				for (int j = 0; j < components.Count; j++)
				{
					if (components[j].isPowerupReplacementSuspended)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool isSlotMatchingSuspended
		{
			get
			{
				for (int i = 0; i < slotLocks.Count; i++)
				{
					if (slotLocks[i].isSlotMatchingSuspended)
					{
						return true;
					}
				}
				for (int j = 0; j < components.Count; j++)
				{
					if (components[j].isSlotMatchingSuspended)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool isSlotTouchingSuspended
		{
			get
			{
				for (int i = 0; i < slotLocks.Count; i++)
				{
					if (slotLocks[i].isSlotTouchingSuspended)
					{
						return true;
					}
				}
				for (int j = 0; j < components.Count; j++)
				{
					if (components[j].isSlotTapSuspended)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool isSlotSwipingSuspended
		{
			get
			{
				for (int i = 0; i < slotLocks.Count; i++)
				{
					if (slotLocks[i].isSlotSwipeSuspended)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool isPreventingGravity
		{
			get
			{
				for (int i = 0; i < components.Count; i++)
				{
					if (components[i].isPreventingGravity)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool isSomethingMoveableByGravityInSlot
		{
			get
			{
				for (int i = 0; i < components.Count; i++)
				{
					if (components[i].isMovedByGravity)
					{
						return true;
					}
				}
				return false;
			}
		}

		public float lastMoveTime => GetSlotComponent<Chip>()?.lastMoveTime ?? 0f;

		public long lastMoveFrameIndex => GetSlotComponent<Chip>()?.lastMoveFrameIndex ?? 0;

		protected List<MoveToSlot> allMoveToSlots
		{
			get
			{
				allMoveToSlots_.Clear();
				List<IntVector2> forceDirections = gravity.forceDirections;
				for (int i = 0; i < forceDirections.Count; i++)
				{
					IntVector2 intVector = forceDirections[i];
					IntVector2 intVector2 = position + intVector;
					Slot slot = game.GetSlot(intVector2);
					if (slot != null && !IsBlockingPath(intVector) && !slot.IsBlockingPath(-intVector))
					{
						allMoveToSlots_.Add(new MoveToSlot(MoveToSlotType.Gravity, slot));
					}
				}
				for (int j = 0; j < portalDestinationSlots.Count; j++)
				{
					Slot slot2 = portalDestinationSlots[j];
					if (slot2 != null)
					{
						IntVector2 direction = forceDirections[0];
						IntVector2 direction2 = -slot2.gravity.forceDirections[0];
						if (!IsBlockingPath(direction) && !slot2.IsBlockingPath(direction2))
						{
							allMoveToSlots_.Add(new MoveToSlot(MoveToSlotType.Portal, slot2));
						}
					}
				}
				for (int k = 0; k < jumpDestinationSlots.Count; k++)
				{
					Slot slot3 = jumpDestinationSlots[k];
					if (slot3 != null)
					{
						IntVector2 intVector3 = forceDirections[0];
						if (!IsBlockingPath(intVector3) && !slot3.IsBlockingPath(-intVector3))
						{
							allMoveToSlots_.Add(new MoveToSlot(MoveToSlotType.Jump, slot3));
						}
					}
				}
				List<Gravity.SandflowDirection> sandflowDirections = gravity.sandflowDirections;
				for (int l = 0; l < sandflowDirections.Count; l++)
				{
					Gravity.SandflowDirection sandflowDirection = sandflowDirections[l];
					IntVector2 intVector4 = position + sandflowDirection.offset;
					Slot slot4 = game.GetSlot(intVector4);
					if (slot4 != null)
					{
						IntVector2 intVector5 = position + sandflowDirection.direction;
						Slot slot5 = game.GetSlot(intVector5);
						IntVector2 intVector6 = position + sandflowDirection.forceDirection;
						Slot slot6 = game.GetSlot(intVector6);
						if ((!IsPathBlockedBetween(this, slot5) || !IsPathBlockedBetween(this, slot6) || (!IsWallBetween(game, position, intVector5) && !IsWallBetween(game, position, intVector6))) && (!IsPathBlockedBetween(slot5, slot4) || !IsPathBlockedBetween(slot6, slot4) || (!IsWallBetween(game, intVector5, intVector4) && !IsWallBetween(game, intVector6, intVector4))) && (!IsPathBlockedBetween(slot5, slot4) || !IsPathBlockedBetween(this, slot6) || (!IsWallBetween(game, intVector5, intVector4) && !IsWallBetween(game, position, intVector6))) && (!IsPathBlockedBetween(this, slot5) || !IsPathBlockedBetween(slot4, slot6) || (!IsWallBetween(game, position, intVector5) && !IsWallBetween(game, intVector4, intVector6))))
						{
							allMoveToSlots_.Add(new MoveToSlot(MoveToSlotType.Sandflow, slot4));
						}
					}
				}
				return allMoveToSlots_;
			}
		}

		public List<Slot> neigbourSlots
		{
			get
			{
				if (isNeigbourSlotsListSet)
				{
					return neigbourSlots_;
				}
				List<Slot> list = neigbourSlots_;
				list.Clear();
				for (int i = position.x - 1; i <= position.x + 1; i++)
				{
					for (int j = position.y - 1; j <= position.y + 1; j++)
					{
						IntVector2 intVector = new IntVector2(i, j);
						if (Mathf.Abs(intVector.x - position.x) + Mathf.Abs(intVector.y - position.y) == 1)
						{
							Slot slot = game.GetSlot(intVector);
							if (slot != null && slot != this)
							{
								list.Add(slot);
							}
						}
					}
				}
				isNeigbourSlotsListSet = true;
				return list;
			}
		}

		public void AddLock(Lock slotLock)
		{
			if (slotLock != null && !slotLocks.Contains(slotLock))
			{
				slotLocks.Add(slotLock);
			}
		}

		public void ClearLocks()
		{
			slotLocks.Clear();
		}

		public void RemoveLock(Lock slotLock)
		{
			if (slotLock != null)
			{
				slotLocks.Remove(slotLock);
			}
		}

		public void AddSetLock(SetLock slotLock)
		{
			if (slotLock != null && !setSlotLocks.Contains(slotLock))
			{
				setSlotLocks.Add(slotLock);
			}
		}

		public void RemoveSetLock(SetLock slotLock)
		{
			if (slotLock != null)
			{
				setSlotLocks.Remove(slotLock);
			}
		}

		public void Init(Match3Game game)
		{
			this.game = game;
		}

		public void AddToGoalsAtStart(Match3Goals goals)
		{
			for (int i = 0; i < components.Count; i++)
			{
				components[i].AddToGoalsAtStart(goals);
			}
		}

		public T GetSlotComponent<T>() where T : SlotComponent
		{
			for (int i = 0; i < components.Count; i++)
			{
				T val = components[i] as T;
				if (val != null)
				{
					return val;
				}
			}
			return null;
		}

		public bool TryGetSlotComponent<T>(out T component) where T : SlotComponent
		{
			component = null;
			
			for (int i = 0; i < components.Count; i++)
			{
				T val = components[i] as T;
				if (val != null)
				{
					component = val;
					return true;
				}
			}
			return false;
		}

		public bool TryGetSlotComponents<T>(out T[] componentArray) where T : SlotComponent
		{
			componentArray = null;
			var componentsList = new List<T>();
			
			for (int i = 0; i < components.Count; i++)
			{
				T val = components[i] as T;
				if (val != null)
				{
					componentsList.Add(val);
				}
			}

			componentArray = componentsList.ToArray();
			
			return componentArray is { Length: > 0 };
		}

		public bool IsCompatibleWithPickupGoal(Match3Goals.ChipTypeDef chipTypeDef)
		{
			for (int i = 0; i < components.Count; i++)
			{
				if (components[i].IsCompatibleWithPickupGoal(chipTypeDef))
				{
					return true;
				}
			}
			if (game.board.burriedElements.IsCompatibleWithPickupGoal(this, chipTypeDef))
			{
				return true;
			}
			if (game.board.monsterElements.IsCompatibleWithPickupGoal(this, chipTypeDef))
			{
				return true;
			}
			return false;
		}

		public void FillIncomingGravitySlots()
		{
			List<MoveToSlot> allMoveToSlots = this.allMoveToSlots;
			for (int i = 0; i < allMoveToSlots.Count; i++)
			{
				MoveToSlot moveToSlot = allMoveToSlots[i];
				Slot slot = moveToSlot.slot;
				if (moveToSlot.type != MoveToSlotType.Sandflow && !slot.incomingGravitySlots.Contains(this))
				{
					slot.incomingGravitySlots.Add(this);
				}
			}
		}

		private static bool IsReacheableByGeneratorOrChip(Slot slot, int depth = 0)
		{
			if (slot.isPreventingGravity)
			{
				return false;
			}
			if (slot.canGenerateChip)
			{
				return true;
			}
			if (slot.isSomethingMoveableByGravityInSlot)
			{
				return true;
			}
			if (depth > 30)
			{
				UnityEngine.Debug.LogError("MAX DEPTH REACHED");
				return false;
			}
			for (int i = 0; i < slot.incomingGravitySlots.Count; i++)
			{
				Slot slot2 = slot.incomingGravitySlots[i];
				if (slot2 != null && IsReacheableByGeneratorOrChip(slot2, depth + 1))
				{
					return true;
				}
			}
			return false;
		}

		private static Chip FirstReacheableChip(Slot slot, int depth = 0)
		{
			if (slot == null)
			{
				return null;
			}
			Chip slotComponent = slot.GetSlotComponent<Chip>();
			if (slotComponent != null)
			{
				return slotComponent;
			}
			if (slot.isPreventingGravity)
			{
				return null;
			}
			if (depth > 30)
			{
				UnityEngine.Debug.LogError("MAX DEPTH REACHED");
				return null;
			}
			for (int i = 0; i < slot.incomingGravitySlots.Count; i++)
			{
				Slot slot2 = slot.incomingGravitySlots[i];
				if (slot2 != null)
				{
					Chip chip = FirstReacheableChip(slot2, depth + 1);
					if (chip != null)
					{
						return chip;
					}
				}
			}
			return null;
		}

		public void OnSlotComponentMadeAStartMove(SlotComponent.SlotStartMoveParams startMoveParams)
		{
			for (int i = 0; i < components.Count; i++)
			{
				components[i].OnSlotComponentMadeAStartMove(startMoveParams);
			}
		}

		public void OnSlotComponentMadeATransformChange(SlotComponent component)
		{
			for (int i = 0; i < components.Count; i++)
			{
				components[i].OnSlotComponentMadeATransformChange(component);
			}
		}

		public bool CanAcceptFallingChip(Slot slotFromWhichToAccept)
		{
			if (isMoveIntoSlotSuspended)
			{
				return false;
			}
			for (int i = 0; i < components.Count; i++)
			{
				if (components[i].isPreventingOtherChipsToFallIntoSlot)
				{
					return false;
				}
			}
			return true;
		}

		public bool CanAttachGrowingElement()
		{
			if (isAttachGrowingElementSuspended)
			{
				return false;
			}
			if (isDestroySuspended)
			{
				return false;
			}
			if (GetSlotComponent<Chip>() != null)
			{
				return true;
			}
			for (int i = 0; i < components.Count; i++)
			{
				if (components[i].isPreventingOtherChipsToFallIntoSlot)
				{
					return false;
				}
			}
			return true;
		}

		public bool CanParticipateInDiscoBombAffectedArea(ItemColor itemColor, bool replaceWithBombs)
		{
			Chip slotComponent = GetSlotComponent<Chip>();
			if (slotComponent == null)
			{
				return false;
			}
			if (!slotComponent.canFormColorMatches)
			{
				return false;
			}
			if (slotComponent.itemColor != itemColor)
			{
				return false;
			}
			for (int i = 0; i < components.Count; i++)
			{
				if (components[i].IsAvailableForDiscoBombSuspended(replaceWithBombs))
				{
					return false;
				}
			}
			for (int j = 0; j < slotLocks.Count; j++)
			{
				if (slotLocks[j].isAvailableForDiscoBombSuspended)
				{
					return false;
				}
			}
			return true;
		}

		public int IsAboutToBeDestroyedLocksCount()
		{
			int num = 0;
			for (int i = 0; i < slotLocks.Count; i++)
			{
				if (slotLocks[i].isAboutToBeDestroyed)
				{
					num++;
				}
			}
			return num;
		}

		public bool isSlotGravitySuspendedByComponentOtherThan(SlotComponent excludedComponent)
		{
			for (int i = 0; i < components.Count; i++)
			{
				SlotComponent slotComponent = components[i];
				if (slotComponent != excludedComponent && slotComponent.isSlotGravitySuspended)
				{
					return true;
				}
			}
			return false;
		}

		private bool IsBlockingPassTo(Slot to)
		{
			IntVector2 direction = to.position - position;
			for (int i = 0; i < components.Count; i++)
			{
				if (components[i].isBlockingDirection(direction))
				{
					return true;
				}
			}
			return false;
		}

		public bool isSwipeSuspendedTo(Slot slot)
		{
			if (IsBlockingPassTo(slot) || slot.IsBlockingPassTo(this))
			{
				return true;
			}
			return false;
		}

		public bool isSlotSwipingSuspendedForSlot(Slot slot)
		{
			for (int i = 0; i < setSlotLocks.Count; i++)
			{
				if (setSlotLocks[i].GetIsSwappingSuspended(slot))
				{
					return true;
				}
			}
			return false;
		}

		public void OnUpdate(float deltaTime)
		{
			backLight.OnUpdate(deltaTime);
			ApplySlotGravity();
			for (int i = 0; i < components.Count; i++)
			{
				components[i].OnUpdate(deltaTime);
			}
		}

		public Slot NextSlotToPushToWithoutSandflow()
		{
			List<MoveToSlot> allMoveToSlots = this.allMoveToSlots;
			for (int i = 0; i < allMoveToSlots.Count; i++)
			{
				MoveToSlot moveToSlot = allMoveToSlots[i];
				if (moveToSlot.type != MoveToSlotType.Sandflow)
				{
					return moveToSlot.slot;
				}
			}
			return null;
		}

		public void Wobble(WobbleAnimation.Settings wobbleSettings)
		{
			for (int i = 0; i < components.Count; i++)
			{
				components[i].Wobble(wobbleSettings);
			}
		}

		public void SetMaxDistanceToEnd(int depth = 0)
		{
			if (isMaxDistanceToEndSet)
			{
				return;
			}
			if (depth > 100)
			{
				UnityEngine.Debug.LogError("LOOP DETECTED");
				isMaxDistanceToEndSet = true;
				maxDistanceToEnd = depth;
				return;
			}
			int num = 0;
			List<MoveToSlot> allMoveToSlots = this.allMoveToSlots;
			for (int i = 0; i < allMoveToSlots.Count; i++)
			{
				Slot slot = allMoveToSlots[i].slot;
				slot.SetMaxDistanceToEnd(depth + 1);
				num = Mathf.Max(slot.maxDistanceToEnd, num);
			}
			maxDistanceToEnd = num + 1;
			isMaxDistanceToEndSet = true;
		}

		public static bool IsWallBetween(Match3Game game, IntVector2 originPos, IntVector2 destinationPos)
		{
			Slot slot = game.GetSlot(originPos);
			IntVector2 intVector = destinationPos - originPos;
			if (slot != null && slot.IsBlockingPath(intVector))
			{
				return true;
			}
			Slot slot2 = game.GetSlot(destinationPos);
			if (slot2 != null && slot2.IsBlockingPath(-intVector))
			{
				return true;
			}
			return false;
		}

		public static bool IsPathBlockedBetween(Slot origin, Slot destination)
		{
			if (origin == null || destination == null)
			{
				return true;
			}
			IntVector2 intVector = destination.position - origin.position;
			if (origin.IsBlockingPath(intVector))
			{
				return true;
			}
			if (destination.IsBlockingPath(-intVector))
			{
				return true;
			}
			return false;
		}

		public bool IsBlockingPath(IntVector2 direction)
		{
			for (int i = 0; i < components.Count; i++)
			{
				if (components[i].isBlockingDirection(direction))
				{
					return true;
				}
			}
			return false;
		}

		public void ApplySlotGravity()
		{
			if (isSlotGravitySuspended)
			{
				return;
			}
			if (canGenerateChip && !isSomethingMoveableByGravityInSlot)
			{
				GenerateChip();
			}
			else
			{
				if (!isSomethingMoveableByGravityInSlot)
				{
					return;
				}
				MoveContentsToSlotParams moveParams = default(MoveContentsToSlotParams);
				List<MoveToSlot> allMoveToSlots = this.allMoveToSlots;
				int num = 0;
				MoveToSlot moveToSlot;
				Slot slot;
				while (true)
				{
					if (num < allMoveToSlots.Count)
					{
						moveToSlot = allMoveToSlots[num];
						slot = moveToSlot.slot;
						if (slot.CanAcceptFallingChip(this) && (moveToSlot.type != MoveToSlotType.Sandflow || !slot.isReacheableByGeneratorOrChip))
						{
							break;
						}
						num++;
						continue;
					}
					return;
				}
				moveParams.isFromPortal = (moveToSlot.type == MoveToSlotType.Portal);
				MoveContentsToSlotByGravity(slot, moveParams);
			}
		}

		public void AddComponent(SlotComponent c)
		{
			if (c == null)
			{
				return;
			}
			c.slot = this;
			c.lastConnectedSlot = this;
			if (c is Chip && GetSlotComponent<Chip>() != null)
			{
				UnityEngine.Debug.LogError("CHIP ADDED TWICE TO SLOT " + position);
			}
			bool flag = true;
			for (int i = 0; i < components.Count; i++)
			{
				if (components[i].sortingOrder < c.sortingOrder)
				{
					components.Insert(i, c);
					flag = false;
					break;
				}
			}
			if (flag)
			{
				components.Add(c);
			}
			c.OnAddedToSlot(this);
		}

		public void RemoveComponent(SlotComponent c)
		{
			if (c != null)
			{
				c.slot = null;
				components.Remove(c);
				c.OnRemovedFromSlot(this);
			}
		}

		public void OnDestroyNeighbourSlot(Slot slotBeingDestroyed, SlotDestroyParams destroyParams)
		{
			tempSlotComponentsList.Clear();
			tempSlotComponentsList.AddRange(components);
			for (int i = 0; i < tempSlotComponentsList.Count && !tempSlotComponentsList[i].OnDestroyNeighbourSlotComponent(slotBeingDestroyed, destroyParams).stopPropagation; i++)
			{
			}
		}

		public void OnDestroySlot(SlotDestroyParams destroyParams)
		{
			if (isDestroySuspended)
			{
				return;
			}
			bool flag = false;
			destroyParams.StartSlot(this);
			tempSlotComponentsList.Clear();
			tempSlotComponentsList.AddRange(components);
			bool flag2 = false;
			for (int i = 0; i < tempSlotComponentsList.Count; i++)
			{
				SlotComponent slotComponent = tempSlotComponentsList[i];
				bool isBlockingChip = slotComponent.isBlockingChip;
				bool flag3 = slotComponent is Chip;
				SlotDestroyResolution slotDestroyResolution = slotComponent.OnDestroySlotComponent(destroyParams);
				if (slotDestroyResolution.isDestroyed && flag3)
				{
					destroyParams.chipsDestroyed++;
				}
				if (slotDestroyResolution.isDestroyed && isBlockingChip)
				{
					destroyParams.chipBlockersDestroyed++;
				}
				if (slotDestroyResolution.isNeigbourDestroySuspendedForThisChipOnly)
				{
					flag = true;
				}
				if (slotDestroyResolution.isNeigbourDestroySuspended)
				{
					destroyParams.isNeigbourDestroySuspended = true;
				}
				if (slotDestroyResolution.stopPropagation)
				{
					flag2 = true;
					break;
				}
			}
			CollectPointsAction.OnSlotDestroy(this, destroyParams);
			destroyParams.EndSlot(this);
			if (!flag2 && destroyParams.isHavingCarpet)
			{
				game.board.carpet.AddCarpetFromGame(position);
			}
			if (flag)
			{
				destroyParams.AddSlotForSuspendedNeighbor(this);
			}
			tempSlotComponentsList.Clear();
		}

		private void MoveContentsToSlotByGravity(Slot nextSlot, MoveContentsToSlotParams moveParams)
		{
			if (nextSlot.isMoveIntoSlotSuspended)
			{
				return;
			}
			componentsToRemove.Clear();
			for (int i = 0; i < components.Count; i++)
			{
				SlotComponent slotComponent = components[i];
				if (slotComponent.isMovedByGravity)
				{
					componentsToRemove.Add(slotComponent);
				}
			}
			for (int j = 0; j < componentsToRemove.Count; j++)
			{
				SlotComponent slotComponent2 = componentsToRemove[j];
				RemoveComponent(slotComponent2);
				nextSlot.AddComponent(slotComponent2);
				slotComponent2.OnMovedBySlotGravity(this, nextSlot, moveParams);
			}
		}

		private ItemColor ColorToIgnore()
		{
			if (selectedColors.Count == 0)
			{
				return ItemColor.Unknown;
			}
			ItemColor itemColor = selectedColors[selectedColors.Count - 1];
			int num = 0;
			int num2 = selectedColors.Count - 1;
			while (num2 >= 0 && selectedColors[num2] == itemColor)
			{
				num++;
				num2--;
			}
			if (num < 2)
			{
				return ItemColor.Unknown;
			}
			return itemColor;
		}

		private bool TryGenerateFallingChip()
		{
			ExtraFallingChips extraFallingChips = game.extraFallingChips;
			if (generatorSettings.maxFallingElementsToGenerate <= generatedFallingElements)
			{
				return false;
			}
			if (!extraFallingChips.ShouldGenerateExtraFallingChip(this))
			{
				return false;
			}
			statsToBottom.Fill(this);
			int placesToGoDown = Mathf.Max(0, statsToBottom.totalDepth - statsToBottom.movingChips - 1);
			if (statsToBottom.GetPathSlot(placesToGoDown) != this)
			{
				return false;
			}
			Chip chip = game.CreateFallingElement(this, ChipType.FallingGingerbreadMan);
			chip.chipTag = generatorSettings.chipTag;
			chip.OnCreatedBySlot(this);
			generatedFallingElements++;
			extraFallingChips.OnExtraFallingChipGenerated(this);
			return true;
		}

		private void GenerateChip()
		{
			if (isChipGeneratorSuspended || isSlotGravitySuspended)
			{
				return;
			}
			if (TryGenerateFallingChip())
			{
				createdChips++;
				return;
			}
			if (generatorSetup != null && generatorSetup.chips.Count > createdChips)
			{
				ItemColor itemColor = generatorSetup.chips[createdChips].itemColor;
				game.CreateChipInSlot(this, itemColor).OnCreatedBySlot(this);
			}
			else if (generatorSettings.generateOnlyBunnies)
			{
				game.CreateCharacterInSlot(this, ChipType.BunnyChip, 0).OnCreatedBySlot(this);
			}
			else
			{
				Match3Board.ChipCreateParams chipCreateParams = default(Match3Board.ChipCreateParams);
				chipCreateParams.chipType = ChipType.Unknown;
				if (generatorSlotSettings != null)
				{
					chipCreateParams = game.board.RandomChip(generatorSlotSettings);
				}
				if (chipCreateParams.chipType == ChipType.Unknown)
				{
					chipCreateParams = game.board.RandomChip(ColorToIgnore());
				}
				if (chipCreateParams.chipType == ChipType.Bomb || chipCreateParams.chipType == ChipType.HorizontalRocket || chipCreateParams.chipType == ChipType.VerticalRocket || chipCreateParams.chipType == ChipType.DiscoBall)
				{
					game.CreatePowerupInSlot(this, chipCreateParams.chipType).OnCreatedBySlot(this);
				}
				else if (chipCreateParams.chipType == ChipType.BunnyChip)
				{
					game.CreateCharacterInSlot(this, ChipType.BunnyChip, 0).OnCreatedBySlot(this);
				}
				else
				{
					ItemColor itemColor2 = chipCreateParams.itemColor;
					bool num = game.IsPreventAutomatedMachesIfPossible();
					bool strictAsPossibleToprevent = game.strictAsPossibleToprevent;
					if (num)
					{
						statsToBottom.Fill(this);
						int placesToGoDown = Mathf.Max(0, statsToBottom.totalDepth - statsToBottom.movingChips - 1);
						Slot pathSlot = statsToBottom.GetPathSlot(placesToGoDown);
						statsToBottom.availableColors.Clear();
						statsToBottom.availableColors.AddRange(game.board.availableColors);
						List<IntVector2> orthoDirections = pathSlot.gravity.orthoDirections;
						for (int i = 0; i < orthoDirections.Count; i++)
						{
							IntVector2 intVector = orthoDirections[i];
							Slot slot = game.GetSlot(position + intVector);
							Slot slot2 = game.GetSlot(position + 2 * intVector);
							Slot slot3 = game.GetSlot(position - intVector);
							Chip chip = FirstReacheableChip(slot);
							Chip chip2 = FirstReacheableChip(slot2);
							Chip chip3 = FirstReacheableChip(slot3);
							if (strictAsPossibleToprevent)
							{
								if (chip != null)
								{
									statsToBottom.availableColors.Remove(chip.itemColor);
								}
								continue;
							}
							if (chip != null && chip2 != null && chip.itemColor == chip2.itemColor)
							{
								statsToBottom.availableColors.Remove(chip.itemColor);
							}
							if (chip != null && chip3 != null && chip.itemColor == chip3.itemColor)
							{
								statsToBottom.availableColors.Remove(chip.itemColor);
							}
						}
						Chip firstChipBelow = statsToBottom.firstChipBelow;
						if (firstChipBelow != null)
						{
							statsToBottom.availableColors.Remove(firstChipBelow.itemColor);
						}
						itemColor2 = game.board.RandomColor();
						if (statsToBottom.availableColors.Count > 0)
						{
							itemColor2 = statsToBottom.availableColors[game.RandomRange(0, statsToBottom.availableColors.Count) % statsToBottom.availableColors.Count];
						}
						game.board.generatedChipsCount++;
					}
					selectedColors.Add(itemColor2);
					if (selectedColors.Count > maxSelectedColorCount)
					{
						selectedColors.RemoveAt(0);
					}
					Chip chip4 = null;
					((chipCreateParams.chipType != ChipType.MonsterChip) ? game.CreateChipInSlot(this, itemColor2) : game.CreateChipInSlot(this, chipCreateParams.chipType, itemColor2))?.OnCreatedBySlot(this);
				}
				if (chipCreateParams.hasIce)
				{
					game.AddIceToSlot(this, chipCreateParams.iceLevel);
				}
			}
			createdChips++;
		}

		public static void GetStatsToBottom(Slot slot, StatsToBottom sb)
		{
			if (slot == null || slot.isBlockForGravity)
			{
				return;
			}
			sb.pathSlots.Add(slot);
			sb.totalDepth++;
			Chip slotComponent = slot.GetSlotComponent<Chip>();
			sb.TryAddChip(slotComponent);
			if (slotComponent != null)
			{
				if (!slotComponent.isSlotGravitySuspended)
				{
					sb.totalDepth--;
					return;
				}
				sb.movingChips++;
			}
			List<MoveToSlot> allMoveToSlots = slot.allMoveToSlots;
			int num = 0;
			MoveToSlot moveToSlot;
			while (true)
			{
				if (num < allMoveToSlots.Count)
				{
					moveToSlot = allMoveToSlots[num];
					if (moveToSlot.type != MoveToSlotType.Sandflow)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			GetStatsToBottom(moveToSlot.slot, sb);
		}

		public static void SwitchChips(Slot slot1, Slot slot2, bool changePosition = false)
		{
			if (slot1 == null || slot2 == null)
			{
				return;
			}
			Chip slotComponent = slot1.GetSlotComponent<Chip>();
			Chip slotComponent2 = slot2.GetSlotComponent<Chip>();
			slot1.RemoveComponent(slotComponent);
			slot2.RemoveComponent(slotComponent2);
			slot1.AddComponent(slotComponent2);
			slot2.AddComponent(slotComponent);
			if (changePosition)
			{
				TransformBehaviour transformBehaviour = null;
				TransformBehaviour transformBehaviour2 = null;
				if (slotComponent != null)
				{
					transformBehaviour = slotComponent.GetComponentBehaviour<TransformBehaviour>();
				}
				if (slotComponent2 != null)
				{
					transformBehaviour2 = slotComponent2.GetComponentBehaviour<TransformBehaviour>();
				}
				if (transformBehaviour != null)
				{
					transformBehaviour.localPosition = slot2.localPositionOfCenter;
				}
				if (transformBehaviour2 != null)
				{
					transformBehaviour2.localPosition = slot1.localPositionOfCenter;
				}
			}
		}

		public static bool HasNeighboursAffectedByMatchingSlots(List<Slot> matchingSlots, Match3Game game)
		{
			for (int i = 0; i < matchingSlots.Count; i++)
			{
				Slot slot = matchingSlots[i];
				IntVector2[] upDownLeftRight = IntVector2.upDownLeftRight;
				foreach (IntVector2 b in upDownLeftRight)
				{
					Slot slot2 = game.GetSlot(slot.position + b);
					if (slot2 != null && slot2.isDestroyedByMatchingNextTo && !matchingSlots.Contains(slot2))
					{
						return true;
					}
				}
			}
			return false;
		}

		public static void RemoveLocks(Slot slot, Lock slotLock)
		{
			if (slotLock != null)
			{
				slot?.RemoveLock(slotLock);
			}
		}

		public static void RemoveLocks(List<Slot> slots, Lock slotLock)
		{
			if (slotLock != null)
			{
				for (int i = 0; i < slots.Count; i++)
				{
					slots[i]?.RemoveLock(slotLock);
				}
			}
		}
	}
}
