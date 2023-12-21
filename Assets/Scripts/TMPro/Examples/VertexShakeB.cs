using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace TMPro.Examples
{
	public class VertexShakeB : MonoBehaviour
	{
		private sealed class _003CAnimateVertexColors_003Ed__10 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public VertexShakeB _003C_003E4__this;

			private TMP_TextInfo _003CtextInfo_003E5__2;

			private Vector3[][] _003CcopyOfVertices_003E5__3;

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
			public _003CAnimateVertexColors_003Ed__10(int _003C_003E1__state)
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
				VertexShakeB vertexShakeB = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					vertexShakeB.m_TextComponent.ForceMeshUpdate();
					_003CtextInfo_003E5__2 = vertexShakeB.m_TextComponent.textInfo;
					_003CcopyOfVertices_003E5__3 = new Vector3[0][];
					vertexShakeB.hasTextChanged = true;
					break;
				case 1:
					_003C_003E1__state = -1;
					break;
				case 2:
					_003C_003E1__state = -1;
					break;
				}
				if (vertexShakeB.hasTextChanged)
				{
					if (_003CcopyOfVertices_003E5__3.Length < _003CtextInfo_003E5__2.meshInfo.Length)
					{
						_003CcopyOfVertices_003E5__3 = new Vector3[_003CtextInfo_003E5__2.meshInfo.Length][];
					}
					for (int i = 0; i < _003CtextInfo_003E5__2.meshInfo.Length; i++)
					{
						int num2 = _003CtextInfo_003E5__2.meshInfo[i].vertices.Length;
						_003CcopyOfVertices_003E5__3[i] = new Vector3[num2];
					}
					vertexShakeB.hasTextChanged = false;
				}
				if (_003CtextInfo_003E5__2.characterCount == 0)
				{
					_003C_003E2__current = new WaitForSeconds(0.25f);
					_003C_003E1__state = 1;
					return true;
				}
				int lineCount = _003CtextInfo_003E5__2.lineCount;
				for (int j = 0; j < lineCount; j++)
				{
					int firstCharacterIndex = _003CtextInfo_003E5__2.lineInfo[j].firstCharacterIndex;
					int lastCharacterIndex = _003CtextInfo_003E5__2.lineInfo[j].lastCharacterIndex;
					Vector3 vector = (_003CtextInfo_003E5__2.characterInfo[firstCharacterIndex].bottomLeft + _003CtextInfo_003E5__2.characterInfo[lastCharacterIndex].topRight) / 2f;
					Quaternion q = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(-0.25f, 0.25f));
					for (int k = firstCharacterIndex; k <= lastCharacterIndex; k++)
					{
						if (_003CtextInfo_003E5__2.characterInfo[k].isVisible)
						{
							int materialReferenceIndex = _003CtextInfo_003E5__2.characterInfo[k].materialReferenceIndex;
							int vertexIndex = _003CtextInfo_003E5__2.characterInfo[k].vertexIndex;
							Vector3[] vertices = _003CtextInfo_003E5__2.meshInfo[materialReferenceIndex].vertices;
							Vector3 vector2 = (vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2f;
							_003CcopyOfVertices_003E5__3[materialReferenceIndex][vertexIndex] = vertices[vertexIndex] - vector2;
							_003CcopyOfVertices_003E5__3[materialReferenceIndex][vertexIndex + 1] = vertices[vertexIndex + 1] - vector2;
							_003CcopyOfVertices_003E5__3[materialReferenceIndex][vertexIndex + 2] = vertices[vertexIndex + 2] - vector2;
							_003CcopyOfVertices_003E5__3[materialReferenceIndex][vertexIndex + 3] = vertices[vertexIndex + 3] - vector2;
							float d = UnityEngine.Random.Range(0.95f, 1.05f);
							Matrix4x4 matrix4x = Matrix4x4.TRS(Vector3.one, Quaternion.identity, Vector3.one * d);
							_003CcopyOfVertices_003E5__3[materialReferenceIndex][vertexIndex] = matrix4x.MultiplyPoint3x4(_003CcopyOfVertices_003E5__3[materialReferenceIndex][vertexIndex]);
							_003CcopyOfVertices_003E5__3[materialReferenceIndex][vertexIndex + 1] = matrix4x.MultiplyPoint3x4(_003CcopyOfVertices_003E5__3[materialReferenceIndex][vertexIndex + 1]);
							_003CcopyOfVertices_003E5__3[materialReferenceIndex][vertexIndex + 2] = matrix4x.MultiplyPoint3x4(_003CcopyOfVertices_003E5__3[materialReferenceIndex][vertexIndex + 2]);
							_003CcopyOfVertices_003E5__3[materialReferenceIndex][vertexIndex + 3] = matrix4x.MultiplyPoint3x4(_003CcopyOfVertices_003E5__3[materialReferenceIndex][vertexIndex + 3]);
							_003CcopyOfVertices_003E5__3[materialReferenceIndex][vertexIndex] += vector2;
							_003CcopyOfVertices_003E5__3[materialReferenceIndex][vertexIndex + 1] += vector2;
							_003CcopyOfVertices_003E5__3[materialReferenceIndex][vertexIndex + 2] += vector2;
							_003CcopyOfVertices_003E5__3[materialReferenceIndex][vertexIndex + 3] += vector2;
							_003CcopyOfVertices_003E5__3[materialReferenceIndex][vertexIndex] -= vector;
							_003CcopyOfVertices_003E5__3[materialReferenceIndex][vertexIndex + 1] -= vector;
							_003CcopyOfVertices_003E5__3[materialReferenceIndex][vertexIndex + 2] -= vector;
							_003CcopyOfVertices_003E5__3[materialReferenceIndex][vertexIndex + 3] -= vector;
							matrix4x = Matrix4x4.TRS(Vector3.one, q, Vector3.one);
							_003CcopyOfVertices_003E5__3[materialReferenceIndex][vertexIndex] = matrix4x.MultiplyPoint3x4(_003CcopyOfVertices_003E5__3[materialReferenceIndex][vertexIndex]);
							_003CcopyOfVertices_003E5__3[materialReferenceIndex][vertexIndex + 1] = matrix4x.MultiplyPoint3x4(_003CcopyOfVertices_003E5__3[materialReferenceIndex][vertexIndex + 1]);
							_003CcopyOfVertices_003E5__3[materialReferenceIndex][vertexIndex + 2] = matrix4x.MultiplyPoint3x4(_003CcopyOfVertices_003E5__3[materialReferenceIndex][vertexIndex + 2]);
							_003CcopyOfVertices_003E5__3[materialReferenceIndex][vertexIndex + 3] = matrix4x.MultiplyPoint3x4(_003CcopyOfVertices_003E5__3[materialReferenceIndex][vertexIndex + 3]);
							_003CcopyOfVertices_003E5__3[materialReferenceIndex][vertexIndex] += vector;
							_003CcopyOfVertices_003E5__3[materialReferenceIndex][vertexIndex + 1] += vector;
							_003CcopyOfVertices_003E5__3[materialReferenceIndex][vertexIndex + 2] += vector;
							_003CcopyOfVertices_003E5__3[materialReferenceIndex][vertexIndex + 3] += vector;
						}
					}
				}
				for (int l = 0; l < _003CtextInfo_003E5__2.meshInfo.Length; l++)
				{
					_003CtextInfo_003E5__2.meshInfo[l].mesh.vertices = _003CcopyOfVertices_003E5__3[l];
					vertexShakeB.m_TextComponent.UpdateGeometry(_003CtextInfo_003E5__2.meshInfo[l].mesh, l);
				}
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

		public float AngleMultiplier = 1f;

		public float SpeedMultiplier = 1f;

		public float CurveScale = 1f;

		private TMP_Text m_TextComponent;

		private bool hasTextChanged;

		private void Awake()
		{
			m_TextComponent = GetComponent<TMP_Text>();
		}

		private void OnEnable()
		{
			TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED);
		}

		private void OnDisable()
		{
			TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(ON_TEXT_CHANGED);
		}

		private void Start()
		{
			StartCoroutine(AnimateVertexColors());
		}

		private void ON_TEXT_CHANGED(UnityEngine.Object obj)
		{
			if ((bool)(obj = m_TextComponent))
			{
				hasTextChanged = true;
			}
		}

		private IEnumerator AnimateVertexColors()
		{
			return new _003CAnimateVertexColors_003Ed__10(0)
			{
				_003C_003E4__this = this
			};
		}
	}
}
