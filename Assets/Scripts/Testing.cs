using System;
using UnityEngine;
using Merge;


public class Testing : MonoBehaviour
{
    public ChainBase chain;
    public int level;

    public void SpawnObject()
    {
        chain.NewObject(GridManager.FreeBox(), chain.ChainBaseObject, level);
    }
}