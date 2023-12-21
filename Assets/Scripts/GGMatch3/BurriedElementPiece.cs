using System;
using UnityEngine;

namespace GGMatch3
{
	public class BurriedElementPiece
	{
		[Serializable]
		public class Settings
		{
			public Color colorWhenBurried = Color.white;

			public Color colorWhenDugOut = Color.white;
		}

		public LevelDefinition.BurriedElement elementDefinition;

		private Match3Game game;
		public IntVector2 position;
		public IntVector2 size;

		public BurriedElementBehaviour burriedElementBehaviour;

		private BurriedElements burriedElements;

		private WobbleAnimation wobbleAnimation = new WobbleAnimation();

		public Settings settings => Match3Settings.instance.burriedElementPieceSettings;

		public TransformBehaviour transformBehaviour
		{
			get
			{
				if (burriedElementBehaviour == null)
				{
					return null;
				}
				return burriedElementBehaviour.GetComponent<TransformBehaviour>();
			}
		}

		public void Wobble(WobbleAnimation.Settings settings)
		{
			if (settings != null && !wobbleAnimation.isActive)
			{
				wobbleAnimation.Init(settings, burriedElementBehaviour.GetComponent<TransformBehaviour>());
			}
		}

		public void Init(Match3Game game, BurriedElements burriedElements, LevelDefinition.BurriedElement elementDefinition)
		{
			this.game = game;
			this.elementDefinition = elementDefinition;
			this.burriedElements = burriedElements;
			position = elementDefinition.position;
			size = elementDefinition.size;
			BurriedElementBehaviour burriedElementBehaviour = this.burriedElementBehaviour = game.CreateBurriedElement();
			if (burriedElementBehaviour != null)
			{
				burriedElementBehaviour.Init(elementDefinition);
				GGUtil.SetActive(burriedElementBehaviour, active: true);
				TransformBehaviour component = burriedElementBehaviour.GetComponent<TransformBehaviour>();
				Vector3 a = game.LocalPositionOfCenter(elementDefinition.position);
				Vector3 b = game.LocalPositionOfCenter(elementDefinition.oppositeCornerPosition);
				Vector3 vector2 = component.localPosition = Vector3.Lerp(a, b, 0.5f);
				component.SetColor(settings.colorWhenBurried);
			}
		}

		public bool IsCompatibleWithPickupGoal(Slot slot, Match3Goals.ChipTypeDef chipTypeDef)
		{
			if (chipTypeDef.chipType != ChipType.BurriedElement)
			{
				return false;
			}
			if (!ContainsPosition(slot.position))
			{
				return false;
			}
			if (!IsSlotBlocking(slot))
			{
				return false;
			}
			return true;
		}

		public bool ContainsPosition(IntVector2 position)
		{
			return elementDefinition.ContainsPosition(position);
		}

		public bool IsSlotBlocking(Slot slot)
		{
			if (slot == null)
			{
				return false;
			}
			if (!slot.isBlockingBurriedElement)
			{
				return false;
			}
			if (slot.totalBlockerLevel > 0)
			{
				return true;
			}
			return false;
		}

		private void SetColor(Color color)
		{
			TransformBehaviour transformBehaviour = this.transformBehaviour;
			if (!(transformBehaviour == null))
			{
				transformBehaviour.SetColor(color);
			}
		}

		public void OnSlateDestroyed(Slot destroyedSlot, SlotDestroyParams destroyParams)
		{
			IntVector2 position = elementDefinition.position;
			IntVector2 oppositeCornerPosition = elementDefinition.oppositeCornerPosition;
			int num = Mathf.Min(position.x, oppositeCornerPosition.x);
			int num2 = Mathf.Max(position.x, oppositeCornerPosition.x);
			int num3 = Mathf.Min(position.y, oppositeCornerPosition.y);
			int num4 = Mathf.Max(position.y, oppositeCornerPosition.y);
			if (Application.isEditor)
			{
				SetColor(settings.colorWhenBurried);
			}
			for (int i = num; i <= num2; i++)
			{
				for (int j = num3; j <= num4; j++)
				{
					Slot slot = game.GetSlot(new IntVector2(i, j));
					if (IsSlotBlocking(slot))
					{
						return;
					}
				}
			}
			SetColor(settings.colorWhenDugOut);
			RemoveFromElements();
			Match3Goals.ChipTypeDef chipTypeDef = default(Match3Goals.ChipTypeDef);
			chipTypeDef.chipType = ChipType.BurriedElement;
			chipTypeDef.itemColor = ItemColor.Unknown;
			Match3Goals.GoalBase activeGoal = game.goals.GetActiveGoal(chipTypeDef);
			GoalCollectParams goalCollectParams = default(GoalCollectParams);
			goalCollectParams.goal = activeGoal;
			CollectBurriedElementAction.CollectGoalParams collectParams = default(CollectBurriedElementAction.CollectGoalParams);
			collectParams.burriedElement = this;
			collectParams.destroyParams = null;
			collectParams.game = game;
			collectParams.goal = activeGoal;
			collectParams.slotToLock = destroyedSlot;
			if (destroyParams != null)
			{
				collectParams.explosionCentre = destroyParams.explosionCentre;
			}
			CollectBurriedElementAction collectBurriedElementAction = new CollectBurriedElementAction();
			collectBurriedElementAction.Init(collectParams);
			game.board.actionManager.AddAction(collectBurriedElementAction);
		}

		public void Update(float deltaTime)
		{
			OnSlateDestroyed(null, null);
			wobbleAnimation.Update(deltaTime);
		}

		private void RemoveFromElements()
		{
			burriedElements.Remove(this);
		}

		public void RemoveFromGame()
		{
			RemoveFromElements();
			if (burriedElementBehaviour != null)
			{
				burriedElementBehaviour.RemoveFromGame();
			}
		}
	}
}
