using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	[RequireComponent(typeof(AudioSource))]
	public class LightningWhipScript : MonoBehaviour
	{
		private sealed class _003CWhipForward_003Ed__10 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public LightningWhipScript _003C_003E4__this;

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
			public _003CWhipForward_003Ed__10(int _003C_003E1__state)
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
				LightningWhipScript lightningWhipScript = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					if (!lightningWhipScript.canWhip)
					{
						break;
					}
					lightningWhipScript.canWhip = false;
					for (int j = 0; j < lightningWhipScript.whipStart.transform.childCount; j++)
					{
						Rigidbody2D component2 = lightningWhipScript.whipStart.transform.GetChild(j).gameObject.GetComponent<Rigidbody2D>();
						if (component2 != null)
						{
							component2.drag = 0f;
						}
					}
					lightningWhipScript.audioSource.PlayOneShot(lightningWhipScript.WhipCrack);
					lightningWhipScript.whipSpring.GetComponent<SpringJoint2D>().enabled = true;
					lightningWhipScript.whipSpring.GetComponent<Rigidbody2D>().position = lightningWhipScript.whipHandle.GetComponent<Rigidbody2D>().position + new Vector2(-15f, 5f);
					_003C_003E2__current = new WaitForSecondsLightning(0.2f);
					_003C_003E1__state = 1;
					return true;
				case 1:
					_003C_003E1__state = -1;
					lightningWhipScript.whipSpring.GetComponent<Rigidbody2D>().position = lightningWhipScript.whipHandle.GetComponent<Rigidbody2D>().position + new Vector2(15f, 2.5f);
					_003C_003E2__current = new WaitForSecondsLightning(0.15f);
					_003C_003E1__state = 2;
					return true;
				case 2:
					_003C_003E1__state = -1;
					lightningWhipScript.audioSource.PlayOneShot(lightningWhipScript.WhipCrackThunder, 0.5f);
					_003C_003E2__current = new WaitForSecondsLightning(0.15f);
					_003C_003E1__state = 3;
					return true;
				case 3:
					_003C_003E1__state = -1;
					lightningWhipScript.whipEndStrike.GetComponent<ParticleSystem>().Play();
					lightningWhipScript.whipSpring.GetComponent<SpringJoint2D>().enabled = false;
					_003C_003E2__current = new WaitForSecondsLightning(0.65f);
					_003C_003E1__state = 4;
					return true;
				case 4:
					_003C_003E1__state = -1;
					for (int i = 0; i < lightningWhipScript.whipStart.transform.childCount; i++)
					{
						Rigidbody2D component = lightningWhipScript.whipStart.transform.GetChild(i).gameObject.GetComponent<Rigidbody2D>();
						if (component != null)
						{
							component.velocity = Vector2.zero;
							component.drag = 0.5f;
						}
					}
					lightningWhipScript.canWhip = true;
					break;
				}
				return false;
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

		public AudioClip WhipCrack;

		public AudioClip WhipCrackThunder;

		private AudioSource audioSource;

		private GameObject whipStart;

		private GameObject whipEndStrike;

		private GameObject whipHandle;

		private GameObject whipSpring;

		private Vector2 prevDrag;

		private bool dragging;

		private bool canWhip = true;

		private IEnumerator WhipForward()
		{
			return new _003CWhipForward_003Ed__10(0)
			{
				_003C_003E4__this = this
			};
		}

		private void Start()
		{
			whipStart = GameObject.Find("WhipStart");
			whipEndStrike = GameObject.Find("WhipEndStrike");
			whipHandle = GameObject.Find("WhipHandle");
			whipSpring = GameObject.Find("WhipSpring");
			audioSource = GetComponent<AudioSource>();
		}

		private void Update()
		{
			if (!dragging && Input.GetMouseButtonDown(0))
			{
				Vector2 point = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
				Collider2D collider2D = Physics2D.OverlapPoint(point);
				if (collider2D != null && collider2D.gameObject == whipHandle)
				{
					dragging = true;
					prevDrag = point;
				}
			}
			else if (dragging && Input.GetMouseButton(0))
			{
				Vector2 a = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
				Vector2 b = a - prevDrag;
				Rigidbody2D component = whipHandle.GetComponent<Rigidbody2D>();
				component.MovePosition(component.position + b);
				prevDrag = a;
			}
			else
			{
				dragging = false;
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
			{
				StartCoroutine(WhipForward());
			}
		}
	}
}
