using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace GGMatch3
{
	public class ChipJumpBehaviour
	{
		[Serializable]
		public class Settings
		{
			public Vector3 startOffset;

			public Vector3 offset;

			public bool useStartScale;

			public Vector3 startScale = Vector3.one;

			public Vector3 scale;

			public float rotationAngle;

			public float duration;

			public AnimationCurve animationCurve;

			public float delay;

			public float delayPerX;
		}

		private sealed class _003CDoAnimateJump_003Ed__9 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public Chip chip;

			public ChipJumpBehaviour _003C_003E4__this;

			private Match3Game _003Cgame_003E5__2;

			private TransformBehaviour _003Ct_003E5__3;

			private float _003Ctime_003E5__4;

			private float _003CtotalDuration_003E5__5;

			private float _003CinitialDelay_003E5__6;

			private float _003CprevTime_003E5__7;

			private float _003CprevRepetition_003E5__8;

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
			public _003CDoAnimateJump_003Ed__9(int _003C_003E1__state)
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
				ChipJumpBehaviour chipJumpBehaviour = _003C_003E4__this;
				float num2;
				int num3;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003Cgame_003E5__2 = chip.slot.game;
					_003Ct_003E5__3 = chip.GetComponentBehaviour<TransformBehaviour>();
					if (chipJumpBehaviour.settings.useStartScale)
					{
						chipJumpBehaviour.SetScale(_003Ct_003E5__3, chipJumpBehaviour.settings.startScale);
					}
					chipJumpBehaviour.SetOffset(_003Ct_003E5__3, chipJumpBehaviour.settings.startOffset);
					goto IL_008f;
				case 1:
					_003C_003E1__state = -1;
					goto IL_017a;
				case 2:
					_003C_003E1__state = -1;
					goto IL_025b;
				case 3:
					{
						_003C_003E1__state = -1;
						goto IL_038c;
					}
					IL_017a:
					num2 = Mathf.Repeat(_003Cgame_003E5__2.board.currentTime, _003CtotalDuration_003E5__5);
					num3 = Mathf.FloorToInt(_003Cgame_003E5__2.board.currentTime / _003CtotalDuration_003E5__5);
					if ((!((float)num3 > _003CprevRepetition_003E5__8) && !(_003CprevTime_003E5__7 <= _003CinitialDelay_003E5__6)) || !(num2 >= _003CinitialDelay_003E5__6))
					{
						_003CprevTime_003E5__7 = num2;
						_003CprevRepetition_003E5__8 = num3;
						_003C_003E2__current = null;
						_003C_003E1__state = 1;
						return true;
					}
					_003Ctime_003E5__4 = Mathf.Repeat(_003Cgame_003E5__2.board.currentTime, _003CtotalDuration_003E5__5) - _003CinitialDelay_003E5__6;
					goto IL_025b;
					IL_025b:
					if (_003Ctime_003E5__4 < chipJumpBehaviour.settings.delay)
					{
						_003Ctime_003E5__4 += chipJumpBehaviour.deltaTime;
						_003C_003E2__current = null;
						_003C_003E1__state = 2;
						return true;
					}
					_003Ctime_003E5__4 = Mathf.Repeat(_003Cgame_003E5__2.board.currentTime, _003CtotalDuration_003E5__5) - _003CinitialDelay_003E5__6 - chipJumpBehaviour.settings.delay;
					goto IL_038c;
					IL_008f:
					_003Ctime_003E5__4 = 0f;
					if (chipJumpBehaviour.settings.delay + chipJumpBehaviour.settings.delayPerX > 0f)
					{
						_003CtotalDuration_003E5__5 = chipJumpBehaviour.settings.delay + chipJumpBehaviour.settings.duration + chipJumpBehaviour.settings.delayPerX * (float)_003Cgame_003E5__2.board.size.x;
						_003CinitialDelay_003E5__6 = 0f;
						if (_003Ct_003E5__3 != null)
						{
							_003CinitialDelay_003E5__6 = _003Ct_003E5__3.localPosition.x * chipJumpBehaviour.settings.delayPerX;
						}
						_003CprevTime_003E5__7 = Mathf.Repeat(_003Cgame_003E5__2.board.currentTime, _003CtotalDuration_003E5__5);
						_003CprevRepetition_003E5__8 = Mathf.FloorToInt(_003Cgame_003E5__2.board.currentTime / _003CtotalDuration_003E5__5);
						goto IL_017a;
					}
					goto IL_038c;
					IL_038c:
					if (_003Ctime_003E5__4 <= chipJumpBehaviour.settings.duration)
					{
						_003Ctime_003E5__4 += chipJumpBehaviour.deltaTime;
						Settings settings = chipJumpBehaviour.settings;
						float num4 = Mathf.InverseLerp(0f, settings.duration, _003Ctime_003E5__4);
						if (settings.animationCurve != null)
						{
							num4 = settings.animationCurve.Evaluate(num4);
						}
						Vector3 offset = Vector3.LerpUnclamped(settings.startOffset, settings.offset, num4);
						Vector3 a = Vector3.one;
						if (settings.useStartScale)
						{
							a = settings.startScale;
						}
						Vector3 scale = Vector3.LerpUnclamped(a, settings.scale, num4);
						float angle = Mathf.LerpUnclamped(0f, settings.rotationAngle, num4);
						chipJumpBehaviour.SetOffset(_003Ct_003E5__3, offset);
						chipJumpBehaviour.SetScale(_003Ct_003E5__3, scale);
						chipJumpBehaviour.SetRotation(_003Ct_003E5__3, angle);
						_003C_003E2__current = null;
						_003C_003E1__state = 3;
						return true;
					}
					goto IL_008f;
				}
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

		public bool isActive = true;

		private Settings settings;

		private IEnumerator animation;

		private float deltaTime;

		public void Init(Settings settings)
		{
			this.settings = settings;
		}

		private void SetOffset(TransformBehaviour t, Vector3 offset)
		{
			if (!(t == null))
			{
				t.localOffsetPosition = offset;
			}
		}

		private void SetScale(TransformBehaviour t, Vector3 scale)
		{
			if (!(t == null))
			{
				t.showMatchActionLocalScale = scale;
			}
		}

		private void SetRotation(TransformBehaviour t, float angle)
		{
			if (!(t == null))
			{
				t.localRotationOffset = Quaternion.AngleAxis(angle, Vector3.forward);
			}
		}

		private IEnumerator DoAnimateJump(Chip chip)
		{
			return new _003CDoAnimateJump_003Ed__9(0)
			{
				_003C_003E4__this = this,
				chip = chip
			};
		}

		public void Update(Chip chip, float deltaTime)
		{
			if (isActive)
			{
				this.deltaTime = deltaTime;
				if (animation == null)
				{
					animation = DoAnimateJump(chip);
				}
				animation.MoveNext();
			}
		}
	}
}
