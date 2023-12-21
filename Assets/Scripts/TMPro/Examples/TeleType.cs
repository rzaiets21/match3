using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace TMPro.Examples
{
	public class TeleType : MonoBehaviour
	{
		private sealed class _003CStart_003Ed__4 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public TeleType _003C_003E4__this;

			private int _003CtotalVisibleCharacters_003E5__2;

			private int _003Ccounter_003E5__3;

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
			public _003CStart_003Ed__4(int _003C_003E1__state)
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
				TeleType teleType = _003C_003E4__this;
				int num2;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					teleType.m_textMeshPro.ForceMeshUpdate();
					_003CtotalVisibleCharacters_003E5__2 = teleType.m_textMeshPro.textInfo.characterCount;
					_003Ccounter_003E5__3 = 0;
					num2 = 0;
					goto IL_005b;
				case 1:
					_003C_003E1__state = -1;
					teleType.m_textMeshPro.text = teleType.label02;
					_003C_003E2__current = new WaitForSeconds(1f);
					_003C_003E1__state = 2;
					return true;
				case 2:
					_003C_003E1__state = -1;
					teleType.m_textMeshPro.text = teleType.label01;
					_003C_003E2__current = new WaitForSeconds(1f);
					_003C_003E1__state = 3;
					return true;
				case 3:
					_003C_003E1__state = -1;
					break;
				case 4:
					{
						_003C_003E1__state = -1;
						goto IL_005b;
					}
					IL_005b:
					num2 = _003Ccounter_003E5__3 % (_003CtotalVisibleCharacters_003E5__2 + 1);
					teleType.m_textMeshPro.maxVisibleCharacters = num2;
					if (num2 >= _003CtotalVisibleCharacters_003E5__2)
					{
						_003C_003E2__current = new WaitForSeconds(1f);
						_003C_003E1__state = 1;
						return true;
					}
					break;
				}
				_003Ccounter_003E5__3++;
				_003C_003E2__current = new WaitForSeconds(0.05f);
				_003C_003E1__state = 4;
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

		private string label01 = "Example <sprite=2> of using <sprite=7> <#ffa000>Graphics Inline</color> <sprite=5> with Text in <font=\"Bangers SDF\" material=\"Bangers SDF - Drop Shadow\">TextMesh<#40a0ff>Pro</color></font><sprite=0> and Unity<sprite=1>";

		private string label02 = "Example <sprite=2> of using <sprite=7> <#ffa000>Graphics Inline</color> <sprite=5> with Text in <font=\"Bangers SDF\" material=\"Bangers SDF - Drop Shadow\">TextMesh<#40a0ff>Pro</color></font><sprite=0> and Unity<sprite=2>";

		private TMP_Text m_textMeshPro;

		private void Awake()
		{
			m_textMeshPro = GetComponent<TMP_Text>();
			m_textMeshPro.text = label01;
			m_textMeshPro.enableWordWrapping = true;
			m_textMeshPro.alignment = TextAlignmentOptions.Top;
		}

		private IEnumerator Start()
		{
			return new _003CStart_003Ed__4(0)
			{
				_003C_003E4__this = this
			};
		}
	}
}
