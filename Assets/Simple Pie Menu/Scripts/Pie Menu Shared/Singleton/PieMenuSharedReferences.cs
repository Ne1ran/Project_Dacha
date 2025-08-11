using Simple_Pie_Menu.Scripts.Others;
using Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Menu_Items_Manager;
using Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Menu_Toggler;
using Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Settings_Handlers;
using Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Settings_Handlers.General_Settings_Handler;
using Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Settings_Handlers.Icon_Settings_Handler;
using Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Settings_Handlers.Info_Panel_Settings_Handler;
using UnityEngine;

namespace Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Singleton
{
    [ExecuteInEditMode]
    public class PieMenuSharedReferences : MonoBehaviour
    {
        [SerializeField] Transform menuItemsManager;
        public MenuItemsManager MenuItemsManager { get; private set; }
        public MenuItemTemplate MenuItemTemplate { get; private set; }


        [SerializeField] Transform backgroundSettingsHandler;
        public PieMenuBackgroundSettingsHandler BackgroundSettingsHandler { get; private set; }


        [SerializeField] Transform shapeSettingsHandler;
        public PieMenuShapeSettingsHandler ShapeSettingsHandler { get; private set; }


        [SerializeField] Transform generalSettingsHandler;
        public PieMenuGeneralSettingsHandler GeneralSettingsHandler { get; private set; }


        [SerializeField] Transform positionSettingsHandler;
        public PieMenuPositionSettingsHandler PositionSettingsHandler { get; private set; }


        [SerializeField] Transform menuItemColorSettingsHandler;
        public MenuItemColorSettingsHandler MenuItemColorSettingsHandler { get; private set; }


        [SerializeField] Transform iconsSettingsHandler;
        public MenuItemIconsSettingsHandler IconsSettingsHandler { get; private set; }


        [SerializeField] Transform infoPanelSettingsHandler;
        public PieMenuInfoPanelSettingsHandler InfoPanelSettingsHandler { get; private set; }


        [SerializeField] Transform animationsSettingsHandler;
        public PieMenuAnimationsSettingsHandler AnimationsSettingsHandler { get; private set; }


        [SerializeField] Transform audioSettingsHandler;
        public PieMenuAudioSettingsHandler AudioSettingsHandler { get; private set; }


        [SerializeField] Transform selectionSettingsHandler;
        public PieMenuSelectionSettingsHandler SelectionSettingsHandler { get; private set; }


        [SerializeField] Transform closeSettingsHandler;
        public PieMenuCloseFunctionalitySettingsHandler CloseSettingsHandler { get; private set; }


        [SerializeField] Transform previewModeSettingsHandler;
        public PieMenuPreviewModeSettingsHandler PreviewModeSettingsHandler { get; private set; }


        [SerializeField] AudioSource testAudioSource;

        public AudioSource TestAudioSource
        {
            get { return testAudioSource; }
        }


        [SerializeField] Transform pieMenuToggler;
        public PieMenuToggler PieMenuToggler { get; private set; }

        private void OnEnable()
        {
            if (PrefabIsolationModeHelper.IsInPrefabIsolationMode()) return;

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            MenuItemsManager = menuItemsManager.GetComponent<MenuItemsManager>();
            MenuItemTemplate = menuItemsManager.GetComponent<MenuItemTemplate>();

            BackgroundSettingsHandler = backgroundSettingsHandler.GetComponent<PieMenuBackgroundSettingsHandler>();
            ShapeSettingsHandler = shapeSettingsHandler.GetComponent<PieMenuShapeSettingsHandler>();
            GeneralSettingsHandler = generalSettingsHandler.GetComponent<PieMenuGeneralSettingsHandler>();
            PositionSettingsHandler = positionSettingsHandler.GetComponent<PieMenuPositionSettingsHandler>();
            MenuItemColorSettingsHandler = menuItemColorSettingsHandler.GetComponent<MenuItemColorSettingsHandler>();
            IconsSettingsHandler = iconsSettingsHandler.GetComponent<MenuItemIconsSettingsHandler>();
            InfoPanelSettingsHandler = infoPanelSettingsHandler.GetComponent<PieMenuInfoPanelSettingsHandler>();
            AnimationsSettingsHandler = animationsSettingsHandler.GetComponent<PieMenuAnimationsSettingsHandler>();
            AudioSettingsHandler = audioSettingsHandler.GetComponent<PieMenuAudioSettingsHandler>();
            SelectionSettingsHandler = selectionSettingsHandler.GetComponent<PieMenuSelectionSettingsHandler>();
            CloseSettingsHandler = closeSettingsHandler.GetComponent<PieMenuCloseFunctionalitySettingsHandler>();
            PreviewModeSettingsHandler = previewModeSettingsHandler.GetComponent<PieMenuPreviewModeSettingsHandler>();
            PieMenuToggler = pieMenuToggler.GetComponent<PieMenuToggler>();

        }
    }
}
