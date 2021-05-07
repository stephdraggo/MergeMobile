
using UnityEngine;

namespace Merge
{
    public class Testing : MonoBehaviour
    {
        public ChainBase chain;

        public void SpawnObject()
        {
            chain.NewObject(GridManager.FreeBox());
        }

    }


}