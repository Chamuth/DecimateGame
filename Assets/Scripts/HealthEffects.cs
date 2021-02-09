using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class HealthEffects : MonoBehaviour
{
    PostProcessVolume ppp;

    ChromaticAberration caLayer = null;
    Bloom bloomLayer = null;

    private void Start()
    {
        ppp = GetComponent<PostProcessVolume>();
        ppp.profile.TryGetSettings(out caLayer);
        ppp.profile.TryGetSettings(out bloomLayer);
    }

    private void Update()
    {
        caLayer.intensity.value = Mathf.Lerp(0.15f, 1, 1 - PlayerController.Instance.Health);
        bloomLayer.color.value = Color.Lerp(Color.red, Color.white, PlayerController.Instance.Health);
    }
}
