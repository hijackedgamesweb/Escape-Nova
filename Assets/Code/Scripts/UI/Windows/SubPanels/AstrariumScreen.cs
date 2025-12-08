using System.Collections.Generic;
using Code.Scripts.Core.Systems.Astrarium;
using Code.Scripts.Patterns.ServiceLocator;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.Scripts.UI.Windows
{
    public class AstrariumScreen : BaseUIScreen, IPointerClickHandler
    {
        [Header("Category Navigation")]
        [SerializeField] private Button _btnPlanets;
        [SerializeField] private Button _btnResources;
        [SerializeField] private Button _btnSatellites;
        [SerializeField] private Button _btnSpecies;
        [SerializeField] private Button _btnConstellations;
        [SerializeField] private Button _btnSpecial;

        [Header("List Panel")]
        [SerializeField] private Transform _listContainer;
        [SerializeField] private GameObject _listEntryPrefab;

        [Header("Details Panel")]
        [SerializeField] private GameObject _detailsContainer;
        [SerializeField] private Image _detailImage;
        [SerializeField] private TextMeshProUGUI _detailTitle;
        [SerializeField] private TextMeshProUGUI _detailDescription;

        private AstrariumManager _astrariumManager;
        private AstrariumCategory _currentCategory = AstrariumCategory.Planet;

        private void Start()
        {
            if (_btnPlanets) _btnPlanets.onClick.AddListener(() => SetCategory(AstrariumCategory.Planet));
            if (_btnResources) _btnResources.onClick.AddListener(() => SetCategory(AstrariumCategory.Resource));
            if (_btnSatellites) _btnSatellites.onClick.AddListener(() => SetCategory(AstrariumCategory.Satellite));
            if (_btnSpecies) _btnSpecies.onClick.AddListener(() => SetCategory(AstrariumCategory.Species));
            if (_btnConstellations) _btnConstellations.onClick.AddListener(() => SetCategory(AstrariumCategory.Constellation));
            if (_btnSpecial) _btnSpecial.onClick.AddListener(() => SetCategory(AstrariumCategory.Special));

            _astrariumManager = AstrariumManager.Instance;
            if (_astrariumManager != null)
            {
                _astrariumManager.OnEntryUpdated += HandleEntryUpdated;
            }
        }
        
        private void OnDestroy()
        {
            if (_astrariumManager != null)
            {
                _astrariumManager.OnEntryUpdated -= HandleEntryUpdated;
            }
        }

        public override void Show(object parameter = null)
        {
            base.Show(parameter);
            
            if (parameter is AstrariumCategory cat)
            {
                SetCategory(cat);
            }
            else
            {
                RefreshList();
                _detailsContainer.SetActive(false);
            }
        }

        private void HandleEntryUpdated(string id, DiscoveryStatus status)
        {
            if (gameObject.activeSelf)
            {
                RefreshList();
                if (_detailsContainer.activeSelf && _astrariumManager.GetEntryData(id).GetDisplayName() == _detailTitle.text)
                {
                   ShowDetails(_astrariumManager.GetEntryData(id)); 
                }
            }
        }

        private void SetCategory(AstrariumCategory category)
        {
            _currentCategory = category;
            _detailsContainer.SetActive(false);
            AudioManager.Instance.PlaySFX("Astrarium");
            RefreshList();
        }

        private void RefreshList()
        {
            if (_astrariumManager == null || _listContainer == null) return;

            foreach (Transform child in _listContainer) Destroy(child.gameObject);

            var entries = _astrariumManager.GetUnlockedEntriesByCategory(_currentCategory);

            foreach (var entry in entries)
            {
                var status = _astrariumManager.GetStatus(entry.GetAstrariumID());
                if (status == DiscoveryStatus.Unknown) continue;

                GameObject obj = Instantiate(_listEntryPrefab, _listContainer);
                obj.transform.localScale = Vector3.one;

                var txt = obj.GetComponentInChildren<TextMeshProUGUI>();
                Image iconImg = null;
                var images = obj.GetComponentsInChildren<Image>();
                if (images.Length > 1) iconImg = images[1]; else if (images.Length == 1) iconImg = images[0];
                Button btn = obj.GetComponent<Button>();

                if (txt != null && iconImg != null)
                {
                    if (status == DiscoveryStatus.Discovered)
                    {
                        txt.text = "???";
                        iconImg.color = Color.black;
                        iconImg.sprite = entry.GetIcon();
                    }
                    else
                    {
                        txt.text = entry.GetDisplayName();
                        iconImg.sprite = entry.GetIcon();
                        iconImg.color = Color.white;
                    }
                }

                if (btn != null)
                {
                    btn.onClick.AddListener(() => ShowDetails(entry));
                }
            }
        }

        private void ShowDetails(IAstrariumEntry entry)
        {
            var status = _astrariumManager.GetStatus(entry.GetAstrariumID());
            _detailsContainer.SetActive(true);

            if (status == DiscoveryStatus.Discovered)
            {
                _detailTitle.text = "Unknown Data";
                _detailImage.sprite = entry.GetIcon();
                _detailImage.color = Color.black;
                _detailDescription.text = GetLockedMessage(entry.GetCategory());
            }
            else if (status == DiscoveryStatus.Analyzed)
            {
                _detailTitle.text = entry.GetDisplayName();
                _detailImage.sprite = entry.GetIcon();
                _detailImage.color = Color.white;
                _detailImage.preserveAspect = true;
                _detailDescription.text = entry.GetDescription();
            }
            
            AudioManager.Instance.PlaySFX("Astrarium");
        }

        private string GetLockedMessage(AstrariumCategory category)
        {
            switch (category)
            {
                case AstrariumCategory.Planet: return "<color=red>ANALYSIS INCOMPLETE.</color>\n\nLaunch an expedition and <b>Construct</b> this planet to decrypt full data.";
                case AstrariumCategory.Satellite: return "<color=red>BLUEPRINT ACQUIRED.</color>\n\n<b>Manufacture</b> this satellite to view operational specifications.";
                case AstrariumCategory.Resource: return "Data corrupted. Acquire sample to analyze.";
                default: return "More research or construction required to access this entry.";
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(_detailDescription, Input.mousePosition, null);
            if (linkIndex != -1)
            {
                TMP_LinkInfo linkInfo = _detailDescription.textInfo.linkInfo[linkIndex];
                string linkID = linkInfo.GetLinkID();
                var targetEntry = _astrariumManager.GetEntryData(linkID);
                if (targetEntry != null) {
                    SetCategory(targetEntry.GetCategory());
                    ShowDetails(targetEntry);
                }
            }
        }
    }
}