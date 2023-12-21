using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class CombineChipAffectors : ChipAffectorBase
	{
		public class Settings
		{
			public float durationToGetClose = 0.2f;

			public float maxScale = 1.2f;

			public float normalizedClosePosition = 0.5f;
		}

		private CompositeAffector.SwipedSlot swipedSlot;

		private List<LightingBolt> bolts = new List<LightingBolt>();

		private Match3Game game;

		private float time;

		private Settings settings => Match3Settings.instance.combineChipAffectorSettings;

		public override void Clear()
		{
			base.Clear();
			DiscoBallAffector.RemoveFromGame(bolts);
			bolts.Clear();
			Reset(swipedSlot.slot);
			Reset(swipedSlot.mixSlot);
		}

		private void Reset(Slot slot)
		{
			slot.offsetScale = Vector3.one;
			slot.offsetPosition = Vector3.zero;
		}

		public void Init(CompositeAffector.SwipedSlot swipedSlot, Match3Game game)
		{
			this.swipedSlot = swipedSlot;
			this.game = game;
			LightingBolt lightingBolt = game.CreateLightingBoltPowerup();
			lightingBolt.Init(swipedSlot.slot, swipedSlot.mixSlot);
			bolts.Add(lightingBolt);
			Slot slot = swipedSlot.slot;
			Slot mixSlot = swipedSlot.mixSlot;
		}

		public override void Update()
		{
			base.Update();
			time += Time.deltaTime;
			LightingBolt lightingBolt = bolts[0];
			Slot slot = swipedSlot.slot;
			Slot mixSlot = swipedSlot.mixSlot;
			Settings settings = this.settings;
			float t = Mathf.InverseLerp(0f, settings.durationToGetClose, time);
			float d = Mathf.Lerp(1f, settings.maxScale, t);
			slot.offsetPosition = Vector3.Lerp(t: Mathf.Lerp(0f, settings.normalizedClosePosition, t), a: slot.localPositionOfCenter, b: mixSlot.localPositionOfCenter) - slot.localPositionOfCenter;
			mixSlot.offsetScale = Vector3.one * d;
			lightingBolt.SetStartPosition(slot.localPositionOfCenter + slot.offsetPosition);
			lightingBolt.SetEndPosition(mixSlot.localPositionOfCenter + mixSlot.offsetPosition);
		}
	}
}
