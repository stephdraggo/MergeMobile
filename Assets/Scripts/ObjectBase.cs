using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Merge
{
    /// <summary>
    /// Functionality, behaviour, and information for individual objects on the grid
    /// </summary>
    public class ObjectBase : MonoBehaviour
    {
        #region Variables

        //------------Static------------

        //----------Properties----------
        public int MergeLevel
        {
            get => mergeLevel;
        }

        public ChainBase Chain
        {
            get => chain;
        }

        public BoxCollider2D Collider { get; private set; }

        public bool Mergeable
        {
            get
            {
                if (mergeLevel >= chain.ChainLength) return false;
                return true;
            }
        }
        //------------Public------------

        //----------Serialised----------
        [SerializeField, Range(0, 20),
         Tooltip("Theoretical max is 20, but actual max varies according to number " +
                 "of levels in this object's merge column type. Values higher than " +
                 "allowed will result in default sprite being applied.")]
        protected int mergeLevel = 0;

        [SerializeField, Tooltip("Merge chain this object belongs to.")]
        protected ChainBase chain;

        [SerializeField] protected bool spawnsObjects = false;

        [Header("Only fill these if this object can spawn objects.")] [SerializeField]
        protected int spawnCount;

        //-----------Private------------
        protected Sprite sprite;
        protected SpriteRenderer render;
        protected ChainBase[] spawnableObjects;

        protected Box currentBox;
        //------------Const-------------

        #endregion

        //Awake, Start
        //OnMouseDown, OnMouseDrag, OnMouseUpAsButton

        #region Default Methods

        protected void Awake()
        {
            enabled = true;
        }

        protected virtual void Start()
        {
            //display
            render ??= GetComponent<SpriteRenderer>(); //get renderer
            CheckVisual(); //attach and display correct sprite

            //collider
            Collider = GetComponent<BoxCollider2D>(); //get own collider
            if (!Collider.enabled) Collider.enabled = true;

            //manager references
            SetBox();
            GridManager.ObjectsInPlay.Add(this); //tell gridmanager about this object
        }

        protected void OnMouseDown()
        {
            GridManager.EnableObjectColliders(
                false); //disable other object colliders so only the colliders for the boxes are accessible
            Collider.enabled = true; //enable own collider so OnMouseUpAsButton() gets called correctly
        }

        /// <summary>
        /// Make selected object follow mouse.
        /// Objects that have reached the highest merge level cannot currently be moved.
        /// </summary>
        protected void OnMouseDrag()
        {
            if (Mergeable)
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = new Vector3(mousePosition.x, mousePosition.y, -1);
            }
        }

        /// <summary>
        /// Start object release coroutine
        /// </summary>
        protected void OnMouseUpAsButton()
        {
            StartCoroutine(ReleaseObject());
        }

        #endregion

        //Setup, CheckVisual, KillObject
        //Click, SpawnObjects, ReleaseObject
        //SetBox

        #region Other Methods

        /// <summary>
        /// Manually setup specifics when instatiating a new object
        /// </summary>
        public void Setup(ChainBase _chain, int _level = 0, bool _spawnsObjects = false, ChainBase[] _spawnable = null,
            int _spawnCount = 0)
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
            GridManager.ObjectsInPlay.Remove(this);
            Destroy(gameObject);
        }

        /// <summary>
        /// If this object spawns anything on click/tap
        /// This function will spawn it
        /// </summary>
        protected virtual void Click()
        {
            if (spawnsObjects) //if this object can spawn objects
            {
                if (spawnCount > 0) //and if there are still objects for it to spawn
                {
                    ObjectBase newObject =
                        SpawnObject(
                            spawnableObjects[
                                    Random.Range(0, spawnableObjects.Length - 1)]
                                .ChainBaseObject); //spawn random spawnable prefab
                    
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
            int level = 0;
            if (_replacement)
            {
                location = currentBox;
                KillObject();
                level = mergeLevel + 1;
                if (location != null) //if we have a valid location to spawn to
                {
                    return Chain.NewObject(location, _prefab, level);
                }

                return null;
            }

            location = GridManager.FreeBox();
            level = Mathf.Max(0, mergeLevel-chain.SpawningLevel);


            if (location != null) //if we have a valid location to spawn to
            {
                return Chain.NewObject(location, _prefab, Random.Range(0, level));
            }

            return null;
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
            else if (currentlyOver.CurrentObject.Chain == chain &&
                     currentlyOver.CurrentObject.MergeLevel == mergeLevel &&
                     Mergeable) //if over a box containing an identical object and mergeable
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
        /// Gets own box component and assigns itself, otherwise log error
        /// </summary>
        private void SetBox()
        {
            currentBox = GetComponentInParent<Box>(); //get box parent
            if (currentBox)
            {
                currentBox.SetObject(this); //tell box parent about this child
            }
            else
            {
                Debug.LogError($"{name} is not placed correctly in the hierarchy and is not the child of a box.",
                    gameObject);
            }
        }

        #endregion
    }
}