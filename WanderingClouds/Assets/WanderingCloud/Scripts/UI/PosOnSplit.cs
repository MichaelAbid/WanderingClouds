using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace WanderingCloud
{
    [ExecuteAlways]
    public class PosOnSplit : MonoBehaviour
    {
        public SplitScreen screen;
        public List<RectTransform> elements;

        void Update()
        {
            foreach (var element in elements)
            {
                element.position = new Vector2(screen.splitValue * Screen.width, element.position.y);
            }
        }
    }
}
