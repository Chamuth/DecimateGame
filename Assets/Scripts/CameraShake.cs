using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float ShakeTimer = 100f;
    public float ShakeFrequency = 10f;
    public float ShakeAmplitude = 2f;

    public static CameraShake Instance;

    private void Start()
    {
        Instance = this;
    }

    public void Shake()
    {
        ShakeTimer = 0.2f;
    }

    void Update()
    {
        if (MatchManager.Instance.Playing)
        {
            if (ShakeTimer > 0)
            {
                ShakeTimer -= Time.deltaTime;

                // Shake screen
                transform.localPosition = Vector3.right * Mathf.Sin(Time.time * ShakeFrequency) * ShakeAmplitude + Vector3.forward * Mathf.Sin(Time.time * ShakeFrequency / 1.5f) * ShakeAmplitude / 2f;
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 4f);
            }
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.up * -10f, Time.deltaTime * 4f);
        }
    }
}
