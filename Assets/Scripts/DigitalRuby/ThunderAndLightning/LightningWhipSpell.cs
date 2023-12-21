using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	public class LightningWhipSpell : LightningSpellScript
	{
		private sealed class _003CWhipForward_003Ed__7 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public LightningWhipSpell _003C_003E4__this;

			private Vector3 _003CwhipPositionForwards_003E5__2;

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
			public _003CWhipForward_003Ed__7(int _003C_003E1__state)
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
				LightningWhipSpell lightningWhipSpell = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
				{
					_003C_003E1__state = -1;
					for (int j = 0; j < lightningWhipSpell.WhipStart.transform.childCount; j++)
					{
						Rigidbody component2 = lightningWhipSpell.WhipStart.transform.GetChild(j).gameObject.GetComponent<Rigidbody>();
						if (component2 != null)
						{
							component2.drag = 0f;
							component2.velocity = Vector3.zero;
							component2.angularVelocity = Vector3.zero;
						}
					}
					lightningWhipSpell.WhipSpring.SetActive(value: true);
					Vector3 position = lightningWhipSpell.WhipStart.GetComponent<Rigidbody>().position;
					Vector3 position2;
					if (Physics.Raycast(position, lightningWhipSpell.Direction, out RaycastHit hitInfo, lightningWhipSpell.MaxDistance, lightningWhipSpell.CollisionMask))
					{
						Vector3 normalized = (hitInfo.point - position).normalized;
						_003CwhipPositionForwards_003E5__2 = position + normalized * lightningWhipSpell.MaxDistance;
						position2 = position - normalized * 25f;
					}
					else
					{
						_003CwhipPositionForwards_003E5__2 = position + lightningWhipSpell.Direction * lightningWhipSpell.MaxDistance;
						position2 = position - lightningWhipSpell.Direction * 25f;
					}
					lightningWhipSpell.WhipSpring.GetComponent<Rigidbody>().position = position2;
					_003C_003E2__current = new WaitForSecondsLightning(0.25f);
					_003C_003E1__state = 1;
					return true;
				}
				case 1:
					_003C_003E1__state = -1;
					lightningWhipSpell.WhipSpring.GetComponent<Rigidbody>().position = _003CwhipPositionForwards_003E5__2;
					_003C_003E2__current = new WaitForSecondsLightning(0.1f);
					_003C_003E1__state = 2;
					return true;
				case 2:
					_003C_003E1__state = -1;
					if (lightningWhipSpell.WhipCrackAudioSource != null)
					{
						lightningWhipSpell.WhipCrackAudioSource.Play();
					}
					_003C_003E2__current = new WaitForSecondsLightning(0.1f);
					_003C_003E1__state = 3;
					return true;
				case 3:
					_003C_003E1__state = -1;
					if (lightningWhipSpell.CollisionParticleSystem != null)
					{
						lightningWhipSpell.CollisionParticleSystem.Play();
					}
					lightningWhipSpell.ApplyCollisionForce(lightningWhipSpell.SpellEnd.transform.position);
					lightningWhipSpell.WhipSpring.SetActive(value: false);
					if (lightningWhipSpell.CollisionCallback != null)
					{
						lightningWhipSpell.CollisionCallback(lightningWhipSpell.SpellEnd.transform.position);
					}
					_003C_003E2__current = new WaitForSecondsLightning(0.1f);
					_003C_003E1__state = 4;
					return true;
				case 4:
					_003C_003E1__state = -1;
					for (int i = 0; i < lightningWhipSpell.WhipStart.transform.childCount; i++)
					{
						Rigidbody component = lightningWhipSpell.WhipStart.transform.GetChild(i).gameObject.GetComponent<Rigidbody>();
						if (component != null)
						{
							component.velocity = Vector3.zero;
							component.angularVelocity = Vector3.zero;
							component.drag = 0.5f;
						}
					}
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

		public GameObject AttachTo;

		public GameObject RotateWith;

		public GameObject WhipHandle;

		public GameObject WhipStart;

		public GameObject WhipSpring;

		public AudioSource WhipCrackAudioSource;

		public Action<Vector3> CollisionCallback;

		private IEnumerator WhipForward()
		{
			return new _003CWhipForward_003Ed__7(0)
			{
				_003C_003E4__this = this
			};
		}

		protected override void Start()
		{
			base.Start();
			WhipSpring.SetActive(value: false);
			WhipHandle.SetActive(value: false);
		}

		protected override void Update()
		{
			base.Update();
			base.gameObject.transform.position = AttachTo.transform.position;
			base.gameObject.transform.rotation = RotateWith.transform.rotation;
		}

		protected override void OnCastSpell()
		{
			StartCoroutine(WhipForward());
		}

		protected override void OnStopSpell()
		{
		}

		protected override void OnActivated()
		{
			base.OnActivated();
			WhipHandle.SetActive(value: true);
		}

		protected override void OnDeactivated()
		{
			base.OnDeactivated();
			WhipHandle.SetActive(value: false);
		}
	}
}
