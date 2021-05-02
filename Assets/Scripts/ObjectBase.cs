using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;

namespace Merge.Objects
{
    public class ObjectBase : MonoBehaviour
    {
        [SerializeField, Range(0, 20), Tooltip("Theoretical max is 20, but actual max varies according to number of levels in this object's merge column type. Values higher than allowed will result in default sprite being applied.")]
        protected int mergeLevel = 0;
        public int MergeLevel { get => mergeLevel; }
        private int maxLevel;

        [SerializeField, Tooltip("Merge chain this object belongs to.")]
        protected MergeChain chain;
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
            CheckVisual(); //attach and display correct sprite

            collider = GetComponent<BoxCollider2D>(); //get own collider
            currentBox = GetComponentInParent<Box>(); //get box parent
            if (currentBox) currentBox.SetObject(this); //tell box parent about this child
            else Debug.LogError($"{name} is not placed correctly in the hierarchy and is not the child of a box.", gameObject);

            GridManager.objectsInPlay.Add(this); //tell gridmanager about this object

            maxLevel = SpriteHolder.instance.GetChainLength(chain);
        }

        /// <summary>
        /// If this object spawns anything on click/tap
        /// This function will spawn it
        /// </summary>
        protected void Click()
        {
            if (spawnsObjects) //if this object can spawn objects
            {
                if (spawnCount > 0) //and if there are still objects for it to spawn
                {
                    //find free box
                    Box emptyBox = null;
                    foreach (Box _box in GridManager.instance.GridBoxes)
                    {
                        if (_box.CurrentObject == null)
                        {
                            emptyBox = _box;
                            break;
                        }
                    }
                    if (emptyBox == null) return; //if no free box was found, click does nothing

                    ObjectBase newObject = Instantiate(spawnableObjects[Random.Range(0, spawnableObjects.Length - 1)]); //get random spawnable prefab
                    newObject.transform.SetParent(emptyBox.transform); //set position in hierarchy
                    newObject.transform.position = emptyBox.transform.position + new Vector3(0, 0, -1); //set position in scene

                    spawnCount--; //decrease spawnable objects

                    if (spawnCount <= 0) KillObject(); //kill object if empty
                }
            }
            GridManager.EnableObjectColliders(true); //enable object colliders again in case they got disabled
        }


        /// <summary>
        /// Make selected object follow mouse.
        /// Objects that have reached the highest merge level cannot currently be moved.
        /// </summary>
        protected void OnMouseDrag()
        {
            if (mergeLevel != maxLevel - 1)
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = new Vector3(mousePosition.x, mousePosition.y, -1);

                GridManager.EnableObjectColliders(false); //disable other object colliders so only the colliders for the boxes are accessible
                collider.enabled = true; //enable own collider so OnMouseUpAsButton() gets called correctly
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void OnMouseUpAsButton()
        {
            if (mergeLevel == maxLevel - 1)
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

            transform.position += new Vector3(0, 0, -1);

            GridManager.EnableObjectColliders(true);
        }

        /// <summary>
        /// Get the appropriate sprite and display it.
        /// </summary>
        public void CheckVisual()
        {
            sprite = SpriteHolder.instance.GetSprite(Chain, MergeLevel); //get sprite
            render.sprite = sprite; //put sprite in renderer
        }

        /// <summary>
        /// Remove this object from list of active objects and destroy the gameobject.
        /// </summary>
        public void KillObject()
        {
            GridManager.objectsInPlay.Remove(this);
            Destroy(gameObject);
        }
    }
}