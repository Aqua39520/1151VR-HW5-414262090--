using UnityEngine;
using System.Collections;

public class PokeballThrow : MonoBehaviour
{
    public Transform target;
    public float throwDelay = 1f;
    public float flyTime = 1f;
    public float height = 3f;
    public ParticleSystem successEffect;
    [Header("Capture UI")]
    public GameObject gotchaText;
    public Vector3 landingRotation = Vector3.zero;
    public CaptureCamera captureCamera;

    // 收服特效
    public Material captureGlow;
    public ParticleSystem captureEffect;
    public float captureTime = 0.8f;
    public float dropTime = 0.6f;
    public float ballRadius = 0.2f;
    public float shakeAngle = 25f;
    public float shakeTime = 0.6f;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(throwDelay);

        transform.SetParent(null, true);

        Vector3 start = transform.position;
        Vector3 end = target.position + Vector3.up;

        float time = 0;

        // 精靈球飛行
        while (time < 1f)
        {
            time += Time.deltaTime / flyTime;
            float t = Mathf.Clamp01(time);

            Vector3 pos = Vector3.Lerp(start, end, t);
            pos.y += 4 * height * t * (1 - t);

            transform.position = pos;

            yield return null;
        }

        // 開始收服
        yield return StartCoroutine(Capture());
    }

    IEnumerator Capture()
    {
        // 將乘龍的所有材質切換成紅光
        Renderer[] renderers =
            target.GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            Material[] mats = r.materials;

            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = captureGlow;
            }

            r.materials = mats;
        }

        // 播放粒子特效
        if (captureEffect != null)
        {
            captureEffect.Play();
        }

        yield return new WaitForSeconds(0.3f);

        Vector3 startScale = target.localScale;
        Vector3 startPos = target.position;

        float time = 0;

        // 乘龍縮小並被吸入精靈球
        while (time < 1f)
        {
            time += Time.deltaTime / captureTime;
            float t = Mathf.Clamp01(time);

            target.localScale =
                Vector3.Lerp(startScale, Vector3.zero, t);

            target.position =
                Vector3.Lerp(startPos, transform.position, t);

            yield return null;
        }

        // 乘龍消失
        target.gameObject.SetActive(false);
        yield return StartCoroutine(DropAndShake());
    }
	IEnumerator DropAndShake()
{
    Vector3 start = transform.position;
    Vector3 end = start;

    // 偵測下方地面的高度
    RaycastHit hit;

    if (Physics.Raycast(start, Vector3.down, out hit, 100f))
    {
        end.y = hit.point.y + ballRadius;
    }
    else
    {
        yield break;
    }

    float time = 0f;

Quaternion startRotation = transform.rotation;
Quaternion endRotation = Quaternion.Euler(landingRotation);

// 精靈球落地並逐漸轉正
while (time < 1f)
{
    time += Time.deltaTime / dropTime;
    float t = Mathf.Clamp01(time);

    transform.position =
        Vector3.Lerp(start, end, t * t);

    transform.rotation =
        Quaternion.Slerp(startRotation, endRotation, t);

    yield return null;
}

transform.rotation = endRotation;

if (captureCamera != null)
{
    captureCamera.MoveToPokeball();
}

    yield return new WaitForSeconds(0.5f);

    Quaternion originalRotation = transform.rotation;

    // 左右搖晃三次
    for (int i = 0; i < 3; i++)
    {
        time = 0f;

        while (time < shakeTime)
        {
            time += Time.deltaTime;

            float angle = Mathf.Sin(
                time / shakeTime * Mathf.PI * 2
            ) * shakeAngle;

            transform.rotation =
    		Quaternion.AngleAxis(angle, Camera.main.transform.forward)
    		* originalRotation;

            yield return null;
        }

        transform.rotation = originalRotation;

        yield return new WaitForSeconds(0.4f);
    }
    	if (successEffect != null)
	{
    successEffect.transform.position = transform.position;
    successEffect.Play();
	}
    if (gotchaText != null)
{
    gotchaText.SetActive(true);
}
    Debug.Log("成功收服乘龍！");
}
}