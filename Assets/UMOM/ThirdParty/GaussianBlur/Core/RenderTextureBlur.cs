using UnityEngine;
using UnityEngine.UI;

namespace AcrylicUI
{
    [ExecuteInEditMode]
    public class RenderTextureBlur : MonoBehaviour
    {
        public BlurKernelSize kernelSize = BlurKernelSize.Small;

        [Range(0f, 1f)]
        public float interpolation = 1f;

        [Range(0, 4)]
        public int downsample = 1;

        [Range(1, 8)]
        public int iterations = 1;

        public Material blurMaterial;


        private Camera        m_Camera       = null;
        private RenderTexture _TextureToBlur = null;

        [SerializeField] private RenderTexture _ResultTexture;
        public                   RenderTexture ResultTexture => _ResultTexture;

        [SerializeField] private RenderTexture _OriginalTexture;
        public                   RenderTexture OriginalTexture => _OriginalTexture;

        [Header("Separate textures to blur")]
        [SerializeField] private RawImage _TargetRawImage;

        [SerializeField] private Texture _Texture;

        public void SetTextureToBlur(Texture texture)
        {
            _Texture = texture;
        }

        public void ClearTexture()
        {
            _Texture = null;
        }

        protected void Blur(RenderTexture source, RenderTexture destination)
        {
            int kernel = 0;

            switch (kernelSize)
            {
                case BlurKernelSize.Small:
                    kernel = 0;
                    break;
                case BlurKernelSize.Medium:
                    kernel = 2;
                    break;
                case BlurKernelSize.Big:
                    kernel = 4;
                    break;
            }

            var tempRenderTexture = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);

            for (int i = 0; i < iterations; i++)
            {
                // helps to achieve a larger blur
                float radius = (float) i * interpolation + interpolation;
                blurMaterial.SetFloat(Uniforms._Radius, radius);

                Graphics.Blit(source, tempRenderTexture, blurMaterial, 1 + kernel);
                source.DiscardContents();

                // is it a last iteration? If so, then blit to destination
                if (i == iterations - 1)
                {
                    Graphics.Blit(tempRenderTexture, destination, blurMaterial, 2 + kernel);
                }
                else
                {
                    Graphics.Blit(tempRenderTexture, source, blurMaterial, 2 + kernel);
                    tempRenderTexture.DiscardContents();
                }
            }

            RenderTexture.ReleaseTemporary(tempRenderTexture);
        }

        void Awake()
        {
            int tw = Screen.width  >> downsample;
            int th = Screen.height >> downsample;
            _ResultTexture   = new RenderTexture(tw,           th,            24, RenderTextureFormat.Default);
            _TextureToBlur   = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.Default);
            _OriginalTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.Default);
            if (_TargetRawImage != null)
                _TargetRawImage.texture = _TextureToBlur;
            m_Camera = GetComponent<Camera>();
        }

        public void OnValidate()
        {
            if (m_Camera == null)
                m_Camera = GetComponent<Camera>();
        }

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (blurMaterial == null) return;
            if (_Texture != null)
            {
                Graphics.Blit(_Texture, _ResultTexture);
            }
            if (_TextureToBlur != null)
            {
                Graphics.Blit(src,            _TextureToBlur);
                Graphics.Blit(src,            _OriginalTexture);
                Graphics.Blit(_TextureToBlur, _ResultTexture);
            }
            Blur(_ResultTexture, null);
            Graphics.Blit(src, dest);
        }

        private void OnDestroy()
        {
            DestroyImmediate(_TextureToBlur);
            DestroyImmediate(_ResultTexture);
        }
    }
}