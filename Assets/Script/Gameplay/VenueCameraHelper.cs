using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using YARG.Settings;

namespace YARG.Gameplay
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class VenueCameraHelper : MonoBehaviour
    {
        [Range(0.01F, 1.0F)]
        public float renderScale = 1.0F;

        private Camera _renderCamera;
        private float _originalFactor;
        private UniversalRenderPipelineAsset UniversalRenderPipelineAsset;

        private static RawImage                _venueOutput;
        private static RenderTexture           _venueTexture;
        private static CancellationTokenSource _cts;
        private static int                     activeInstances = 0;

        private void Awake()
        {
            renderScale = GraphicsManager.Instance.VenueRenderScale;
            _renderCamera = GetComponent<Camera>();
            // Disable the camera so we can control when it renders
            _renderCamera.enabled = false;

            _renderCamera.allowMSAA = false;
            RenderPipelineManager.beginCameraRendering += OnPreCameraRender;
            var cameraData = _renderCamera.GetUniversalAdditionalCameraData();
            cameraData.antialiasing = AntialiasingMode.None;
            switch (GraphicsManager.Instance.VenueAntiAliasing)
            {
                case VenueAntiAliasingMethod.None:
                    break;
                case VenueAntiAliasingMethod.FXAA:
                    cameraData.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
                    break;
                case VenueAntiAliasingMethod.MSAA:
                    _renderCamera.allowMSAA = true;
                    cameraData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                    break;
                case VenueAntiAliasingMethod.FSR3:
                    _renderCamera.gameObject.AddComponent<FSRCameraManager>();
                    break;
            }
            RenderPipelineManager.endCameraRendering += OnPostCameraRender;
            UniversalRenderPipelineAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            _originalFactor = UniversalRenderPipelineAsset.renderScale;
        }

        private void OnEnable()
        {
            activeInstances++;

            if (activeInstances == 1)
            {
                var venueOutputObject = GameObject.Find("Venue Output");
                if (venueOutputObject != null)
                {
                    _venueOutput = venueOutputObject.GetComponent<RawImage>();

                    if (_venueOutput != null)
                    {
                        _venueTexture = new RenderTexture(Screen.width, Screen.height, 0);
                        _venueOutput.texture = _venueTexture;
                    }
                }
            }

            _cts?.Cancel();
            _cts?.Dispose();

            _cts = new CancellationTokenSource();
            RenderLoop(_cts.Token).Forget();
        }

        private void OnDisable()
        {
            activeInstances--;

            if (activeInstances == 0)
            {
                _cts?.Cancel();
                _cts?.Dispose();
                _cts = null;

                if (_venueTexture != null)
                {
                    _venueTexture.Release();
                    _venueTexture = null;
                }

                _venueOutput = null;
            }
        }

        private void OnDestroy()
        {
            UniversalRenderPipelineAsset.renderScale = _originalFactor;
        }

        private async UniTask RenderLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var interval = TimeSpan.FromSeconds(1f / SettingsManager.Settings.VenueFpsCap.Value);

                if (_renderCamera != null && _venueTexture != null)
                {
                    _renderCamera.targetTexture = _venueTexture;
                    _renderCamera.Render();
                }

                try
                {
                    await UniTask.Delay(interval, cancellationToken: token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        private void OnPreCameraRender(ScriptableRenderContext ctx, Camera cam)
        {
            if (cam == _renderCamera)
            {
                UniversalRenderPipelineAsset.renderScale = renderScale;
            }
        }

        private void OnPostCameraRender(ScriptableRenderContext ctx, Camera cam)
        {
            if (cam == _renderCamera)
            {
                UniversalRenderPipelineAsset.renderScale = _originalFactor;
            }
        }
    }
}
