using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Merge
{
    public class BuyObject : MonoBehaviour
    {
        #region Variables

        //------------Static------------
        //----------Properties----------
        //------------Public------------
        //----------Serialised----------
        [SerializeField] private ChainBase[] chains;

        //-----------Private------------
        private int cost;
        //------------Const-------------

        #endregion

        public void Buy()
        {
            if (GameManager.Points >= cost)
            {
                ChainBase chain = chains[Random.Range(0, chains.Length - 1)];
                int level = Random.Range(0, cost);
                ObjectBase newObject = chain.NewObject(GridManager.FreeBox(), chain.ChainBaseObject, level);
                if (newObject)
                {
                    GameManager.Instance.PointChange(-cost);
                    cost++;
                }
            }
            else
            {
                Debug.Log("Not enough gold");
            }
        }
    }
}