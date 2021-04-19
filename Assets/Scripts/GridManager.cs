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
        private Box[,] gridBoxes;
        public Box[,] GridBoxes => gridBoxes;

        public Box currentlyOver;

        private void Awake()
        {
            instance = this;
        }

        void Start()
        {
            gridBoxes = new Box[rows.Length, columnCount];
            for (int i = 0; i < rows.Length; i++)
            {
                Box[] boxes = rows[i].GetComponentsInChildren<Box>();
                for (int j = 0; j < columnCount; j++)
                {
                    gridBoxes[i, j] = boxes[j];
                }
            }
        }

        void Update()
        {

        }
    }
}