using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace TMPro.Examples
{
	public class TextMeshProFloatingText : MonoBehaviour
	{
		private sealed class _003CDisplayTextMeshProFloatingText_003Ed__12 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public TextMeshProFloatingText _003C_003E4__this;

			private float _003CCountDuration_003E5__2;

			private float _003Cstarting_Count_003E5__3;

			private float _003Ccurrent_Count_003E5__4;

			private Vector3 _003Cstart_pos_003E5__5;

			private Color32 _003Cstart_color_003E5__6;

			private float _003Calpha_003E5__7;

			private float _003CfadeDuration_003E5__8;

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
			public _003CDisplayTextMeshProFloatingText_003Ed__12(int _003C_003E1__state)
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
				TextMeshProFloatingText textMeshProFloatingText = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
				{
					_003C_003E1__state = -1;
					_003CCountDuration_003E5__2 = 2f;
					_003Cstarting_Count_003E5__3 = UnityEngine.Random.Range(5f, 20f);
					_003Ccurrent_Count_003E5__4 = _003Cstarting_Count_003E5__3;
					_003Cstart_pos_003E5__5 = textMeshProFloatingText.m_floatingText_Transform.position;
					_003Cstart_color_003E5__6 = textMeshProFloatingText.m_textMeshPro.color;
					_003Calpha_003E5__7 = 255f;
					int num2 = 0;
					_003CfadeDuration_003E5__8 = 3f / _003Cstarting_Count_003E5__3 * _003CCountDuration_003E5__2;
					goto IL_024a;
				}
				case 1:
					_003C_003E1__state = -1;
					goto IL_024a;
				case 2:
					{
						_003C_003E1__state = -1;
						textMeshProFloatingText.m_floatingText_Transform.position = _003Cstart_pos_003E5__5;
						textMeshProFloatingText.StartCoroutine(textMeshProFloatingText.DisplayTextMeshProFloatingText());
						return false;
					}
					IL_024a:
					if (_003Ccurrent_Count_003E5__4 > 0f)
					{
						_003Ccurrent_Count_003E5__4 -= Time.deltaTime / _003CCountDuration_003E5__2 * _003Cstarting_Count_003E5__3;
						if (_003Ccurrent_Count_003E5__4 <= 3f)
						{
							_003Calpha_003E5__7 = Mathf.Clamp(_003Calpha_003E5__7 - Time.deltaTime / _003CfadeDuration_003E5__8 * 255f, 0f, 255f);
						}
						int num2 = (int)_003Ccurrent_Count_003E5__4;
						textMeshProFloatingText.m_textMeshPro.text = num2.ToString();
						textMeshProFloatingText.m_textMeshPro.color = new Color32(_003Cstart_color_003E5__6.r, _003Cstart_color_003E5__6.g, _003Cstart_color_003E5__6.b, (byte)_003Calpha_003E5__7);
						textMeshProFloatingText.m_floatingText_Transform.position += new Vector3(0f, _003Cstarting_Count_003E5__3 * Time.deltaTime, 0f);
						if (!TMPro_ExtensionMethods.Compare(textMeshProFloatingText.lastPOS, textMeshProFloatingText.m_cameraTransform.position, 1000) || !TMPro_ExtensionMethods.Compare(textMeshProFloatingText.lastRotation, textMeshProFloatingText.m_cameraTransform.rotation, 1000))
						{
							textMeshProFloatingText.lastPOS = textMeshProFloatingText.m_cameraTransform.position;
							textMeshProFloatingText.lastRotation = textMeshProFloatingText.m_cameraTransform.rotation;
							textMeshProFloatingText.m_floatingText_Transform.rotation = textMeshProFloatingText.lastRotation;
							Vector3 vector = textMeshProFloatingText.m_transform.position - textMeshProFloatingText.lastPOS;
							textMeshProFloatingText.m_transform.forward = new Vector3(vector.x, 0f, vector.z);
						}
						_003C_003E2__current = new WaitForEndOfFrame();
						_003C_003E1__state = 1;
						return true;
					}
					_003C_003E2__current = new WaitForSeconds(UnityEngine.Random.Range(0.1f, 1f));
					_003C_003E1__state = 2;
					return true;
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

		private sealed class _003CDisplayTextMeshFloatingText_003Ed__13 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public TextMeshProFloatingText _003C_003E4__this;

			private float _003CCountDuration_003E5__2;

			private float _003Cstarting_Count_003E5__3;

			private float _003Ccurrent_Count_003E5__4;

			private Vector3 _003Cstart_pos_003E5__5;

			private Color32 _003Cstart_color_003E5__6;

			private float _003Calpha_003E5__7;

			private float _003CfadeDuration_003E5__8;

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
			public _003CDisplayTextMeshFloatingText_003Ed__13(int _003C_003E1__state)
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
				TextMeshProFloatingText textMeshProFloatingText = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
				{
					_003C_003E1__state = -1;
					_003CCountDuration_003E5__2 = 2f;
					_003Cstarting_Count_003E5__3 = UnityEngine.Random.Range(5f, 20f);
					_003Ccurrent_Count_003E5__4 = _003Cstarting_Count_003E5__3;
					_003Cstart_pos_003E5__5 = textMeshProFloatingText.m_floatingText_Transform.position;
					_003Cstart_color_003E5__6 = textMeshProFloatingText.m_textMesh.color;
					_003Calpha_003E5__7 = 255f;
					int num2 = 0;
					_003CfadeDuration_003E5__8 = 3f / _003Cstarting_Count_003E5__3 * _003CCountDuration_003E5__2;
					goto IL_024a;
				}
				case 1:
					_003C_003E1__state = -1;
					goto IL_024a;
				case 2:
					{
						_003C_003E1__state = -1;
						textMeshProFloatingText.m_floatingText_Transform.position = _003Cstart_pos_003E5__5;
						textMeshProFloatingText.StartCoroutine(textMeshProFloatingText.DisplayTextMeshFloatingText());
						return false;
					}
					IL_024a:
					if (_003Ccurrent_Count_003E5__4 > 0f)
					{
						_003Ccurrent_Count_003E5__4 -= Time.deltaTime / _003CCountDuration_003E5__2 * _003Cstarting_Count_003E5__3;
						if (_003Ccurrent_Count_003E5__4 <= 3f)
						{
							_003Calpha_003E5__7 = Mathf.Clamp(_003Calpha_003E5__7 - Time.deltaTime / _003CfadeDuration_003E5__8 * 255f, 0f, 255f);
						}
						int num2 = (int)_003Ccurrent_Count_003E5__4;
						textMeshProFloatingText.m_textMesh.text = num2.ToString();
						textMeshProFloatingText.m_textMesh.color = new Color32(_003Cstart_color_003E5__6.r, _003Cstart_color_003E5__6.g, _003Cstart_color_003E5__6.b, (byte)_003Calpha_003E5__7);
						textMeshProFloatingText.m_floatingText_Transform.position += new Vector3(0f, _003Cstarting_Count_003E5__3 * Time.deltaTime, 0f);
						if (!TMPro_ExtensionMethods.Compare(textMeshProFloatingText.lastPOS, textMeshProFloatingText.m_cameraTransform.position, 1000) || !TMPro_ExtensionMethods.Compare(textMeshProFloatingText.lastRotation, textMeshProFloatingText.m_cameraTransform.rotation, 1000))
						{
							textMeshProFloatingText.lastPOS = textMeshProFloatingText.m_cameraTransform.position;
							textMeshProFloatingText.lastRotation = textMeshProFloatingText.m_cameraTransform.rotation;
							textMeshProFloatingText.m_floatingText_Transform.rotation = textMeshProFloatingText.lastRotation;
							Vector3 vector = textMeshProFloatingText.m_transform.position - textMeshProFloatingText.lastPOS;
							textMeshProFloatingText.m_transform.forward = new Vector3(vector.x, 0f, vector.z);
						}
						_003C_003E2__current = new WaitForEndOfFrame();
						_003C_003E1__state = 1;
						return true;
					}
					_003C_003E2__current = new WaitForSeconds(UnityEngine.Random.Range(0.1f, 1f));
					_003C_003E1__state = 2;
					return true;
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

		public Font TheFont;

		private GameObject m_floatingText;

		private TextMeshPro m_textMeshPro;

		private TextMesh m_textMesh;

		private Transform m_transform;

		private Transform m_floatingText_Transform;

		private Transform m_cameraTransform;

		private Vector3 lastPOS = Vector3.zero;

		private Quaternion lastRotation = Quaternion.identity;

		public int SpawnType;

		private void Awake()
		{
			m_transform = base.transform;
			m_floatingText = new GameObject(base.name + " floating text");
			m_cameraTransform = Camera.main.transform;
		}

		private void Start()
		{
			if (SpawnType == 0)
			{
				m_textMeshPro = m_floatingText.AddComponent<TextMeshPro>();
				m_textMeshPro.rectTransform.sizeDelta = new Vector2(3f, 3f);
				m_floatingText_Transform = m_floatingText.transform;
				m_floatingText_Transform.position = m_transform.position + new Vector3(0f, 15f, 0f);
				m_textMeshPro.alignment = TextAlignmentOptions.Center;
				m_textMeshPro.color = new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), byte.MaxValue);
				m_textMeshPro.fontSize = 24f;
				m_textMeshPro.enableKerning = false;
				m_textMeshPro.text = string.Empty;
				StartCoroutine(DisplayTextMeshProFloatingText());
			}
			else if (SpawnType == 1)
			{
				m_floatingText_Transform = m_floatingText.transform;
				m_floatingText_Transform.position = m_transform.position + new Vector3(0f, 15f, 0f);
				m_textMesh = m_floatingText.AddComponent<TextMesh>();
				m_textMesh.font = Resources.Load<Font>("Fonts/ARIAL");
				m_textMesh.GetComponent<Renderer>().sharedMaterial = m_textMesh.font.material;
				m_textMesh.color = new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), byte.MaxValue);
				m_textMesh.anchor = TextAnchor.LowerCenter;
				m_textMesh.fontSize = 24;
				StartCoroutine(DisplayTextMeshFloatingText());
			}
			else
			{
				int spawnType = SpawnType;
			}
		}

		public IEnumerator DisplayTextMeshProFloatingText()
		{
			return new _003CDisplayTextMeshProFloatingText_003Ed__12(0)
			{
				_003C_003E4__this = this
			};
		}

		public IEnumerator DisplayTextMeshFloatingText()
		{
			return new _003CDisplayTextMeshFloatingText_003Ed__13(0)
			{
				_003C_003E4__this = this
			};
		}
	}
}
