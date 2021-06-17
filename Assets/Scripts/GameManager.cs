using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Merge
{
    /// <summary>
    /// Tells objects to start spawning
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Variables

        //------------Static------------
        public static GameManager Instance;

        public static int Points = 0;

        //----------Properties----------
        //------------Public------------
        //----------Serialised----------
        [SerializeField] private List<ChainBase> chains;

        [SerializeField] private TMP_Text coinDisplay, goalDisplay;

        [SerializeField] private int goal;

        [SerializeField] private UnityEvent winEvent;
        //-----------Private------------
        //------------Const-------------

        #endregion

        //Start

        #region Default Methods

        void Start()
        {
            Instance = this;

            //spawn some objects on start with Ienumerator
            foreach (ChainBase _chain in chains)
            {
                StartCoroutine(_chain.ObjectsOnStart());
                StartCoroutine(_chain.ObjectsThroughGame());
            }

            coinDisplay.text = Points.ToString();
            goalDisplay.text = "Goal: " + goal.ToString() + " gold";
        }

        #endregion

        #region Other Methods

        public void PointChange(int _change)
        {
            Points += _change;
            coinDisplay.text = Points.ToString();
            if(Points>=goal) WinLevel();
        }

        private void WinLevel()
        {
            winEvent.Invoke();
            goal *= 3;
            goalDisplay.text = "Goal: " + goal.ToString() + " gold";
            
            PointChange(-goal/3);
        }
        
        #endregion
    }
}