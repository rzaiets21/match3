using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class LootBoxController : MonoBehaviour
{
	private sealed class _003CPlayFx_003Ed__12 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public LootBoxController _003C_003E4__this;

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
		public _003CPlayFx_003Ed__12(int _003C_003E1__state)
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
			LootBoxController lootBoxController = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				lootBoxController.isOpened = true;
				lootBoxController.idEffect = Mathf.Clamp(lootBoxController.idEffect, 1, 25);
				lootBoxController.effectsText.text = "Type       " + lootBoxController.idEffect + " / 25";
				_003C_003E2__current = new WaitForSeconds(0.2f);
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				UnityEngine.Object.Destroy(lootBoxController.Lootbox);
				lootBoxController.Lootbox = UnityEngine.Object.Instantiate(lootBoxController.IconPrefabs[2], lootBoxController.transform.position, lootBoxController.transform.rotation);
				_003C_003E2__current = new WaitForSeconds(0.1f);
				_003C_003E1__state = 2;
				return true;
			case 2:
				_003C_003E1__state = -1;
				UnityEngine.Object.Instantiate(lootBoxController.EffectPrefabs[lootBoxController.idEffect], lootBoxController.transform.position, lootBoxController.transform.rotation);
				CameraShake.myCameraShake.ShakeCamera(0.3f, 0.1f);
				return false;
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

	private sealed class _003CPlayIcon_003Ed__13 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public LootBoxController _003C_003E4__this;

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
		public _003CPlayIcon_003Ed__13(int _003C_003E1__state)
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
			LootBoxController lootBoxController = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
			{
				_003C_003E1__state = -1;
				lootBoxController.DesIconObjs = GameObject.FindGameObjectsWithTag("Icon");
				GameObject[] desIconObjs = lootBoxController.DesIconObjs;
				for (int i = 0; i < desIconObjs.Length; i++)
				{
					UnityEngine.Object.Destroy(desIconObjs[i].gameObject);
				}
				_003C_003E2__current = new WaitForSeconds(0.1f);
				_003C_003E1__state = 1;
				return true;
			}
			case 1:
				_003C_003E1__state = -1;
				lootBoxController.Lootbox = UnityEngine.Object.Instantiate(lootBoxController.IconPrefabs[1], lootBoxController.transform.position, lootBoxController.transform.rotation);
				return false;
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

	public int idIcon;

	public int idEffect;

	public bool isOpened;

	public GameObject[] IconPrefabs;

	public GameObject[] EffectPrefabs;

	public GameObject[] DesFxObjs;

	public GameObject[] DesIconObjs;

	private GameObject Lootbox;

	public Text effectsText;

	public Text nameEffectText;

	private void Start()
	{
		idEffect++;
		idIcon++;
		effectsText.text = "Type       " + idEffect + " / 25";
		nameEffectText.text = EffectPrefabs[idEffect].gameObject.name;
		SetupVfx();
		isOpened = false;
	}

	private void OnMouseDown()
	{
		if (!isOpened)
		{
			StartCoroutine(PlayFx());
		}
	}

	private IEnumerator PlayFx()
	{
		return new _003CPlayFx_003Ed__12(0)
		{
			_003C_003E4__this = this
		};
	}

	private IEnumerator PlayIcon()
	{
		return new _003CPlayIcon_003Ed__13(0)
		{
			_003C_003E4__this = this
		};
	}

	public void ChangedFx(int i)
	{
		ResetVfx();
		idEffect += i;
		idEffect = Mathf.Clamp(idEffect, 1, 25);
		effectsText.text = "Type       " + idEffect + " / 25";
		nameEffectText.text = EffectPrefabs[idEffect].gameObject.name;
	}

	public void SetupVfx()
	{
		Lootbox = UnityEngine.Object.Instantiate(IconPrefabs[1], base.transform.position, base.transform.rotation);
	}

	public void PlayAllVfx()
	{
		if (!isOpened)
		{
			StartCoroutine(PlayFx());
		}
	}

	public void ResetVfx()
	{
		DesFxObjs = GameObject.FindGameObjectsWithTag("Effects");
		GameObject[] desFxObjs = DesFxObjs;
		for (int i = 0; i < desFxObjs.Length; i++)
		{
			UnityEngine.Object.Destroy(desFxObjs[i].gameObject);
		}
		isOpened = false;
		DesIconObjs = GameObject.FindGameObjectsWithTag("Icon");
		desFxObjs = DesIconObjs;
		for (int i = 0; i < desFxObjs.Length; i++)
		{
			UnityEngine.Object.Destroy(desFxObjs[i].gameObject);
		}
		StartCoroutine(PlayIcon());
	}
}
