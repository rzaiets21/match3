namespace GGMatch3
{
	public class EmptyConveyorSpace : SlotComponent
	{
		public override bool isMoveIntoSlotSuspended => true;

		public override bool isSlotGravitySuspended => true;

		public override bool isSlotSwapSuspended => true;

		public override bool isPreventingOtherChipsToFallIntoSlot => true;

		public override bool isPreventingGravity => true;

		public override bool isCreatePowerupWithThisSlotSuspended => true;

		public override bool isMovingWithConveyor => true;

		public override bool isAttachGrowingElementSuspended => true;

		public override bool isPlaceBubbleSuspended => true;
	}
}
