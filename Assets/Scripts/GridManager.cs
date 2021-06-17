using System.Collections.Generic;
using UnityEngine;

namespace Merge
{
    /// <summary>
    /// Holds information about the grid and its contents
    /// </summary>
    public class GridManager : MonoBehaviour
    {
        #region Variables

        //------------Static------------
        public static GridManager Instance;
        public static Box[,] GridBoxes => gridBoxes;

        public static Box currentlyOver;

        [Tooltip("List of objects currently in the level.")]
        public static List<ObjectBase> ObjectsInPlay = new List<ObjectBase>();

        //----------Properties----------
        //------------Public------------
        //----------Serialised----------
        [SerializeField, Tooltip("Rows in grid.")]
        private GameObject[] rows;

        [SerializeField, Tooltip("Number of columns.")]
        private int columnCount;

        [Tooltip("Transforms that can hold items.")]
        private static Box[,] gridBoxes;

        //-----------Private------------
        //------------Const-------------

        #endregion

        //Awake, Start

        #region Default Methods

        private void Awake()
        {
            CheckInstanceAwake();
        }

        private void Start()
        {
            SetupGridReferences();
        }

        #endregion

        //EnableObjectColliders, FreeBox
        //CheckInstanceAwake, SetupGridReferences

        #region Other Methods

        /// <summary>
        /// Enable or disable all colliders on objects.
        /// Need to do this when dragging an object onto another object bc active object checks for the contents of the gridbox, not an object.
        /// </summary>
        /// <param name="_enable">Enable colliders?</param>
        public static void EnableObjectColliders(bool _enable)
        {
            //this should clear any null objects
            for (int i = 0; i < ObjectsInPlay.Count; i++)
            {
                if (ObjectsInPlay[i] == null)
                    ObjectsInPlay.RemoveAt(i);
            }

            foreach (ObjectBase _object in ObjectsInPlay)
            {
                if (_object != null)
                {
                    _object.Collider.enabled = _enable;

                    if (_object.Collider == null)
                        Debug.Log($"{_object.name} is currently missing a collider or just not refering to it",
                            _object);
                }
                else
                {
                    Debug.Log("object null");
                }
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

        private void CheckInstanceAwake()
        {
            if (Instance == null) //if none
            {
                Instance = this; //it this
            }
            else if (Instance != this) //if one, but not this
            {
                Destroy(this); //get rid of this
                return;
            }
        }

        /// <summary>
        /// Uses information provided to set up 2d array reference to grid
        /// </summary>
        private void SetupGridReferences()
        {
            gridBoxes = new Box[rows.Length, columnCount]; //gridboxes get correct size
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
    }
}