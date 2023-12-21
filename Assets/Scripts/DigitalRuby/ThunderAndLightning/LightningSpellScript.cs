using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	public abstract class LightningSpellScript : MonoBehaviour
	{
		private sealed class _003CStopAfterSecondsCoRoutine_003Ed__19 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public LightningSpellScript _003C_003E4__this;

			public float seconds;

			private int _003Ctoken_003E5__2;

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
			public _003CStopAfterSecondsCoRoutine_003Ed__19(int _003C_003E1__state)
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
				LightningSpellScript lightningSpellScript = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003Ctoken_003E5__2 = lightningSpellScript.stopToken;
					_003C_003E2__current = new WaitForSecondsLightning(seconds);
					_003C_003E1__state = 1;
					return true;
				case 1:
					_003C_003E1__state = -1;
					if (_003Ctoken_003E5__2 == lightningSpellScript.stopToken)
					{
						lightningSpellScript.StopSpell();
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

		public GameObject SpellStart;

		public GameObject SpellEnd;

		public Vector3 Direction;

		public float MaxDistance = 15f;

		public bool CollisionIsExplosion;

		public float CollisionRadius = 1f;

		public float CollisionForce = 50f;

		public ForceMode CollisionForceMode = ForceMode.Impulse;

		public ParticleSystem CollisionParticleSystem;

		public LayerMask CollisionMask = -1;

		public AudioSource CollisionAudioSource;

		public AudioClip[] CollisionAudioClips;

		public RangeOfFloats CollisionVolumeRange = new RangeOfFloats
		{
			Minimum = 0.4f,
			Maximum = 0.6f
		};

		public float Duration;

		public float Cooldown;

		public AudioSource EmissionSound;

		public ParticleSystem EmissionParticleSystem;

		public Light EmissionLight;

		private int stopToken;

		private float _003CDurationTimer_003Ek__BackingField;

		private float _003CCooldownTimer_003Ek__BackingField;

		private bool _003CCasting_003Ek__BackingField;

		protected float DurationTimer
		{
			get
			{
				return _003CDurationTimer_003Ek__BackingField;
			}
			private set
			{
				_003CDurationTimer_003Ek__BackingField = value;
			}
		}

		protected float CooldownTimer
		{
			get
			{
				return _003CCooldownTimer_003Ek__BackingField;
			}
			private set
			{
				_003CCooldownTimer_003Ek__BackingField = value;
			}
		}

		public bool Casting
		{
			get
			{
				return _003CCasting_003Ek__BackingField;
			}
			private set
			{
				_003CCasting_003Ek__BackingField = value;
			}
		}

		public bool CanCastSpell
		{
			get
			{
				if (!Casting)
				{
					return CooldownTimer <= 0f;
				}
				return false;
			}
		}

		private IEnumerator StopAfterSecondsCoRoutine(float seconds)
		{
			return new _003CStopAfterSecondsCoRoutine_003Ed__19(0)
			{
				_003C_003E4__this = this,
				seconds = seconds
			};
		}

		protected void ApplyCollisionForce(Vector3 point)
		{
			if (!(CollisionForce > 0f) || !(CollisionRadius > 0f))
			{
				return;
			}
			Collider[] array = Physics.OverlapSphere(point, CollisionRadius, CollisionMask);
			for (int i = 0; i < array.Length; i++)
			{
				Rigidbody component = array[i].GetComponent<Rigidbody>();
				if (component != null)
				{
					if (CollisionIsExplosion)
					{
						component.AddExplosionForce(CollisionForce, point, CollisionRadius, CollisionForce * 0.02f, CollisionForceMode);
					}
					else
					{
						component.AddForce(CollisionForce * Direction, CollisionForceMode);
					}
				}
			}
		}

		protected void PlayCollisionSound(Vector3 pos)
		{
			if (CollisionAudioSource != null && CollisionAudioClips != null && CollisionAudioClips.Length != 0)
			{
				int num = UnityEngine.Random.Range(0, CollisionAudioClips.Length - 1);
				float volumeScale = UnityEngine.Random.Range(CollisionVolumeRange.Minimum, CollisionVolumeRange.Maximum);
				CollisionAudioSource.transform.position = pos;
				CollisionAudioSource.PlayOneShot(CollisionAudioClips[num], volumeScale);
			}
		}

		protected virtual void Start()
		{
			if (EmissionLight != null)
			{
				EmissionLight.enabled = false;
			}
		}

		protected virtual void Update()
		{
			CooldownTimer = Mathf.Max(0f, CooldownTimer - LightningBoltScript.DeltaTime);
			DurationTimer = Mathf.Max(0f, DurationTimer - LightningBoltScript.DeltaTime);
		}

		protected virtual void LateUpdate()
		{
		}

		protected virtual void OnDestroy()
		{
		}

		protected abstract void OnCastSpell();

		protected abstract void OnStopSpell();

		protected virtual void OnActivated()
		{
		}

		protected virtual void OnDeactivated()
		{
		}

		public bool CastSpell()
		{
			if (!CanCastSpell)
			{
				return false;
			}
			Casting = true;
			DurationTimer = Duration;
			CooldownTimer = Cooldown;
			OnCastSpell();
			if (Duration > 0f)
			{
				StopAfterSeconds(Duration);
			}
			if (EmissionParticleSystem != null)
			{
				EmissionParticleSystem.Play();
			}
			if (EmissionLight != null)
			{
				EmissionLight.transform.position = SpellStart.transform.position;
				EmissionLight.enabled = true;
			}
			if (EmissionSound != null)
			{
				EmissionSound.Play();
			}
			return true;
		}

		public void StopSpell()
		{
			if (Casting)
			{
				stopToken++;
				if (EmissionParticleSystem != null)
				{
					EmissionParticleSystem.Stop();
				}
				if (EmissionLight != null)
				{
					EmissionLight.enabled = false;
				}
				if (EmissionSound != null && EmissionSound.loop)
				{
					EmissionSound.Stop();
				}
				DurationTimer = 0f;
				Casting = false;
				OnStopSpell();
			}
		}

		public void ActivateSpell()
		{
			OnActivated();
		}

		public void DeactivateSpell()
		{
			OnDeactivated();
		}

		public void StopAfterSeconds(float seconds)
		{
			StartCoroutine(StopAfterSecondsCoRoutine(seconds));
		}

		public static GameObject FindChildRecursively(Transform t, string name)
		{
			if (t.name == name)
			{
				return t.gameObject;
			}
			for (int i = 0; i < t.childCount; i++)
			{
				GameObject gameObject = FindChildRecursively(t.GetChild(i), name);
				if (gameObject != null)
				{
					return gameObject;
				}
			}
			return null;
		}
	}
}
