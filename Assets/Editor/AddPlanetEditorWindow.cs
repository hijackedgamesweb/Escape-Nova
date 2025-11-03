using System;
using Code.Scripts.Core.World;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(SolarSystem))]
    public class AddPlanetEditorWindow : UnityEditor.Editor
    {
        /*private int orbitIndex = 0;
        private int positionInOrbit = 0;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Añadir planeta", EditorStyles.boldLabel);

            orbitIndex = EditorGUILayout.IntField("Indice de Orbita", orbitIndex);
            positionInOrbit = EditorGUILayout.IntField("Posición en Orbita", positionInOrbit);

            EditorGUILayout.Space();

            // Botón
            if (GUILayout.Button("Añadir Planeta"))
            {
                SolarSystem comp = (SolarSystem)target;
               // comp.AddPlanet(orbitIndex, positionInOrbit);
            }
        }*/
    }
}