using UnityEngine;

namespace AcrylicUI
{
    public static class Uniforms
    {
        public static readonly int _Radius            = Shader.PropertyToID("_Radius");
        public static readonly int _BackgroundTexture = Shader.PropertyToID("_SuperBlurTexture");
    }
}