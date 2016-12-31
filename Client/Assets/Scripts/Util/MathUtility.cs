using UnityEngine;

public static class MathUtility {
    public static Quaternion GetRotation(Vector3 from, Vector3 target) {
        target.Normalize();
        from.Normalize();
        float dot = Vector3.Dot(from, target);
        float angle = Mathf.Acos(dot) * 0.5f;

        Quaternion rotation = new Quaternion(0.0f, Mathf.Sin(angle), 0.0f, Mathf.Cos(angle));
        rotation = rotation * Quaternion.LookRotation(from);
        return rotation;
    }
}
