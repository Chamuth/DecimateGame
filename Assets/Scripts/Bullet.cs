using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float BulletSpeed = 100f;

    float timer = 0f;

    private void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * BulletSpeed);

        timer += Time.deltaTime;

        if (timer > 5f)
        {
            Destroy(gameObject);
        }
    }
}
