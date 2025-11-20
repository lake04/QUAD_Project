using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    private GameObject target;
    private Rigidbody2D playerRb; 

    public float smooth = 0.1f;
    public Vector3 adjustCamPos;
    private bool isFix = false;

    public Vector2 minCamLimit;
    public Vector2 maxCamLimit;

    public float lookAheadDistance = 2f;
    public float lookAheadSmoothing = 5f;
    private float currentLookAheadX; 

    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.player != null)
        {
            target = GameManager.Instance.player;
            playerRb = target.GetComponent<Rigidbody2D>();
        }

        if (target != null)
        {
            Vector3 initialPos = target.transform.position + adjustCamPos;
            transform.position = new Vector3(
                Mathf.Clamp(initialPos.x, minCamLimit.x, maxCamLimit.x),
                Mathf.Clamp(initialPos.y, minCamLimit.y, maxCamLimit.y),
                -10f + adjustCamPos.z
            );
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (target == null || playerRb == null || isFix)
        {
            return;
        }

        float playerSpeedX = playerRb.velocity.x;
        float lookDirX = 0f;

        if (Mathf.Abs(playerSpeedX) > 0.1f)
        {
            lookDirX = Mathf.Sign(playerSpeedX);
        }

        currentLookAheadX = Mathf.Lerp(
            currentLookAheadX,
            lookDirX * lookAheadDistance,
            Time.fixedDeltaTime * lookAheadSmoothing
        );

        Vector3 targetPos = target.transform.position + adjustCamPos;
        targetPos.x += currentLookAheadX;

        Vector3 finalTargetPos = targetPos;
        if (CameraShake.Instance != null) 
        {
            finalTargetPos += CameraShake.Instance.shakeOffset;
        }

        Vector3 newCamPos = Vector3.Lerp(transform.position, finalTargetPos, smooth);

        transform.position = new Vector3(
            Mathf.Clamp(newCamPos.x, minCamLimit.x, maxCamLimit.x),
            Mathf.Clamp(newCamPos.y, minCamLimit.y, maxCamLimit.y),
            -10f + adjustCamPos.z
        );
    }

    public void SetLimits(Vector2 min, Vector2 max)
    {
        minCamLimit = min;
        maxCamLimit = max;
    }

    public void SetFixed(bool fixedState)
    {
        isFix = fixedState;
    }
}