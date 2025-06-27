using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void FlashRed()
    {
        if (sr == null) return;
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        Color original = sr.color;
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = original;
    }

    public void Shake()
    {
        StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        Vector3 origin = transform.position;
        float t = 0;
        while (t < 0.3f)
        {
            float x = Random.Range(-0.1f, 0.1f);
            transform.position = origin + new Vector3(x, 0, 0);
            t += Time.deltaTime;
            yield return null;
        }

        transform.position = origin;
    }
}