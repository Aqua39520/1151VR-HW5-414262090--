
using UnityEngine;
using System.Collections;

public class CaptureCamera : MonoBehaviour
{
    public Transform pokeball;

    [Header("Close-up Settings")]
    public Vector3 cameraOffset = new Vector3(0f, 1.2f, -3f);
    public float moveDuration = 1.2f;
    public float lookHeight = 0.1f;

    private bool isMoving = false;

    public void MoveToPokeball()
    {
        if (!isMoving && pokeball != null)
        {
            StartCoroutine(MoveCamera());
        }
    }

    IEnumerator MoveCamera()
    {
        isMoving = true;

        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / moveDuration);
            t = t * t * (3f - 2f * t);

            Vector3 targetPosition =
                pokeball.position + cameraOffset;

            Quaternion targetRotation =
                Quaternion.LookRotation(
                    pokeball.position + Vector3.up * lookHeight
                    - targetPosition
                );

            transform.position =
                Vector3.Lerp(startPosition, targetPosition, t);

            transform.rotation =
                Quaternion.Slerp(startRotation, targetRotation, t);

            yield return null;
        }

        isMoving = false;
    }
}
