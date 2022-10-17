using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

[System.Serializable]
public class TransformData
{
    public Vector3 pos = Vector3.zero;
    public Quaternion rot = Quaternion.identity;
    public Vector3 size = Vector3.one;

    public TransformData() { }
    public TransformData(Transform t)
    {
        pos = t.position;
        rot = t.rotation;
        size = t.localScale;
    }
    public TransformData(Vector3 pos, Quaternion rot, Vector3 size)
    {
        this.pos = pos;
        this.rot = rot;
        this.size = size;
    }
    public TransformData Lerp(TransformData from, TransformData to, float time)
    {
        return new TransformData(
            Vector3.Lerp(from.pos, to.pos, time),
            Quaternion.Slerp(from.rot, to.rot, time),
            Vector3.Lerp(from.size, to.size, time)
        );
    }

    public static implicit operator TransformData(Transform t)
    {
        return new TransformData(t);
    }
}

public static class Extention_TransformData
{
    public static void LerpTransformData(this Transform target, TransformData from, TransformData to, float time)
    {
        target.position = Vector3.Lerp(from.pos, to.pos, time);
        target.rotation = Quaternion.Slerp(from.rot, to.rot, time);
        target.localScale = Vector3.Lerp(from.size, to.size, time);
    }
    public static void DoTransform(this Transform target, TransformData to, float time, Ease ease = Ease.Linear)
    {
        float t = 0;
        TransformData from = target;
        DOTween.To(() => t, x => t = x, 1, time).
            SetEase(ease).
            OnUpdate(() => {target.LerpTransformData(from, to, t);});
    }
    public static void SetTransformData(this Transform target, TransformData data)
    {
        target.position = data.pos;
        target.rotation = data.rot;
        target.localScale = data.size;
    }
}