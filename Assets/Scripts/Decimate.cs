using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decimate : MonoBehaviour
{
    [HideInInspector]
    public int CurrentIndex = 0;

    [Header("Properties")]
    public int[] Decimations = new int[] { 4, 2 };
    public float Health = 1f;
    public float StartupSpeed = 250f;
    public Color SetColor;

    [Header("Prefabs")]
    public GameObject ExplosionPrefab;

    Rigidbody rb;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Projectile")
        {
            // A Projectile hit a swarmer
            if (Health > 0)
            {
                Health -= 0.1f * (CurrentIndex * 0.5f + 1);
            }
            else
            {
                _Decimate();
            }

            PlayerController.Instance.Health += 0.015f * (1 / (float)(CurrentIndex + 1));

            MatchManager.Instance.CurrentScore += 10 + 5 * (CurrentIndex);

            Destroy(other.gameObject);
        }
        else if (other.tag == "Player")
        {
            // Swarmer hit the player
            Health = 0;
            PlayerController.Instance.Health -= 0.1f * (1 / (float)(CurrentIndex + 1));
            _Decimate();
        }
    }

    float randomSpeed = 0;
    float randomTorque = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        randomSpeed = Random.Range(0f, 2f);
        randomTorque = Random.Range(-180f, 180f);
    }

    float scanningTimeout = 0.5f;
    Vector3 escapeV = Vector3.zero;
    float oldTimer = 0f;

    private void Update()
    {
        // Swarm the player but randomly
        var pDirection = PlayerController.Instance.gameObject.transform.position - transform.position;
        rb.AddForce(pDirection.normalized * Time.deltaTime * (StartupSpeed * (1 + 0.25f * CurrentIndex + randomSpeed)) + escapeV * Time.deltaTime * 75f);

        // Random rotation
        transform.Rotate(Vector3.up * Time.deltaTime * randomTorque);

        if (scanningTimeout > 0)
        {
            scanningTimeout -= Time.deltaTime;
        }
        else
        {
            escapeV = Vector3.zero;

            // Scan background for other followers
            var swarmers = GameObject.FindGameObjectsWithTag("Swarmers");
            
            foreach (var swarmer in swarmers)
            {
                if (swarmer != this)
                {
                    var delta = swarmer.transform.position - transform.position;
                    escapeV += -delta.normalized;
                }
            }

            scanningTimeout = 0.5f;
        }

        // GC
        oldTimer += Time.deltaTime;

        if (oldTimer > 30f)
        {
            _Decimate();
        }
    }

    void _Decimate()
    {
        if (CurrentIndex < Decimations.Length)
        {
            var color = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color;

            for (var i = 0; i < Decimations[CurrentIndex]; i++)
            {
                var x = Instantiate(gameObject, transform.position, transform.rotation);

                // Increase the index and reset health
                var decimation = x.GetComponent<Decimate>();
                decimation.CurrentIndex++;
                decimation.Health = 1f;

                // position randomly
                var xRand = Random.Range(-2.5f, 2.5f);
                var yRand = Random.Range(-2.5f, 2.5f);

                x.transform.position = x.transform.position + (Vector3.right * xRand);
                x.transform.position = x.transform.position + (Vector3.forward * yRand);

                var rb = x.GetComponent<Rigidbody>();
                rb.AddForce(Vector3.right * xRand * 100);
                rb.AddForce(Vector3.forward * yRand * 100);

                // Set sprite color from parent
                x.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = color;

                // scale according to the current size
                x.transform.localScale = Vector3.one * (1 / (float)(CurrentIndex + 2));
            }
        }

        // Instantiate the explosive particle system prefab
        var exGO = Instantiate(ExplosionPrefab, transform.position, transform.rotation);
        exGO.transform.localScale = Vector3.one * (1 / (float)(CurrentIndex + 1));
        var particles = exGO.GetComponent<ParticleSystem>();

        ParticleSystem.MainModule psMain = particles.main;
        psMain.startColor = new ParticleSystem.MinMaxGradient(SetColor, Color.white);

        particles.Play();

        // Shake the camera
        CameraShake.Instance.Shake();

        Destroy(gameObject);
    }
}
