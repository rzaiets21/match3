using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace TMPro.Examples
{
	public class VertexJitter : MonoBehaviour
	{
		private struct VertexAnim
		{
			public float angleRange;

			public float angle;

			public float speed;
		}

		private sealed class _003CAnimateVertexColors_003Ed__11 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public VertexJitter _003C_003E4__this;

			private TMP_TextInfo _003CtextInfo_003E5__2;

			private int _003CloopCount_003E5__3;

			private VertexAnim[] _003CvertexAnim_003E5__4;

			private TMP_MeshInfo[] _003CcachedMeshInfo_003E5__5;

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
			public _003CAnimateVertexColors_003Ed__11(int _003C_003E1__state)
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
				VertexJitter vertexJitter = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					vertexJitter.m_TextComponent.ForceMeshUpdate();
					_003CtextInfo_003E5__2 = vertexJitter.m_TextComponent.textInfo;
					_003CloopCount_003E5__3 = 0;
					vertexJitter.hasTextChanged = true;
					_003CvertexAnim_003E5__4 = new VertexAnim[1024];
					for (int i = 0; i < 1024; i++)
					{
						_003CvertexAnim_003E5__4[i].angleRange = UnityEngine.Random.Range(10f, 25f);
						_003CvertexAnim_003E5__4[i].speed = UnityEngine.Random.Range(1f, 3f);
					}
					_003CcachedMeshInfo_003E5__5 = _003CtextInfo_003E5__2.CopyMeshInfoVertexData();
					break;
				case 1:
					_003C_003E1__state = -1;
					break;
				case 2:
					_003C_003E1__state = -1;
					break;
				}
				if (vertexJitter.hasTextChanged)
				{
					_003CcachedMeshInfo_003E5__5 = _003CtextInfo_003E5__2.CopyMeshInfoVertexData();
					vertexJitter.hasTextChanged = false;
				}
				int characterCount = _003CtextInfo_003E5__2.characterCount;
				if (characterCount == 0)
				{
					_003C_003E2__current = new WaitForSeconds(0.25f);
					_003C_003E1__state = 1;
					return true;
				}
				for (int j = 0; j < characterCount; j++)
				{
					if (_003CtextInfo_003E5__2.characterInfo[j].isVisible)
					{
						VertexAnim vertexAnim = _003CvertexAnim_003E5__4[j];
						int materialReferenceIndex = _003CtextInfo_003E5__2.characterInfo[j].materialReferenceIndex;
						int vertexIndex = _003CtextInfo_003E5__2.characterInfo[j].vertexIndex;
						Vector3[] vertices = _003CcachedMeshInfo_003E5__5[materialReferenceIndex].vertices;
						Vector3 vector = (Vector2)((vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2f);
						Vector3[] vertices2 = _003CtextInfo_003E5__2.meshInfo[materialReferenceIndex].vertices;
						vertices2[vertexIndex] = vertices[vertexIndex] - vector;
						vertices2[vertexIndex + 1] = vertices[vertexIndex + 1] - vector;
						vertices2[vertexIndex + 2] = vertices[vertexIndex + 2] - vector;
						vertices2[vertexIndex + 3] = vertices[vertexIndex + 3] - vector;
						vertexAnim.angle = Mathf.SmoothStep(0f - vertexAnim.angleRange, vertexAnim.angleRange, Mathf.PingPong((float)_003CloopCount_003E5__3 / 25f * vertexAnim.speed, 1f));
						Matrix4x4 matrix4x = Matrix4x4.TRS(new Vector3(UnityEngine.Random.Range(-0.25f, 0.25f), UnityEngine.Random.Range(-0.25f, 0.25f), 0f) * vertexJitter.CurveScale, Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(-5f, 5f) * vertexJitter.AngleMultiplier), Vector3.one);
						vertices2[vertexIndex] = matrix4x.MultiplyPoint3x4(vertices2[vertexIndex]);
						vertices2[vertexIndex + 1] = matrix4x.MultiplyPoint3x4(vertices2[vertexIndex + 1]);
						vertices2[vertexIndex + 2] = matrix4x.MultiplyPoint3x4(vertices2[vertexIndex + 2]);
						vertices2[vertexIndex + 3] = matrix4x.MultiplyPoint3x4(vertices2[vertexIndex + 3]);
						vertices2[vertexIndex] += vector;
						vertices2[vertexIndex + 1] += vector;
						vertices2[vertexIndex + 2] += vector;
						vertices2[vertexIndex + 3] += vector;
						_003CvertexAnim_003E5__4[j] = vertexAnim;
					}
				}
				for (int k = 0; k < _003CtextInfo_003E5__2.meshInfo.Length; k++)
				{
					_003CtextInfo_003E5__2.meshInfo[k].mesh.vertices = _003CtextInfo_003E5__2.meshInfo[k].vertices;
					vertexJitter.m_TextComponent.UpdateGeometry(_003CtextInfo_003E5__2.meshInfo[k].mesh, k);
				}
				_003CloopCount_003E5__3++;
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
			if (obj == m_TextComponent)
			{
				hasTextChanged = true;
			}
		}

		private IEnumerator AnimateVertexColors()
		{
			return new _003CAnimateVertexColors_003Ed__11(0)
			{
				_003C_003E4__this = this
			};
		}
	}
}
