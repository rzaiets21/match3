using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace TMPro.Examples
{
	public class Benchmark01 : MonoBehaviour
	{
		private sealed class _003CStart_003Ed__10 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public Benchmark01 _003C_003E4__this;

			private int _003Ci_003E5__2;

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
			public _003CStart_003Ed__10(int _003C_003E1__state)
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
				Benchmark01 benchmark = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					if (benchmark.BenchmarkType == 0)
					{
						benchmark.m_textMeshPro = benchmark.gameObject.AddComponent<TextMeshPro>();
						benchmark.m_textMeshPro.autoSizeTextContainer = true;
						if (benchmark.TMProFont != null)
						{
							benchmark.m_textMeshPro.font = benchmark.TMProFont;
						}
						benchmark.m_textMeshPro.fontSize = 48f;
						benchmark.m_textMeshPro.alignment = TextAlignmentOptions.Center;
						benchmark.m_textMeshPro.extraPadding = true;
						benchmark.m_textMeshPro.enableWordWrapping = false;
						benchmark.m_material01 = benchmark.m_textMeshPro.font.material;
						benchmark.m_material02 = Resources.Load<Material>("Fonts & Materials/LiberationSans SDF - Drop Shadow");
					}
					else if (benchmark.BenchmarkType == 1)
					{
						benchmark.m_textMesh = benchmark.gameObject.AddComponent<TextMesh>();
						if (benchmark.TextMeshFont != null)
						{
							benchmark.m_textMesh.font = benchmark.TextMeshFont;
							benchmark.m_textMesh.GetComponent<Renderer>().sharedMaterial = benchmark.m_textMesh.font.material;
						}
						else
						{
							benchmark.m_textMesh.font = (Resources.Load("Fonts/ARIAL", typeof(Font)) as Font);
							benchmark.m_textMesh.GetComponent<Renderer>().sharedMaterial = benchmark.m_textMesh.font.material;
						}
						benchmark.m_textMesh.fontSize = 48;
						benchmark.m_textMesh.anchor = TextAnchor.MiddleCenter;
					}
					_003Ci_003E5__2 = 0;
					goto IL_0280;
				case 1:
					_003C_003E1__state = -1;
					_003Ci_003E5__2++;
					goto IL_0280;
				case 2:
					{
						_003C_003E1__state = -1;
						return false;
					}
					IL_0280:
					if (_003Ci_003E5__2 <= 1000000)
					{
						if (benchmark.BenchmarkType == 0)
						{
							benchmark.m_textMeshPro.SetText("The <#0050FF>count is: </color>{0}", _003Ci_003E5__2 % 1000);
							if (_003Ci_003E5__2 % 1000 == 999)
							{
								TextMeshPro textMeshPro = benchmark.m_textMeshPro;
								object fontSharedMaterial;
								if (!(benchmark.m_textMeshPro.fontSharedMaterial == benchmark.m_material01))
								{
									Material material2 = benchmark.m_textMeshPro.fontSharedMaterial = benchmark.m_material01;
									fontSharedMaterial = material2;
								}
								else
								{
									Material material2 = benchmark.m_textMeshPro.fontSharedMaterial = benchmark.m_material02;
									fontSharedMaterial = material2;
								}
								textMeshPro.fontSharedMaterial = (Material)fontSharedMaterial;
							}
						}
						else if (benchmark.BenchmarkType == 1)
						{
							benchmark.m_textMesh.text = "The <color=#0050FF>count is: </color>" + (_003Ci_003E5__2 % 1000).ToString();
						}
						_003C_003E2__current = null;
						_003C_003E1__state = 1;
						return true;
					}
					_003C_003E2__current = null;
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

		public int BenchmarkType;

		public TMP_FontAsset TMProFont;

		public Font TextMeshFont;

		private TextMeshPro m_textMeshPro;

		private TextContainer m_textContainer;

		private TextMesh m_textMesh;

		private const string label01 = "The <#0050FF>count is: </color>{0}";

		private const string label02 = "The <color=#0050FF>count is: </color>";

		private Material m_material01;

		private Material m_material02;

		private IEnumerator Start()
		{
			return new _003CStart_003Ed__10(0)
			{
				_003C_003E4__this = this
			};
		}
	}
}
