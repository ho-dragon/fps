using System.Collections;
using UnityEngine;

public class BulletEffect : MonoBehaviour {
    public LineRenderer lineRenderer;
    private IEnumerator coFadeOut;
    private readonly Color colorOrigin = Color.gray;

    public void Show(Vector3 start, Vector3 end, float duration) {        
        if (this.gameObject.activeSelf == false) {
            this.gameObject.SetActive(true);
        }
        this.lineRenderer.SetPositions(new Vector3[] { start, end });
        this.lineRenderer.enabled = true;
        this.lineRenderer.startColor = colorOrigin;
        this.lineRenderer.endColor = colorOrigin;

        if(this.coFadeOut != null) {
            StopCoroutine(this.coFadeOut);
        }

        this.coFadeOut = FadeOut(duration);
        StartCoroutine(this.coFadeOut);
    }

    IEnumerator FadeOut(float duration) {
        float remainDuraion = duration;
        float alpha = 1f;
        while(alpha > 0) {
            yield return null;
            remainDuraion -= Time.deltaTime;
            alpha -= 0.05f;
            if(alpha < 0) {
                alpha = 0f;
            }
            Color temp = new Color(this.colorOrigin.r, this.colorOrigin.g, this.colorOrigin.b, alpha);
            this.lineRenderer.startColor = temp;
            this.lineRenderer.endColor = temp; 
        }
        this.lineRenderer.enabled = false;
    }
}