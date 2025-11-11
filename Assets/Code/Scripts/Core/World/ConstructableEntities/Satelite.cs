using System.Collections.Generic;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using UnityEngine;

namespace Code.Scripts.Core.World.ConstructableEntities
{
    public class Satelite
    {
        private SpriteRenderer _spriteRenderer;
        public int TimeToBuild { get; private set; }
        public Planet Planet { get; set; }
        public string Name { get; private set; }

        public void InitializeSatelite(SateliteDataSO sateliteDataSo, Planet planet)
        {
            Name = sateliteDataSo.constructibleName;
            TimeToBuild = sateliteDataSo.timeToBuild;
            Planet = planet;
        }
    }
}