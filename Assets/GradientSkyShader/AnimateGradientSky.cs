using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateGradientSky : MonoBehaviour
{
    Color c1 = new Color(1, 65 / 255f, 108 / 255f);
    Color c2 = new Color(1, 75 / 255f, 43 / 255f);

    void Update()
    {
        RenderSettings.skybox.SetColor("_Color1", Color.Lerp(RenderSettings.skybox.GetColor("_Color1"), c1, Time.deltaTime * 0.5f));
        RenderSettings.skybox.SetColor("_Color2", Color.Lerp(RenderSettings.skybox.GetColor("_Color2"), c2, Time.deltaTime * 0.5f));
    }
}
