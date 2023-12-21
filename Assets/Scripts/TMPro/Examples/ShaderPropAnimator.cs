using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace TMPro.Examples
{
	public class ShaderPropAnimator : MonoBehaviour
	{
		private sealed class _003CAnimateProperties_003Ed__6 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public ShaderPropAnimator _003C_003E4__this;

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
			public _003CAnimateProperties_003Ed__6(int _003C_003E1__state)
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
				ShaderPropAnimator shaderPropAnimator = _003C_003E4__this;
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
					shaderPropAnimator.m_frame = UnityEngine.Random.Range(0f, 1f);
				}
				float value = shaderPropAnimator.GlowCurve.Evaluate(shaderPropAnimator.m_frame);
				shaderPropAnimator.m_Material.SetFloat(ShaderUtilities.ID_GlowPower, value);
				shaderPropAnimator.m_frame += Time.deltaTime * UnityEngine.Random.Range(0.2f, 0.3f);
				_003C_003E2__current = new WaitForEndOfFrame();
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

		private Renderer m_Renderer;

		private Material m_Material;

		public AnimationCurve GlowCurve;

		public float m_frame;

		private void Awake()
		{
			m_Renderer = GetComponent<Renderer>();
			m_Material = m_Renderer.material;
		}

		private void Start()
		{
			StartCoroutine(AnimateProperties());
		}

		private IEnumerator AnimateProperties()
		{
			return new _003CAnimateProperties_003Ed__6(0)
			{
				_003C_003E4__this = this
			};
		}
	}
}
