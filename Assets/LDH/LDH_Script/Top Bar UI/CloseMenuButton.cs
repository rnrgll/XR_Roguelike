using Managers;
using UnityEngine;
using UnityEngine.UI;
using UI;


namespace TopBarUI
{ 
    public class CloseMenuButton : MonoBehaviour
    {
        [SerializeField] private GlobalUI _globalUIType;
        
        private Button _button;
        // Start is called before the first frame update
        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        void Start()
        {
            _button.onClick.AddListener(CloseMenuUI);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }

        private void CloseMenuUI()
        {
            Manager.UI.TopBarUI.GetComponent<ToggleGroup>().SetAllTogglesOff();
        }

    }
}