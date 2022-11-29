using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WanderingCloud
{
    public class floatingObject : MonoBehaviour
    {

        [Foldout("X Movement")]public AnimationCurve Xcurve;
        [Foldout("X Movement")] public float XAxisMovementMax;

        [Foldout("Y Movement")] public AnimationCurve Ycurve;
        [Foldout("Y Movement")] public float YAxisMovementMax;

        [Foldout("Z Movement")] public AnimationCurve Zcurve;
        [Foldout("Z Movement")] public float ZAxisMovementMax;

        Vector3 startPos;

        public float loopTime;
        private float timer;
        // Start is called before the first frame update
        void Start()
        {
            startPos = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            if (timer > loopTime)
            {
                timer = 0;
            }
            else
            {
                timer += Time.deltaTime;
            }
            float ratio = timer / loopTime;
            transform.position = startPos + new Vector3(Xcurve.Evaluate(ratio) * XAxisMovementMax, Ycurve.Evaluate(ratio) * YAxisMovementMax, Zcurve.Evaluate(ratio) * ZAxisMovementMax);


        }
    }
}
