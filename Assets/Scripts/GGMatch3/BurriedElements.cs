using System.Collections.Generic;

namespace GGMatch3
{
	public class BurriedElements
	{
		private Match3Game game;

		public List<BurriedElementPiece> elementPieces = new List<BurriedElementPiece>();

		private List<BurriedElementPiece> tempList = new List<BurriedElementPiece>();

		public void Init(Match3Game game, LevelDefinition level)
		{
			List<LevelDefinition.BurriedElement> elements = level.burriedElements.elements;
			LevelDefinition.ConveyorBelts conveyorBelts = level.GetConveyorBelts();
			for (int i = 0; i < elements.Count; i++)
			{
				LevelDefinition.BurriedElement burriedElement = elements[i];
				if (conveyorBelts.IsPartOfConveyor(burriedElement.position))
				{
					Slot slot = game.GetSlot(burriedElement.position);
					game.AddBurriedElementSlot(slot, burriedElement);
				}
				else
				{
					BurriedElementPiece burriedElementPiece = new BurriedElementPiece();
					burriedElementPiece.Init(game, this, burriedElement);
					elementPieces.Add(burriedElementPiece);
				}
			}
		}

		public bool IsCompatibleWithPickupGoal(Slot slot, Match3Goals.ChipTypeDef chipTypeDef)
		{
			if (chipTypeDef.chipType != ChipType.BurriedElement)
			{
				return false;
			}
			for (int i = 0; i < elementPieces.Count; i++)
			{
				if (elementPieces[i].IsCompatibleWithPickupGoal(slot, chipTypeDef))
				{
					return true;
				}
			}
			return false;
		}

		public bool ContainsPosition(IntVector2 position)
		{
			for (int i = 0; i < elementPieces.Count; i++)
			{
				if (elementPieces[i].ContainsPosition(position))
				{
					return true;
				}
			}
			return false;
		}

		public void AddToGoalsAtStart(Match3Goals goals)
		{
			Match3Goals.ChipTypeDef chipTypeDef = default(Match3Goals.ChipTypeDef);
			chipTypeDef.chipType = ChipType.BurriedElement;
			chipTypeDef.itemColor = ItemColor.Unknown;
			goals.GetChipTypeCounter(chipTypeDef).countAtStart += elementPieces.Count;
		}

		public void Remove(BurriedElementPiece piece)
		{
			elementPieces.Remove(piece);
		}

		public void WobbleAll()
		{
			for (int i = 0; i < elementPieces.Count; i++)
			{
				elementPieces[i].Wobble(Match3Settings.instance.chipWobbleSettings);
			}
		}

		public void OnSlateDestroyed(Slot slot, SlotDestroyParams destroyParams)
		{
			tempList.Clear();
			tempList.AddRange(elementPieces);
			for (int i = 0; i < tempList.Count; i++)
			{
				tempList[i].OnSlateDestroyed(slot, destroyParams);
			}
		}

		public void Update(float deltaTime)
		{
			tempList.Clear();
			tempList.AddRange(elementPieces);
			for (int i = 0; i < tempList.Count; i++)
			{
				tempList[i].Update(deltaTime);
			}
		}
	}
}
