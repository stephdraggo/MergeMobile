using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Merge.Objects;


namespace Merge
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Box : MonoBehaviour
    {
        [Tooltip("The current contents of this box.")]
        private ObjectBase currentObject;
        public ObjectBase CurrentObject => currentObject;
        private void Awake()
        {
            //Error catch and fix for if this box doesn't have a collider
            if (GetComponent<BoxCollider2D>() == null)
            {
                Debug.LogWarning($"Object {name} doesn't have a box collider 2d, adding one now. Please assign a box collider 2d in the inspector to fix this in future.", gameObject);
                gameObject.AddComponent<BoxCollider2D>();
            }
        }

        /// <summary>
        /// Tell the gridmanager that the player just entered this box's collider
        /// </summary>
        private void OnMouseEnter()
        {
            GridManager.instance.currentlyOver = this;
        }

        /// <summary>
        /// Update the gridmanager continuously that the player is over this box's collider
        /// Theoretically unnecessary but implemented as a quick fix for some errors
        /// </summary>
        private void OnMouseOver()
        {
            GridManager.instance.currentlyOver = this;
        }

        /// <summary>
        /// Tell this box it has a new object to hold
        /// </summary>
        /// <param name="_object">new content for this box</param>
        public void SetObject(ObjectBase _object)
        {
            currentObject = _object;
        }

        /// <summary>
        /// Tell this box its contents has been removed
        /// </summary>
        public void RemoveObject() => currentObject = null;



    }
}