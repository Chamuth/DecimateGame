using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthProgressRenderer : MonoBehaviour
{
    Image img;

    private void Start()
    {
        img = GetComponent<Image>();
    }

    void Update()
    {
        img.fillAmount = PlayerController.Instance.Health;
    }
}
