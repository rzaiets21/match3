using GGMatch3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PlacePowerupAction : BoardAction
{
	[Serializable]
	public class Settings
	{
		public float delayBetweenPowerups = 0.5f;

		public float durationForPowerup = 1f;

		public AnimationCurve powerupCurve;

		public float startScale = 2f;

		public float startAlpha;

		public float normalizedTimeForParticles;
	}

	public class Parameters
	{
		public Match3Game game;

		public ChipType powerup;

		public float initialDelay;

		public int addCoins;

		public Slot slotWhereToPlacePowerup;

		public bool internalAnimation;

		public Action onComplete;
	}

	public class ConstraintsFilter
	{
		public List<Slot> GetSlotsThatCanBeReplacedWithPowerup(List<Slot> slots)
		{
			List<Slot> list = new List<Slot>();
			for (int i = 0; i < slots.Count; i++)
			{
				Slot slot = slots[i];
				if (slot != null)
				{
					Chip slotComponent = slot.GetSlotComponent<Chip>();
					if (slotComponent != null && slotComponent.chipType == ChipType.Chip)
					{
						list.Add(slot);
					}
				}
			}
			return list;
		}

		public List<Slot> GetTappableSlots(List<Slot> slots)
		{
			List<Slot> list = new List<Slot>();
			for (int i = 0; i < slots.Count; i++)
			{
				Slot slot = slots[i];
				if (slot != null && !slot.isTapToActivateSuspended && !slot.isPowerupReplacementSuspended)
				{
					list.Add(slot);
				}
			}
			return list;
		}

		public List<Slot> GetSwappableSlotsForDiscoBomb(List<Slot> slots)
		{
			List<Slot> list = new List<Slot>();
			for (int i = 0; i < slots.Count; i++)
			{
				Slot slot = slots[i];
				if (slot == null || slot.isSlotSwapSuspended || slot.isPowerupReplacementSuspended)
				{
					continue;
				}
				List<Slot> neigbourSlots = slot.neigbourSlots;
				for (int j = 0; j < neigbourSlots.Count; j++)
				{
					Slot slot2 = neigbourSlots[j];
					Chip slotComponent = slot2.GetSlotComponent<Chip>();
					if (slotComponent != null && (slotComponent.canFormColorMatches || slotComponent.isPowerup) && !slot2.isSlotSwapSuspended)
					{
						list.Add(slot);
						break;
					}
				}
			}
			return list;
		}

		public List<Slot> GetNoPowerupPlacementSlots(List<Slot> slots)
		{
			List<Slot> list = new List<Slot>();
			for (int i = 0; i < slots.Count; i++)
			{
				Slot slot = slots[i];
				if (slot != null && !slot.isPowerupReplacementSuspended)
				{
					list.Add(slot);
				}
			}
			return list;
		}
	}

	private sealed class _003CDoReplaceChipWithPowerup_003Ed__12 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public PlacePowerupAction _003C_003E4__this;

		private float _003Ctime_003E5__2;

		private IEnumerator _003Cenumerator_003E5__3;

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
		public _003CDoReplaceChipWithPowerup_003Ed__12(int _003C_003E1__state)
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
			PlacePowerupAction placePowerupAction = _003C_003E4__this;
			CreatePowerupAction createPowerupAction;
			CreatePowerupAction.CreateParams createParams;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				if (placePowerupAction.slot == null)
				{
					placePowerupAction.isAlive = false;
					return false;
				}
				if (placePowerupAction.parameters.initialDelay > 0f)
				{
					_003Ctime_003E5__2 = 0f;
					goto IL_0087;
				}
				goto IL_009a;
			case 1:
				_003C_003E1__state = -1;
				goto IL_0087;
			case 2:
				_003C_003E1__state = -1;
				goto IL_00ec;
			case 3:
				{
					_003C_003E1__state = -1;
					break;
				}
				IL_009a:
				placePowerupAction.slot.GetSlotComponent<Chip>()?.RemoveFromGame();
				if (placePowerupAction.parameters.internalAnimation)
				{
					_003Cenumerator_003E5__3 = placePowerupAction.PowerupCreation(placePowerupAction.slot.position);
					goto IL_00ec;
				}
				createPowerupAction = new CreatePowerupAction();
				createParams = default(CreatePowerupAction.CreateParams);
				createParams.positionWherePowerupWillBeCreated = placePowerupAction.slot.position;
				createParams.powerupToCreate = placePowerupAction.parameters.powerup;
				createParams.game = placePowerupAction.parameters.game;
				createParams.addCoins = placePowerupAction.parameters.addCoins;
				createPowerupAction.Init(createParams);
				placePowerupAction.parameters.game.board.actionManager.AddAction(createPowerupAction);
				_003C_003E2__current = null;
				_003C_003E1__state = 3;
				return true;
				IL_00ec:
				if (_003Cenumerator_003E5__3.MoveNext())
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 2;
					return true;
				}
				_003Cenumerator_003E5__3 = null;
				break;
				IL_0087:
				if (_003Ctime_003E5__2 < placePowerupAction.parameters.initialDelay)
				{
					_003Ctime_003E5__2 += placePowerupAction.deltaTime;
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				goto IL_009a;
			}
			placePowerupAction.slotLock.UnlockAll();
			if (placePowerupAction.parameters.onComplete != null)
			{
				placePowerupAction.parameters.onComplete();
			}
			placePowerupAction.isAlive = false;
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

	private sealed class _003CPowerupCreation_003Ed__13 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public PlacePowerupAction _003C_003E4__this;

		public IntVector2 positionWherePowerupWillBeCreated;

		private Match3Game _003Cgame_003E5__2;

		private float _003Ctime_003E5__3;

		private Slot _003CpowerupSlot_003E5__4;

		private TransformBehaviour _003CchipTransform_003E5__5;

		private float _003Cduration_003E5__6;

		private AnimationCurve _003Ccurve_003E5__7;

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
		public _003CPowerupCreation_003Ed__13(int _003C_003E1__state)
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
			PlacePowerupAction placePowerupAction = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
			{
				_003C_003E1__state = -1;
				_003Cgame_003E5__2 = placePowerupAction.parameters.game;
				_003Ctime_003E5__3 = 0f;
				_003CpowerupSlot_003E5__4 = _003Cgame_003E5__2.GetSlot(positionWherePowerupWillBeCreated);
				_003CpowerupSlot_003E5__4.GetSlotComponent<Chip>()?.RemoveFromGame();
				Chip chip = _003Cgame_003E5__2.CreatePowerupInSlot(_003CpowerupSlot_003E5__4, placePowerupAction.parameters.powerup);
				chip.carriesCoins = placePowerupAction.parameters.addCoins;
				_003CchipTransform_003E5__5 = chip.GetComponentBehaviour<TransformBehaviour>();
				_003Ctime_003E5__3 = 0f;
				_003Cduration_003E5__6 = placePowerupAction.settings.durationForPowerup;
				_003Ccurve_003E5__7 = placePowerupAction.settings.powerupCurve;
				_003Cgame_003E5__2.particles.CreateParticles(_003CpowerupSlot_003E5__4, Match3Particles.PositionType.BombCreate);
				break;
			}
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			if (_003Ctime_003E5__3 <= _003Cduration_003E5__6)
			{
				float num2 = Mathf.InverseLerp(0f, _003Cduration_003E5__6, _003Ctime_003E5__3);
				_003Ctime_003E5__3 += placePowerupAction.deltaTime;
				float num3 = Mathf.InverseLerp(0f, _003Cduration_003E5__6, _003Ctime_003E5__3);
				float t = _003Ccurve_003E5__7.Evaluate(num3);
				Vector3 localScale = Vector3.LerpUnclamped(new Vector3(placePowerupAction.settings.startScale, placePowerupAction.settings.startScale, 1f), Vector3.one, t);
				float alpha = Mathf.Lerp(placePowerupAction.settings.startAlpha, 1f, t);
				if (_003CchipTransform_003E5__5 != null)
				{
					_003CchipTransform_003E5__5.localScale = localScale;
					_003CchipTransform_003E5__5.SetAlpha(alpha);
				}
				if (num2 <= placePowerupAction.settings.normalizedTimeForParticles && num3 > placePowerupAction.settings.normalizedTimeForParticles)
				{
					_003Cgame_003E5__2.particles.CreateParticles(_003CpowerupSlot_003E5__4, Match3Particles.PositionType.BombCreate);
				}
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			}
			if (placePowerupAction.settings.normalizedTimeForParticles >= 1f)
			{
				_003Cgame_003E5__2.particles.CreateParticles(_003CpowerupSlot_003E5__4, Match3Particles.PositionType.BombCreate);
			}
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

	private Slot slot;

	private Parameters parameters;

	private Lock slotLock;

	private IEnumerator replaceChipWithPowerup;

	private float deltaTime;

	private Settings settings => Match3Settings.instance.placePowerupActionSettings;

	public static Slot GetSlotThatCanBeReplacedWithPowerup(Match3Game game, ChipType powerup)
	{
		List<Slot> slots = new List<Slot>(game.board.slots);
		ConstraintsFilter constraintsFilter = new ConstraintsFilter();
		List<Slot> list = null;
		if (powerup == ChipType.DiscoBall)
		{
			list = constraintsFilter.GetSlotsThatCanBeReplacedWithPowerup(slots);
			list = constraintsFilter.GetSwappableSlotsForDiscoBomb(list);
		}
		else
		{
			list = constraintsFilter.GetSlotsThatCanBeReplacedWithPowerup(slots);
			list = constraintsFilter.GetTappableSlots(list);
		}
		if (list.Count == 0)
		{
			list = ((powerup != ChipType.DiscoBall) ? constraintsFilter.GetTappableSlots(slots) : constraintsFilter.GetSwappableSlotsForDiscoBomb(slots));
		}
		if (list.Count == 0)
		{
			list = constraintsFilter.GetNoPowerupPlacementSlots(slots);
		}
		if (list.Count == 0)
		{
			return null;
		}
		return list[game.RandomRange(0, list.Count)];
	}

	public void Init(Parameters parameters)
	{
		this.parameters = parameters;
		slotLock = lockContainer.NewLock();
		slot = parameters.slotWhereToPlacePowerup;
		if (slot == null)
		{
			List<Slot> slots = new List<Slot>(parameters.game.board.slots);
			ConstraintsFilter constraintsFilter = new ConstraintsFilter();
			List<Slot> list = null;
			if (parameters.powerup == ChipType.DiscoBall)
			{
				list = constraintsFilter.GetSlotsThatCanBeReplacedWithPowerup(slots);
				list = constraintsFilter.GetSwappableSlotsForDiscoBomb(list);
			}
			else
			{
				list = constraintsFilter.GetSlotsThatCanBeReplacedWithPowerup(slots);
				list = constraintsFilter.GetTappableSlots(list);
			}
			if (list.Count == 0)
			{
				list = ((parameters.powerup != ChipType.DiscoBall) ? constraintsFilter.GetTappableSlots(slots) : constraintsFilter.GetSwappableSlotsForDiscoBomb(slots));
			}
			if (list.Count == 0)
			{
				list = constraintsFilter.GetNoPowerupPlacementSlots(slots);
			}
			if (list.Count == 0)
			{
				return;
			}
			slot = list[parameters.game.RandomRange(0, list.Count)];
		}
		slotLock.isPowerupReplacementSuspended = true;
		slotLock.isDestroySuspended = true;
		slotLock.isSlotGravitySuspended = true;
		slotLock.isSlotMatchingSuspended = true;
		slotLock.isAvailableForSeekingMissileSuspended = true;
		slotLock.isAvailableForDiscoBombSuspended = true;
		slotLock.LockSlot(slot);
	}

	public override void OnUpdate(float deltaTime)
	{
		this.deltaTime = deltaTime;
		base.OnUpdate(deltaTime);
		if (replaceChipWithPowerup == null)
		{
			replaceChipWithPowerup = DoReplaceChipWithPowerup();
		}
		replaceChipWithPowerup.MoveNext();
	}

	public IEnumerator DoReplaceChipWithPowerup()
	{
		return new _003CDoReplaceChipWithPowerup_003Ed__12(0)
		{
			_003C_003E4__this = this
		};
	}

	private IEnumerator PowerupCreation(IntVector2 positionWherePowerupWillBeCreated)
	{
		return new _003CPowerupCreation_003Ed__13(0)
		{
			_003C_003E4__this = this,
			positionWherePowerupWillBeCreated = positionWherePowerupWillBeCreated
		};
	}
}
