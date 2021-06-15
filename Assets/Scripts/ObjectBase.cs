using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Merge
{
    public class ObjectBase : MonoBehaviour
    {
        [SerializeField, Range(0, 20), Tooltip("Theoretical max is 20, but actual max varies according to number of levels in this object's merge column type. Values higher than allowed will result in default sprite being applied.")]
        protected int mergeLevel = 0;
        public int MergeLevel { get => mergeLevel; }
        protected bool mergeable
        {
            get
            {
                if (mergeLevel >= chain.ChainLength) return false;
                return true;
            }
        }

        [SerializeField, Tooltip("Merge chain this object belongs to.")]
        protected ChainBase chain;
        public ChainBase Chain { get => chain; }

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

        public BoxCollider2D Collider { get; private set; }

        protected void Awake()
        {
            enabled = true;
        }

        protected void Start()
        {
            if (render == null) render = GetComponent<SpriteRenderer>(); //get renderer
            CheckVisual(); //attach and display correct sprite

            Collider = GetComponent<BoxCollider2D>(); //get own collider
            if (!Collider.enabled) Collider.enabled = true;

            currentBox = GetComponentInParent<Box>(); //get box parent
            if (currentBox) currentBox.SetObject(this); //tell box parent about this child
            else Debug.LogError($"{name} is not placed correctly in the hierarchy and is not the child of a box.", gameObject);

            GridManager.objectsInPlay.Add(this); //tell gridmanager about this object
        }

        /// <summary>
        /// Manually setup specifics when instatiating a new object
        /// </summary>
        public void Setup(ChainBase _chain, int _level = 0, bool _spawnsObjects = false, ObjectBase[] _spawnable = null, int _spawnCount = 0)
        {
            chain = _chain;
            mergeLevel = _level;
            spawnsObjects = _spawnsObjects;
            if (_spawnsObjects)
            {
                spawnableObjects = _spawnable;
                spawnCount = _spawnCount;
            }

            CheckVisual();
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
                    ObjectBase newObject = SpawnObject(spawnableObjects[Random.Range(0, spawnableObjects.Length - 1)]); //spawn random spawnable prefab

                    if (newObject != null) //if spawn object succeeded
                    {
                        spawnCount--; //decrease spawnable objects
                    }

                    if (spawnCount <= 0) KillObject(); //kill object if empty
                }
            }
            GridManager.EnableObjectColliders(true); //enable object colliders again in case they got disabled
        }

        /// <summary>
        /// Tries to spawn a given object prefab.
        /// </summary>
        /// <param name="_prefab">Object prefab to spawn</param>
        /// <param name="_replacement">Is this new object to</param>
        protected ObjectBase SpawnObject(ObjectBase _prefab, bool _replacement = false)
        {
            Box location;
            if (_replacement)
            {
                location = currentBox;
                KillObject();
            }
            else
            {
                location = GridManager.FreeBox();
            }

            if (location != null) //if we have a valid location to spawn to
            {
                return Chain.NewObject(location, _prefab, mergeLevel+1);
            }
            else //if we failed to find a valid location, do not spawn anything
            {
                return null;
            }
        }

        protected void OnMouseDown()
        {
            GridManager.EnableObjectColliders(false); //disable other object colliders so only the colliders for the boxes are accessible
            Collider.enabled = true; //enable own collider so OnMouseUpAsButton() gets called correctly
        }

        /// <summary>
        /// Make selected object follow mouse.
        /// Objects that have reached the highest merge level cannot currently be moved.
        /// </summary>
        protected void OnMouseDrag()
        {
            if (mergeable)
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = new Vector3(mousePosition.x, mousePosition.y, -1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void OnMouseUpAsButton()
        {
            StartCoroutine(ReleaseObject());
        }


        protected IEnumerator ReleaseObject()
        {
            Collider.enabled = false;
            yield return null;

            Box currentlyOver = GridManager.currentlyOver;

            if (currentlyOver == currentBox) //if have not changed box
            {
                transform.position = currentBox.transform.position; //reset position
                Click(); //check for click spawning
            }
            else if (currentlyOver.CurrentObject == null) //if over an empty box
            {
                currentBox.RemoveObject(); //remove from old box
                currentlyOver.SetObject(this); //inform new box that it's moving in
                currentBox = currentlyOver; //get new box data

                transform.SetParent(currentlyOver.transform); //hierarchy
                transform.position = currentlyOver.transform.position; //scene position
            }
            else if (currentlyOver.CurrentObject.Chain == chain && currentlyOver.CurrentObject.MergeLevel == mergeLevel && mergeable) //if over a box containing an identical object and mergeable
            {
                SpawnObject(this, true);

                currentlyOver.CurrentObject.KillObject();
                KillObject();
            }
            else //catch
            {
                transform.position = currentBox.transform.position; //reset position
            }

            transform.position += new Vector3(0, 0, -1);

            GridManager.EnableObjectColliders(true);
        }

        /// <summary>
        /// Get the appropriate sprite and display it.
        /// </summary>
        public void CheckVisual()
        {
            if (render == null) render = GetComponent<SpriteRenderer>(); //get renderer
            sprite = Chain.GetSprite(MergeLevel); //get sprite
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