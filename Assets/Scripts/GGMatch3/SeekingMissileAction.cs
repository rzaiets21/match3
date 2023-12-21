using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace GGMatch3
{
	public class SeekingMissileAction : BoardAction
	{
		public class Parameters
		{
			public bool hasOtherChip;

			public ChipType otherChipType;

			public Match3Game game;

			public Slot startSlot;

			public bool isHavingCarpet;

			public bool doCrossExplosion;

			public IntVector2 startPosition => startSlot.position;
		}

		[Serializable]
		public class Settings
		{
			[Serializable]
			public class CurveParameters
			{
				public AnimationCurve curve;

				public FloatRange range;

				public float duration;

				public float Evaluate(float time)
				{
					float time2 = time / duration;
					float t = curve.Evaluate(time2);
					return range.Lerp(t);
				}
			}

			public float bigRadiusNormalized;

			public float radiusRatio;

			public FloatRange altutudeScale = new FloatRange(1f, 1.5f);

			public float maxAltitude = 1f;

			public float timeToMaxAltitude = 0.25f;

			public float keepParticlesFor = 2f;

			public FloatRange bigExitDistanceRange;

			public FloatRange smallExitDistanceRange;

			public CurveParameters bigCircleAccelerationCurve = new CurveParameters();

			public CurveParameters smallCircleAccelerationCurve = new CurveParameters();

			public CurveParameters lineAccelerationCurve = new CurveParameters();

			public bool overrideStartPosition;

			public Vector3 startPosition;

			public bool overrideEndPosition;

			public Vector3 endPosition;

			public bool overrideBigAngle;

			public int bigAngle;

			public bool overrideT;

			public float t;

			public IntensityChange lightIntensityChange;

			public float hitLightIntensity;

			public float shockWaveOffset = 0.2f;

			public float shockWaveOffsetR1 = 0.1f;

			public bool useCameraShakeOnLand;

			public GeneralSettings.CameraShakeSettings cameraShake = new GeneralSettings.CameraShakeSettings();

			public bool useCameraShakeOnTakeOff;

			public GeneralSettings.CameraShakeSettings takeOffCameraShake = new GeneralSettings.CameraShakeSettings();

			public float bigExitDistance => bigExitDistanceRange.Lerp(t);

			public float smallExitDistance => smallExitDistanceRange.Lerp(1f - t);
		}

		public enum State
		{
			BigCircle,
			SmallCircle,
			Line
		}

		public class Trajectory
		{
			public Vector3 bigCenter;

			public float bigRadius;

			public float bigAngle;

			public Vector3 bigDirection;

			public float bigExitAngle;

			public Vector3 smallCenter;

			public float smallRadius;

			public float smallAngle;

			public Vector3 smallDirection;

			public float smallExitAngle;

			public Vector3 lineStart;

			public Vector3 lineEnd;

			public float exitDistance;
		}

		public struct Tangents
		{
			public float gamaRad;

			public float betaRad;

			public float alphaRad;

			public float tan1AngleRad;

			public float tan2AngleRad;

			public Circle c1;

			public Circle c2;

			public Vector3 c1Tan1;

			public Vector3 c2Tan1;

			public Vector3 c1Tan2;

			public Vector3 c2Tan2;
		}

		public class Circle
		{
			public Vector3 position;

			public float radius;

			public Circle(Vector3 position, float radius)
			{
				this.position = position;
				this.radius = radius;
			}
		}

		private sealed class _003CDoFly_003Ed__18 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public SeekingMissileAction _003C_003E4__this;

			private Settings _003Csettings_003E5__2;

			private SeekingMissileBehaviour _003Cbehaviour_003E5__3;

			private Vector3 _003CbigCenter_003E5__4;

			private float _003CbigRadius_003E5__5;

			private float _003CbigExitAngle_003E5__6;

			private Vector3 _003CsmallDirection_003E5__7;

			private Vector3 _003CsmallCenter_003E5__8;

			private float _003CsmallRadius_003E5__9;

			private float _003CsmallExitAngle_003E5__10;

			private Vector3 _003ClineStart_003E5__11;

			private Vector3 _003ClineEnd_003E5__12;

			private float _003CexitDistance_003E5__13;

			private float _003Cangle_003E5__14;

			private float _003Cdistance_003E5__15;

			private Vector3 _003Cdirection_003E5__16;

			private float _003CaccelerationTimer_003E5__17;

			private float _003ClinearSpeed_003E5__18;

			private State _003Cstate_003E5__19;

			private float _003Cheight_003E5__20;

			private float _003CheightWhenStartDescent_003E5__21;

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
			public _003CDoFly_003Ed__18(int _003C_003E1__state)
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
				SeekingMissileAction seekingMissileAction = _003C_003E4__this;
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
					_003Csettings_003E5__2 = Match3Settings.instance.seekingMissileSettings;
					if (_003Csettings_003E5__2.useCameraShakeOnTakeOff)
					{
						seekingMissileAction.parameters.game.ShakeCamera(_003Csettings_003E5__2.takeOffCameraShake);
					}
					seekingMissileAction.sourceLock.UnlockAll();
					if (!seekingMissileAction.animationParameters.overrideStartPosition)
					{
						seekingMissileAction.animationParameters.startPosition = seekingMissileAction.parameters.game.LocalPositionOfCenter(seekingMissileAction.parameters.startPosition);
					}
					if (!seekingMissileAction.animationParameters.overrideEndPosition)
					{
						seekingMissileAction.animationParameters.endPosition = seekingMissileAction.parameters.game.LocalPositionOfCenter(seekingMissileAction.endPosition);
					}
					if (!seekingMissileAction.animationParameters.overrideT)
					{
						seekingMissileAction.animationParameters.t = AnimRandom.Range(0f, 1f);
					}
					ComponentPool pool = seekingMissileAction.parameters.game.GetPool(PieceType.SeekingMissile);
					_003Cbehaviour_003E5__3 = pool.Next<SeekingMissileBehaviour>();
					_003Cbehaviour_003E5__3.Init();
					_003Cbehaviour_003E5__3.localPosition = seekingMissileAction.animationParameters.startPosition;
					_003Cbehaviour_003E5__3.ClearTrail();
					_003Cbehaviour_003E5__3.gameObject.SetActive(value: true);
					Trajectory trajectory = null;
					for (int i = 0; i < 360; i += 10)
					{
						Trajectory trajectory2 = seekingMissileAction.TrajectoryFromAngle(i);
						if (trajectory2 == null)
						{
							continue;
						}
						if (trajectory == null)
						{
							trajectory = trajectory2;
							continue;
						}
						float num2 = seekingMissileAction.animationParameters.smallExitDistance * 360f;
						if (Mathf.Abs(trajectory2.smallExitAngle - num2) < Mathf.Abs(trajectory.smallExitAngle - num2))
						{
							trajectory = trajectory2;
						}
					}
					if (seekingMissileAction.animationParameters.overrideBigAngle)
					{
						trajectory = seekingMissileAction.TrajectoryFromAngle(seekingMissileAction.animationParameters.bigAngle);
					}
					Vector3 bigDirection = trajectory.bigDirection;
					_003CbigCenter_003E5__4 = trajectory.bigCenter;
					float bigAngle = trajectory.bigAngle;
					_003CbigRadius_003E5__5 = trajectory.bigRadius;
					_003CbigExitAngle_003E5__6 = trajectory.bigExitAngle;
					_003CsmallDirection_003E5__7 = trajectory.smallDirection;
					_003CsmallCenter_003E5__8 = trajectory.smallCenter;
					float smallAngle = trajectory.smallAngle;
					_003CsmallRadius_003E5__9 = trajectory.smallRadius;
					_003CsmallExitAngle_003E5__10 = trajectory.smallExitAngle;
					_003ClineStart_003E5__11 = trajectory.lineStart;
					_003ClineEnd_003E5__12 = trajectory.lineEnd;
					_003CexitDistance_003E5__13 = trajectory.exitDistance;
					_003Cangle_003E5__14 = 0f;
					_003Cdistance_003E5__15 = 0f;
					_003Cdirection_003E5__16 = -bigDirection;
					_003CaccelerationTimer_003E5__17 = 0f;
					_003ClinearSpeed_003E5__18 = 0f;
					_003Cstate_003E5__19 = State.BigCircle;
					seekingMissileAction.endSlot.light.AddIntensityChange(_003Csettings_003E5__2.lightIntensityChange);
					_003Cheight_003E5__20 = 0f;
					float maxAltitude = _003Csettings_003E5__2.maxAltitude;
					_003CheightWhenStartDescent_003E5__21 = _003Cheight_003E5__20;
				}
				if (_003Cstate_003E5__19 == State.BigCircle)
				{
					_003Cbehaviour_003E5__3.localPosition = _003CbigCenter_003E5__4 + Quaternion.AngleAxis(_003Cangle_003E5__14, Vector3.forward) * _003Cdirection_003E5__16 * _003CbigRadius_003E5__5;
					Vector3 v = Vector3.Cross((_003Cbehaviour_003E5__3.localPosition - _003CbigCenter_003E5__4).normalized, Vector3.forward);
					_003Cbehaviour_003E5__3.SetDirection(v);
					_003ClinearSpeed_003E5__18 += seekingMissileAction.animationParameters.bigCircleAccelerationCurve.Evaluate(_003CaccelerationTimer_003E5__17);
					float num3 = _003ClinearSpeed_003E5__18 / _003CbigRadius_003E5__5 * 57.29578f;
					_003Cangle_003E5__14 += seekingMissileAction.deltaTime * num3;
					_003CaccelerationTimer_003E5__17 += seekingMissileAction.deltaTime;
				}
				else if (_003Cstate_003E5__19 == State.SmallCircle)
				{
					_003Cbehaviour_003E5__3.localPosition = _003CsmallCenter_003E5__8 + Quaternion.AngleAxis(_003Cangle_003E5__14, Vector3.forward) * _003Cdirection_003E5__16 * _003CsmallRadius_003E5__9;
					Vector3 v2 = Vector3.Cross((_003Cbehaviour_003E5__3.localPosition - _003CsmallCenter_003E5__8).normalized, Vector3.forward);
					_003Cbehaviour_003E5__3.SetDirection(v2);
					_003ClinearSpeed_003E5__18 += seekingMissileAction.animationParameters.smallCircleAccelerationCurve.Evaluate(_003CaccelerationTimer_003E5__17);
					float num4 = _003ClinearSpeed_003E5__18 / _003CsmallRadius_003E5__9 * 57.29578f;
					_003Cangle_003E5__14 += seekingMissileAction.deltaTime * num4;
					_003CaccelerationTimer_003E5__17 += seekingMissileAction.deltaTime;
				}
				else if (_003Cstate_003E5__19 == State.Line)
				{
					_003Cbehaviour_003E5__3.localPosition = _003ClineStart_003E5__11 + _003Cdirection_003E5__16 * _003Cdistance_003E5__15;
					_003Cbehaviour_003E5__3.SetDirection(-_003Cdirection_003E5__16);
					_003ClinearSpeed_003E5__18 += seekingMissileAction.animationParameters.lineAccelerationCurve.Evaluate(_003CaccelerationTimer_003E5__17);
					_003Cdistance_003E5__15 += seekingMissileAction.deltaTime * _003ClinearSpeed_003E5__18;
					_003CaccelerationTimer_003E5__17 += seekingMissileAction.deltaTime;
				}
				if (_003Cstate_003E5__19 == State.BigCircle && _003Cangle_003E5__14 > _003CbigExitAngle_003E5__6)
				{
					_003Cstate_003E5__19 = State.SmallCircle;
					_003Cdirection_003E5__16 = _003CsmallDirection_003E5__7;
					_003Cangle_003E5__14 = 0f;
					_003CaccelerationTimer_003E5__17 = 0f;
				}
				else if (_003Cstate_003E5__19 == State.SmallCircle && _003Cangle_003E5__14 > _003CsmallExitAngle_003E5__10)
				{
					_003Cstate_003E5__19 = State.Line;
					_003Cdirection_003E5__16 = (_003ClineEnd_003E5__12 - _003ClineStart_003E5__11).normalized;
					_003Cangle_003E5__14 = 0f;
					_003CaccelerationTimer_003E5__17 = 0f;
				}
				else if (_003Cstate_003E5__19 == State.Line && _003Cdistance_003E5__15 > _003CexitDistance_003E5__13)
				{
					_003Cbehaviour_003E5__3.RemoveFromGameAfter(_003Csettings_003E5__2.keepParticlesFor);
					seekingMissileAction.targetLock.UnlockAll();
					seekingMissileAction.sourceLock.UnlockAll();
					seekingMissileAction.endSlot.light.AddLight(_003Csettings_003E5__2.hitLightIntensity);
					seekingMissileAction.parameters.game.particles.CreateParticles(seekingMissileAction.endSlot, Match3Particles.PositionType.MissleHitTarget);
					if (_003Csettings_003E5__2.useCameraShakeOnLand)
					{
						seekingMissileAction.parameters.game.ShakeCamera(_003Csettings_003E5__2.cameraShake);
					}
					if (!seekingMissileAction.parameters.hasOtherChip)
					{
						seekingMissileAction.parameters.game.Play(GGSoundSystem.SFXType.SeekingMissleLand);
						SlotDestroyParams slotDestroyParams = new SlotDestroyParams();
						slotDestroyParams.isHitByBomb = true;
						slotDestroyParams.bombType = ChipType.SeekingMissle;
						slotDestroyParams.isHavingCarpet = seekingMissileAction.hasCarpet;
						seekingMissileAction.endSlot.OnDestroySlot(slotDestroyParams);
					}
					else if (seekingMissileAction.parameters.otherChipType == ChipType.Bomb)
					{
						ExplosionAction explosionAction = new ExplosionAction();
						ExplosionAction.ExplosionSettings settings = default(ExplosionAction.ExplosionSettings);
						settings.bombChip = null;
						settings.position = seekingMissileAction.endPosition;
						settings.radius = 3;
						settings.isHavingCarpet = seekingMissileAction.hasCarpet;
						settings.isUsingBombAreaOfEffect = true;
						explosionAction.Init(seekingMissileAction.parameters.game, settings);
						seekingMissileAction.parameters.game.board.actionManager.AddAction(explosionAction);
					}
					else if (seekingMissileAction.parameters.otherChipType == ChipType.HorizontalRocket || seekingMissileAction.parameters.otherChipType == ChipType.VerticalRocket)
					{
						FlyLineRocketAction flyLineRocketAction = new FlyLineRocketAction();
						FlyLineRocketAction.Params flyParams = default(FlyLineRocketAction.Params);
						flyParams.bombChip = null;
						flyParams.game = seekingMissileAction.parameters.game;
						flyParams.position = seekingMissileAction.endPosition;
						flyParams.rocketType = seekingMissileAction.parameters.otherChipType;
						flyParams.prelock = true;
						flyParams.isHavingCarpet = seekingMissileAction.hasCarpet;
						flyLineRocketAction.Init(flyParams);
						seekingMissileAction.parameters.game.board.actionManager.AddAction(flyLineRocketAction);
					}
					seekingMissileAction.AffectOuterCircleWithExplosion(seekingMissileAction.endPosition, 1, _003Csettings_003E5__2.shockWaveOffset);
					seekingMissileAction.AffectOuterCircleWithExplosion(seekingMissileAction.endPosition, 2, _003Csettings_003E5__2.shockWaveOffsetR1);
					seekingMissileAction.isAlive = false;
					return false;
				}
				if (_003Cstate_003E5__19 == State.Line)
				{
					float t = Mathf.InverseLerp(0f, _003CexitDistance_003E5__13, _003Cdistance_003E5__15);
					_003Cheight_003E5__20 = Mathf.Lerp(_003CheightWhenStartDescent_003E5__21, 0f, t);
				}
				else
				{
					_003Cheight_003E5__20 += _003Csettings_003E5__2.maxAltitude / _003Csettings_003E5__2.timeToMaxAltitude * seekingMissileAction.deltaTime;
					_003Cheight_003E5__20 = Mathf.Min(_003Cheight_003E5__20, _003Csettings_003E5__2.maxAltitude);
					_003CheightWhenStartDescent_003E5__21 = _003Cheight_003E5__20;
				}
				float d = _003Csettings_003E5__2.altutudeScale.Lerp(Mathf.InverseLerp(0f, _003Csettings_003E5__2.maxAltitude, _003Cheight_003E5__20));
				_003Cbehaviour_003E5__3.localScale = Vector3.one * d;
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
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

		public Parameters parameters;

		public List<Slot> crossExplosionSlots = new List<Slot>();

		private Lock targetLock;

		private Lock sourceLock;

		private Slot endSlot;

		private bool hasCarpet;

		private IEnumerator flyCoroutine;

		private float deltaTime;

		public Settings animationParameters => Match3Settings.instance.seekingMissileSettings;

		public IntVector2 endPosition => endSlot.position;

		public override void OnStart(ActionManager manager)
		{
			base.OnStart(manager);
		}

		public void Init(Parameters parameters)
		{
			targetLock = lockContainer.NewLock();
			sourceLock = lockContainer.NewLock();
			this.parameters = parameters;
			hasCarpet = parameters.isHavingCarpet;
			Slot startSlot = parameters.startSlot;
			if (startSlot.canCarpetSpreadFromHere)
			{
				hasCarpet = true;
			}
			for (int i = 0; i < startSlot.neigbourSlots.Count; i++)
			{
				if (startSlot.neigbourSlots[i].canCarpetSpreadFromHere)
				{
					hasCarpet = true;
				}
			}
			targetLock.isSlotGravitySuspended = true;
			targetLock.isChipGeneratorSuspended = true;
			targetLock.isAvailableForSeekingMissileSuspended = true;
			targetLock.isAboutToBeDestroyed = true;
			sourceLock.isSlotGravitySuspended = true;
			sourceLock.isChipGeneratorSuspended = true;
			sourceLock.isAvailableForSeekingMissileSuspended = true;
			crossExplosionSlots.Clear();
			if (parameters.doCrossExplosion)
			{
				CrossExplosionAction crossExplosionAction = new CrossExplosionAction();
				CrossExplosionAction.Parameters parameters2 = new CrossExplosionAction.Parameters();
				parameters2.game = parameters.game;
				parameters2.startPosition = parameters.startPosition;
				parameters2.radius = parameters.game.SeekingMissleCrossRadius;
				parameters2.isHavingCarpet = hasCarpet;
				crossExplosionAction.Init(parameters2);
				parameters.game.board.actionManager.AddAction(crossExplosionAction);
				crossExplosionSlots.AddRange(crossExplosionAction.GetAffectedSlots());
			}
			Slot[] slot = parameters.game.board.slots;
			endSlot = parameters.startSlot;
			List<Slot> list = null;
			list = ((!parameters.hasOtherChip) ? parameters.game.goals.BestSlotsForSeekingMissle(parameters.game, parameters.startSlot) : parameters.game.goals.BestSlotsForSeekingMissleWithChip(parameters.game, parameters.startSlot, parameters.otherChipType));
			if (list != null && list.Count > 0)
			{
				int index = parameters.game.RandomRange(0, list.Count);
				endSlot = list[index];
			}
			targetLock.LockSlot(endSlot);
			sourceLock.LockSlot(parameters.startSlot);
			parameters.game.Play(GGSoundSystem.SFXType.SeekingMissleTakeOff);
		}

		public override void OnUpdate(float deltaTime)
		{
			this.deltaTime = deltaTime;
			if (flyCoroutine == null)
			{
				flyCoroutine = DoFly();
			}
			flyCoroutine.MoveNext();
		}

		public IEnumerator DoFly()
		{
			return new _003CDoFly_003Ed__18(0)
			{
				_003C_003E4__this = this
			};
		}

		private void AffectOuterCircleWithExplosion(IntVector2 center, int radius, float shockWaveOffset)
		{
			Match3Game game = parameters.game;
			Vector3 b = game.LocalPositionOfCenter(center);
			for (int i = center.x - radius; i <= center.x + radius; i++)
			{
				for (int j = center.y - radius; j <= center.y + radius; j++)
				{
					int a = Mathf.Abs(center.x - i);
					int b2 = Mathf.Abs(center.y - j);
					if (Mathf.Max(a, b2) == radius)
					{
						Slot slot = game.board.GetSlot(new IntVector2(i, j));
						if (slot != null)
						{
							slot.offsetPosition = (slot.localPositionOfCenter - b).normalized * shockWaveOffset;
							slot.positionIntegrator.currentPosition = slot.offsetPosition;
						}
					}
				}
			}
		}

		public Trajectory TrajectoryFromAngle(int bigAngle)
		{
			Trajectory trajectory = new Trajectory();
			Vector3 vector = Quaternion.AngleAxis(bigAngle, Vector3.forward) * Vector3.right;
			float num = parameters.game.slotPhysicalSize.x * animationParameters.bigRadiusNormalized;
			Vector3 vector2 = animationParameters.startPosition + vector * num;
			float num2 = (float)(bigAngle + 180) + Mathf.Lerp(0f, 360f, animationParameters.bigExitDistance);
			Vector3 vector3 = Quaternion.AngleAxis(num2, Vector3.forward) * Vector3.right;
			float num3 = num * animationParameters.radiusRatio;
			Vector3 vector4 = vector2 + vector3 * (num - num3);
			if (Vector3.Distance(vector4, animationParameters.endPosition) < num3 + 0.001f)
			{
				return null;
			}
			Vector3 c1Tan = FindOuterTangents(new Circle(vector4, num3), new Circle(animationParameters.endPosition, 0.001f)).c1Tan1;
			float num4 = GGUtil.SignedAngle(vector3, (c1Tan - vector4).normalized);
			if (num4 < 0f)
			{
				num4 += 360f;
			}
			Vector3 a = vector4 + vector3 * num3;
			float num5 = GGUtil.SignedAngle(-vector, (a - vector2).normalized);
			if (num5 < 0f)
			{
				num5 += 360f;
			}
			Vector3 vector5 = c1Tan;
			Vector3 endPosition = animationParameters.endPosition;
			float exitDistance = Vector3.Distance(endPosition, vector5);
			trajectory.bigCenter = vector2;
			trajectory.bigDirection = vector;
			trajectory.bigAngle = bigAngle;
			trajectory.bigRadius = num;
			trajectory.bigExitAngle = num5;
			trajectory.smallCenter = vector4;
			trajectory.smallDirection = vector3;
			trajectory.smallAngle = num2;
			trajectory.smallRadius = num3;
			trajectory.smallExitAngle = num4;
			trajectory.lineStart = vector5;
			trajectory.lineEnd = endPosition;
			trajectory.exitDistance = exitDistance;
			return trajectory;
		}

		public static Tangents FindOuterTangents(Circle c1, Circle c2)
		{
			Tangents tangents = default(Tangents);
			bool flag = false;
			if (c1.radius > c2.radius)
			{
				Circle circle = c1;
				c1 = c2;
				c2 = circle;
				flag = true;
			}
			tangents.c1 = c1;
			tangents.c2 = c2;
			Vector3 vector = c2.position - c1.position;
			vector.z = 0f;
			float magnitude = vector.magnitude;
			tangents.gamaRad = Mathf.Atan2(c2.position.y - c1.position.y, c2.position.x - c1.position.x);
			tangents.betaRad = Mathf.Asin((c2.radius - c1.radius) / magnitude);
			tangents.alphaRad = tangents.gamaRad + tangents.betaRad;
			tangents.tan1AngleRad = tangents.alphaRad;
			tangents.tan2AngleRad = (float)Math.PI + (tangents.gamaRad - tangents.betaRad);
			Vector3 a = Quaternion.AngleAxis(tangents.tan1AngleRad * 57.29578f, Vector3.forward) * Vector3.up;
			Vector3 a2 = Quaternion.AngleAxis(tangents.tan2AngleRad * 57.29578f, Vector3.forward) * Vector3.up;
			if (flag)
			{
				tangents.c2Tan1 = c1.position + a * c1.radius;
				tangents.c1Tan1 = c2.position + a * c2.radius;
				tangents.c2Tan2 = c1.position + a2 * c1.radius;
				tangents.c1Tan2 = c2.position + a2 * c2.radius;
			}
			else
			{
				tangents.c1Tan1 = c1.position + a * c1.radius;
				tangents.c2Tan1 = c2.position + a * c2.radius;
				tangents.c1Tan2 = c1.position + a2 * c1.radius;
				tangents.c2Tan2 = c2.position + a2 * c2.radius;
			}
			return tangents;
		}
	}
}
