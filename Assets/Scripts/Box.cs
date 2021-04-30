using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Merge.Objects;


namespace Merge
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Box : MonoBehaviour
    {

        private ObjectBase currentObject;
        public ObjectBase CurrentObject => currentObject;
        private void Awake()
        {
            if (GetComponent<BoxCollider2D>() == null)
            {
                gameObject.AddComponent<BoxCollider2D>();
            }
        }
        private void OnMouseEnter()
        {
            GridManager.instance.currentlyOver = this;
        }

        private void OnMouseOver()
        {
            GridManager.instance.currentlyOver = this;
        }

        public void SetObject(ObjectBase _object)
        {
            currentObject = _object;
        }

        public void RemoveObject() => currentObject = null;



    }
}