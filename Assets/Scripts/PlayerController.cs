using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Connections")]
    public Transform CameraContainer;
    public Camera MainCamera;
    public Transform ParticleSystem;

    [Header("Properties")]
    public float Health = 1f;
    public float MovementSpeed = 20f;
    public float RotationSpeed = 20f;

    [Header("Projectiles")]
    public GameObject StandardBullet;

    [Header("Emitters")]
    public Transform StandardEmitter;
    public Transform[] DoubleEmitters;

    [Header("Powerups")]
    public float DoubleWeapons = 0f;
    public float Shield = 0f;

    [Header("Powerup UI Elements")]
    public CanvasGroup DoubleWeaponIcon;
    public Image DoubleWeaponProgress;

    [Header("Boost")]
    public float BoostTime = 2f;
    public float BoostSpeed = 4000f;
    public float BoostedSpeed = 0f;

    [Header("Camera Properties")]
    public float StandardFOV;
    public float ShootingFOV;

    Rigidbody myRB;

    public static PlayerController Instance;

    SpriteRenderer mySprite;

    private void Start()
    {
        Instance = this;
        myRB = GetComponent<Rigidbody>();

        mySprite = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        ParticleSystem.position = transform.position;
        CameraContainer.position = Vector3.LerpUnclamped(CameraContainer.position, transform.position, Time.deltaTime * 5f);
    }

    Rect left = new Rect(0, 0, Screen.width / 2, Screen.height);
    Rect right = new Rect(Screen.width / 2, 0, Screen.width / 2, Screen.height);

    Vector2 leftStart = Vector3.zero;
    Vector2 rightStart = Vector2.zero;

    bool shooting = false;

    private void Update()
    {
        if (!MatchManager.Instance.Dead)
        {
            #region Input Management
            var horizontal = 0f;
            var vertical = 0f;
            var mouseDeltaX = 0f;
            var mouseDeltaY = 0f;

            foreach (var touch in Input.touches)
            {
                if (left.Contains(touch.position))
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        leftStart = touch.position;
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        leftStart = Vector2.zero;
                    }
                    else
                    {
                        var delta = Vector2.ClampMagnitude((touch.position - leftStart) / 25f, 1f);
                        horizontal = delta.x;
                        vertical = delta.y;
                    }
                }
                else if (right.Contains(touch.position))
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        rightStart = touch.position;
                        shooting = true;
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        rightStart = Vector2.zero;
                        shooting = false;
                    }
                    else
                    {
                        var delta = Vector2.ClampMagnitude((touch.position - rightStart), 1f);
                        mouseDeltaX = delta.x;
                        mouseDeltaY = delta.y;
                    }
                }
            }
            #endregion

            #region Locomotion
            myRB.AddForce(horizontal * Vector3.right * Time.deltaTime * (MovementSpeed + BoostedSpeed));
            myRB.AddForce(vertical * Vector3.forward * Time.deltaTime * (MovementSpeed + BoostedSpeed));

            var angle = Mathf.Atan2(mouseDeltaY, -mouseDeltaX);
            transform.rotation = Quaternion.Euler(Vector3.up * Mathf.LerpAngle(transform.rotation.eulerAngles.y, angle * Mathf.Rad2Deg - 90f, Time.deltaTime * 6f));
            #endregion

            #region Shooting
            if (shooting)
            {   
                Shoot();
            }

            MainCamera.fieldOfView = Mathf.Lerp(MainCamera.fieldOfView, (shooting) ? ShootingFOV : StandardFOV, Time.deltaTime * 4f);
            #endregion

            #region Boost
            if (Input.GetKey(KeyCode.LeftShift))
            {
                // Boost
                BoostedSpeed = BoostSpeed;
            }
            else
            {
                BoostedSpeed = 0;
            }
            #endregion

            if (Health <= 0)
            {
                // Dead
                if (MatchManager.Instance.CurrentScore > PlayerPrefs.GetInt("HIGHSCORE", 0))
                {
                    PlayerPrefs.SetInt("HIGHSCORE", MatchManager.Instance.CurrentScore);
                }

                // Reset the match
                MatchManager.Instance.Dead = true;
                MatchManager.Instance.Playing = false;
            }

            Health = Mathf.Clamp(Health, 0, 1);

            mySprite.color = Color.Lerp(Color.white, Color.red, 1 - Health);
        }

        #region Render Powerup UIs

        #endregion
    }

    float shootingTimer = 0f;
    float shootingResetTimer = 0.025f;

    // For double shooters
    bool altShootIndex = false;

    void Shoot()
    {
        if (shootingTimer > 0)
        {
            if (DoubleWeapons > 0)
            {
                // Shooting is 2 times faster
                shootingTimer -= Time.deltaTime * 2f;
            }
            else
            {
                shootingTimer -= Time.deltaTime;
            }
        }
        else
        {
            shootingTimer = shootingResetTimer;

            if (DoubleWeapons > 0)
            {
                Instantiate(StandardBullet, (altShootIndex) ? DoubleEmitters[0].position : DoubleEmitters[1].position, transform.rotation);
                DoubleWeapons -= 0.05f;
            }
            else
            {
                Instantiate(StandardBullet, StandardEmitter.position, transform.rotation);
            }
        }
    }
}
