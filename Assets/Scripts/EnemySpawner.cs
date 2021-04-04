using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Spawnable[] Spawnables;
    public Color[] Colors;

    public void StartSpawning()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(1f);

        while (MatchManager.Instance.Playing)
        {
            if (Spawnables != null)
            {
                foreach (var spawn in Spawnables)
                {
                    yield return new WaitForSeconds(Random.Range(0, 3f) * spawn.Odds + 20f / (MatchManager.Instance.GameTime + 5f));
                    Spawn(spawn.Enemy);
                }
            }

            yield return null;
        }
    }

    void Spawn(GameObject go)
    {
        var randX = (Random.Range(0f, 1f) > 0.5f) ? Random.Range(10f, 20f) : Random.Range(-5f, -7f);
        var randY = (Random.Range(0f, 1f) > 0.5f) ? Random.Range(10f, 20f) : Random.Range(-5f, -7f);

        var selectedColor = Colors[Random.Range(0, Colors.Length)];

        var x = Instantiate(go, PlayerController.Instance.transform.position + Vector3.right * randX + Vector3.forward * randY, Quaternion.Euler(Vector3.zero));
        x.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = selectedColor;
        x.GetComponent<Decimate>().SetColor = selectedColor;
    }
}

[System.Serializable]
public class Spawnable
{
    public GameObject Enemy;
    public float Odds;
}