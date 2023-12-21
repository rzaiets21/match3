using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace TMPro.Examples
{
	public class VertexZoom : MonoBehaviour
	{
		private sealed class _003C_003Ec__DisplayClass10_0
		{
			public List<float> modifiedCharScale;

			public Comparison<int> _003C_003E9__0;

			internal int _003CAnimateVertexColors_003Eb__0(int a, int b)
			{
				return modifiedCharScale[a].CompareTo(modifiedCharScale[b]);
			}
		}

		private sealed class _003CAnimateVertexColors_003Ed__10 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public VertexZoom _003C_003E4__this;

			private _003C_003Ec__DisplayClass10_0 _003C_003E8__1;

			private TMP_TextInfo _003CtextInfo_003E5__2;

			private TMP_MeshInfo[] _003CcachedMeshInfoVertexData_003E5__3;

			private List<int> _003CscaleSortingOrder_003E5__4;

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
				VertexZoom vertexZoom = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003C_003E8__1 = new _003C_003Ec__DisplayClass10_0();
					vertexZoom.m_TextComponent.ForceMeshUpdate();
					_003CtextInfo_003E5__2 = vertexZoom.m_TextComponent.textInfo;
					_003CcachedMeshInfoVertexData_003E5__3 = _003CtextInfo_003E5__2.CopyMeshInfoVertexData();
					_003C_003E8__1.modifiedCharScale = new List<float>();
					_003CscaleSortingOrder_003E5__4 = new List<int>();
					vertexZoom.hasTextChanged = true;
					break;
				case 1:
					_003C_003E1__state = -1;
					break;
				case 2:
					_003C_003E1__state = -1;
					break;
				}
				if (vertexZoom.hasTextChanged)
				{
					_003CcachedMeshInfoVertexData_003E5__3 = _003CtextInfo_003E5__2.CopyMeshInfoVertexData();
					vertexZoom.hasTextChanged = false;
				}
				int characterCount = _003CtextInfo_003E5__2.characterCount;
				if (characterCount == 0)
				{
					_003C_003E2__current = new WaitForSeconds(0.25f);
					_003C_003E1__state = 1;
					return true;
				}
				_003C_003E8__1.modifiedCharScale.Clear();
				_003CscaleSortingOrder_003E5__4.Clear();
				for (int i = 0; i < characterCount; i++)
				{
					if (_003CtextInfo_003E5__2.characterInfo[i].isVisible)
					{
						int materialReferenceIndex = _003CtextInfo_003E5__2.characterInfo[i].materialReferenceIndex;
						int vertexIndex = _003CtextInfo_003E5__2.characterInfo[i].vertexIndex;
						Vector3[] vertices = _003CcachedMeshInfoVertexData_003E5__3[materialReferenceIndex].vertices;
						Vector3 vector = (Vector2)((vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2f);
						Vector3[] vertices2 = _003CtextInfo_003E5__2.meshInfo[materialReferenceIndex].vertices;
						vertices2[vertexIndex] = vertices[vertexIndex] - vector;
						vertices2[vertexIndex + 1] = vertices[vertexIndex + 1] - vector;
						vertices2[vertexIndex + 2] = vertices[vertexIndex + 2] - vector;
						vertices2[vertexIndex + 3] = vertices[vertexIndex + 3] - vector;
						float num2 = UnityEngine.Random.Range(1f, 1.5f);
						_003C_003E8__1.modifiedCharScale.Add(num2);
						_003CscaleSortingOrder_003E5__4.Add(_003C_003E8__1.modifiedCharScale.Count - 1);
						Matrix4x4 matrix4x = Matrix4x4.TRS(new Vector3(0f, 0f, 0f), Quaternion.identity, Vector3.one * num2);
						vertices2[vertexIndex] = matrix4x.MultiplyPoint3x4(vertices2[vertexIndex]);
						vertices2[vertexIndex + 1] = matrix4x.MultiplyPoint3x4(vertices2[vertexIndex + 1]);
						vertices2[vertexIndex + 2] = matrix4x.MultiplyPoint3x4(vertices2[vertexIndex + 2]);
						vertices2[vertexIndex + 3] = matrix4x.MultiplyPoint3x4(vertices2[vertexIndex + 3]);
						vertices2[vertexIndex] += vector;
						vertices2[vertexIndex + 1] += vector;
						vertices2[vertexIndex + 2] += vector;
						vertices2[vertexIndex + 3] += vector;
						Vector2[] uvs = _003CcachedMeshInfoVertexData_003E5__3[materialReferenceIndex].uvs0;
						Vector2[] uvs2 = _003CtextInfo_003E5__2.meshInfo[materialReferenceIndex].uvs0;
						uvs2[vertexIndex] = uvs[vertexIndex];
						uvs2[vertexIndex + 1] = uvs[vertexIndex + 1];
						uvs2[vertexIndex + 2] = uvs[vertexIndex + 2];
						uvs2[vertexIndex + 3] = uvs[vertexIndex + 3];
						Color32[] colors = _003CcachedMeshInfoVertexData_003E5__3[materialReferenceIndex].colors32;
						Color32[] colors2 = _003CtextInfo_003E5__2.meshInfo[materialReferenceIndex].colors32;
						colors2[vertexIndex] = colors[vertexIndex];
						colors2[vertexIndex + 1] = colors[vertexIndex + 1];
						colors2[vertexIndex + 2] = colors[vertexIndex + 2];
						colors2[vertexIndex + 3] = colors[vertexIndex + 3];
					}
				}
				for (int j = 0; j < _003CtextInfo_003E5__2.meshInfo.Length; j++)
				{
					_003CscaleSortingOrder_003E5__4.Sort(_003C_003E8__1._003CAnimateVertexColors_003Eb__0);
					_003CtextInfo_003E5__2.meshInfo[j].SortGeometry(_003CscaleSortingOrder_003E5__4);
					_003CtextInfo_003E5__2.meshInfo[j].mesh.vertices = _003CtextInfo_003E5__2.meshInfo[j].vertices;
					_003CtextInfo_003E5__2.meshInfo[j].mesh.uv = _003CtextInfo_003E5__2.meshInfo[j].uvs0;
					_003CtextInfo_003E5__2.meshInfo[j].mesh.colors32 = _003CtextInfo_003E5__2.meshInfo[j].colors32;
					vertexZoom.m_TextComponent.UpdateGeometry(_003CtextInfo_003E5__2.meshInfo[j].mesh, j);
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
			if (obj == m_TextComponent)
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
