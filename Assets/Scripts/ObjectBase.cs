using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;

namespace Merge.Objects
{
    public class ObjectBase : MonoBehaviour
    {
        [SerializeField, Range(0, 10), Tooltip("Theoretical max is 10, but actual max varies according to number of levels in this object's merge column type. Values higher than allowed will result in default sprite being applied.")]
        protected int mergeLevel = 0;
        public int MergeLevel { get => mergeLevel; }

        [SerializeField]
        protected MergeChain chain = MergeChain.Flower;
        public MergeChain Chain { get => chain; }

        protected Sprite sprite;
        protected SpriteRenderer render;

        [SerializeField]
        protected bool spawnsObjects = false;
        [Header("Only fill these if this object can spawn objects.")]
        [SerializeField]
        protected int spawnCount;
        [SerializeField]
        protected ObjectBase[] spawnableObjects;

        private void Start()
        {
            render = GetComponent<SpriteRenderer>(); //get renderer
            sprite = SpriteHolder.instance.GetSprite(Chain, MergeLevel); //get sprite
            render.sprite = sprite; //put sprite in renderer
        }

        /// <summary>
        /// If this object spawns anything on click/tap
        /// This function will spawn it
        /// </summary>
        protected void Click()
        {
            if (spawnsObjects)
            {
                if (spawnCount > 0)
                {
                    //spawn object here
                }
                spawnCount--;
            }
        }

        /// <summary>
        /// When click/tap lasts longer than a certain interval
        /// Attach object to finger and move
        /// </summary>
        protected void DragObject()
        {

        }

        /// <summary>
        /// When drag ends
        /// Compare to target location
        /// If same object and level is mergeable, merge
        /// Else if object, switch places
        /// Else place
        /// </summary>
        /// <param name="_target">If there is an object, this is it</param>
        protected void DragOnto(ObjectBase _target = null)
        {

        }
    }
}