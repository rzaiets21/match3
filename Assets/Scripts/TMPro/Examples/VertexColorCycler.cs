using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace TMPro.Examples
{
	public class VertexColorCycler : MonoBehaviour
	{
		private sealed class _003CAnimateVertexColors_003Ed__3 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public VertexColorCycler _003C_003E4__this;

			private TMP_TextInfo _003CtextInfo_003E5__2;

			private int _003CcurrentCharacter_003E5__3;

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
			public _003CAnimateVertexColors_003Ed__3(int _003C_003E1__state)
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
				VertexColorCycler vertexColorCycler = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
				{
					_003C_003E1__state = -1;
					_003CtextInfo_003E5__2 = vertexColorCycler.m_TextComponent.textInfo;
					_003CcurrentCharacter_003E5__3 = 0;
					Color32 color = vertexColorCycler.m_TextComponent.color;
					break;
				}
				case 1:
					_003C_003E1__state = -1;
					break;
				case 2:
					_003C_003E1__state = -1;
					break;
				}
				int characterCount = _003CtextInfo_003E5__2.characterCount;
				if (characterCount == 0)
				{
					_003C_003E2__current = new WaitForSeconds(0.25f);
					_003C_003E1__state = 1;
					return true;
				}
				int materialReferenceIndex = _003CtextInfo_003E5__2.characterInfo[_003CcurrentCharacter_003E5__3].materialReferenceIndex;
				Color32[] colors = _003CtextInfo_003E5__2.meshInfo[materialReferenceIndex].colors32;
				int vertexIndex = _003CtextInfo_003E5__2.characterInfo[_003CcurrentCharacter_003E5__3].vertexIndex;
				if (_003CtextInfo_003E5__2.characterInfo[_003CcurrentCharacter_003E5__3].isVisible)
				{
					colors[vertexIndex + 3] = (colors[vertexIndex + 2] = (colors[vertexIndex + 1] = (colors[vertexIndex] = new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), byte.MaxValue))));
					vertexColorCycler.m_TextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
				}
				_003CcurrentCharacter_003E5__3 = (_003CcurrentCharacter_003E5__3 + 1) % characterCount;
				_003C_003E2__current = new WaitForSeconds(0.05f);
				_003C_003E1__state = 2;
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

		private TMP_Text m_TextComponent;

		private void Awake()
		{
			m_TextComponent = GetComponent<TMP_Text>();
		}

		private void Start()
		{
			StartCoroutine(AnimateVertexColors());
		}

		private IEnumerator AnimateVertexColors()
		{
			return new _003CAnimateVertexColors_003Ed__3(0)
			{
				_003C_003E4__this = this
			};
		}
	}
}
