using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Merge
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private int countForEach = 4;
        [SerializeField] private float timer = 10;
        [SerializeField] private List<ChainBase> chains;

        void Start()
        {
            //spawn some objects on start with Ienumerator
            foreach (ChainBase _chain in chains)
            {
                StartCoroutine(ObjectsOnStart(_chain, countForEach));
            }

            //spawn objects through game with Ienumerator
            StartCoroutine(ObjectsThroughGame());
        }

        private IEnumerator ObjectsOnStart(ChainBase _chain, int _count)
        {
            while (_count > 0)
            {
                yield return new WaitForSeconds(0.1f);
                _chain.NewObject(GridManager.FreeBox(), _chain.ChainBaseObject);
                _count--;
            }
        }

        private IEnumerator ObjectsThroughGame()
        {
            while (gameObject.activeSelf)
            {
                yield return new WaitForSeconds(timer);
                ChainBase _chain = chains[Random.Range(0, chains.Count)];
                _chain.NewObject(GridManager.FreeBox(), _chain.ChainBaseObject);
            }
        }
    }
}