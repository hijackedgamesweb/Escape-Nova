using System;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.Core.GameInfo
{
    [CreateAssetMenu(fileName = "New GameInfo", menuName = "Info/GameInfo")]
    public class GameInfoSO : ScriptableObject
    {
        //Variables
        public int pages;
        public String[] titles;
        public String[] messages;
        public Sprite[] images;
    }
}