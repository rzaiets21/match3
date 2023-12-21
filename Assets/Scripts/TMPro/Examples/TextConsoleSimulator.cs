using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace TMPro.Examples
{
	public class TextConsoleSimulator : MonoBehaviour
	{
		private sealed class _003CRevealCharacters_003Ed__7 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public TMP_Text textComponent;

			public TextConsoleSimulator _003C_003E4__this;

			private TMP_TextInfo _003CtextInfo_003E5__2;

			private int _003CtotalVisibleCharacters_003E5__3;

			private int _003CvisibleCount_003E5__4;

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
			public _003CRevealCharacters_003Ed__7(int _003C_003E1__state)
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
				TextConsoleSimulator textConsoleSimulator = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					textComponent.ForceMeshUpdate();
					_003CtextInfo_003E5__2 = textComponent.textInfo;
					_003CtotalVisibleCharacters_003E5__3 = _003CtextInfo_003E5__2.characterCount;
					_003CvisibleCount_003E5__4 = 0;
					goto IL_005d;
				case 1:
					_003C_003E1__state = -1;
					_003CvisibleCount_003E5__4 = 0;
					break;
				case 2:
					{
						_003C_003E1__state = -1;
						goto IL_005d;
					}
					IL_005d:
					if (textConsoleSimulator.hasTextChanged)
					{
						_003CtotalVisibleCharacters_003E5__3 = _003CtextInfo_003E5__2.characterCount;
						textConsoleSimulator.hasTextChanged = false;
					}
					if (_003CvisibleCount_003E5__4 > _003CtotalVisibleCharacters_003E5__3)
					{
						_003C_003E2__current = new WaitForSeconds(1f);
						_003C_003E1__state = 1;
						return true;
					}
					break;
				}
				textComponent.maxVisibleCharacters = _003CvisibleCount_003E5__4;
				_003CvisibleCount_003E5__4++;
				_003C_003E2__current = null;
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

		private sealed class _003CRevealWords_003Ed__8 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public TMP_Text textComponent;

			private int _003CtotalWordCount_003E5__2;

			private int _003CtotalVisibleCharacters_003E5__3;

			private int _003Ccounter_003E5__4;

			private int _003CvisibleCount_003E5__5;

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
			public _003CRevealWords_003Ed__8(int _003C_003E1__state)
			{
				this._003C_003E1__state = _003C_003E1__state;
			}

			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			private bool MoveNext()
			{
				int num;
				switch (_003C_003E1__state)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					textComponent.ForceMeshUpdate();
					_003CtotalWordCount_003E5__2 = textComponent.textInfo.wordCount;
					_003CtotalVisibleCharacters_003E5__3 = textComponent.textInfo.characterCount;
					_003Ccounter_003E5__4 = 0;
					num = 0;
					_003CvisibleCount_003E5__5 = 0;
					goto IL_0069;
				case 1:
					_003C_003E1__state = -1;
					break;
				case 2:
					{
						_003C_003E1__state = -1;
						goto IL_0069;
					}
					IL_0069:
					num = _003Ccounter_003E5__4 % (_003CtotalWordCount_003E5__2 + 1);
					if (num == 0)
					{
						_003CvisibleCount_003E5__5 = 0;
					}
					else if (num < _003CtotalWordCount_003E5__2)
					{
						_003CvisibleCount_003E5__5 = textComponent.textInfo.wordInfo[num - 1].lastCharacterIndex + 1;
					}
					else if (num == _003CtotalWordCount_003E5__2)
					{
						_003CvisibleCount_003E5__5 = _003CtotalVisibleCharacters_003E5__3;
					}
					textComponent.maxVisibleCharacters = _003CvisibleCount_003E5__5;
					if (_003CvisibleCount_003E5__5 >= _003CtotalVisibleCharacters_003E5__3)
					{
						_003C_003E2__current = new WaitForSeconds(1f);
						_003C_003E1__state = 1;
						return true;
					}
					break;
				}
				_003Ccounter_003E5__4++;
				_003C_003E2__current = new WaitForSeconds(0.1f);
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

		private bool hasTextChanged;

		private void Awake()
		{
			m_TextComponent = base.gameObject.GetComponent<TMP_Text>();
		}

		private void Start()
		{
			StartCoroutine(RevealCharacters(m_TextComponent));
		}

		private void OnEnable()
		{
			TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED);
		}

		private void OnDisable()
		{
			TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(ON_TEXT_CHANGED);
		}

		private void ON_TEXT_CHANGED(UnityEngine.Object obj)
		{
			hasTextChanged = true;
		}

		private IEnumerator RevealCharacters(TMP_Text textComponent)
		{
			return new _003CRevealCharacters_003Ed__7(0)
			{
				_003C_003E4__this = this,
				textComponent = textComponent
			};
		}

		private IEnumerator RevealWords(TMP_Text textComponent)
		{
			return new _003CRevealWords_003Ed__8(0)
			{
				textComponent = textComponent
			};
		}
	}
}
