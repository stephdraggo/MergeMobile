using Serializable = System.SerializableAttribute;
using System.Collections.Generic;
using UnityEngine;

namespace Merge
{
    public class SpriteHolder : MonoBehaviour
    {
        public static SpriteHolder instance;

        [SerializeField]
        private List<ChainSprites> chains;

        [Serializable]
        public struct ChainSprites
        {

            [Tooltip("Be careful not to double up on chains.")]
            public MergeChain chainName;

            public List<Sprite> sprites;

            public int spritesLength => sprites.Count;
        }

        #region awake instance
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
        #endregion

        public Sprite GetSprite(MergeChain _chain, int _level)
        {
            switch (_chain)
            {
                case MergeChain.PlaceHolder:
                    return chains[0].sprites[_level];

                default:
                    Debug.LogError("GetSprite broke, assigning default sprite.");
                    return chains[0].sprites[0];
            }
        }

        public int GetChainLength(MergeChain _chain)
        {
            switch (_chain)
            {
                case MergeChain.PlaceHolder:
                    return chains[0].spritesLength;


                default:
                    return 0;
            }
        }
    }

    public enum MergeChain
    {
        PlaceHolder,
    }
}