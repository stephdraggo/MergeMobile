using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Merge
{
    public class Coin : ObjectBase
    {
        protected int Points => Mathf.Max(mergeLevel+1,mergeLevel*mergeLevel);


        protected override void Click()
        {
            GameManager.Instance.PointChange(Points);
            KillObject();
        }
    }
}