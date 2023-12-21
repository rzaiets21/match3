using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace DigitalRuby.ThunderAndLightning
{
	public class LightningBoltScript : MonoBehaviour
	{
		public Camera Camera;

		public CameraMode CameraMode;

		internal CameraMode calculatedCameraMode = CameraMode.Unknown;

		public bool UseWorldSpace = true;

		public bool CompensateForParentTransform;

		public LightningBoltQualitySetting QualitySetting;

		public bool MultiThreaded;

		public float LevelOfDetailDistance;

		public bool UseGameTime;

		public string SortLayerName;

		public int SortOrderInLayer;

		public float SoftParticlesFactor = 3f;

		public int RenderQueue = -1;

		public Material LightningMaterialMesh;

		public Material LightningMaterialMeshNoGlow;

		public Texture2D LightningTexture;

		public Texture2D LightningGlowTexture;

		public ParticleSystem LightningOriginParticleSystem;

		public ParticleSystem LightningDestinationParticleSystem;

		public Color LightningTintColor = Color.white;

		public Color GlowTintColor = new Color(0.1f, 0.2f, 1f, 1f);

		public BlendMode SourceBlendMode = BlendMode.SrcAlpha;

		public BlendMode DestinationBlendMode = BlendMode.One;

		public float JitterMultiplier;

		public float Turbulence;

		public Vector3 TurbulenceVelocity = Vector3.zero;

		public Vector4 IntensityFlicker = intensityFlickerDefault;

		private static readonly Vector4 intensityFlickerDefault = new Vector4(1f, 1f, 1f, 0f);

		public Texture2D IntensityFlickerTexture;

		private Action<LightningBoltParameters, Vector3, Vector3> _003CLightningStartedCallback_003Ek__BackingField;

		private Action<LightningBoltParameters, Vector3, Vector3> _003CLightningEndedCallback_003Ek__BackingField;

		private Action<Light> _003CLightAddedCallback_003Ek__BackingField;

		private Action<Light> _003CLightRemovedCallback_003Ek__BackingField;

		private static Vector4 _003CTimeVectorSinceStart_003Ek__BackingField;

		private static float _003CTimeSinceStart_003Ek__BackingField;

		private static float _003CDeltaTime_003Ek__BackingField;

		public static float TimeScale = 1f;

		private static bool needsTimeUpdate = true;

		private Material _003ClightningMaterialMeshInternal_003Ek__BackingField;

		private Material _003ClightningMaterialMeshNoGlowInternal_003Ek__BackingField;

		private Texture2D lastLightningTexture;

		private Texture2D lastLightningGlowTexture;

		private readonly List<LightningBolt> activeBolts = new List<LightningBolt>();

		private readonly LightningBoltParameters[] oneParameterArray = new LightningBoltParameters[1];

		private readonly List<LightningBolt> lightningBoltCache = new List<LightningBolt>();

		private readonly List<LightningBoltDependencies> dependenciesCache = new List<LightningBoltDependencies>();

		private LightningThreadState threadState;

		private static int shaderId_MainTex = int.MinValue;

		private static int shaderId_TintColor;

		private static int shaderId_JitterMultiplier;

		private static int shaderId_Turbulence;

		private static int shaderId_TurbulenceVelocity;

		private static int shaderId_SrcBlendMode;

		private static int shaderId_DstBlendMode;

		private static int shaderId_InvFade;

		private static int shaderId_LightningTime;

		private static int shaderId_IntensityFlicker;

		private static int shaderId_IntensityFlickerTexture;

		public Action<LightningBoltParameters, Vector3, Vector3> LightningStartedCallback
		{
			get
			{
				return _003CLightningStartedCallback_003Ek__BackingField;
			}
			set
			{
				_003CLightningStartedCallback_003Ek__BackingField = value;
			}
		}

		public Action<LightningBoltParameters, Vector3, Vector3> LightningEndedCallback
		{
			get
			{
				return _003CLightningEndedCallback_003Ek__BackingField;
			}
			set
			{
				_003CLightningEndedCallback_003Ek__BackingField = value;
			}
		}

		public Action<Light> LightAddedCallback
		{
			get
			{
				return _003CLightAddedCallback_003Ek__BackingField;
			}
			set
			{
				_003CLightAddedCallback_003Ek__BackingField = value;
			}
		}

		public Action<Light> LightRemovedCallback
		{
			get
			{
				return _003CLightRemovedCallback_003Ek__BackingField;
			}
			set
			{
				_003CLightRemovedCallback_003Ek__BackingField = value;
			}
		}

		public bool HasActiveBolts => activeBolts.Count != 0;

		public static Vector4 TimeVectorSinceStart
		{
			get
			{
				return _003CTimeVectorSinceStart_003Ek__BackingField;
			}
			private set
			{
				_003CTimeVectorSinceStart_003Ek__BackingField = value;
			}
		}

		public static float TimeSinceStart
		{
			get
			{
				return _003CTimeSinceStart_003Ek__BackingField;
			}
			private set
			{
				_003CTimeSinceStart_003Ek__BackingField = value;
			}
		}

		public static float DeltaTime
		{
			get
			{
				return _003CDeltaTime_003Ek__BackingField;
			}
			private set
			{
				_003CDeltaTime_003Ek__BackingField = value;
			}
		}

		internal Material lightningMaterialMeshInternal
		{
			get
			{
				return _003ClightningMaterialMeshInternal_003Ek__BackingField;
			}
			private set
			{
				_003ClightningMaterialMeshInternal_003Ek__BackingField = value;
			}
		}

		internal Material lightningMaterialMeshNoGlowInternal
		{
			get
			{
				return _003ClightningMaterialMeshNoGlowInternal_003Ek__BackingField;
			}
			private set
			{
				_003ClightningMaterialMeshNoGlowInternal_003Ek__BackingField = value;
			}
		}

		public virtual void CreateLightningBolt(LightningBoltParameters p)
		{
			if (p != null && Camera != null)
			{
				UpdateTexture();
				oneParameterArray[0] = p;
				LightningBolt orCreateLightningBolt = GetOrCreateLightningBolt();
				LightningBoltDependencies dependencies = CreateLightningBoltDependencies(oneParameterArray);
				orCreateLightningBolt.SetupLightningBolt(dependencies);
			}
		}

		public void CreateLightningBolts(ICollection<LightningBoltParameters> parameters)
		{
			if (parameters != null && parameters.Count != 0 && Camera != null)
			{
				UpdateTexture();
				LightningBolt orCreateLightningBolt = GetOrCreateLightningBolt();
				LightningBoltDependencies dependencies = CreateLightningBoltDependencies(parameters);
				orCreateLightningBolt.SetupLightningBolt(dependencies);
			}
		}

		protected virtual void Awake()
		{
			UpdateShaderIds();
		}

		protected virtual void Start()
		{
			UpdateCamera();
			UpdateMaterialsForLastTexture();
			UpdateShaderParameters();
			CheckCompensateForParentTransform();
			SceneManager.sceneLoaded += OnSceneLoaded;
			if (MultiThreaded)
			{
				threadState = new LightningThreadState();
			}
		}

		protected virtual void Update()
		{
			if (needsTimeUpdate)
			{
				needsTimeUpdate = false;
				DeltaTime = (UseGameTime ? Time.deltaTime : Time.unscaledDeltaTime) * TimeScale;
				TimeSinceStart += DeltaTime;
			}
			if (HasActiveBolts)
			{
				UpdateCamera();
				UpdateShaderParameters();
				CheckCompensateForParentTransform();
				UpdateActiveBolts();
				Shader.SetGlobalVector(shaderId_LightningTime, TimeVectorSinceStart = new Vector4(TimeSinceStart * 0.05f, TimeSinceStart, TimeSinceStart * 2f, TimeSinceStart * 3f));
			}
			if (threadState != null)
			{
				threadState.UpdateMainThreadActions();
			}
		}

		protected virtual void LateUpdate()
		{
			needsTimeUpdate = true;
		}

		protected virtual LightningBoltParameters OnCreateParameters()
		{
			return LightningBoltParameters.GetOrCreateParameters();
		}

		protected LightningBoltParameters CreateParameters()
		{
			LightningBoltParameters lightningBoltParameters = OnCreateParameters();
			lightningBoltParameters.quality = QualitySetting;
			PopulateParameters(lightningBoltParameters);
			return lightningBoltParameters;
		}

		protected virtual void PopulateParameters(LightningBoltParameters p)
		{
		}

		private Coroutine StartCoroutineWrapper(IEnumerator routine)
		{
			if (base.isActiveAndEnabled)
			{
				return StartCoroutine(routine);
			}
			return null;
		}

		private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
		{
			LightningBolt.ClearCache();
		}

		private LightningBoltDependencies CreateLightningBoltDependencies(ICollection<LightningBoltParameters> parameters)
		{
			LightningBoltDependencies lightningBoltDependencies;
			if (dependenciesCache.Count == 0)
			{
				lightningBoltDependencies = new LightningBoltDependencies();
				lightningBoltDependencies.AddActiveBolt = AddActiveBolt;
				lightningBoltDependencies.LightAdded = OnLightAdded;
				lightningBoltDependencies.LightRemoved = OnLightRemoved;
				lightningBoltDependencies.ReturnToCache = ReturnLightningDependenciesToCache;
				lightningBoltDependencies.StartCoroutine = StartCoroutineWrapper;
				lightningBoltDependencies.Parent = base.gameObject;
			}
			else
			{
				int index = dependenciesCache.Count - 1;
				lightningBoltDependencies = dependenciesCache[index];
				dependenciesCache.RemoveAt(index);
			}
			lightningBoltDependencies.CameraPos = Camera.transform.position;
			lightningBoltDependencies.CameraIsOrthographic = Camera.orthographic;
			lightningBoltDependencies.CameraMode = calculatedCameraMode;
			lightningBoltDependencies.LevelOfDetailDistance = LevelOfDetailDistance;
			lightningBoltDependencies.DestParticleSystem = LightningDestinationParticleSystem;
			lightningBoltDependencies.LightningMaterialMesh = lightningMaterialMeshInternal;
			lightningBoltDependencies.LightningMaterialMeshNoGlow = lightningMaterialMeshNoGlowInternal;
			lightningBoltDependencies.OriginParticleSystem = LightningOriginParticleSystem;
			lightningBoltDependencies.SortLayerName = SortLayerName;
			lightningBoltDependencies.SortOrderInLayer = SortOrderInLayer;
			lightningBoltDependencies.UseWorldSpace = UseWorldSpace;
			lightningBoltDependencies.ThreadState = threadState;
			if (threadState != null)
			{
				lightningBoltDependencies.Parameters = new List<LightningBoltParameters>(parameters);
			}
			else
			{
				lightningBoltDependencies.Parameters = parameters;
			}
			lightningBoltDependencies.LightningBoltStarted = LightningStartedCallback;
			lightningBoltDependencies.LightningBoltEnded = LightningEndedCallback;
			return lightningBoltDependencies;
		}

		private void ReturnLightningDependenciesToCache(LightningBoltDependencies d)
		{
			d.Parameters = null;
			d.OriginParticleSystem = null;
			d.DestParticleSystem = null;
			d.LightningMaterialMesh = null;
			d.LightningMaterialMeshNoGlow = null;
			dependenciesCache.Add(d);
		}

		internal void OnLightAdded(Light l)
		{
			if (LightAddedCallback != null)
			{
				LightAddedCallback(l);
			}
		}

		internal void OnLightRemoved(Light l)
		{
			if (LightRemovedCallback != null)
			{
				LightRemovedCallback(l);
			}
		}

		internal void AddActiveBolt(LightningBolt bolt)
		{
			activeBolts.Add(bolt);
		}

		private void UpdateShaderIds()
		{
			if (shaderId_MainTex == int.MinValue)
			{
				shaderId_MainTex = Shader.PropertyToID("_MainTex");
				shaderId_TintColor = Shader.PropertyToID("_TintColor");
				shaderId_JitterMultiplier = Shader.PropertyToID("_JitterMultiplier");
				shaderId_Turbulence = Shader.PropertyToID("_Turbulence");
				shaderId_TurbulenceVelocity = Shader.PropertyToID("_TurbulenceVelocity");
				shaderId_SrcBlendMode = Shader.PropertyToID("_SrcBlendMode");
				shaderId_DstBlendMode = Shader.PropertyToID("_DstBlendMode");
				shaderId_InvFade = Shader.PropertyToID("_InvFade");
				shaderId_LightningTime = Shader.PropertyToID("_LightningTime");
				shaderId_IntensityFlicker = Shader.PropertyToID("_IntensityFlicker");
				shaderId_IntensityFlickerTexture = Shader.PropertyToID("_IntensityFlickerTexture");
			}
		}

		private void UpdateMaterialsForLastTexture()
		{
			if (Application.isPlaying)
			{
				calculatedCameraMode = CameraMode.Unknown;
				lightningMaterialMeshInternal = new Material(LightningMaterialMesh);
				lightningMaterialMeshNoGlowInternal = new Material(LightningMaterialMeshNoGlow);
				if (LightningTexture != null)
				{
					lightningMaterialMeshNoGlowInternal.SetTexture(shaderId_MainTex, LightningTexture);
				}
				if (LightningGlowTexture != null)
				{
					lightningMaterialMeshInternal.SetTexture(shaderId_MainTex, LightningGlowTexture);
				}
				SetupMaterialCamera();
			}
		}

		private void UpdateTexture()
		{
			if (LightningTexture != null && LightningTexture != lastLightningTexture)
			{
				lastLightningTexture = LightningTexture;
				UpdateMaterialsForLastTexture();
			}
			if (LightningGlowTexture != null && LightningGlowTexture != lastLightningGlowTexture)
			{
				lastLightningGlowTexture = LightningGlowTexture;
				UpdateMaterialsForLastTexture();
			}
		}

		private void SetMaterialPerspective()
		{
			if (calculatedCameraMode != CameraMode.Perspective)
			{
				calculatedCameraMode = CameraMode.Perspective;
				lightningMaterialMeshInternal.EnableKeyword("PERSPECTIVE");
				lightningMaterialMeshNoGlowInternal.EnableKeyword("PERSPECTIVE");
				lightningMaterialMeshInternal.DisableKeyword("ORTHOGRAPHIC_XY");
				lightningMaterialMeshNoGlowInternal.DisableKeyword("ORTHOGRAPHIC_XY");
				lightningMaterialMeshInternal.DisableKeyword("ORTHOGRAPHIC_XZ");
				lightningMaterialMeshNoGlowInternal.DisableKeyword("ORTHOGRAPHIC_XZ");
			}
		}

		private void SetMaterialOrthographicXY()
		{
			if (calculatedCameraMode != CameraMode.OrthographicXY)
			{
				calculatedCameraMode = CameraMode.OrthographicXY;
				lightningMaterialMeshInternal.EnableKeyword("ORTHOGRAPHIC_XY");
				lightningMaterialMeshNoGlowInternal.EnableKeyword("ORTHOGRAPHIC_XY");
				lightningMaterialMeshInternal.DisableKeyword("ORTHOGRAPHIC_XZ");
				lightningMaterialMeshNoGlowInternal.DisableKeyword("ORTHOGRAPHIC_XZ");
				lightningMaterialMeshInternal.DisableKeyword("PERSPECTIVE");
				lightningMaterialMeshNoGlowInternal.DisableKeyword("PERSPECTIVE");
			}
		}

		private void SetMaterialOrthographicXZ()
		{
			if (calculatedCameraMode != CameraMode.OrthographicXZ)
			{
				calculatedCameraMode = CameraMode.OrthographicXZ;
				lightningMaterialMeshInternal.EnableKeyword("ORTHOGRAPHIC_XZ");
				lightningMaterialMeshNoGlowInternal.EnableKeyword("ORTHOGRAPHIC_XZ");
				lightningMaterialMeshInternal.DisableKeyword("ORTHOGRAPHIC_XY");
				lightningMaterialMeshNoGlowInternal.DisableKeyword("ORTHOGRAPHIC_XY");
				lightningMaterialMeshInternal.DisableKeyword("PERSPECTIVE");
				lightningMaterialMeshNoGlowInternal.DisableKeyword("PERSPECTIVE");
			}
		}

		private void SetupMaterialCamera()
		{
			if (Camera == null && CameraMode == CameraMode.Auto)
			{
				SetMaterialPerspective();
			}
			else if (CameraMode == CameraMode.Auto)
			{
				if (Camera.orthographic)
				{
					SetMaterialOrthographicXY();
				}
				else
				{
					SetMaterialPerspective();
				}
			}
			else if (CameraMode == CameraMode.Perspective)
			{
				SetMaterialPerspective();
			}
			else if (CameraMode == CameraMode.OrthographicXY)
			{
				SetMaterialOrthographicXY();
			}
			else
			{
				SetMaterialOrthographicXZ();
			}
		}

		private void EnableKeyword(string keyword, bool enable, Material m)
		{
			if (enable)
			{
				m.EnableKeyword(keyword);
			}
			else
			{
				m.DisableKeyword(keyword);
			}
		}

		private void UpdateShaderParameters()
		{
			lightningMaterialMeshInternal.SetColor(shaderId_TintColor, GlowTintColor);
			lightningMaterialMeshInternal.SetFloat(shaderId_JitterMultiplier, JitterMultiplier);
			lightningMaterialMeshInternal.SetFloat(shaderId_Turbulence, Turbulence * LightningBoltParameters.Scale);
			lightningMaterialMeshInternal.SetVector(shaderId_TurbulenceVelocity, TurbulenceVelocity * LightningBoltParameters.Scale);
			lightningMaterialMeshInternal.SetInt(shaderId_SrcBlendMode, (int)SourceBlendMode);
			lightningMaterialMeshInternal.SetInt(shaderId_DstBlendMode, (int)DestinationBlendMode);
			lightningMaterialMeshInternal.renderQueue = RenderQueue;
			lightningMaterialMeshInternal.SetFloat(shaderId_InvFade, SoftParticlesFactor);
			lightningMaterialMeshNoGlowInternal.SetColor(shaderId_TintColor, LightningTintColor);
			lightningMaterialMeshNoGlowInternal.SetFloat(shaderId_JitterMultiplier, JitterMultiplier);
			lightningMaterialMeshNoGlowInternal.SetFloat(shaderId_Turbulence, Turbulence * LightningBoltParameters.Scale);
			lightningMaterialMeshNoGlowInternal.SetVector(shaderId_TurbulenceVelocity, TurbulenceVelocity * LightningBoltParameters.Scale);
			lightningMaterialMeshNoGlowInternal.SetInt(shaderId_SrcBlendMode, (int)SourceBlendMode);
			lightningMaterialMeshNoGlowInternal.SetInt(shaderId_DstBlendMode, (int)DestinationBlendMode);
			lightningMaterialMeshNoGlowInternal.renderQueue = RenderQueue;
			lightningMaterialMeshNoGlowInternal.SetFloat(shaderId_InvFade, SoftParticlesFactor);
			if (IntensityFlicker != intensityFlickerDefault && IntensityFlickerTexture != null)
			{
				lightningMaterialMeshInternal.SetVector(shaderId_IntensityFlicker, IntensityFlicker);
				lightningMaterialMeshInternal.SetTexture(shaderId_IntensityFlickerTexture, IntensityFlickerTexture);
				lightningMaterialMeshNoGlowInternal.SetVector(shaderId_IntensityFlicker, IntensityFlicker);
				lightningMaterialMeshNoGlowInternal.SetTexture(shaderId_IntensityFlickerTexture, IntensityFlickerTexture);
				lightningMaterialMeshInternal.EnableKeyword("INTENSITY_FLICKER");
				lightningMaterialMeshNoGlowInternal.EnableKeyword("INTENSITY_FLICKER");
			}
			else
			{
				lightningMaterialMeshInternal.DisableKeyword("INTENSITY_FLICKER");
				lightningMaterialMeshNoGlowInternal.DisableKeyword("INTENSITY_FLICKER");
			}
			SetupMaterialCamera();
		}

		private void CheckCompensateForParentTransform()
		{
			if (CompensateForParentTransform)
			{
				Transform parent = base.transform.parent;
				if (parent != null)
				{
					
					base.transform.localPosition = Vector3.zero;
					base.transform.localScale = new Vector3(1f / parent.localScale.x, 1f / parent.localScale.y, 1f / parent.localScale.z);
					base.transform.rotation = parent.rotation;
				}
			}
		}

		private void UpdateCamera()
		{
			Camera = ((!(Camera == null)) ? Camera : ((Camera.current == null) ? Camera.main : Camera.current));
		}

		private LightningBolt GetOrCreateLightningBolt()
		{
			if (lightningBoltCache.Count == 0)
			{
				return new LightningBolt();
			}
			LightningBolt result = lightningBoltCache[lightningBoltCache.Count - 1];
			lightningBoltCache.RemoveAt(lightningBoltCache.Count - 1);
			return result;
		}

		private void UpdateActiveBolts()
		{
			for (int num = activeBolts.Count - 1; num >= 0; num--)
			{
				LightningBolt lightningBolt = activeBolts[num];
				if (!lightningBolt.Update())
				{
					activeBolts.RemoveAt(num);
					lightningBolt.Cleanup();
					lightningBoltCache.Add(lightningBolt);
				}
			}
		}

		private void OnApplicationQuit()
		{
			if (threadState != null)
			{
				threadState.Running = false;
			}
		}

		private void Cleanup()
		{
			foreach (LightningBolt activeBolt in activeBolts)
			{
				activeBolt.Cleanup();
			}
			activeBolts.Clear();
		}

		private void OnDestroy()
		{
			if (threadState != null)
			{
				threadState.TerminateAndWaitForEnd();
			}
			if (lightningMaterialMeshInternal != null)
			{
				UnityEngine.Object.Destroy(lightningMaterialMeshInternal);
			}
			if (lightningMaterialMeshNoGlowInternal != null)
			{
				UnityEngine.Object.Destroy(lightningMaterialMeshNoGlowInternal);
			}
			Cleanup();
		}

		private void OnDisable()
		{
			Cleanup();
		}
	}
}
