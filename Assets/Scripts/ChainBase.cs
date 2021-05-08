using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Merge
{
    [CreateAssetMenu(fileName = "new chain", menuName = "Merge/New Chain")]
    public class ChainBase : ScriptableObject
    {
        [SerializeField, Tooltip("BasObject Prefab")]
        protected ObjectBase chainBase;

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
        protected ObjectBase[] spawnables;

        /// <summary> Size of chain taken from sprite count. </summary>
        public int ChainLength => sprites.Count;





        /// <summary>
        /// Get sprite by level.
        /// </summary>
        public Sprite GetSprite(int _level) => sprites[_level];

        public ObjectBase NewObject(Box _location, ObjectBase _prefab = null, int _level = 0)
        {
            if (_prefab == null) _prefab = chainBase;

            if (_location == null) return null;

            ObjectBase newObject = Instantiate(_prefab, _location.transform.position + new Vector3(0, 0, -1), Quaternion.identity, _location.transform);

            bool spawns = _level >= spawningLevel;
            int amount = (_level - spawningLevel + 1) * 4;
            newObject.Setup(this, _level, spawns, spawnables, amount);

            GridManager.objectsInPlay.Add(newObject); //let the gridmanager know we succeeded



            return newObject;
        }
    }
}