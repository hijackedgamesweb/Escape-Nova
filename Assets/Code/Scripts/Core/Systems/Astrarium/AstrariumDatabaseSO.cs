using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Astrarium
{
    [CreateAssetMenu(fileName = "AstrariumDatabase", menuName = "Core/Astrarium/Database")]
    public class AstrariumDatabaseSO : ScriptableObject
    {
        public List<ScriptableObject> allEntries = new List<ScriptableObject>();

        private Dictionary<string, IAstrariumEntry> _lookup;

        public void Initialize()
        {
            if (_lookup != null && _lookup.Count > 0) return;

            _lookup = new Dictionary<string, IAstrariumEntry>();
            
            foreach (var obj in allEntries)
            {
                if (obj == null) continue;

                if (obj is IAstrariumEntry entry)
                {
                    string id = entry.GetAstrariumID();
                    if (!string.IsNullOrEmpty(id))
                    {
                        if (!_lookup.ContainsKey(id))
                        {
                            _lookup.Add(id, entry);
                        }
                        else
                        {
                            Debug.LogWarning($"[AstrariumDB] ID DUPLICADO ignorado: {id} en objeto {obj.name}");
                        }
                    }
                }
            }
            
            Debug.Log($"[AstrariumDB] Inicializada con {_lookup.Count} entradas.");
        }

        public IAstrariumEntry GetEntry(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            if (_lookup == null) Initialize();

            return _lookup.GetValueOrDefault(id);
        }

        public List<IAstrariumEntry> GetAllEntries()
        {
             List<IAstrariumEntry> list = new List<IAstrariumEntry>();
             foreach(var obj in allEntries) 
             {
                 if(obj is IAstrariumEntry entry) list.Add(entry);
             }
             return list;
        }

        private void OnEnable()
        {
            _lookup = null;
        }
    }
}