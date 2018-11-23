using UnityEngine;
using System.Collections;

public class EffectManager : MonoBehaviourInstance<EffectManager> {
    public BulletEffect bulletEffect;

    public void OnBulletTail(Vector3 start, Vector3 end, float duration) {
        this.bulletEffect.Show(start, end, duration);
    }
}
