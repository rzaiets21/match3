using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class ConveyorBeltBehaviour : MonoBehaviour
	{
		[SerializeField]
		private ComponentPool pool = new ComponentPool();

		private Match3Game game;

		[NonSerialized]
		public PipeBehaviour entrancePipe;

		[NonSerialized]
		public PipeBehaviour exitPipe;

		private List<ConveyorBeltSegment> segments = new List<ConveyorBeltSegment>();

		public void Init(Match3Game game, LevelDefinition.ConveyorBelt conveyorBeltDef, int index)
		{
			base.transform.localPosition = Vector3.zero;
			this.game = game;
			List<LevelDefinition.ConveyorBeltLinearSegment> segmentList = conveyorBeltDef.segmentList;
			for (int i = 0; i < segmentList.Count; i++)
			{
				LevelDefinition.ConveyorBeltLinearSegment segment = segmentList[i];
				InitSegment(segment);
			}
			if (!conveyorBeltDef.isLoop)
			{
				Color color = Match3Settings.instance.pipeSettings.GetColor(index);
				exitPipe = game.CreatePipeDontAddToSlot();
				exitPipe.Init(game.LocalPositionOfCenter(conveyorBeltDef.firstPosition), conveyorBeltDef.firstSegment.direction.ToVector3(), isExit: true);
				exitPipe.SetColor(color);
				if (game.GetSlot(conveyorBeltDef.firstPosition - conveyorBeltDef.firstSegment.direction) != null)
				{
					GGUtil.SetActive(exitPipe, active: false);
				}
				entrancePipe = game.CreatePipeDontAddToSlot();
				entrancePipe.Init(game.LocalPositionOfCenter(conveyorBeltDef.lastPosition), conveyorBeltDef.lastSegment.direction.ToVector3(), isExit: false);
				entrancePipe.SetColor(color);
				if (game.GetSlot(conveyorBeltDef.lastPosition + conveyorBeltDef.lastSegment.direction) != null)
				{
					GGUtil.SetActive(entrancePipe, active: false);
				}
				SetTile(0f);
			}
		}

		private void InitSegment(LevelDefinition.ConveyorBeltLinearSegment segment)
		{
		}

		public void SetTile(float tile)
		{
			for (int i = 0; i < segments.Count; i++)
			{
				segments[i].SetTile(tile);
			}
		}

		public Color GetColor()
		{
			int num = 0;
			if (num < segments.Count)
			{
				return segments[num].GetColor();
			}
			return Color.white;
		}

		public void SetColor(Color color)
		{
			for (int i = 0; i < segments.Count; i++)
			{
				segments[i].SetColor(color);
			}
		}
	}
}
