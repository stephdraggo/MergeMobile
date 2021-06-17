using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Merge
{
    /// <summary>
    /// Data for a chain of objects
    /// </summary>
    [CreateAssetMenu(fileName = "new chain", menuName = "Merge/New Chain")]
    public class ChainBase : ScriptableObject
    {
        #region Variables

        //------------Static------------
        //----------Properties----------

        public string objectName;
        public ObjectBase ChainBaseObject => chainBase;

        public int SpawningLevel
        {
            get
            {
                if (spawningLevel >= ChainLength)
                    return ChainLength - 1;
                return spawningLevel;
            }
        }

        public List<Sprite> Sprites => sprites;
        public int ChainLength => sprites.Count;

        public Sprite GetSprite(int _level)
        {
            if (_level >= ChainLength)
                _level = ChainLength - 1;
            return sprites[_level];
        }

        //------------Public------------
        //----------Serialised----------
        [SerializeField, Tooltip("BaseObject Prefab")]
        protected ObjectBase chainBase;

        [SerializeField, Tooltip("At what level this chain should start spawning items.")]
        protected int spawningLevel;

        [SerializeField, Tooltip("Sprites for this chain.")]
        protected List<Sprite> sprites;

        [SerializeField, Tooltip("Objects that can be spawned by objects in this chain.")]
        protected ChainBase[] spawnables;

        [SerializeField] private int countOnStart = 4;

        [SerializeField] private float spawnTimer = 10;

        //-----------Private------------
        //------------Const-------------
        [ReadOnly] private Vector3 bringToFront = new Vector3(0, 0, -1);
        private const float staggerStart = 0.1f;

        #endregion

        //ObjectBase, ObjectsOnStart, ObjectsThroughGame

        #region Other Methods

        public ObjectBase NewObject(Box _location, ObjectBase _prefab = null, int _level = 0)
        {
            if (_prefab == null)
                _prefab = chainBase; //default prefab if none passed

            if (_location == null) return null; //do not continue if no valid location passed

            ObjectBase newObject = Instantiate(_prefab, _location.transform.position + bringToFront,
                Quaternion.identity, _location.transform);

            bool spawns = _level >= spawningLevel;
            int amount = (_level - spawningLevel + 1) * 4;
            newObject.Setup(_prefab.Chain, _level, spawns, _prefab.Chain.spawnables, amount);

            //GridManager.objectsInPlay.Add(newObject); //let the gridmanager know we succeeded

            _location.SetObject(newObject);

            //GridManager.EnableObjectColliders(true);
            
            return newObject;
        }

        public IEnumerator ObjectsOnStart()
        {
            int count = countOnStart;
            while (count > 0)
            {
                yield return new WaitForSeconds(staggerStart);
                NewObject(GridManager.FreeBox(), ChainBaseObject);
                count--;
            }
        }

        public IEnumerator ObjectsThroughGame()
        {
            while (GridManager.Instance.gameObject.activeSelf)
            {
                yield return new WaitForSeconds(spawnTimer);
                NewObject(GridManager.FreeBox(), ChainBaseObject);
            }
        }

        #endregion
    }
}