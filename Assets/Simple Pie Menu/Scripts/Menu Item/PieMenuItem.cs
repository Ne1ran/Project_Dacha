using Simple_Pie_Menu.Scripts.Others;
using Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Menu_Toggler;
using Simple_Pie_Menu.Scripts.Pie_Menu;
using Simple_Pie_Menu.Scripts.Pie_Menu.Settings;
using Unity.Collections;
using UnityEngine;

namespace Simple_Pie_Menu.Scripts.Menu_Item
{
    public class PieMenuItem : MonoBehaviour
    {
        [Header("Menu Item")]
        [ReadOnly]
        [SerializeField]
        private int _id;

        public int Id => _id;

        [SerializeField]
        private string _header = null!;

        public string Header => _header;

        [HideInInspector]
        [SerializeField]
        private string _details;

        public string Details => _details;

        public AudioSource HoverAudioSource { get; private set; } = null!;

        private Animator _animator = null!;
        private IMenuItemClickHandler _clickHandler = null!;

        private bool _idAssigned;
        private bool _mouseOverButton;
        private readonly string _mouseEnterTrigger = "MouseEnter";
        private readonly string _mouseExitTrigger = "MouseExit";

        private PieMenuGeneralSettings _generalSettings = null!;
        private PieMenuToggler _pieMenuToggler = null!;

        private RectTransform _pieRectTransform = null!;
        private PieMenuMain _pieMenuMain = null!;

        public float SizeDeltaX => _pieRectTransform.sizeDelta.x;

        private void Awake()
        {
            _pieRectTransform = GetComponent<RectTransform>();
        }

        // todo
        public void Initialize(PieMenuMain pieMenuMain)
        {
            if (PrefabIsolationModeHelper.IsInPrefabIsolationMode()) {
                return;
            }

            _pieMenuMain = pieMenuMain;

            HoverAudioSource = GetComponent<AudioSource>();
            _animator = GetComponent<Animator>();
            _clickHandler = GetComponent<IMenuItemClickHandler>();

            PieMenuSettingsModel settingsModel = pieMenuMain.PieMenuSettingsModel;
            _generalSettings = settingsModel.GeneralSettings;
            _pieMenuToggler = settingsModel.PieMenuToggler;
        }

        public void SetId(int newId)
        {
            if (_idAssigned) {
                return;
            }

            _id = newId;
            _idAssigned = true;
        }

        public void SetHeader(string newHeader)
        {
            _header = newHeader;
        }

        public void SetDetails(string newDetails)
        {
            _details = newDetails;
        }

        public void DisplayHeader()
        {
            if (_generalSettings != null) {
                _generalSettings.ModifyHeader(_pieMenuMain, _header);
            }
        }

        public void DisplayDetails()
        {
            if (_generalSettings != null) {
                _generalSettings.ModifyDetails(_pieMenuMain, _details);
            }
        }

        public void OnPointerEnter()
        {
            if (_mouseOverButton) {
                return;
            }

            _mouseOverButton = true;

            // can play sound here
            _animator.SetTrigger(_mouseEnterTrigger);

            DisplayHeader();
            DisplayDetails();
        }

        public void BeforePointerExit()
        {
            _mouseOverButton = false;
        }

        public void OnPointerExit()
        {
            if (_mouseOverButton) {
                return;
            }
            
            _animator.SetTrigger(_mouseExitTrigger);
        }

        public void OnClick()
        {
            if (_clickHandler != null) {
                BeforePointerExit();
                _clickHandler.Handle();
            } else {
                Debug.Log("To handle clicks, you need to create a new script in which you implement the IMenuItemClickHandler interface."
                          + " Then, attach it to the appropriate Menu Item. Check the documentation to learn more.");
            }

            _pieMenuToggler.SetActive(_pieMenuMain, false);
        }
    }
}