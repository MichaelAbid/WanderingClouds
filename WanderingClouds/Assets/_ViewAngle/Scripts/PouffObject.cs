
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider))]
public class PouffObject : MonoBehaviour
{
    public Material material;
    public bool isPuffing;

    public Vector3 pos;
    public Vector3 scale;
    
    public Vector3 impact;
    public float pouffSensi = 1f;
    public float pouffEffect = 0.5f;
    public float pouffResorb = 3f;
    public float pouffScale = 3f;
    public Ease inEasing;
    public Ease outEasing;
    
    [Button("Reset")]
    public void Awake()
    {
        pos = transform.position;
        scale = transform.localScale;
        material = GetComponent<Renderer>().material;
        isPuffing = false;
        StopAllCoroutines();
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody body = other.GetComponentInParent<Rigidbody>();
        impact = (transform.position - body.position).normalized * body.velocity.magnitude * pouffSensi;

        if (!isPuffing) StartCoroutine(Pouffing());
    }

    private IEnumerator Pouffing()
    {
        isPuffing = true; 
        
        transform.DOScale(Vector3.one * pouffScale, pouffEffect).SetEase(inEasing);
        transform.DOMove(pos + impact, pouffEffect).SetEase(inEasing);
        material.DOFade(0,pouffEffect).SetEase(inEasing);
        
        yield return new WaitForSeconds(pouffEffect);
        
        transform.DOScale(scale, pouffResorb).SetEase(outEasing);
        transform.DOMove(pos, pouffResorb).SetEase(outEasing);
        material.DOFade(1,pouffResorb).SetEase(outEasing);

        yield return new WaitForSeconds(pouffResorb);

        isPuffing = false;
    }
}
