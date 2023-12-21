using System;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	public class LightningParticleSpellScript : LightningSpellScript, ICollisionHandler
	{
		public ParticleSystem ParticleSystem;

		public float CollisionInterval;

		protected float collisionTimer;

		public Action<GameObject, List<ParticleCollisionEvent>, int> CollisionCallback;

		public bool EnableParticleLights = true;

		public RangeOfFloats ParticleLightRange;

		public RangeOfFloats ParticleLightIntensity;

		public Color ParticleLightColor1;

		public Color ParticleLightColor2;

		public LayerMask ParticleLightCullingMask;

		private ParticleSystem.Particle[] particles;

		private readonly List<GameObject> particleLights;

		private void PopulateParticleLight(Light src)
		{
			src.bounceIntensity = 0f;
			src.type = LightType.Point;
			src.shadows = LightShadows.None;
			src.color = new Color(UnityEngine.Random.Range(ParticleLightColor1.r, ParticleLightColor2.r), UnityEngine.Random.Range(ParticleLightColor1.g, ParticleLightColor2.g), UnityEngine.Random.Range(ParticleLightColor1.b, ParticleLightColor2.b), 1f);
			src.cullingMask = ParticleLightCullingMask;
			src.intensity = UnityEngine.Random.Range(ParticleLightIntensity.Minimum, ParticleLightIntensity.Maximum);
			src.range = UnityEngine.Random.Range(ParticleLightRange.Minimum, ParticleLightRange.Maximum);
		}

		private void UpdateParticleLights()
		{
			if (EnableParticleLights)
			{
				int num = ParticleSystem.GetParticles(particles);
				while (particleLights.Count < num)
				{
					GameObject gameObject = new GameObject("LightningParticleSpellLight");
					gameObject.hideFlags = HideFlags.HideAndDontSave;
					PopulateParticleLight(gameObject.AddComponent<Light>());
					particleLights.Add(gameObject);
				}
				while (particleLights.Count > num)
				{
					UnityEngine.Object.Destroy(particleLights[particleLights.Count - 1]);
					particleLights.RemoveAt(particleLights.Count - 1);
				}
				for (int i = 0; i < num; i++)
				{
					particleLights[i].transform.position = particles[i].position;
				}
			}
		}

		private void UpdateParticleSystems()
		{
			if (EmissionParticleSystem != null && EmissionParticleSystem.isPlaying)
			{
				EmissionParticleSystem.transform.position = SpellStart.transform.position;
				EmissionParticleSystem.transform.forward = Direction;
			}
			if (ParticleSystem != null)
			{
				if (ParticleSystem.isPlaying)
				{
					ParticleSystem.transform.position = SpellStart.transform.position;
					ParticleSystem.transform.forward = Direction;
				}
				UpdateParticleLights();
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			foreach (GameObject particleLight in particleLights)
			{
				UnityEngine.Object.Destroy(particleLight);
			}
		}

		protected override void Start()
		{
			base.Start();
		}

		protected override void Update()
		{
			base.Update();
			UpdateParticleSystems();
			collisionTimer -= LightningBoltScript.DeltaTime;
		}

		protected override void OnCastSpell()
		{
			if (ParticleSystem != null)
			{
				ParticleSystem.Play();
				UpdateParticleSystems();
			}
		}

		protected override void OnStopSpell()
		{
			if (ParticleSystem != null)
			{
				ParticleSystem.Stop();
			}
		}

		void ICollisionHandler.HandleCollision(GameObject obj, List<ParticleCollisionEvent> collisions, int collisionCount)
		{
			if (collisionTimer <= 0f)
			{
				collisionTimer = CollisionInterval;
				PlayCollisionSound(collisions[0].intersection);
				ApplyCollisionForce(collisions[0].intersection);
				if (CollisionCallback != null)
				{
					CollisionCallback(obj, collisions, collisionCount);
				}
			}
		}

		public LightningParticleSpellScript()
		{
			RangeOfFloats rangeOfFloats = new RangeOfFloats
			{
				Minimum = 2f,
				Maximum = 5f
			};
			ParticleLightRange = rangeOfFloats;
			rangeOfFloats = new RangeOfFloats
			{
				Minimum = 0.2f,
				Maximum = 0.3f
			};
			ParticleLightIntensity = rangeOfFloats;
			ParticleLightColor1 = Color.white;
			ParticleLightColor2 = Color.white;
			ParticleLightCullingMask = -1;
			particles = new ParticleSystem.Particle[512];
			particleLights = new List<GameObject>();
			// base._002Ector();
		}
	}
}
