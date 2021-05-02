using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Merge.Objects;

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
        private Box[,] gridBoxes;
        public Box[,] GridBoxes => gridBoxes;

        public Box currentlyOver;

        [Tooltip("List of objects currently in the level.")]
        public static List<ObjectBase> objectsInPlay = new List<ObjectBase>();

        private void Awake()
        {
            //there can only be one active at any time, and a new one should always overwrite an old one
            instance = this;
        }

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
            }
        }


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

    }
}