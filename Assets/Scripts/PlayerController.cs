using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Boost")]
    public float BoostTime = 2f;
    public float BoostSpeed = 4000f;
    public float BoostedSpeed = 0f;

    [Header("Camera Properties")]
    public float StandardFOV;
    public float ShootingFOV;

    Rigidbody myRB;

    public static PlayerController Instance;

    private void Start()
    {
        Instance = this;
        myRB = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        ParticleSystem.position = transform.position;
        CameraContainer.position = Vector3.LerpUnclamped(CameraContainer.position, transform.position, Time.deltaTime * 5f);
    }

    bool LeftSet, RightSet = false;
    Touch LeftTouch = new Touch();
    Vector2 leftTouchStartPosition = Vector2.zero;
    Touch RightTouch = new Touch();
    Vector2 rightTouchStartPosition = Vector2.zero;

    private void Update()
    {
        if (!MatchManager.Instance.Dead)
        {
            #region Input Management
            RightSet = false;
            LeftSet = false;

            var horizontal = 0f;
            var vertical = 0f;
            var mouseDeltaX = 0f;
            var mouseDeltaY = 0f;
            bool shooting = false;

            foreach (var touch in Input.touches)
            {
                    if (touch.position.x > (Screen.width / 2))
                    {
                        RightTouch = touch;
                        if (touch.phase == TouchPhase.Began)
                            leftTouchStartPosition = touch.position;

                        RightSet = true;
                    }
                    else
                    {
                        LeftTouch = touch;
                        if (touch.phase == TouchPhase.Began)
                            rightTouchStartPosition = touch.position;
                        LeftSet = true;
                    }
            }


            if (LeftSet)
            {
                horizontal = (LeftTouch.position - leftTouchStartPosition).normalized.x;
                vertical = (LeftTouch.position - leftTouchStartPosition).normalized.y;
            }

            if (RightSet)
            {
                // currently touching 
                mouseDeltaX = (RightTouch.deltaPosition - rightTouchStartPosition).normalized.x;
                mouseDeltaY = (RightTouch.deltaPosition - rightTouchStartPosition).normalized.y;
                shooting = true;
            }

            #endregion

            #region Locomotion
            myRB.AddForce(horizontal * transform.right * Time.deltaTime * (MovementSpeed + BoostedSpeed));
            myRB.AddForce(vertical * transform.forward * Time.deltaTime * (MovementSpeed + BoostedSpeed));

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
                MatchManager.Instance.Dead = true;
                MatchManager.Instance.Playing = false;
            }

            Health = Mathf.Clamp(Health, 0, 1);
        }
    }

    float shootingTimer = 0f;
    float shootingResetTimer = 0.025f;

    void Shoot()
    {
        if (shootingTimer > 0)
        {
            shootingTimer -= Time.deltaTime;
        }
        else
        {
            shootingTimer = shootingResetTimer;

            // Actual shooting mechanic
            var g = Instantiate(StandardBullet, StandardEmitter.position, transform.rotation);
        }
    }
}
