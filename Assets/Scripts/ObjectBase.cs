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

        Box currentBox;


        private new BoxCollider2D collider;
        public BoxCollider2D Collider => collider;

        private void Start()
        {
            render = GetComponent<SpriteRenderer>(); //get renderer
            CheckVisual();

            collider = GetComponent<BoxCollider2D>();
            currentBox = GetComponentInParent<Box>();
            if (currentBox) currentBox.SetObject(this);

            GridManager.objectsInPlay.Add(this);
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
                    Box emptyBox=null;
                    foreach (Box _box in GridManager.instance.GridBoxes)
                    {
                        if (_box.CurrentObject == null)
                        {
                            emptyBox = _box;
                            break;
                        }
                    }
                    if (emptyBox == null) return;

                    ObjectBase newObject = Instantiate(spawnableObjects[0]);
                    newObject.transform.SetParent(emptyBox.transform);
                    newObject.transform.position = emptyBox.transform.position;
                    spawnCount--;
                }
                
            }
        }



        protected void OnMouseDrag()
        {
            if (mergeLevel != SpriteHolder.instance.FlowerLength - 1)
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = new Vector3(mousePosition.x, mousePosition.y, -1);

                GridManager.EnableObjectColliders(false);
                collider.enabled = true;
            }
            
        }

        protected void OnMouseUpAsButton()
        {

            if(mergeLevel== SpriteHolder.instance.FlowerLength - 1)
            {
                Click();
            }
            else
            {
                StartCoroutine(ReleaseObject());
            }
        }


        private IEnumerator ReleaseObject()
        {
            collider.enabled = false;
            yield return null;

            Box currentlyOver = GridManager.instance.currentlyOver;

            if (currentlyOver == currentBox)
            {
                transform.position = currentBox.transform.position;
            }
            else if (currentlyOver.CurrentObject == null)
            {
                currentBox.RemoveObject();
                currentlyOver.SetObject(this);
                currentBox = currentlyOver;

                transform.SetParent(currentlyOver.transform);
                transform.position = currentlyOver.transform.position;
            }
            else if (currentlyOver.CurrentObject.Chain == chain && currentlyOver.CurrentObject.MergeLevel == mergeLevel)
            {
                currentlyOver.CurrentObject.mergeLevel++;
                currentlyOver.CurrentObject.CheckVisual();

                GridManager.objectsInPlay.Remove(this);
                Destroy(gameObject);
            }
            else
            {
                transform.position = currentBox.transform.position;
            }

            GridManager.EnableObjectColliders(true);
        }

        public void CheckVisual()
        {
            sprite = SpriteHolder.instance.GetSprite(Chain, MergeLevel); //get sprite
            render.sprite = sprite; //put sprite in renderer
        }
    }
}