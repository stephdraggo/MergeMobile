using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Merge
{
    public class GridManager : MonoBehaviour
    {
        public static GridManager instance;

        [SerializeField, Tooltip("Rows in grid.")]
        private GameObject[] rows;

        [SerializeField, Tooltip("Number of columns.")]
        private int columnCount;

        [Tooltip("Transforms that can hold items.")]
        private static Box[,] gridBoxes;
        public static Box[,] GridBoxes => gridBoxes;

        public static Box currentlyOver;

        [Tooltip("List of objects currently in the level.")]
        public static List<ObjectBase> objectsInPlay = new List<ObjectBase>();

        #region awake instance
        private void Awake()
        {
            if (instance == null) //if none
            {
                instance = this; //it this
            }
            else if (instance != this) //if one, but not this
            {
                Destroy(this); //get rid of this
                return;
            }
        }
        #endregion

        #region start grid data
        private void Start()
        {
            gridBoxes = new Box[rows.Length, columnCount]; //gridboxes has correct size
            for (int i = 0; i < rows.Length; i++) //go through each row
            {
                Box[] boxes = rows[i].GetComponentsInChildren<Box>(); //each row has an array of boxes in it
                for (int j = 0; j < columnCount; j++) //for every column
                {
                    gridBoxes[i, j] = boxes[j]; //assign to 2d array
                }
            }
        }
        #endregion

        /// <summary>
        /// Enable or disable all colliders on objects.
        /// Need to do this when dragging an object onto another object bc active object checks for the contents of the gridbox, not an object.
        /// </summary>
        /// <param name="_enable">Enable colliders?</param>
        public static void EnableObjectColliders(bool _enable)
        {
            foreach (ObjectBase _object in objectsInPlay)
            {
                _object.Collider.enabled = _enable;
                //get a weird null reference here, doesn't seem to break anything tho
            }
        }

        /// <summary>
        /// Returns the first empty box if there are any.
        /// </summary>
        public static Box FreeBox()
        {
            Box emptyBox = null;
            foreach (Box _box in GridBoxes)
            {
                if (_box.CurrentObject == null)
                {
                    emptyBox = _box;
                    break;
                }
            }
            return emptyBox;
        }

    }
}