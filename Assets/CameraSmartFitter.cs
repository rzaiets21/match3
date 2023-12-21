using System;
using UnityEngine;

namespace Base.CameraFitter
{
    [RequireComponent(typeof(Camera))]
    public class CameraSmartFitter : MonoBehaviour
    {
        [SerializeField] protected SpriteRenderer renderer;
        [SerializeField] protected ManualControl manualControl;

        protected Camera camera;
        protected float screenRatio;
        protected float calculatedSize;
        #if UNITY_EDITOR
        private System.Reflection.MethodInfo getSizeOfGameView;
        #endif
        
        private void Awake()
        {
            screenRatio = (float)Screen.height / (float)Screen.width;
            camera = GetComponent<Camera>();
            calculatedSize = camera.orthographicSize;
        }
        
        private void Start() => FitCamera();

        public virtual void FitCamera()
        {
            if(renderer == null) return;
            
            var targetRatio = renderer.bounds.size.x / renderer.bounds.size.y;
            
            if(!manualControl.isManualControlled) manualControl.fitMode = screenRatio > 0.6f 
                ? CameraFitMode.Width 
                : CameraFitMode.Height;
            
            switch(manualControl.fitMode)
            {
                case CameraFitMode.Width:
                    calculatedSize = renderer.bounds.size.y / 2;
                    if(calculatedSize <= targetRatio) calculatedSize *= targetRatio / screenRatio; 
                    break;
                case CameraFitMode.Height:
                    calculatedSize = renderer.bounds.size.x * screenRatio / 2;
                    break;
            };
            
            camera.orthographicSize = calculatedSize;
        }

        [System.Serializable]
        public class ManualControl
        {
            [SerializeField] internal CameraFitMode fitMode;
            [SerializeField] internal bool isManualControlled = false;
        }
    }

    public enum CameraFitMode
    {
        Width,
        Height
    }
}