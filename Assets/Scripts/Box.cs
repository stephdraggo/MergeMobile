using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Merge
{
    public class Box : MonoBehaviour
    {
        private void OnMouseEnter()
        {
            GridManager.instance.currentlyOver = this;
        }
    }
}