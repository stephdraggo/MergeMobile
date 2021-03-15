using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Merge
{
    public class SpriteHolder : MonoBehaviour
    {
        public static SpriteHolder instance;

        [SerializeField]
        private List<Sprite> flowerSprites;

        public int FlowerLength => flowerSprites.Count;
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
            DontDestroyOnLoad(this); //make this immortal
        }

        public Sprite GetSprite(MergeColumn _column, int _level)
        {
            switch (_column)
            {
                case MergeColumn.Flower:
                    return flowerSprites[_level];

                default:
                    Debug.LogError("GetSprite broke, assigning default sprite.");
                    return flowerSprites[0];
            }
        }

        void Start()
        {

        }

        void Update()
        {

        }
    }

    public enum MergeColumn
    {
        Flower,
    }
}