using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBoundary : MonoBehaviour
{
    public Vector2 minCameraLimit;
    public Vector2 maxCameraLimit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            MoveCamera cameraScript = Camera.main.GetComponent<MoveCamera>();

            if (cameraScript != null)
            {
                cameraScript.SetLimits(minCameraLimit, maxCameraLimit);
            }
        }
    }
}
