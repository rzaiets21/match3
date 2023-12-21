using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace GGMatch3
{
	public class SwapToMatchAction : BoardAction
	{
		[Serializable]
		public class Settings
		{
			public float slot1LightIntensity = 2f;

			public float slot2LightIntensity = 1f;
		}

		public struct SwapActionProperties
		{
			public Slot slot1;

			public Slot slot2;

			public bool isInstant;

			public List<LightingBolt> bolts;

			public Match3Game.SwitchSlotsArguments switchSlotsArgument;
		}

		public class PowerupList
		{
			public List<Chip> powerupList = new List<Chip>();

			private List<Chip> coloredChips = new List<Chip>();

			public Chip FirstPowerup => powerupList[0];

			public bool isActivatingPowerup
			{
				get
				{
					if (!isMixingDiscoBallWithColorElement && !isContainingSingleActivateablePowerup)
					{
						return isMixingPowerups;
					}
					return true;
				}
			}

			public bool isMixingPowerups => powerupList.Count >= 2;

			public bool isMixingDiscoBallWithColorElement
			{
				get
				{
					if (CountChipType(ChipType.DiscoBall) == 1)
					{
						return coloredChips.Count == 1;
					}
					return false;
				}
			}

			public bool isContainingSingleActivateablePowerup
			{
				get
				{
					if (powerupList.Count == 1)
					{
						return FirstPowerup.chipType != ChipType.DiscoBall;
					}
					return false;
				}
			}

			public ItemColor mixingColor => coloredChips[0].itemColor;

			public Chip OtherPowerup(ChipType chipType)
			{
				for (int i = 0; i < powerupList.Count; i++)
				{
					Chip chip = powerupList[i];
					if (chip.chipType != chipType)
					{
						return chip;
					}
				}
				return null;
			}

			public Chip PowerupOfType(ChipType chipType)
			{
				for (int i = 0; i < powerupList.Count; i++)
				{
					Chip chip = powerupList[i];
					if (chip.chipType == chipType)
					{
						return chip;
					}
				}
				return null;
			}

			public int CountChipTypes(ChipType chipType1, ChipType chipType2)
			{
				int num = 0;
				for (int i = 0; i < powerupList.Count; i++)
				{
					Chip chip = powerupList[i];
					if (chip.chipType == chipType1)
					{
						num++;
					}
					else if (chip.chipType == chipType2)
					{
						num++;
					}
				}
				return num;
			}

			public int CountChipType(ChipType chipType)
			{
				int num = 0;
				for (int i = 0; i < powerupList.Count; i++)
				{
					if (powerupList[i].chipType == chipType)
					{
						num++;
					}
				}
				return num;
			}

			public void Add(Chip chip)
			{
				if (chip != null)
				{
					if (chip.isPowerup)
					{
						powerupList.Add(chip);
					}
					if (chip.canFormColorMatches)
					{
						coloredChips.Add(chip);
					}
				}
			}
		}

		private sealed class _003C_003Ec__DisplayClass11_0
		{
			public bool swapForwardComplete;

			internal void _003CDoSwapToMixPowerups_003Eb__0()
			{
				swapForwardComplete = true;
			}
		}

		private sealed class _003CDoSwapToMixPowerups_003Ed__11 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public SwapToMatchAction _003C_003E4__this;

			private _003C_003Ec__DisplayClass11_0 _003C_003E8__1;

			public PowerupList powerupList;

			private Match3Game _003Cgame_003E5__2;

			private bool _003ChasCarpet_003E5__3;

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
			public _003CDoSwapToMixPowerups_003Ed__11(int _003C_003E1__state)
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
				SwapToMatchAction swapToMatchAction = _003C_003E4__this;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					_003C_003E1__state = -1;
				}
				else
				{
					_003C_003E1__state = -1;
					_003C_003E8__1 = new _003C_003Ec__DisplayClass11_0();
					bool isInstant = swapToMatchAction.swapProperties.isInstant;
					_003Cgame_003E5__2 = swapToMatchAction.slot1.game;
					_003C_003E8__1.swapForwardComplete = false;
					SwapChipsAction swapChipsAction = new SwapChipsAction();
					SwapChipsAction.SwapChipsParams swapChipsParams = default(SwapChipsAction.SwapChipsParams);
					swapChipsParams.slot1 = swapToMatchAction.slot1;
					swapChipsParams.slot2 = swapToMatchAction.slot2;
					swapChipsParams.onComplete = _003C_003E8__1._003CDoSwapToMixPowerups_003Eb__0;
					swapChipsParams.switchSlots = true;
					swapChipsParams.game = _003Cgame_003E5__2;
					swapChipsParams.moveToSpecificPos = true;
					swapChipsParams.positionToMoveSlot1 = swapChipsParams.slot2.position;
					swapChipsParams.positionToMoveSlot2 = swapChipsParams.slot2.position;
					_003ChasCarpet_003E5__3 = false;
					if (swapToMatchAction.slot1.canCarpetSpreadFromHere || swapToMatchAction.slot2.canCarpetSpreadFromHere)
					{
						_003ChasCarpet_003E5__3 = true;
					}
					if (isInstant)
					{
						TransformBehaviour transformBehaviour = null;
						TransformBehaviour transformBehaviour2 = null;
						Chip chip = powerupList.powerupList[0];
						Chip chip2 = powerupList.powerupList[1];
						if (chip != null)
						{
							transformBehaviour = chip.GetComponentBehaviour<TransformBehaviour>();
						}
						if (chip2 != null)
						{
							transformBehaviour2 = chip2.GetComponentBehaviour<TransformBehaviour>();
						}
						if (transformBehaviour != null)
						{
							transformBehaviour.localPosition = swapToMatchAction.slot2.localPositionOfCenter;
						}
						if (transformBehaviour2 != null)
						{
							transformBehaviour2.localPosition = swapToMatchAction.slot1.localPositionOfCenter;
						}
						goto IL_01c2;
					}
					swapChipsAction.Init(swapChipsParams);
					_003Cgame_003E5__2.board.actionManager.AddAction(swapChipsAction);
				}
				if (!_003C_003E8__1.swapForwardComplete)
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				goto IL_01c2;
				IL_01c2:
				SwapParams swapParams = new SwapParams();
				swapParams.startPosition = swapToMatchAction.slot1.position;
				swapParams.swipeToPosition = swapToMatchAction.slot2.position;
				swapParams.affectorExport = swapToMatchAction.swapProperties.switchSlotsArgument.affectorExport;
				_003Cgame_003E5__2.particles.CreateParticles(swapToMatchAction.slot2.localPositionOfCenter, Match3Particles.PositionType.BombCombine);
				if (powerupList.CountChipType(ChipType.SeekingMissle) == 2)
				{
					ComboSeekingMissileAction comboSeekingMissileAction = new ComboSeekingMissileAction();
					ComboSeekingMissileAction.Parameters parameters = new ComboSeekingMissileAction.Parameters();
					parameters.game = _003Cgame_003E5__2;
					parameters.rocketsCount = 3;
					parameters.startSlot = swapToMatchAction.slot2;
					parameters.isHavingCarpet = _003ChasCarpet_003E5__3;
					comboSeekingMissileAction.Init(parameters);
					for (int i = 0; i < powerupList.powerupList.Count; i++)
					{
						powerupList.powerupList[i].RemoveFromGame();
					}
					swapToMatchAction.slotLock.UnlockAll();
					_003Cgame_003E5__2.board.actionManager.AddAction(comboSeekingMissileAction);
				}
				else if (powerupList.CountChipType(ChipType.SeekingMissle) == 1 && (powerupList.CountChipType(ChipType.Bomb) == 1 || powerupList.CountChipTypes(ChipType.HorizontalRocket, ChipType.VerticalRocket) == 1))
				{
					SeekingMissileAction seekingMissileAction = new SeekingMissileAction();
					SeekingMissileAction.Parameters parameters2 = new SeekingMissileAction.Parameters();
					powerupList.PowerupOfType(ChipType.SeekingMissle).RemoveFromGame();
					parameters2.game = _003Cgame_003E5__2;
					parameters2.startSlot = swapToMatchAction.slot2;
					parameters2.hasOtherChip = true;
					parameters2.otherChipType = powerupList.OtherPowerup(ChipType.SeekingMissle).chipType;
					parameters2.doCrossExplosion = true;
					parameters2.isHavingCarpet = _003ChasCarpet_003E5__3;
					powerupList.OtherPowerup(ChipType.SeekingMissle).RemoveFromGame();
					seekingMissileAction.Init(parameters2);
					_003Cgame_003E5__2.board.actionManager.AddAction(seekingMissileAction);
				}
				else if (powerupList.CountChipTypes(ChipType.HorizontalRocket, ChipType.VerticalRocket) == 1 && powerupList.CountChipType(ChipType.Bomb) == 1)
				{
					FlyCrossRocketAction flyCrossRocketAction = new FlyCrossRocketAction();
					FlyCrossRocketAction.FlyParams flyParams = default(FlyCrossRocketAction.FlyParams);
					flyParams.game = _003Cgame_003E5__2;
					for (int j = 0; j < powerupList.powerupList.Count; j++)
					{
						Chip item = powerupList.powerupList[j];
						flyParams.bombChips.Add(item);
					}
					flyParams.prelockAll = true;
					flyParams.originPosition = swapToMatchAction.slot2.position;
					flyParams.rows = 3;
					flyParams.columns = 3;
					flyParams.useDelayBetweenRowsAndColumns = true;
					flyParams.isHavingCarpet = _003ChasCarpet_003E5__3;
					flyParams.affectorExport = swapParams.affectorExport;
					flyCrossRocketAction.Init(flyParams);
					_003Cgame_003E5__2.board.actionManager.AddAction(flyCrossRocketAction);
				}
				else if (powerupList.CountChipTypes(ChipType.HorizontalRocket, ChipType.VerticalRocket) == 2)
				{
					FlyCrossRocketAction flyCrossRocketAction2 = new FlyCrossRocketAction();
					FlyCrossRocketAction.FlyParams flyParams2 = default(FlyCrossRocketAction.FlyParams);
					flyParams2.game = _003Cgame_003E5__2;
					for (int k = 0; k < powerupList.powerupList.Count; k++)
					{
						Chip item2 = powerupList.powerupList[k];
						flyParams2.bombChips.Add(item2);
					}
					flyParams2.prelockAll = true;
					flyParams2.originPosition = swapToMatchAction.slot2.position;
					flyParams2.rows = 1;
					flyParams2.columns = 1;
					flyParams2.isHavingCarpet = _003ChasCarpet_003E5__3;
					flyParams2.useDelayBetweenRowsAndColumns = false;
					flyCrossRocketAction2.Init(flyParams2);
					_003Cgame_003E5__2.board.actionManager.AddAction(flyCrossRocketAction2);
				}
				else if (powerupList.CountChipType(ChipType.Bomb) == 2)
				{
					ExplosionAction explosionAction = new ExplosionAction();
					ExplosionAction.ExplosionSettings settings = default(ExplosionAction.ExplosionSettings);
					settings.position = swapToMatchAction.slot2.position;
					settings.radius = 4;
					settings.bombChip = null;
					settings.isUsingBombAreaOfEffect = false;
					explosionAction.Init(_003Cgame_003E5__2, settings);
					for (int l = 0; l < powerupList.powerupList.Count; l++)
					{
						powerupList.powerupList[l].RemoveFromGame();
					}
					_003Cgame_003E5__2.board.actionManager.AddAction(explosionAction);
				}
				else if (powerupList.CountChipType(ChipType.DiscoBall) == 2)
				{
					ExplosionAction explosionAction2 = new ExplosionAction();
					ExplosionAction.ExplosionSettings settings2 = default(ExplosionAction.ExplosionSettings);
					settings2.position = swapToMatchAction.slot2.position;
					settings2.radius = Mathf.Max(_003Cgame_003E5__2.board.size.x, _003Cgame_003E5__2.board.size.y);
					settings2.bombChip = null;
					settings2.isHavingCarpet = _003ChasCarpet_003E5__3;
					explosionAction2.Init(_003Cgame_003E5__2, settings2);
					for (int m = 0; m < powerupList.powerupList.Count; m++)
					{
						powerupList.powerupList[m].RemoveFromGame();
					}
					_003Cgame_003E5__2.board.actionManager.AddAction(explosionAction2);
				}
				else if (powerupList.CountChipType(ChipType.DiscoBall) == 1)
				{
					Chip chip3 = powerupList.OtherPowerup(ChipType.DiscoBall);
					DiscoBallDestroyAction discoBallDestroyAction = new DiscoBallDestroyAction();
					DiscoBallDestroyAction.DiscoParams discoParams = new DiscoBallDestroyAction.DiscoParams();
					Chip chip4 = powerupList.PowerupOfType(ChipType.DiscoBall);
					Slot lastConnectedSlot = chip4.lastConnectedSlot;
					discoParams.replaceWithBombs = true;
					discoParams.InitWithItemColor(lastConnectedSlot, _003Cgame_003E5__2, _003Cgame_003E5__2.BestItemColorForDiscoBomb(discoParams.replaceWithBombs), discoParams.replaceWithBombs);
					discoParams.bombType = chip3.chipType;
					discoParams.otherBomb = chip3;
					discoParams.originBomb = chip4;
					discoParams.isHavingCarpet = _003ChasCarpet_003E5__3;
					discoBallDestroyAction.Init(discoParams);
					_003Cgame_003E5__2.board.actionManager.AddAction(discoBallDestroyAction);
				}
				else
				{
					SlotDestroyParams slotDestroyParams = new SlotDestroyParams();
					slotDestroyParams.isFromSwap = true;
					slotDestroyParams.swapParams = swapParams;
					for (int n = 0; n < powerupList.powerupList.Count; n++)
					{
						powerupList.powerupList[n].OnDestroySlotComponent(slotDestroyParams);
					}
				}
				swapToMatchAction.slotLock.UnlockAll();
				swapToMatchAction.isAlive = false;
				_003Cgame_003E5__2.OnUserMadeMove();
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

		private sealed class _003C_003Ec__DisplayClass13_0
		{
			public bool swapForwardComplete;

			internal void _003CDoSwap_003Eb__0()
			{
				swapForwardComplete = true;
			}
		}

		private sealed class _003C_003Ec__DisplayClass13_1
		{
			public bool swapBackComplete;

			internal void _003CDoSwap_003Eb__1()
			{
				swapBackComplete = true;
			}
		}

		private sealed class _003CDoSwap_003Ed__13 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public SwapToMatchAction _003C_003E4__this;

			private _003C_003Ec__DisplayClass13_0 _003C_003E8__1;

			private _003C_003Ec__DisplayClass13_1 _003C_003E8__2;

			private Match3Game _003Cgame_003E5__2;

			private Chip _003Cchip1_003E5__3;

			private Chip _003Cchip2_003E5__4;

			private PowerupList _003CpowerupList_003E5__5;

			private bool _003CisInstant_003E5__6;

			private SwapChipsAction.SwapChipsParams _003Cp_003E5__7;

			private EnumeratorsList _003CenumList_003E5__8;

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
			public _003CDoSwap_003Ed__13(int _003C_003E1__state)
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
				SwapToMatchAction swapToMatchAction = _003C_003E4__this;
				Matches matches;
				Island island;
				Island island2;
				bool flag;
				bool isActivatingPowerup;
				bool flag2;
				SwapParams swapParams;
				TransformBehaviour transformBehaviour3;
				TransformBehaviour transformBehaviour4;
				
				switch (num)
				{
				default:
					return false;
				case 0:
				{
					_003C_003E1__state = -1;
					_003C_003E8__1 = new _003C_003Ec__DisplayClass13_0();
					_003Cgame_003E5__2 = swapToMatchAction.slot1.game;
					if (swapToMatchAction.swapProperties.switchSlotsArgument.affectorExport.hasActions)
					{
						swapToMatchAction.slotLock.UnlockAll();
						_003Cgame_003E5__2.OnUserMadeMove();
						swapToMatchAction.swapProperties.switchSlotsArgument.affectorExport.ExecuteActions();
						swapToMatchAction.isAlive = false;
						return false;
					}
					_003Cchip1_003E5__3 = swapToMatchAction.slot1.GetSlotComponent<Chip>();
					_003Cchip2_003E5__4 = swapToMatchAction.slot2.GetSlotComponent<Chip>();
					Settings settings = swapToMatchAction.settings;
					swapToMatchAction.slot1.light.AddLight(settings.slot1LightIntensity);
					swapToMatchAction.slot2.light.AddLight(settings.slot2LightIntensity);
					_003CpowerupList_003E5__5 = new PowerupList();
					_003CpowerupList_003E5__5.Add(_003Cchip1_003E5__3);
					_003CpowerupList_003E5__5.Add(_003Cchip2_003E5__4);
					if (_003CpowerupList_003E5__5.isMixingPowerups)
					{
						_003CenumList_003E5__8 = new EnumeratorsList();
						_003CenumList_003E5__8.Add(swapToMatchAction.DoSwapToMixPowerups(_003CpowerupList_003E5__5));
						goto IL_0167;
					}
					_003C_003E8__1.swapForwardComplete = false;
					_003CisInstant_003E5__6 = swapToMatchAction.swapProperties.isInstant;
					_003Cp_003E5__7 = default(SwapChipsAction.SwapChipsParams);
					_003Cp_003E5__7.slot1 = swapToMatchAction.slot1;
					_003Cp_003E5__7.slot2 = swapToMatchAction.slot2;
					_003Cp_003E5__7.onComplete = _003C_003E8__1._003CDoSwap_003Eb__0;
					_003Cp_003E5__7.switchSlots = true;
					_003Cp_003E5__7.game = _003Cgame_003E5__2;
					if (_003CpowerupList_003E5__5.isMixingPowerups)
					{
						_003Cp_003E5__7.moveToSpecificPos = true;
						_003Cp_003E5__7.positionToMoveSlot1 = _003Cp_003E5__7.slot2.position;
						_003Cp_003E5__7.positionToMoveSlot2 = _003Cp_003E5__7.slot2.position;
					}

					if (!swapToMatchAction.swapProperties.switchSlotsArgument.isAlreadySwitched)
					{
						if (!_003CisInstant_003E5__6)
						{
							SwapChipsAction swapChipsAction = new SwapChipsAction();
							swapChipsAction.Init(_003Cp_003E5__7);
							_003Cgame_003E5__2.board.actionManager.AddAction(swapChipsAction);
							
							goto IL_02aa;
						}
						TransformBehaviour transformBehaviour = null;
						TransformBehaviour transformBehaviour2 = null;
						if (_003Cchip1_003E5__3 != null)
						{
							transformBehaviour = _003Cchip1_003E5__3.GetComponentBehaviour<TransformBehaviour>();
						}
						if (_003Cchip2_003E5__4 != null)
						{
							transformBehaviour2 = _003Cchip2_003E5__4.GetComponentBehaviour<TransformBehaviour>();
						}
						if (transformBehaviour != null)
						{
							transformBehaviour.localPosition = swapToMatchAction.slot2.localPositionOfCenter;
						}
						if (transformBehaviour2 != null)
						{
							transformBehaviour2.localPosition = swapToMatchAction.slot1.localPositionOfCenter;
						}
						goto IL_0321;
					}
					goto IL_0365;
				}
				case 1:
					_003C_003E1__state = -1;
					goto IL_0167;
				case 2:
					_003C_003E1__state = -1;
					goto IL_02aa;
				case 3:
					{
						_003C_003E1__state = -1;
						goto IL_0779;
					}
					IL_0779:
					if (!_003C_003E8__2.swapBackComplete)
					{
						_003C_003E2__current = null;
						_003C_003E1__state = 3;
						return true;
					}
					_003C_003E8__2 = null;
					break;
					IL_02aa:
					if (!_003C_003E8__1.swapForwardComplete)
					{
						
						_003C_003E2__current = null;
						_003C_003E1__state = 2;
						return true;
					}
					goto IL_0321;
					IL_0321:
					swapToMatchAction.slot1.RemoveComponent(_003Cchip1_003E5__3);
					swapToMatchAction.slot2.RemoveComponent(_003Cchip2_003E5__4);
					swapToMatchAction.slot1.AddComponent(_003Cchip2_003E5__4);
					swapToMatchAction.slot2.AddComponent(_003Cchip1_003E5__3);
					goto IL_0365;
					IL_0167:
					if (_003CenumList_003E5__8.Update())
					{
						_003C_003E2__current = null;
						_003C_003E1__state = 1;
						return true;
					}
					return false;
					IL_0365:
					swapToMatchAction.slotLock.Unlock(swapToMatchAction.slot1);
					swapToMatchAction.slotLock.Unlock(swapToMatchAction.slot2);
					matches = _003Cgame_003E5__2.board.findMatchesOutside.FindAllMatches();
					island = matches.GetIsland(swapToMatchAction.slot1.position);
					island2 = matches.GetIsland(swapToMatchAction.slot2.position);
					swapToMatchAction.allSlotsInMatch.Clear();
					if (island != null)
					{
						swapToMatchAction.allSlotsInMatch.AddRange(island.allSlots);
					}
					if (island2 != null)
					{
						swapToMatchAction.allSlotsInMatch.AddRange(island2.allSlots);
					}
					flag = (island != null || island2 != null);
					isActivatingPowerup = _003CpowerupList_003E5__5.isActivatingPowerup;
					flag2 = (!flag && !isActivatingPowerup);
					swapParams = new SwapParams();
					swapParams.startPosition = swapToMatchAction.slot1.position;
					swapParams.swipeToPosition = swapToMatchAction.slot2.position;
					swapParams.affectorExport = swapToMatchAction.swapProperties.switchSlotsArgument.affectorExport;
					if (!flag2)
					{
						swapToMatchAction.allSlotsInMatch.Add(swapToMatchAction.slot1);
						swapToMatchAction.allSlotsInMatch.Add(swapToMatchAction.slot2);
					}
					if (_003CisInstant_003E5__6 && !flag2)
					{
						for (int i = 0; i < swapToMatchAction.allSlotsInMatch.Count; i++)
						{
							Slot slot = swapToMatchAction.allSlotsInMatch[i];
							slot.offsetPosition = Vector3.zero;
							slot.positionIntegrator.SetPosition(Vector3.zero);
							slot.prevOffsetPosition = Vector3.zero;
							Chip slotComponent = slot.GetSlotComponent<Chip>();
							if (slotComponent != null)
							{
								TransformBehaviour componentBehaviour = slotComponent.GetComponentBehaviour<TransformBehaviour>();
								if (!(componentBehaviour == null))
								{
									componentBehaviour.slotOffsetPosition = slot.offsetPosition;
								}
							}
						}
					}
					var hasEnergy = EnergyManager.Instance.HasEnergyForOneLife();
					
					if (!flag2 && hasEnergy)
					{
						_003Cgame_003E5__2.OnUserMadeMove();
					}

					
					var energyConsumed = false;
					if (flag && hasEnergy)
					{
						if (island != null)
						{
							island.isFromSwap = true;
						}
						if (island2 != null)
						{
							island2.isFromSwap = true;
						}
						
						EnergyManager.Instance.SpendEnergy(10);
						energyConsumed = true;
						_003Cgame_003E5__2.ProcessMatches(matches, swapParams);
					}
					if(hasEnergy)
					{
						if (_003CpowerupList_003E5__5.isMixingDiscoBallWithColorElement)
						{
							DiscoBallDestroyAction discoBallDestroyAction = new DiscoBallDestroyAction();
							DiscoBallDestroyAction.DiscoParams discoParams = new DiscoBallDestroyAction.DiscoParams();
							Slot lastConnectedSlot = _003CpowerupList_003E5__5.FirstPowerup.lastConnectedSlot;
							discoParams.replaceWithBombs = false;
							discoParams.InitWithItemColor(lastConnectedSlot, _003Cgame_003E5__2, _003CpowerupList_003E5__5.mixingColor, discoParams.replaceWithBombs);
							discoParams.originBomb = _003CpowerupList_003E5__5.FirstPowerup;
							discoParams.isInstant = _003CisInstant_003E5__6;
							discoParams.bolts = swapToMatchAction.swapProperties.bolts;
							discoParams.affectorDuration = swapToMatchAction.swapProperties.switchSlotsArgument.affectorDuration;
							discoBallDestroyAction.Init(discoParams);
							_003Cgame_003E5__2.board.actionManager.AddAction(discoBallDestroyAction);
							swapToMatchAction.swapProperties.bolts = null;

							if (!energyConsumed)
							{
								EnergyManager.Instance.SpendEnergy(10);
								energyConsumed = true;
							}
							
							break;
						}
						if (_003CpowerupList_003E5__5.isContainingSingleActivateablePowerup)
						{
							SlotDestroyParams slotDestroyParams = new SlotDestroyParams();
							slotDestroyParams.isFromSwap = true;
							slotDestroyParams.swapParams = swapParams;
							_003CpowerupList_003E5__5.FirstPowerup.lastConnectedSlot.OnDestroySlot(slotDestroyParams);
							
							if (!energyConsumed)
							{
								EnergyManager.Instance.SpendEnergy(10);
								energyConsumed = true;
							}
							break;
						}
						if (!flag2 || swapToMatchAction.swapProperties.switchSlotsArgument.isAlreadySwitched)
						{
							break;
						}
					}
					swapToMatchAction.slotLock.LockSlot(swapToMatchAction.slot1);
					swapToMatchAction.slotLock.LockSlot(swapToMatchAction.slot2);
					swapToMatchAction.slot1.RemoveComponent(_003Cchip2_003E5__4);
					swapToMatchAction.slot2.RemoveComponent(_003Cchip1_003E5__3);
					swapToMatchAction.slot1.AddComponent(_003Cchip1_003E5__3);
					swapToMatchAction.slot2.AddComponent(_003Cchip2_003E5__4);
					if (!_003CisInstant_003E5__6)
					{
						_003C_003E8__2 = new _003C_003Ec__DisplayClass13_1();
						_003C_003E8__2.swapBackComplete = false;
						SwapChipsAction swapChipsAction2 = new SwapChipsAction();
						_003Cp_003E5__7.switchSlots = false;
						_003Cp_003E5__7.onComplete = _003C_003E8__2._003CDoSwap_003Eb__1;
						swapChipsAction2.Init(_003Cp_003E5__7);
						_003Cgame_003E5__2.board.actionManager.AddAction(swapChipsAction2);
						goto IL_0779;
					}
					transformBehaviour3 = null;
					transformBehaviour4 = null;
					if (_003Cchip1_003E5__3 != null)
					{
						transformBehaviour3 = _003Cchip1_003E5__3.GetComponentBehaviour<TransformBehaviour>();
					}
					if (_003Cchip2_003E5__4 != null)
					{
						transformBehaviour4 = _003Cchip2_003E5__4.GetComponentBehaviour<TransformBehaviour>();
					}
					if (transformBehaviour3 != null)
					{
						transformBehaviour3.localPosition = swapToMatchAction.slot1.localPositionOfCenter;
					}
					if (transformBehaviour4 != null)
					{
						transformBehaviour4.localPosition = swapToMatchAction.slot2.localPositionOfCenter;
					}
					break;
				}
				DiscoBallAffector.RemoveFromGame(swapToMatchAction.swapProperties.bolts);
				swapToMatchAction.lockContainer.UnlockAll();
				swapToMatchAction.isAlive = false;
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

		private Slot slot1;

		private Slot slot2;

		private Lock slotLock;

		private IEnumerator swapEnumerator;

		private SwapActionProperties swapProperties;

		private List<Slot> allSlotsInMatch = new List<Slot>();

		public Settings settings => Match3Settings.instance.swapToMatchActionSettings;

		public void Init(SwapActionProperties swapProperties)
		{
			slot1 = swapProperties.slot1;
			slot2 = swapProperties.slot2;
			this.swapProperties = swapProperties;
			slotLock = lockContainer.NewLock();
			slotLock.isChipGravitySuspended = true;
			slotLock.isSlotGravitySuspended = true;
			slotLock.isDestroySuspended = true;
			slotLock.isSlotMatchingSuspended = true;
			slotLock.LockSlot(slot1);
			slotLock.LockSlot(slot2);
		}

		private IEnumerator DoSwapToMixPowerups(PowerupList powerupList)
		{
			return new _003CDoSwapToMixPowerups_003Ed__11(0)
			{
				_003C_003E4__this = this,
				powerupList = powerupList
			};
		}

		private IEnumerator DoSwap()
		{
			return new _003CDoSwap_003Ed__13(0)
			{
				_003C_003E4__this = this
			};
		}

		public override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			if (swapEnumerator == null)
			{
				swapEnumerator = DoSwap();
			}
			swapEnumerator.MoveNext();
		}
	}
}
