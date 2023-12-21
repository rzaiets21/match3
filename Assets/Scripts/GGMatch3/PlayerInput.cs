using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace GGMatch3
{
	public class PlayerInput : MonoBehaviour
	{
		public struct AffectorUpdateParams
		{
			public Vector3 currentPosition;

			public MouseParams mouseParams;

			public PlayerInput input;
		}

		public class AffectorBase
		{
			public float affectorDuration;

			private readonly bool _003CwantsToEnd_003Ek__BackingField;

			private readonly float _003CminAffectorDuration_003Ek__BackingField;

			public virtual bool canFinish => affectorDuration >= minAffectorDuration;

			public virtual bool wantsToEnd => _003CwantsToEnd_003Ek__BackingField;

			public virtual float minAffectorDuration => _003CminAffectorDuration_003Ek__BackingField;

			public virtual void ReleaseLocks()
			{
			}

			public virtual void ApplyLocks()
			{
			}

			public virtual void Clear()
			{
			}

			public virtual void OnUpdate(AffectorUpdateParams updateParams)
			{
			}

			public virtual void OnBeforeDestroy()
			{
			}

			public virtual void AddToSwitchSlotArguments(ref Match3Game.SwitchSlotsArguments switchSlotsArguments)
			{
				switchSlotsArguments.affectorDuration = affectorDuration;
			}
		}

		public class ExplosionAffector : AffectorBase
		{
			[Serializable]
			public class Settings
			{
				public int radius = 2;

				public float maxDistance = 10f;

				public FloatRange displaceRange = new FloatRange(0f, 1f);

				public AnimationCurve displaceCurve;

				public float displacePull = 10f;

				public float angleSpeed = 100f;

				public float phaseOffsetMult = 1f;

				public float amplitude = 0.05f;

				public float originScale = 2f;

				public float lightIntensity = 1f;

				public float distanceDelay = 0.05f;

				public FloatRange lightIntensityRange;

				public float maxLightDistance = 5f;

				public float timeToFullIntensity;

				public AnimationCurve intensityCurve;

				public bool lockLine;

				public FloatRange scaleRange = new FloatRange(1f, 0.5f);

				public float orthoScaleInfluence = 0.2f;
			}
		}

		public class MoveFromLineAffector : AffectorBase
		{
			[Serializable]
			public class Settings
			{
				public float maxDistance = 10f;

				public FloatRange displaceRange = new FloatRange(0f, 1f);

				public AnimationCurve displaceCurve;

				public float affectedOrtho = 1f;

				public float displacePull = 10f;

				public float angleSpeed = 100f;

				public float phaseOffsetMult = 1f;

				public float amplitude = 0.05f;

				public float originScale = 2f;

				public float lightIntensity = 1f;

				public float distanceDelay = 0.05f;

				public FloatRange lightIntensityRange;

				public float maxLightDistance = 5f;

				public float timeToFullIntensity;

				public AnimationCurve intensityCurve;

				public bool lockLine;

				public FloatRange scaleRange = new FloatRange(1f, 0.5f);

				public float orthoScaleInfluence = 0.25f;
			}
		}

		[Serializable]
		public class Settings
		{
			public bool tapToDestroy;

			public float maxDistance = 1f;

			public float maxDisplace = 1f;

			public float maxDistanceCurrentPos = 1f;

			public float maxDisplaceFwd = 1f;

			public float maxDisplaceBck = 1f;

			public float noDragFactor = 1f;

			public float maxAcceleration;

			public Vector3 displaceScale;

			public AnimationCurve displaceCurve;

			public AnimationCurve velocityDisplaceCurve;

			public AnimationCurve scaleCurve;

			public float maxOffsetVelocity = 0.2f;

			public float stiffness = 1f;

			public float dampingFactor = 1f;

			public float displacePull = 0.9f;

			public bool enableBouncyMode = true;

			public bool inputDirectMode;

			public bool switchImmediateIfPossible;

			public bool switchSlotPosition;

			public bool inputLineMode;

			public bool useMouse1ForBouncy;

			public float lightIntensityForMatch;

			public float lightIntensityForMatchOff;

			public bool addPowerupVis;

			public float addedLightIntensityForMatch;

			public float velocityDisplace;

			public float velocityDisplaceFwd;

			public float velocityDisplaceBck;

			public float centreMaxDistance;

			public float centreOffset;

			public float bckMaxDistanceX;

			public float bckMaxDistanceY;

			public FloatRange bckOffsetX;

			public FloatRange bckOffsetY;

			public AnimationCurve bckCurveX;

			public AnimationCurve bckCurveY;

			public float fwdMaxDistanceX;

			public float fwdMaxDistanceY;

			public FloatRange fwdOffsetX;

			public FloatRange fwdOffsetY;

			public float totalOffsetForFirstSlot = 1f;

			public float totalOffsetForFirstSlotMatching = 1f;

			public float directionInfluence;

			public float directionInfluenceFwd;

			public AnimationCurve pullCurve;

			public float directionInfluenceControl;

			public bool disableLightingInAffectors;

			public bool useSimpleLineBolts;

			public bool disableBombLighting;
		}

		public class MouseParams
		{
			public enum State
			{
				Touch,
				TapActivated,
				SwapActivated,
				SwapToNothingActivated,
				HasNoEnergy,
				CancelSwap,
				SwapToNoMatchActivated
			}

			public State state;

			public bool isMouseDown;

			public bool isSlotSwitched;

			public Vector3 mouseDownPositon;

			public Vector3 mouseDownUIPosition;

			public int touchId;

			public Slot firstHitSlot;

			public Chip chip;

			public Slot slotToSwitchWith;

			public Slot lastTestedSlotToSwitchWith;

			public Chip chipToSwitchWith;

			public MixClass mixClass = new MixClass();

			public bool isSlotToSwitchWithOffsetPositionSet;

			public Vector3 slotToSwitchWithOffsetPosition;

			public bool isMatching;

			public List<Slot> affectedSlotsForMatch = new List<Slot>();

			public List<Chip> affectedChipsForMatch = new List<Chip>();

			public List<Slot> affectedSlotsForMix = new List<Slot>();

			public IEnumerator mouseUpEnum;

			private AffectorBase activeAffector_;

			public AffectorBase activeAffector
			{
				get
				{
					return activeAffector_;
				}
				set
				{
					if (activeAffector_ != value)
					{
						if (activeAffector_ != null)
						{
							activeAffector_.Clear();
						}
						activeAffector_ = value;
					}
				}
			}

			public void ResetAffectedSlotsForMatch(PlayerInput input)
			{
				for (int i = 0; i < affectedSlotsForMatch.Count; i++)
				{
					affectedSlotsForMatch[i].light.RemoveLight(input.matchLight);
				}
				affectedSlotsForMatch.Clear();
				mixClass.Clear();
				affectedSlotsForMix.Clear();
				activeAffector = null;
			}

			public Vector2 MousePosition(InputHandler input)
			{
				return input.Position(touchId);
			}

			public void SetPointerIdToFirstDownPointer(InputHandler input)
			{
				InputHandler.PointerData pointerData = input.FirstDownPointer();
				if (pointerData != null)
				{
					touchId = pointerData.pointerId;
				}
			}

			public bool IsDown(InputHandler input)
			{
				if (isMouseDown)
				{
					return false;
				}
				InputHandler.PointerData pointerData = input.FirstDownPointer();
				if (pointerData == null)
				{
					return false;
				}
				if (Time.frameCount - pointerData.downFrame > 1)
				{
					return false;
				}
				return true;
			}

			public bool IsUp(InputHandler input)
			{
				if (!isMouseDown)
				{
					return false;
				}
				return !input.IsDown(touchId);
			}

			public void Clear()
			{
				isSlotSwitched = false;
				state = State.Touch;
				mouseUpEnum = null;
				activeAffector = null;
				affectedSlotsForMix.Clear();
				mixClass.Clear();
				isMouseDown = false;
				firstHitSlot = null;
				chip = null;
				slotToSwitchWith = null;
				chipToSwitchWith = null;
				affectedSlotsForMatch.Clear();
				affectedChipsForMatch.Clear();
				isMatching = false;
				isSlotToSwitchWithOffsetPositionSet = false;
				lastTestedSlotToSwitchWith = null;
			}

			public void Reset(Lock mainLock, PlayerInput input)
			{
				ResetAffectedSlotsForMatch(input);
				mainLock.UnlockAll();
				Slot.RemoveLocks(firstHitSlot, mainLock);
				Slot.RemoveLocks(slotToSwitchWith, mainLock);
				if (firstHitSlot != null)
				{
					firstHitSlot.light.RemoveLight(input.exchangeLight);
					firstHitSlot.light.RemoveLight(input.matchLight);
				}
				if (slotToSwitchWith != null)
				{
					slotToSwitchWith.light.RemoveLight(input.exchangeLight);
					slotToSwitchWith.light.RemoveLight(input.matchLight);
				}
				if (chip != null)
				{
					TransformBehaviour componentBehaviour = chip.GetComponentBehaviour<TransformBehaviour>();
					if (componentBehaviour != null)
					{
						componentBehaviour.localScale = Vector3.one;
						componentBehaviour.SetBrightness(1f);
					}
				}
				if (chipToSwitchWith != null)
				{
					TransformBehaviour componentBehaviour2 = chipToSwitchWith.GetComponentBehaviour<TransformBehaviour>();
					if (componentBehaviour2 != null)
					{
						componentBehaviour2.localScale = Vector3.one;
					}
				}
			}
		}

		public class MixClass
		{
			public List<Chip> chips = new List<Chip>();

			public void Clear()
			{
				chips.Clear();
			}

			public int CountOfType(ChipType type, ChipType type2)
			{
				int num = 0;
				for (int i = 0; i < chips.Count; i++)
				{
					Chip chip = chips[i];
					if (chip.chipType == type || chip.chipType == type2)
					{
						num++;
					}
				}
				return num;
			}

			public Chip GetChip(ChipType type)
			{
				for (int i = 0; i < chips.Count; i++)
				{
					Chip chip = chips[i];
					if (chip.chipType == type)
					{
						return chip;
					}
				}
				return null;
			}

			public Chip GetOtherChip(ChipType type)
			{
				for (int i = 0; i < chips.Count; i++)
				{
					Chip chip = chips[i];
					if (chip.chipType != type)
					{
						return chip;
					}
				}
				return null;
			}

			public int CountOfType(ChipType type)
			{
				int num = 0;
				for (int i = 0; i < chips.Count; i++)
				{
					if (chips[i].chipType == type)
					{
						num++;
					}
				}
				return num;
			}

			public void TryAdd(Chip chip)
			{
				if (chip != null && chip.isPowerup)
				{
					chips.Add(chip);
				}
			}
		}

		private sealed class _003CDoWaitForActiveAffector_003Ed__42 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public AffectorBase activeAffector;

			public PlayerInput _003C_003E4__this;

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
			public _003CDoWaitForActiveAffector_003Ed__42(int _003C_003E1__state)
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
				PlayerInput playerInput = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					break;
				case 1:
					_003C_003E1__state = -1;
					break;
				}
				if (!activeAffector.canFinish)
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				playerInput.OnMouseUp();
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

		[NonSerialized]
		public FindMatches findMatches = new FindMatches();

		private CompositeAffector compositeAffector = new CompositeAffector();

		[SerializeField]
		private Camera mainCamera;

		[SerializeField]
		private PlayerInputFollower follower;

		[SerializeField]
		private Transform match3ParentTransform;

		[SerializeField]
		private float sizeToSwitch = 0.5f;

		[SerializeField]
		private bool useUISizeToSwitch;

		[SerializeField]
		private float uiSizeToSwitch = 4f;

		[SerializeField]
		private float chipBrightness = 2f;

		[SerializeField]
		private float angleToSwitch = 0.85f;

		private MouseParams mouseParams = new MouseParams();

		private MouseParams followerMouseParams = new MouseParams();

		private MouseParams mouseParamsDisplace = new MouseParams();

		[NonSerialized]
		public Match3Game game;

		private Lock mainLock;

		private LightSlotComponent.PermanentLight matchLight = new LightSlotComponent.PermanentLight();

		private LightSlotComponent.PermanentLight exchangeLight = new LightSlotComponent.PermanentLight();

		private LockContainer lockContainer = new LockContainer();

		private ItemColor lastColor;

		public Settings settings => Match3Settings.instance.playerInputSettings;

		public bool isActive
		{
			get
			{
				if (mouseParams.mouseUpEnum == null)
				{
					if (mouseParams.firstHitSlot != null)
					{
						return mouseParams.isMouseDown;
					}
					return false;
				}
				return true;
			}
		}

		private InputHandler input => game.gameScreen.inputHandler;

		private void SetFollowerLocalPosition(Vector3 localPosition)
		{
			if (!(follower == null))
			{
				follower.transform.localPosition = localPosition;
			}
		}

		public void UpdateDisplaceIntegration()
		{
			Slot[] slots = game.board.slots;
			Settings settings = this.settings;
			foreach (Slot slot in slots)
			{
				if (slot != null)
				{
					slot.positionIntegrator.Update(Time.deltaTime, settings.dampingFactor, settings.stiffness);
					slot.offsetPosition = slot.positionIntegrator.currentPosition;
				}
			}
		}

		public void ResetDisplace()
		{
			Slot[] slots = game.board.slots;
			foreach (Slot slot in slots)
			{
				if (slot != null)
				{
					slot.prevOffsetPosition = (slot.offsetPosition = Vector3.zero);
					slot.offsetScale = Vector3.one;
					slot.positionIntegrator.ResetAcceleration();
				}
			}
		}

		public void ApplyDisplace(Slot firstHitSlot, Vector3 startPos, Vector3 currentPosition, Vector3 velocity, Slot ignoreSlot_, bool isNoDrag)
		{
			Settings settings = this.settings;
			if (settings.switchSlotPosition)
			{
				Slot firstHitSlot2 = mouseParams.firstHitSlot;
			}
			if (settings.switchSlotPosition)
			{
				Slot slotToSwitchWith = mouseParams.slotToSwitchWith;
			}
			Slot[] slots = game.board.slots;
			float maxDistance = settings.maxDistance;
			Vector3 normalized = velocity.normalized;
			foreach (Slot slot in slots)
			{
				if (slot == null)
				{
					continue;
				}
				if (slot.isSlotGravitySuspendedByComponent)
				{
					slot.prevOffsetPosition = (slot.offsetPosition = Vector3.zero);
					slot.offsetScale = Vector3.one;
					slot.positionIntegrator.ResetAcceleration();
					continue;
				}
				Vector3 b = slot.localPositionOfCenter;
				Chip slotComponent = slot.GetSlotComponent<Chip>();
				if (slotComponent != null)
				{
					TransformBehaviour componentBehaviour = slotComponent.GetComponentBehaviour<TransformBehaviour>();
					if (componentBehaviour != null)
					{
						b = componentBehaviour.localPosition;
					}
				}
				Vector3 vector = currentPosition - b;
				vector.z = 0f;
				float num = Mathf.Abs(vector.x) + Mathf.Abs(vector.y);
				if (num >= maxDistance)
				{
					slot.prevOffsetPosition = (slot.offsetPosition = Vector3.zero);
					slot.offsetScale = Vector3.one;
					slot.positionIntegrator.ResetAcceleration();
					continue;
				}
				float time = Mathf.InverseLerp(0f, maxDistance, num);
				float t = settings.displaceCurve.Evaluate(time);
				float num2 = Vector3.Dot(vector, normalized);
				float num3 = (num2 > 0f) ? settings.velocityDisplaceFwd : settings.velocityDisplaceBck;
				float d = Mathf.Lerp((num2 > 0f) ? settings.maxDisplaceFwd : settings.maxDisplaceBck, 0f, t);
				if (isNoDrag)
				{
					num3 *= settings.noDragFactor;
				}
				float f = Mathf.Lerp(num3, 0f, settings.velocityDisplaceCurve.Evaluate(time));
				f = Mathf.Sign(f) * Mathf.Min(Mathf.Abs(f), settings.maxOffsetVelocity);
				Vector3 b2 = normalized * f;
				Vector3 a = vector.normalized * d;
				if (velocity != Vector3.zero)
				{
					a = Vector3.zero;
				}
				Vector3 b3 = a + b2;
				slot.offsetPosition = Vector3.Lerp(slot.prevOffsetPosition, b3, settings.displacePull * Time.deltaTime);
				Mathf.Abs(slot.offsetPosition.x);
				Mathf.Abs(slot.offsetPosition.y);
				slot.prevOffsetPosition = slot.offsetPosition;
				slot.positionIntegrator.SetPosition(slot.offsetPosition);
				slot.offsetScale = Vector3.Lerp(settings.displaceScale, Vector3.one, settings.scaleCurve.Evaluate(time));
				IntVector2 intVector = slot.position - firstHitSlot.position;
				if ((!(Mathf.Abs(normalized.x) > 0f) || intVector.x != 0) && (!(Mathf.Abs(normalized.y) > 0f) || intVector.y != 0))
				{
					slot.offsetScale = Vector3.one;
				}
				if (float.IsNaN(slot.offsetPosition.x) || float.IsNaN(slot.offsetPosition.y))
				{
					UnityEngine.Debug.LogError("NAN");
				}
			}
		}

		public void SetCamera(Camera mainCamera)
		{
			this.mainCamera = mainCamera;
		}

		public void Init(Match3Game game)
		{
			this.game = game;
			mainLock = lockContainer.NewLock();
			mainLock.isSlotGravitySuspended = true;
			mainLock.isAvailableForSeekingMissileSuspended = true;
			mainLock.isSlotMatchingSuspended = true;
			mainLock.isAvailableForDiscoBombSuspended = true;
			mainLock.isDestroySuspended = true;
			mainLock.isChipGeneratorSuspended = true;
			mainLock.isChipGravitySuspended = true;
			mainLock.isAttachGrowingElementSuspended = true;
			findMatches.Init(game.board);
		}

		public Slot GetSlotFromMousePosition(Vector3 mousePosition)
		{
			Vector3 localPosition = LocalMatch3Pos(mousePosition);
			return GetSlot(localPosition);
		}

		private Vector3 LocalMatch3Pos(Vector3 mousePosScreen)
		{
			Ray ray = mainCamera.ScreenPointToRay(mousePosScreen);
			float distance = Mathf.Abs(match3ParentTransform.position.z - mainCamera.transform.position.z);
			Vector3 point = ray.GetPoint(distance);
			point.z = match3ParentTransform.position.z;
			return match3ParentTransform.InverseTransformPoint(point);
		}

		private Vector3 LocalUIPos(Vector3 mousePosScreen)
		{
			Ray ray = mainCamera.ScreenPointToRay(mousePosScreen);
			float distance = Mathf.Abs(match3ParentTransform.position.z - mainCamera.transform.position.z);
			Vector3 point = ray.GetPoint(distance);
			point.z = match3ParentTransform.position.z;
			Vector3 result = game.gameScreen.transform.InverseTransformPoint(point);
			result.z = 0f;
			return result;
		}

		private Slot GetSlot(Vector3 localPosition)
		{
			IntVector2 position = game.ClosestBoardPositionFromLocalPosition(localPosition);
			return game.GetSlot(position);
		}

		private void UpdateDisplace()
		{
			MouseParams mouseParams = mouseParamsDisplace;
			if (mouseParams.IsDown(input))
			{
				mouseParams.Clear();
				mouseParams.SetPointerIdToFirstDownPointer(input);
				Vector3 vector = LocalMatch3Pos(mouseParams.MousePosition(input));
				Slot slot = GetSlot(vector);
				mouseParams.isMouseDown = true;
				mouseParams.mouseDownPositon = vector;
				mouseParams.firstHitSlot = slot;
				if (slot == null || slot.isSlotSwapSuspended)
				{
					ResetDisplace();
					mouseParams.Clear();
				}
			}
			if (mouseParams.IsUp(input))
			{
				ResetDisplace();
				mouseParams.Clear();
			}
			if (settings.useMouse1ForBouncy && !this.mouseParams.isMouseDown)
			{
				UpdateDisplaceIntegration();
				return;
			}
			if (!mouseParams.isMouseDown)
			{
				UpdateDisplaceIntegration();
				return;
			}
			Vector3 a = LocalMatch3Pos(mouseParams.MousePosition(input));
			if (mouseParams.firstHitSlot == null || mouseParams.firstHitSlot.isSlotSwapSuspended)
			{
				a = mouseParams.mouseDownPositon;
			}
			Vector3 vector2 = a - mouseParams.mouseDownPositon;
			a = mouseParams.mouseDownPositon + vector2.normalized * Mathf.Min(vector2.magnitude, settings.maxDistanceCurrentPos);
			float num = a.x - mouseParams.mouseDownPositon.x;
			float num2 = a.y - mouseParams.mouseDownPositon.y;
			Vector3 zero = Vector3.zero;
			zero = ((!(Mathf.Abs(num) > Mathf.Abs(num2))) ? (Vector3.up * Mathf.Sign(num2)) : (Vector3.right * Mathf.Sign(num)));
			if (num == 0f && num2 == 0f)
			{
				zero = Vector3.zero;
			}
			float num3 = game.slotPhysicalSize.x * sizeToSwitch;
			bool isNoDrag = Mathf.Abs(num) < num3 && Mathf.Abs(num2) < num3;
			UpdateDisplaceIntegration();
			if (settings.inputLineMode)
			{
				return;
			}
			Slot ignoreSlot_ = null;
			if (settings.switchSlotPosition)
			{
				ignoreSlot_ = this.mouseParams.firstHitSlot;
			}
			if (!settings.switchImmediateIfPossible || this.mouseParams.isMouseDown)
			{
				AffectorBase activeAffector = this.mouseParams.activeAffector;
				if (activeAffector != null)
				{
					AffectorUpdateParams updateParams = default(AffectorUpdateParams);
					updateParams.currentPosition = a;
					updateParams.mouseParams = this.mouseParams;
					updateParams.input = this;
					activeAffector.OnUpdate(updateParams);
				}
				else
				{
					ApplyDisplace(mouseParams.firstHitSlot, mouseParams.mouseDownPositon, a, zero, ignoreSlot_, isNoDrag);
				}
			}
		}

		private void OnMouseUpStart()
		{
			AffectorBase activeAffector = mouseParams.activeAffector;
			if (activeAffector == null)
			{
				OnMouseUp();
			}
			else if (!activeAffector.canFinish)
			{
				mouseParams.mouseUpEnum = DoWaitForActiveAffector(activeAffector);
			}
			else
			{
				OnMouseUp();
			}
		}

		private IEnumerator DoWaitForActiveAffector(AffectorBase activeAffector)
		{
			return new _003CDoWaitForActiveAffector_003Ed__42(0)
			{
				_003C_003E4__this = this,
				activeAffector = activeAffector
			};
		}

		private void OnMouseUp()
		{
			AffectorBase activeAffector = mouseParams.activeAffector;
			bool isSlotSwitched = mouseParams.isSlotSwitched;
			if (mouseParams.firstHitSlot != null)
			{
				mouseParams.firstHitSlot.light.AddLight(exchangeLight.currentIntensity);
			}
			if (mouseParams.slotToSwitchWith != null)
			{
				mouseParams.slotToSwitchWith.light.AddLight(exchangeLight.currentIntensity);
			}
			for (int i = 0; i < mouseParams.affectedSlotsForMatch.Count; i++)
			{
				mouseParams.affectedSlotsForMatch[i].light.AddLight(matchLight.currentIntensity);
			}
			Match3Game.SwitchSlotsArguments switchSlotsArguments = default(Match3Game.SwitchSlotsArguments);
			switchSlotsArguments.isAlreadySwitched = isSlotSwitched;
			activeAffector?.AddToSwitchSlotArguments(ref switchSlotsArguments);
			lockContainer.UnlockAll();
			mainLock.UnlockAll();
			mouseParams.Reset(mainLock, this);
			if (mouseParams.firstHitSlot != null && mouseParams.slotToSwitchWith == null && !mouseParams.firstHitSlot.isSlotTouchingSuspended)
			{
				SwapParams swapParams = new SwapParams();
				swapParams.startPosition = (swapParams.swipeToPosition = mouseParams.firstHitSlot.position);
				swapParams.affectorExport = switchSlotsArguments.affectorExport;
				game.TapOnSlot(mouseParams.firstHitSlot.position, swapParams);
				switchSlotsArguments.Clear();
				mouseParams.Clear();
				return;
			}
			Slot firstHitSlot = mouseParams.firstHitSlot;
			Slot slotToSwitchWith = mouseParams.slotToSwitchWith;
			bool flag = false;
			if (firstHitSlot != null && slotToSwitchWith != null)
			{
				switchSlotsArguments.pos1 = firstHitSlot.position;
				switchSlotsArguments.pos2 = slotToSwitchWith.position;
				switchSlotsArguments.instant = true;
				flag = game.TrySwitchSlots(switchSlotsArguments);
				if (!flag)
				{
					DiscoBallAffector.RemoveFromGame(switchSlotsArguments.bolts);
				}
				else
				{
					activeAffector?.OnBeforeDestroy();
				}
			}
			else
			{
				DiscoBallAffector.RemoveFromGame(switchSlotsArguments.bolts);
			}
			mouseParams.Clear();
			if (flag)
			{
				switchSlotsArguments.affectorExport.ExecuteOnAfterDestroy();
			}
			switchSlotsArguments.Clear();
		}

		private void TryActivateTap(MouseParams mouseParams)
		{
			Slot firstHitSlot = mouseParams.firstHitSlot;
			if (firstHitSlot != null && mouseParams.slotToSwitchWith == null && mouseParams.state == MouseParams.State.Touch)
			{
				mainLock.UnlockAllAndSaveToTemporaryList();
				if (!firstHitSlot.canBeTappedToActivate)
				{
					mainLock.LockTemporaryListAndClear();
					return;
				}
				if (firstHitSlot.isSlotSwapSuspended)
				{
					mainLock.LockTemporaryListAndClear();
					return;
				}
				mainLock.LockTemporaryListAndClear();
				mouseParams.state = MouseParams.State.TapActivated;
				mainLock.LockSlot(firstHitSlot);
				CompositeAffector.InitArguments initArguments = new CompositeAffector.InitArguments();
				initArguments.game = game;
				initArguments.AddSwipedSlot(mouseParams.firstHitSlot);
				compositeAffector.Init(initArguments);
				mouseParams.activeAffector = compositeAffector;
				game.Play(GGSoundSystem.SFXType.ChipTap);
			}
		}

		private void UpdateFollower()
		{
			MouseParams mouseParams = followerMouseParams;
			if (mouseParams.IsDown(input))
			{
				mouseParams.SetPointerIdToFirstDownPointer(input);
				Vector3 followerLocalPosition = LocalMatch3Pos(mouseParams.MousePosition(input));
				if (follower != null)
				{
					follower.SetActive(active: false);
				}
				SetFollowerLocalPosition(followerLocalPosition);
				mouseParams.isMouseDown = true;
				if (follower != null)
				{
					follower.SetActive(active: true);
					follower.Clear();
				}
			}
			else
			{
				if (mouseParams.IsUp(input) && mouseParams.isMouseDown)
				{
					mouseParams.isMouseDown = false;
				}
				if (mouseParams.isMouseDown)
				{
					Vector3 followerLocalPosition2 = LocalMatch3Pos(mouseParams.MousePosition(input));
					SetFollowerLocalPosition(followerLocalPosition2);
				}
			}
		}

		private void UpdateSimple()
		{
			if (mouseParams.mouseUpEnum != null)
			{
				if (!mouseParams.mouseUpEnum.MoveNext())
				{
					mouseParams.mouseUpEnum = null;
				}
				return;
			}
			if (game.isUserInteractionSuspended)
			{
				lockContainer.UnlockAll();
				mouseParams.Reset(mainLock, this);
				mouseParams.Clear();
				return;
			}
			if (mouseParams.IsDown(input))
			{
				if (mouseParams.isMouseDown)
				{
					OnMouseUp();
					return;
				}
				mouseParams.Clear();
				mouseParams.SetPointerIdToFirstDownPointer(input);
				Vector3 vector = LocalMatch3Pos(mouseParams.MousePosition(input));
				DoTapParticleOnPosition(vector);
				Slot slot = GetSlot(vector);
				slot?.Wobble(Match3Settings.instance.chipWobbleSettings);
				if (slot == null || slot.isSlotSwapSuspended)
				{
					mouseParams.isMouseDown = true;
					mouseParams.firstHitSlot = null;
					mouseParams.state = MouseParams.State.Touch;
					return;
				}
				if (settings.tapToDestroy && Application.isEditor && slot.isSomethingMoveableByGravityInSlot && !slot.isDestroySuspended && !slot.isSlotGravitySuspended)
				{
					SlotDestroyParams destroyParams = new SlotDestroyParams();
					Chip slotComponent = slot.GetSlotComponent<Chip>();
					slot.OnDestroySlot(destroyParams);
					return;
				}
				mouseParams.isMouseDown = true;
				mouseParams.firstHitSlot = slot;
				mouseParams.chip = slot.GetSlotComponent<Chip>();
				if (mouseParams.chip != null)
				{
					mouseParams.chip.GetComponentBehaviour<TransformBehaviour>().SetBrightness(chipBrightness);
				}
				mouseParams.mouseDownPositon = vector;
				mouseParams.mouseDownUIPosition = LocalUIPos(mouseParams.MousePosition(input));
				mouseParams.state = MouseParams.State.Touch;
				mainLock.LockSlot(slot);
				TryActivateTap(mouseParams);
			}
			bool flag = mouseParams.IsUp(input) && mouseParams.isMouseDown;
			if (mouseParams.activeAffector != null && mouseParams.activeAffector.wantsToEnd && mouseParams.state == MouseParams.State.SwapActivated)
			{
				flag = true;
				mouseParams.isMouseDown = false;
			}
			if (flag)
			{
				if (mouseParams.firstHitSlot == null)
				{
					mouseParams.Clear();
				}
				else
				{
					OnMouseUpStart();
				}
			}
			else
			{
				if (!mouseParams.isMouseDown || mouseParams.firstHitSlot == null)
				{
					return;
				}
				if (mouseParams.state == MouseParams.State.Touch)
				{
					TryActivateTap(mouseParams);
				}
				if (mouseParams.slotToSwitchWith != null)
				{
					return;
				}
				Vector3 vector2 = LocalMatch3Pos(mouseParams.MousePosition(input));
				float num = vector2.x - mouseParams.mouseDownPositon.x;
				float num2 = vector2.y - mouseParams.mouseDownPositon.y;
				float num3 = game.slotPhysicalSize.x * sizeToSwitch;
				if (useUISizeToSwitch)
				{
					Vector3 vector3 = LocalUIPos(mouseParams.MousePosition(input));
					num = vector3.x - mouseParams.mouseDownUIPosition.x;
					num2 = vector3.y - mouseParams.mouseDownUIPosition.y;
					num3 = uiSizeToSwitch;
				}
				if (Mathf.Abs(num) > Mathf.Abs(num2) && Mathf.Abs(num) > num3)
				{
					if (Mathf.Abs(Vector3.Dot(Vector3.right, new Vector3(num, num2).normalized)) < angleToSwitch)
					{
						Cancel();
						return;
					}
					IntVector2 b = new IntVector2((int)Mathf.Sign(num), 0);
					IntVector2 intVector = mouseParams.firstHitSlot.position + b;
					Slot slot2 = game.GetSlot(intVector);
					TrySwitchSlotsSimple(slot2, intVector);
				}
				else if (Mathf.Abs(num2) > Mathf.Abs(num) && Mathf.Abs(num2) > num3)
				{
					if (Mathf.Abs(Vector3.Dot(Vector3.up, new Vector3(num, num2).normalized)) < angleToSwitch)
					{
						Cancel();
						return;
					}
					IntVector2 b2 = new IntVector2(0, (int)Mathf.Sign(num2));
					IntVector2 intVector2 = mouseParams.firstHitSlot.position + b2;
					Slot slot3 = game.GetSlot(intVector2);
					TrySwitchSlotsSimple(slot3, intVector2);
				}
			}
		}

		private void Cancel()
		{
			mouseParams.Reset(mainLock, this);
			ResetDisplace();
			mouseParams.Clear();
			lockContainer.UnlockAll();
			mouseParamsDisplace.Clear();
		}

		private void TrySwitchSlotsSimple(Slot slotToSwitchWith, IntVector2 otherSlotPosition)
		{
			mainLock.UnlockAllAndSaveToTemporaryList();
			AffectorBase activeAffector = mouseParams.activeAffector;
			activeAffector?.ReleaseLocks();
			if (TrySwitchSlotsSimpleInner(slotToSwitchWith))
			{
				DoSwipeParticlesBetweenSlots(mouseParams.firstHitSlot, slotToSwitchWith);
				return;
			}
			
			game.Play(GGSoundSystem.SFXType.ChipSwap);
			
			if (mouseParams.state == MouseParams.State.SwapToNothingActivated)
			{
				Slot firstHitSlot = mouseParams.firstHitSlot;
				Cancel();
				if (firstHitSlot != null)
				{
					game.TrySwitchSlots(firstHitSlot.position, otherSlotPosition, instant: false);
					DoSwipeParticlesBetweenSlots(firstHitSlot.position, otherSlotPosition);
				}
				else
				{
					game.TrySwitchSlots(firstHitSlot, slotToSwitchWith, instant: false);
				}
			}
			else if (mouseParams.state == MouseParams.State.CancelSwap)
			{
				Slot firstHitSlot2 = mouseParams.firstHitSlot;
				Cancel();
				if (firstHitSlot2 != null)
				{
					game.TryShowSwitchNotPossible(firstHitSlot2.position, otherSlotPosition);
					DoSwipeParticlesBetweenSlots(firstHitSlot2.position, otherSlotPosition);
				}
			}
			else
			{
				mainLock.LockTemporaryListAndClear();
				activeAffector?.ApplyLocks();
			}
		}

		private bool TrySwitchSlotsSimpleInner(Slot slotToSwitchWith)
		{
			if (!EnergyManager.Instance.HasEnergyForOneLife(false))
			{
				mouseParams.state = MouseParams.State.SwapToNothingActivated;
				return false;
			}
		
			Slot firstHitSlot = mouseParams.firstHitSlot;
			bool flag = mouseParams.state == MouseParams.State.TapActivated;
			if (slotToSwitchWith == mouseParams.slotToSwitchWith && mouseParams.slotToSwitchWith != null)
			{
				return false;
			}
			if (slotToSwitchWith == null)
			{
				if (!flag)
				{
					mouseParams.state = MouseParams.State.SwapToNothingActivated;
				}
				else
				{
					mouseParams.state = MouseParams.State.CancelSwap;
				}
				return false;
			}
			if (mouseParams.lastTestedSlotToSwitchWith == slotToSwitchWith)
			{
				return false;
			}
			mouseParams.lastTestedSlotToSwitchWith = slotToSwitchWith;
			if (slotToSwitchWith.isSlotSwapSuspended)
			{
				if (!flag)
				{
					mouseParams.state = MouseParams.State.SwapToNothingActivated;
				}
				else
				{
					mouseParams.state = MouseParams.State.CancelSwap;
				}
				return false;
			}
			if (firstHitSlot.isSlotSwipingSuspended)
			{
				if (!flag)
				{
					mouseParams.state = MouseParams.State.SwapToNothingActivated;
				}
				else
				{
					mouseParams.state = MouseParams.State.CancelSwap;
				}
				return false;
			}
			if (firstHitSlot.isSlotSwipingSuspendedForSlot(slotToSwitchWith) || firstHitSlot.isSwipeSuspendedTo(slotToSwitchWith))
			{
				if (!flag)
				{
					mouseParams.state = MouseParams.State.SwapToNothingActivated;
				}
				else
				{
					mouseParams.state = MouseParams.State.CancelSwap;
				}
				return false;
			}

			if (firstHitSlot.GetSlotComponent<Chip>() == null)
			{
				return false;
			}
			
			mainLock.UnlockAll();
			Slot.SwitchChips(firstHitSlot, slotToSwitchWith);
			Matches matches = findMatches.FindAllMatches();
			Island island = matches.GetIsland(firstHitSlot.position);
			Island island2 = matches.GetIsland(slotToSwitchWith.position);
			if (island2 == island)
			{
				island2 = null;
			}
			bool num = island != null || island2 != null;
			Slot.SwitchChips(firstHitSlot, slotToSwitchWith);
			MixClass mixClass = mouseParams.mixClass;
			mixClass.Clear();
			mixClass.TryAdd(firstHitSlot.GetSlotComponent<Chip>());
			mixClass.TryAdd(slotToSwitchWith.GetSlotComponent<Chip>());
			mainLock.LockSlot(firstHitSlot);
			if (!num && mixClass.chips.Count == 0)
			{
				mouseParams.state = MouseParams.State.SwapToNothingActivated;
				return false;
			}

			// if (!EnergyManager.Instance.HasEnergyForOneLife())
			// {
			// 	mouseParams.state = MouseParams.State.SwapToNothingActivated;
			// 	return false;
			// }
			
			Chip chip = null;
			Chip chip2 = null;
			bool flag2 = false;
			if (mixClass.chips.Count == 1 && mixClass.CountOfType(ChipType.DiscoBall) == 1)
			{
				chip = mixClass.chips[0];
				chip2 = ((firstHitSlot.GetSlotComponent<Chip>() != chip) ? firstHitSlot.GetSlotComponent<Chip>() : slotToSwitchWith.GetSlotComponent<Chip>());
				if (chip2 == null)
				{
					return false;
				}
				if (!chip2.canFormColorMatches)
				{
					return false;
				}
				flag2 = true;
			}
			mouseParams.slotToSwitchWith = slotToSwitchWith;
			mainLock.LockSlot(slotToSwitchWith);
			mouseParams.state = MouseParams.State.SwapActivated;
			CompositeAffector.InitArguments initArguments = new CompositeAffector.InitArguments();
			initArguments.game = game;
			if (mixClass.chips.Count == 2)
			{
				initArguments.AddSwipedSlot(firstHitSlot).SetMix(slotToSwitchWith);
			}
			else if (flag2)
			{
				mouseParams.isSlotSwitched = true;
				CompositeAffector.SwipedSlot swipedSlot = initArguments.AddSwipedSlot(chip.lastConnectedSlot);
				swipedSlot.isDiscoCombine = true;
				swipedSlot.mixSlot = chip2.lastConnectedSlot;
			}
			else
			{
				Slot.SwitchChips(firstHitSlot, slotToSwitchWith, changePosition: true);
				mouseParams.isSlotSwitched = true;
				CompositeAffector.SwipedSlot swipedSlot2 = initArguments.AddSwipedSlot(firstHitSlot);
				swipedSlot2.SetMatch(island);
				swipedSlot2.cameFromPosition = slotToSwitchWith.position;
				swipedSlot2.SetOtherChipMatch(slotToSwitchWith, island2);
				CompositeAffector.SwipedSlot swipedSlot3 = initArguments.AddSwipedSlot(slotToSwitchWith);
				swipedSlot3.SetMatch(island2);
				swipedSlot3.cameFromPosition = firstHitSlot.position;
				swipedSlot3.SetOtherChipMatch(firstHitSlot, island);
			}
			compositeAffector.Init(initArguments);
			mouseParams.activeAffector = compositeAffector;
			game.Play(GGSoundSystem.SFXType.ChipSwap);
			return true;
		}

		private void DoTapParticleOnPosition(Vector3 localPosition)
		{
			game.particles.CreateParticles(localPosition, Match3Particles.PositionType.ChipTap, Quaternion.identity);
		}

		private void DoSwipeParticlesBetweenSlots(Slot firstHitSlot, Slot slotToSwitchWith)
		{
			Vector3 localPositionOfCenter = Vector3.Lerp(firstHitSlot.localPositionOfCenter, slotToSwitchWith.localPositionOfCenter, 0f);
			if (slotToSwitchWith.position.x != firstHitSlot.position.x)
			{
				Quaternion rotation = (slotToSwitchWith.position.x > firstHitSlot.position.x) ? Quaternion.identity : Quaternion.AngleAxis(180f, Vector3.forward);
				game.particles.CreateParticles(localPositionOfCenter, Match3Particles.PositionType.ChipSwipeHorizontal, rotation);
			}
			else
			{
				Quaternion rotation2 = (slotToSwitchWith.position.y > firstHitSlot.position.y) ? Quaternion.AngleAxis(90f, Vector3.forward) : Quaternion.AngleAxis(-90f, Vector3.forward);
				game.particles.CreateParticles(localPositionOfCenter, Match3Particles.PositionType.ChipSwipeHorizontal, rotation2);
			}
		}

		private void DoSwipeParticlesBetweenSlots(IntVector2 pos1, IntVector2 pos2)
		{
			Vector3 a = game.LocalPositionOfCenter(pos1);
			Vector3 b = game.LocalPositionOfCenter(pos2);
			Vector3 localPositionOfCenter = Vector3.Lerp(a, b, 0f);
			if (pos1.x != pos2.x)
			{
				Quaternion rotation = (pos2.x > pos1.x) ? Quaternion.identity : Quaternion.AngleAxis(180f, Vector3.forward);
				game.particles.CreateParticles(localPositionOfCenter, Match3Particles.PositionType.ChipSwipeHorizontal, rotation);
			}
			else
			{
				Quaternion rotation2 = (pos2.y > pos1.y) ? Quaternion.AngleAxis(90f, Vector3.forward) : Quaternion.AngleAxis(-90f, Vector3.forward);
				game.particles.CreateParticles(localPositionOfCenter, Match3Particles.PositionType.ChipSwipeHorizontal, rotation2);
			}
		}

		private void OnEnable()
		{
			if (follower != null)
			{
				follower.SetActive(active: false);
			}
		}

		public void DoUpdate(float deltaTime)
		{
			if (game == null)
			{
				return;
			}
			Settings settings = this.settings;
			if (mouseParams.mouseUpEnum != null)
			{
				AffectorBase activeAffector = mouseParams.activeAffector;
				if (activeAffector != null)
				{
					AffectorUpdateParams updateParams = default(AffectorUpdateParams);
					updateParams.currentPosition = mouseParams.mouseDownPositon;
					updateParams.mouseParams = mouseParams;
					updateParams.input = this;
					activeAffector.OnUpdate(updateParams);
				}
			}
			else if (settings.enableBouncyMode || settings.inputLineMode)
			{
				UpdateDisplace();
			}
			else
			{
				UpdateDisplaceIntegration();
				if (mouseParams.isMouseDown)
				{
					Vector3 currentPosition = LocalMatch3Pos(mouseParams.MousePosition(input));
					AffectorBase activeAffector2 = mouseParams.activeAffector;
					if (activeAffector2 != null)
					{
						AffectorUpdateParams updateParams2 = default(AffectorUpdateParams);
						updateParams2.currentPosition = currentPosition;
						updateParams2.mouseParams = mouseParams;
						updateParams2.input = this;
						activeAffector2.OnUpdate(updateParams2);
					}
				}
			}
			UpdateSimple();
			UpdateFollower();
		}
	}
}
