using System.Collections.Generic;
using Simple_Pie_Menu.Scripts.Menu_Item;
using Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Settings_Handlers;
using Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Singleton;
using SimplePieMenu;
using UnityEngine;

namespace Simple_Pie_Menu.Scripts.Pie_Menu.Settings.Shape_Settings
{
    [ExecuteInEditMode]
    public class PieMenuShapeSettings : MonoBehaviour
    {
        public int ShapeDropdownList { get; private set; }
        public List<string> ShapeNames { get; private set; } = new();

        private PieMenu pieMenu;
        private PieMenuShapeSettingsHandler settingsHandler;

        private void OnEnable()
        {
            pieMenu = GetComponent<PieMenu>();
            pieMenu.OnComponentsInitialized += InitializeShapeSettings;
        }

        private void OnDisable()
        {
            pieMenu.OnComponentsInitialized -= InitializeShapeSettings;
        }

        public void CreateDropdownShapesList(int list)
        {
            ShapeDropdownList = list;
        }

        public void OnListSelectionChange()
        {
            settingsHandler.HandleShapeChange(pieMenu, ShapeDropdownList);
        }

        private void InitializeShapeSettings()
        {
            settingsHandler = PieMenuShared.References.ShapeSettingsHandler;
            InitializeShapeNamesList();
            SetCurrentlyUsedShape();
        }

        private void InitializeShapeNamesList()
        {
            foreach (Sprite shape in settingsHandler.Shapes)
            {
                ShapeNames.Add(shape.name);
            }
        }

        private void SetCurrentlyUsedShape()
        {
            Sprite sprite = pieMenu.GetTemplate().GetComponent<ImageFilledClickableSlices>().sprite;
            int index = settingsHandler.Shapes.IndexOf(sprite);

            if (index != -1)
            {
                CreateDropdownShapesList(index);
            }
        }
    }
}
