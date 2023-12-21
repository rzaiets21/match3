using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class CompositeAffector : PlayerInput.AffectorBase
	{
		public class SwipedSlot
		{
			public IntVector2 cameFromPosition;

			public Chip otherChipToMove;

			public Slot slot;

			public bool isCreatingPowerup;

			public Slot mixSlot;

			public PlayerInput.MixClass mixClass;

			public bool isDiscoCombine;

			public List<Slot> matchingSlots = new List<Slot>();

			public bool isMix => mixSlot != null;

			public bool isPowerup => slot.GetSlotComponent<Chip>()?.isPowerup ?? false;

			public ChipType chipType => slot.GetSlotComponent<Chip>()?.chipType ?? ChipType.Unknown;

			public bool isMatching => matchingSlots.Count > 1;

			public void SetMix(Slot mixSlot)
			{
				this.mixSlot = mixSlot;
				mixClass = new PlayerInput.MixClass();
				mixClass.TryAdd(slot.GetSlotComponent<Chip>());
				mixClass.TryAdd(mixSlot.GetSlotComponent<Chip>());
			}

			public void SetOtherChipMatch(Slot otherSlot, Island otherIsland)
			{
				if (otherIsland == null && otherSlot != null)
				{
					otherChipToMove = otherSlot.GetSlotComponent<Chip>();
				}
			}

			public void SetMatch(Island island)
			{
				matchingSlots.Clear();
				if (island != null)
				{
					List<Slot> allSlots = island.allSlots;
					for (int i = 0; i < allSlots.Count; i++)
					{
						Slot item = allSlots[i];
						matchingSlots.Add(item);
					}
					isCreatingPowerup = island.isCreatingPowerup;
				}
			}
		}

		public class InitArguments
		{
			public Match3Game game;

			public List<SwipedSlot> swipedSlots = new List<SwipedSlot>();

			public SwipedSlot AddSwipedSlot(Slot slot)
			{
				SwipedSlot swipedSlot = new SwipedSlot();
				swipedSlot.slot = slot;
				swipedSlots.Add(swipedSlot);
				return swipedSlot;
			}
		}

		private InitArguments initArguments;

		private List<ChipAffectorBase> chipAffectors = new List<ChipAffectorBase>();

		private List<Slot> ignoreSlots = new List<Slot>();

		public override bool wantsToEnd
		{
			get
			{
				if (!Match3Settings.instance.swipeAffectorSettings.hasMaxAffectorDuration)
				{
					return false;
				}
				List<SwipedSlot> swipedSlots = initArguments.swipedSlots;
				for (int i = 0; i < swipedSlots.Count; i++)
				{
					if (swipedSlots[i].isMix)
					{
						return false;
					}
				}
				return affectorDuration > Match3Settings.instance.swipeAffectorSettings.maxMaxAffectorDuration;
			}
		}

		public override bool canFinish
		{
			get
			{
				if (affectorDuration < minAffectorDuration)
				{
					return false;
				}
				for (int i = 0; i < chipAffectors.Count; i++)
				{
					if (!chipAffectors[i].canFinish)
					{
						return false;
					}
				}
				return true;
			}
		}

		public override float minAffectorDuration
		{
			get
			{
				float num = 0f;
				List<SwipedSlot> swipedSlots = initArguments.swipedSlots;
				for (int i = 0; i < swipedSlots.Count; i++)
				{
					float num2 = 0f;
					SwipedSlot swipedSlot = swipedSlots[i];
					if (swipedSlot.isPowerup && swipedSlot.chipType == ChipType.DiscoBall)
					{
						num2 = Mathf.Max(num2, Match3Settings.instance.discoBallAffectorSettings.minDuration);
					}
					else if (swipedSlot.isMix)
					{
						num2 = Match3Settings.instance.swipeAffectorSettings.minAffectorDurationMix;
					}
					else if (swipedSlot.isPowerup)
					{
						num2 = ((swipedSlot.chipType != ChipType.Bomb || !Match3Settings.instance.playerInputSettings.disableBombLighting) ? Mathf.Max(num2, Match3Settings.instance.seekingMissleAffectorSettings.minAffectorDuration) : 0f);
					}
					else if (swipedSlot.isMatching)
					{
						num2 = ((!swipedSlot.isCreatingPowerup) ? Mathf.Max(num2, Match3Settings.instance.swipeAffectorSettings.minAffectorDuration) : Mathf.Max(num2, Match3Settings.instance.swipeAffectorSettings.minAffectorDurationPowerup));
					}
					num = Mathf.Max(num, num2);
				}
				return num;
			}
		}

		public override void ReleaseLocks()
		{
			for (int i = 0; i < chipAffectors.Count; i++)
			{
				chipAffectors[i].ReleaseLocks();
			}
		}

		public override void ApplyLocks()
		{
			for (int i = 0; i < chipAffectors.Count; i++)
			{
				chipAffectors[i].ApplyLocks();
			}
		}

		public void Init(InitArguments initArguments)
		{
			Clear();
			this.initArguments = initArguments;
			Match3Game game = initArguments.game;
			ignoreSlots.Clear();
			List<SwipedSlot> swipedSlots = initArguments.swipedSlots;
			for (int i = 0; i < swipedSlots.Count; i++)
			{
				SwipedSlot swipedSlot = swipedSlots[i];
				ignoreSlots.Add(swipedSlot.slot);
				ignoreSlots.AddRange(swipedSlot.matchingSlots);
			}
			for (int j = 0; j < swipedSlots.Count; j++)
			{
				SwipedSlot swipedSlot2 = swipedSlots[j];
				if (swipedSlot2.isDiscoCombine)
				{
					DiscoChipAffector discoChipAffector = new DiscoChipAffector();
					discoChipAffector.Init(swipedSlot2.slot, swipedSlot2.mixSlot, game);
					chipAffectors.Add(discoChipAffector);
				}
				else if (swipedSlot2.isMix)
				{
					CombineChipAffectors combineChipAffectors = new CombineChipAffectors();
					combineChipAffectors.Init(swipedSlot2, game);
					chipAffectors.Add(combineChipAffectors);
					if (swipedSlot2.mixClass.CountOfType(ChipType.DiscoBall) == 1)
					{
						Chip chip = swipedSlot2.mixClass.GetChip(ChipType.DiscoBall);
						Chip otherChip = swipedSlot2.mixClass.GetOtherChip(ChipType.DiscoBall);
						ItemColor itemColor = game.BestItemColorForDiscoBomb(replaceWithBombs: true);
						List<Slot> slots = game.SlotsThatCanParticipateInDiscoBallAffectedArea(itemColor, replaceWithBombs: true);
						DiscoChipCombineWithPowerupAffector discoChipCombineWithPowerupAffector = new DiscoChipCombineWithPowerupAffector();
						discoChipCombineWithPowerupAffector.Init(chip.lastConnectedSlot, otherChip.lastConnectedSlot, otherChip.chipType, game, slots);
						chipAffectors.Add(discoChipCombineWithPowerupAffector);
					}
					else if (swipedSlot2.mixClass.CountOfType(ChipType.HorizontalRocket, ChipType.VerticalRocket) == 2)
					{
						PowerCrossChipAffector powerCrossChipAffector = new PowerCrossChipAffector();
						powerCrossChipAffector.Init(swipedSlot2.mixSlot, game, 0);
						chipAffectors.Add(powerCrossChipAffector);
					}
					else if (swipedSlot2.mixClass.CountOfType(ChipType.HorizontalRocket, ChipType.VerticalRocket) == 1 && swipedSlot2.mixClass.CountOfType(ChipType.Bomb) == 1)
					{
						PowerCrossChipAffector powerCrossChipAffector2 = new PowerCrossChipAffector();
						powerCrossChipAffector2.Init(swipedSlot2.mixSlot, game, 1);
						chipAffectors.Add(powerCrossChipAffector2);
					}
					else if (swipedSlot2.mixClass.CountOfType(ChipType.Bomb) == 2)
					{
						BombChipAffector bombChipAffector = new BombChipAffector();
						bombChipAffector.Init(swipedSlot2.mixSlot, game, 3, doPlus: false, BombChipAffector.PowerupType.Block);
						chipAffectors.Add(bombChipAffector);
					}
					else if (swipedSlot2.mixClass.CountOfType(ChipType.SeekingMissle) == 1)
					{
						BombChipAffector bombChipAffector2 = new BombChipAffector();
						bombChipAffector2.Init(swipedSlot2.mixSlot, game, 1, doPlus: true, BombChipAffector.PowerupType.Seeking);
						chipAffectors.Add(bombChipAffector2);
					}
				}
				else if (swipedSlot2.isPowerup)
				{
					if (swipedSlot2.chipType == ChipType.SeekingMissle)
					{
						BombChipAffector bombChipAffector3 = new BombChipAffector();
						bombChipAffector3.Init(swipedSlot2.slot, game, 1, doPlus: true, BombChipAffector.PowerupType.Seeking);
						chipAffectors.Add(bombChipAffector3);
					}
					else if (swipedSlot2.chipType == ChipType.Bomb)
					{
						BombChipAffector bombChipAffector4 = new BombChipAffector();
						bombChipAffector4.Init(swipedSlot2.slot, game, 2, doPlus: false, BombChipAffector.PowerupType.Bomb);
						chipAffectors.Add(bombChipAffector4);
					}
					else if (swipedSlot2.chipType == ChipType.VerticalRocket || swipedSlot2.chipType == ChipType.HorizontalRocket)
					{
						LineChipAffector lineChipAffector = new LineChipAffector();
						IntVector2 direction = (swipedSlot2.chipType == ChipType.VerticalRocket) ? IntVector2.up : IntVector2.right;
						lineChipAffector.Init(swipedSlot2.slot, game, direction);
						chipAffectors.Add(lineChipAffector);
					}
					continue;
				}
				if (swipedSlot2.isMatching)
				{
					MatchChipAffector.InitArguments initArguments2 = new MatchChipAffector.InitArguments();
					initArguments2.game = game;
					initArguments2.cameFromPositionSet = true;
					initArguments2.cameFromPosition = swipedSlot2.cameFromPosition;
					initArguments2.otherChipToMove = swipedSlot2.otherChipToMove;
					initArguments2.isCreatingPowerup = swipedSlot2.isCreatingPowerup;
					initArguments2.originSlot = swipedSlot2.slot;
					initArguments2.matchingSlots.AddRange(swipedSlot2.matchingSlots);
					initArguments2.ignoreSlots.AddRange(ignoreSlots);
					MatchChipAffector matchChipAffector = new MatchChipAffector();
					matchChipAffector.Init(initArguments2);
					chipAffectors.Add(matchChipAffector);
				}
			}
		}

		public override void AddToSwitchSlotArguments(ref Match3Game.SwitchSlotsArguments switchSlotsArguments)
		{
			base.AddToSwitchSlotArguments(ref switchSlotsArguments);
			switchSlotsArguments.bolts = new List<LightingBolt>();
			for (int i = 0; i < chipAffectors.Count; i++)
			{
				ChipAffectorBase chipAffectorBase = chipAffectors[i];
				chipAffectorBase.AddToInputAffectorExport(switchSlotsArguments.affectorExport);
				switchSlotsArguments.affectorExport.AddChipAffector(chipAffectorBase);
			}
		}

		public override void Clear()
		{
			affectorDuration = 0f;
			for (int i = 0; i < chipAffectors.Count; i++)
			{
				chipAffectors[i].Clear();
			}
			chipAffectors.Clear();
		}

		public override void OnBeforeDestroy()
		{
		}

		public override void OnUpdate(PlayerInput.AffectorUpdateParams updateParams)
		{
			affectorDuration += Time.deltaTime;
			for (int i = 0; i < chipAffectors.Count; i++)
			{
				chipAffectors[i].Update();
			}
		}
	}
}
