using System.Collections.Generic;
using Code.Scripts.Core.Systems.Astrarium;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Necesario para los hyperlinks

namespace Code.Scripts.UI.Menus
{
    public class AstrariumUIController : MonoBehaviour, IPointerClickHandler
    {
        [Header("Managers")]
        // Referencia al manager (puede ser vía Singleton o inyección)
        private AstrariumManager _manager => AstrariumManager.Instance; 

        [Header("Left Panel - Categories")]
        [SerializeField] private Button btnPlanets;
        [SerializeField] private Button btnResources;
        [SerializeField] private Button btnSatellites;
        [SerializeField] private Button btnSpecies;
        [SerializeField] private Button btnConstellations;

        [Header("Middle Panel - List")]
        [SerializeField] private Transform listContainer;
        [SerializeField] private GameObject listEntryPrefab;

        [Header("Right Panel - Details")]
        [SerializeField] private GameObject detailsPanel;
        [SerializeField] private Image detailImage;
        [SerializeField] private TextMeshProUGUI detailTitle;
        [SerializeField] private TextMeshProUGUI detailDescription;
        
        private AstrariumCategory _currentCategory = AstrariumCategory.Planet;

        private void Start()
        {
            btnPlanets.onClick.AddListener(() => SetCategory(AstrariumCategory.Planet));
            btnResources.onClick.AddListener(() => SetCategory(AstrariumCategory.Resource));
            btnSatellites.onClick.AddListener(() => SetCategory(AstrariumCategory.Satellite));
            btnSpecies.onClick.AddListener(() => SetCategory(AstrariumCategory.Species));
            btnConstellations.onClick.AddListener(() => SetCategory(AstrariumCategory.Constellation));

            if (_manager != null)
            {
                _manager.OnEntryUpdated += OnEntryUpdated;
            }

            SetCategory(AstrariumCategory.Planet);
            detailsPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            if (_manager != null)
            {
                _manager.OnEntryUpdated -= OnEntryUpdated;
            }
        }

        private void OnEntryUpdated(string id, DiscoveryStatus status)
        {
            RefreshList(); 
        }

        private void SetCategory(AstrariumCategory category)
        {
            _currentCategory = category;
            RefreshList();
        }

        private void RefreshList()
        {
            foreach (Transform child in listContainer)
            {
                Destroy(child.gameObject);
            }
            
            var entries = _manager.GetUnlockedEntriesByCategory(_currentCategory);

            foreach (var entry in entries)
            {
                var status = _manager.GetStatus(entry.GetAstrariumID());
                
                if (status == DiscoveryStatus.Unknown) continue;

                GameObject obj = Instantiate(listEntryPrefab, listContainer);
                
                var btn = obj.GetComponent<Button>();
                var img = obj.GetComponentsInChildren<Image>()[1];
                var txt = obj.GetComponentInChildren<TextMeshProUGUI>();

                if (status == DiscoveryStatus.Discovered)
                {
                    txt.text = "???";
                    img.color = Color.black;
                }
                else
                {
                    txt.text = entry.GetDisplayName();
                    img.sprite = entry.GetIcon();
                    img.color = Color.white;
                }

                btn.onClick.AddListener(() => ShowDetails(entry));
            }
        }

        private void ShowDetails(IAstrariumEntry entry)
        {
            DiscoveryStatus status = _manager.GetStatus(entry.GetAstrariumID());
            
            if (status != DiscoveryStatus.Analyzed) return;

            detailsPanel.SetActive(true);
            detailTitle.text = entry.GetDisplayName();
            detailImage.sprite = entry.GetIcon();
            
            detailDescription.text = entry.GetDescription(); 
        }

        
        public void OnPointerClick(PointerEventData eventData)
        {
            // Verificamos si hicimos click en el texto de descripción
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(detailDescription, Input.mousePosition, null);
            
            if (linkIndex != -1)
            {
                TMP_LinkInfo linkInfo = detailDescription.textInfo.linkInfo[linkIndex];
                string linkID = linkInfo.GetLinkID();

                var targetEntry = _manager.GetEntryData(linkID);

                if (targetEntry != null)
                {
                    SetCategory(targetEntry.GetCategory());
                    ShowDetails(targetEntry);
                    
                }
            }
        }
    }
}