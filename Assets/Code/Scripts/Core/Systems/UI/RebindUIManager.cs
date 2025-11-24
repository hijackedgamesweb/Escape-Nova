using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Linq;
using System;

namespace Code.Scripts.UI.Controls
{
    public class RebindMenuManager : MonoBehaviour
    {
        public static RebindMenuManager Instance { get; private set; }

        [Header("Input Action Asset")]
        [SerializeField] private InputActionAsset inputActionAsset;

        [Header("UI Panels")]
        [SerializeField] private GameObject waitingForInputOverlay;
        [SerializeField] private TextMeshProUGUI waitingText;

        [Header("Conflict Warning UI")]
        [SerializeField] private GameObject warningPopup;
        [SerializeField] private TextMeshProUGUI warningText;
        [SerializeField] private UnityEngine.UI.Button warningConfirmBtn;
        [SerializeField] private UnityEngine.UI.Button warningCancelBtn;

        public event Action OnRebindComplete;

        private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;
        private string _targetActionName;
        private int _targetBindingIndex;
        private TextMeshProUGUI _targetButtonText;

        private const string RebindsSaveKey = "rebinds_config";

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            LoadBindings();
        }
        
        private void OnDisable()
        {
            StopRebindingOperation();
        }

        public InputAction GetAction(string actionName)
        {
            return inputActionAsset.FindAction(actionName);
        }

        public void StartRebindProcess(string actionName, int bindingIndex, TextMeshProUGUI statusTextComponent)
        {
            _targetActionName = actionName;
            _targetBindingIndex = bindingIndex;
            _targetButtonText = statusTextComponent;

            InputAction actionToRebind = GetAction(actionName);
            if (actionToRebind == null || actionToRebind.bindings.Count <= bindingIndex) return;

            waitingForInputOverlay.SetActive(true);
            if(waitingText) waitingText.text = $"Esperando input para: {actionName.ToUpper()}... (ESC para cancelar)";
            statusTextComponent.text = "...";
            inputActionAsset.Disable();

            _rebindingOperation?.Cancel();

            _rebindingOperation = actionToRebind.PerformInteractiveRebinding(bindingIndex)
                .WithControlsExcluding("Mouse")
                .WithCancelingThrough("<Keyboard>/escape")
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(operation => RebindComplete(operation, actionToRebind))
                .OnCancel(operation => RebindCancelled(operation, actionToRebind));
            _rebindingOperation.Start();
        }

        private void RebindComplete(InputActionRebindingExtensions.RebindingOperation operation, InputAction action)
        {
            if (CheckBindingConflict(action, _targetBindingIndex, out InputAction conflictingAction, out string newKeyReadableName))
            {
                operation.Cancel();
                ShowConflictWarning(action, conflictingAction, newKeyReadableName);
                return;
            }

            FinishRebindProcess();
        }

        private void RebindCancelled(InputActionRebindingExtensions.RebindingOperation operation, InputAction action)
        {
            FinishRebindProcess();
        }

        private void FinishRebindProcess()
        {
            StopRebindingOperation();
            waitingForInputOverlay.SetActive(false);
            inputActionAsset.Enable();

            SaveBindings();
            OnRebindComplete?.Invoke();
        }

        private void StopRebindingOperation()
        {
            _rebindingOperation?.Dispose();
            _rebindingOperation = null;
        }

        private bool CheckBindingConflict(InputAction targetAction, int targetBindingIndex, out InputAction conflictingAction, out string newKeyName)
        {
            conflictingAction = null;
            string newPath = targetAction.bindings[targetBindingIndex].effectivePath;
            newKeyName = InputControlPath.ToHumanReadableString(newPath, InputControlPath.HumanReadableStringOptions.OmitDevice);

            foreach (InputAction action in inputActionAsset)
            {
                if (action == targetAction) continue;

                for (int i = 0; i < action.bindings.Count; i++)
                {
                    if (action.bindings[i].effectivePath == newPath && !action.bindings[i].isPartOfComposite)
                    {
                        conflictingAction = action;
                        return true;
                    }
                }
            }
            return false;
        }

        private void ShowConflictWarning(InputAction targetAction, InputAction conflictingAction, string keyName)
        {
            waitingForInputOverlay.SetActive(false);
            warningPopup.SetActive(true);

            warningText.text = $"¡Cuidado!\n\nLa tecla '{keyName.ToUpper()}' ya está asignada a la acción:\n'{conflictingAction.name.ToUpper()}'.\n\n¿Quieres asignarla de todas formas?";

            warningConfirmBtn.onClick.RemoveAllListeners();
            warningCancelBtn.onClick.RemoveAllListeners();

            warningConfirmBtn.onClick.AddListener(() => {
                warningPopup.SetActive(false);

                conflictingAction.ApplyBindingOverride(GetKeyboardBindingIndex(conflictingAction), "");
                
                 FinishRebindProcess();
            });
            
            warningCancelBtn.onClick.AddListener(() => {
                warningPopup.SetActive(false);
                targetAction.RemoveBindingOverride(_targetBindingIndex);
                FinishRebindProcess();
            });
        }
        
        private int GetKeyboardBindingIndex(InputAction action)
        {
             for (int i = 0; i < action.bindings.Count; i++)
             {
                 if (!action.bindings[i].isPartOfComposite && action.bindings[i].effectivePath.Contains("Keyboard"))
                    return i;
             }
             return 0;
        }

        public void SaveBindings()
        {
            string rebinds = inputActionAsset.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString(RebindsSaveKey, rebinds);
            PlayerPrefs.Save();
            Debug.Log("Rebinds guardados.");
        }

        public void LoadBindings()
        {
            if (PlayerPrefs.HasKey(RebindsSaveKey))
            {
                string rebinds = PlayerPrefs.GetString(RebindsSaveKey);
                inputActionAsset.LoadBindingOverridesFromJson(rebinds);
                Debug.Log("Rebinds cargados.");
            }
        }
    }
}