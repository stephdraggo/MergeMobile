using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Merge
{
    [CreateAssetMenu(fileName = "new chain", menuName = "Merge/New Chain")]
    public class ChainBase : ScriptableObject
    {
        [SerializeField, Tooltip("BaseObject Prefab")]
        protected ObjectBase chainBase;

        public ObjectBase ChainBaseObject => chainBase;

        [SerializeField, Tooltip("At what level this chain should start spawning items.")]
        protected int spawningLevel;

        public int SpawningLevel
        {
            get
            {
                if (spawningLevel >= ChainLength)
                    return ChainLength - 1;
                return spawningLevel;
            }
        }

        [SerializeField, Tooltip("Sprites for this chain.")]
        protected List<Sprite> sprites;

        public List<Sprite> Sprites => sprites;

        [SerializeField, Tooltip("Objects that can be spawned by objects in this chain.")]
        protected ChainBase[] spawnables;

        /// <summary> Size of chain taken from sprite count. </summary>
        public int ChainLength => sprites.Count;

        /// <summary>
        /// Get sprite by level.
        /// </summary>
        public Sprite GetSprite(int _level)
        {
            if (_level >= ChainLength)
                _level = ChainLength - 1;
            return sprites[_level];
        }

        public ObjectBase NewObject(Box _location, ObjectBase _prefab = null, int _level = 0)
        {
            if (_prefab == null) 
                _prefab = chainBase; //default prefab if none passed

            if (_location == null) return null; //do not continue if no valid location passed

            ObjectBase newObject = Instantiate(_prefab, _location.transform.position + new Vector3(0, 0, -1),
                Quaternion.identity, _location.transform);

            bool spawns = _level >= spawningLevel;
            int amount = (_level - spawningLevel + 1) * 4;
            newObject.Setup(this, _level, spawns, spawnables, amount);

            //GridManager.objectsInPlay.Add(newObject); //let the gridmanager know we succeeded

            _location.SetObject(newObject);

            GridManager.EnableObjectColliders(true);

            return newObject;
        }
    }
}