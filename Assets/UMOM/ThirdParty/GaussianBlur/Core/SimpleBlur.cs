using UnityEngine;
using UnityEngine.Profiling;

namespace AcrylicUI
{

    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Effects/Simple Blur (very fast)", -1)]
    public class SimpleBlur : MonoBehaviour
    {

        public BlurKernelSize kernelSize = BlurKernelSize.Small;

        [Range(0f, 1f)]
        public float interpolation = 1f;

        [Range(0, 4)]
        public int downsample = 1;

        [Range(1, 8)]
        public int iterations = 1;

        public Material blurMaterial;

        Camera m_Camera = null;
        RenderTexture textureToBlur = null;

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
                float radius = (float)i * interpolation + interpolation;
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

        void OnEnable()
        {
            m_Camera = GetComponent<Camera>();
        }

        public void OnValidate()
        {
            if (m_Camera == null)
                m_Camera = GetComponent<Camera>();
        }

        void OnPreCull()
        {
            Profiler.BeginSample("Gaussian blur OnPreCull");
            if (blurMaterial == null)
                return;

            int tw = Screen.width >> downsample;
            int th = Screen.height >> downsample;

            textureToBlur = RenderTexture.GetTemporary(tw, th, 24, RenderTextureFormat.Default);
            m_Camera.targetTexture = textureToBlur;
            Profiler.EndSample();
        }

        void OnPostRender()
        {
            Profiler.BeginSample("Gaussian blur OnPostRender");
            if (blurMaterial == null)
                return;

            m_Camera.targetTexture = null;

            Blur(textureToBlur, null);
            RenderTexture.ReleaseTemporary(textureToBlur);
            Profiler.EndSample();
        }

    }
}