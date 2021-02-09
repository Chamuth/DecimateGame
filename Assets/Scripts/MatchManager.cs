using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public bool Playing = true;
    public bool Dead = false;
    public float GameTime = 0f;

    public static MatchManager Instance;

    [Header("Connections")]
    public EnemySpawner Spawner;
    public CanvasGroup GameplayUI;
    public CanvasGroup MainUI;
    public CanvasGroup RetryUI;

    [Header("Background Colors")]
    public int CurrentColorIndex = 0;
    public Color[] Colors;
    public float ColorTimeout = 0;
    public float TransitionTime = 30f;

    private void Start()
    {
        Instance = this;
    }

    void ResetMatch()
    {
        print("RESETTING THINGS");

        GameTime = 0f;

        PlayerController.Instance.gameObject.transform.position = Vector3.zero;

        // destroy all swarmers
        var swarmers = GameObject.FindGameObjectsWithTag("Swarmers");
        foreach (var swarmer in swarmers)
        {
            Destroy(swarmer);
        }

        PlayerController.Instance.Health = 1f;
        Playing = true;
        Dead = false;
    }

    private void Update()
    {
        if (Playing)
            GameTime += Time.deltaTime;

        var nextIndex = (Colors.Length == CurrentColorIndex + 1) ? 0 : CurrentColorIndex + 1;

        if (ColorTimeout < TransitionTime)
        {
            ColorTimeout += Time.deltaTime;
            RenderSettings.skybox.SetColor("_Color1", Color.Lerp(Colors[CurrentColorIndex], Colors[nextIndex], ColorTimeout / 30f));
        }
        else
        {
            ColorTimeout = 0f;
            CurrentColorIndex = nextIndex;
        }

        if (Input.GetMouseButtonDown(0) && !Playing)
        {
            // Tap on Screen
            ResetMatch();
            Spawner.StartSpawning();
        }

        // Animate the ui
        GameplayUI.alpha = Mathf.MoveTowards(GameplayUI.alpha, (Playing && !Dead) ? 1 : 0, Time.deltaTime * 4f);
        MainUI.alpha = Mathf.MoveTowards(MainUI.alpha, (!Playing && !Dead) ? 1 : 0, Time.deltaTime * 4f);
        RetryUI.alpha = Mathf.MoveTowards(RetryUI.alpha, (!Playing && Dead) ? 1 : 0, Time.deltaTime * 4f);
    }
}
