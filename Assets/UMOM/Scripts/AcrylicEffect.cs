using System.Collections;
using System.Collections.Generic;
using AcrylicUI;
using UnityEngine;

public class AcrylicEffect : MonoBehaviour
{
    private                 RenderTexture     _MainTex;
    private                 RenderTextureBlur _Blur;
    private static readonly int               MainTex = Shader.PropertyToID("_MainTex");

    private void Start()
    {
        _Blur    = FindObjectOfType<RenderTextureBlur>();
        _MainTex = _Blur.ResultTexture;
        var r = GetComponent<Renderer>();
        r.sharedMaterial.SetTexture(MainTex, _MainTex);
    }

    private void OnValidate()
    {
        Start();
    }
}
