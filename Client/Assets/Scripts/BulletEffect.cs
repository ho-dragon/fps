using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEffect : MonoBehaviour {
    public LineRenderer lineRenderer;
    private readonly Color colorOrigin = Color.white;

    public void Show(Vector3 start, Vector3 end, float duration) {        
        if (this.gameObject.activeSelf == false) {
            this.gameObject.SetActive(true);
        }
        Logger.DebugHighlight("[BulletEffect.Show] duration = " + duration);
        this.lineRenderer.SetPositions(new Vector3[] { start, end });
        this.lineRenderer.enabled = true;
        this.lineRenderer.material.color = colorOrigin;
        StartCoroutine(FadeOut(duration));
    }

    IEnumerator FadeOut(float duration) {
        yield return new WaitForSeconds(duration);
        float alpha = lineRenderer.material.color.a;
        while(alpha > 0) {
            yield return null;
            alpha -= 0.05f;
            if(alpha < 0) {
                alpha = 0f;
            }
            lineRenderer.material.color = new Color(this.colorOrigin.r, this.colorOrigin.g, this.colorOrigin.b, alpha);
        }
        this.lineRenderer.enabled = false;
    }
}
