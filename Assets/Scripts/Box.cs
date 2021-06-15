using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
                gameObject.AddComponent<BoxCollider2D>();
            }
        }

        /// <summary>
        /// Tell the gridmanager that the player just entered this box's collider
        /// </summary>
        private void OnMouseEnter()
        {
            GridManager.currentlyOver = this;
        }

        /// <summary>
        /// Update the gridmanager continuously that the player is over this box's collider
        /// Theoretically unnecessary but implemented as a quick fix for some null errors
        /// </summary>
        private void OnMouseOver()
        {
            GridManager.currentlyOver = this;
        }

        /// <summary>
        /// Tell this box it has a new object to hold
        /// </summary>
        /// <param name="_object">new content for this box</param>
        public void SetObject(ObjectBase _object)
        {
            if (currentObject != null)
                Debug.LogWarning($"Overwriting the previous {currentObject} with {_object}", gameObject);
            currentObject = _object;
        }

        /// <summary>
        /// Tell this box its contents has been removed
        /// </summary>
        public void RemoveObject() => currentObject = null;



    }
}