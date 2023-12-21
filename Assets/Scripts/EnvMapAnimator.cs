using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class EnvMapAnimator : MonoBehaviour
{
	private sealed class _003CStart_003Ed__4 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public EnvMapAnimator _003C_003E4__this;

		private Matrix4x4 _003Cmatrix_003E5__2;

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
			EnvMapAnimator envMapAnimator = _003C_003E4__this;
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
				_003Cmatrix_003E5__2 = default(Matrix4x4);
			}
			_003Cmatrix_003E5__2.SetTRS(Vector3.zero, Quaternion.Euler(Time.time * envMapAnimator.RotationSpeeds.x, Time.time * envMapAnimator.RotationSpeeds.y, Time.time * envMapAnimator.RotationSpeeds.z), Vector3.one);
			envMapAnimator.m_material.SetMatrix("_EnvMatrix", _003Cmatrix_003E5__2);
			_003C_003E2__current = null;
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

	public Vector3 RotationSpeeds;

	private TMP_Text m_textMeshPro;

	private Material m_material;

	private void Awake()
	{
		m_textMeshPro = GetComponent<TMP_Text>();
		m_material = m_textMeshPro.fontSharedMaterial;
	}

	private IEnumerator Start()
	{
		return new _003CStart_003Ed__4(0)
		{
			_003C_003E4__this = this
		};
	}
}
