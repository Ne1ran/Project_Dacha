using System.Collections.Generic;
using Simple_Pie_Menu.Scripts.Menu_Item;
using Simple_Pie_Menu.Scripts.Pie_Menu;
using UnityEngine;

namespace Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Settings_Handlers
{
    [ExecuteInEditMode]
    public class PieMenuShapeSettingsHandler : MonoBehaviour
    {
        [SerializeField] List<Sprite> shapes;

        public List<Sprite> Shapes
        {
            get { return shapes; }
        }

        public void HandleShapeChange(PieMenu pieMenu, int shapeIndex)
        {
            foreach (KeyValuePair<int, PieMenuItem> menuItem in pieMenu.GetMenuItems())
            {
                ChangeShape(menuItem.Value.transform, shapeIndex);
            }
        }

        private void ChangeShape(Transform menuItem, int shapeIndex)
        {
            ImageFilledClickableSlices menuItemImage = menuItem.GetComponent<ImageFilledClickableSlices>();
            menuItemImage.sprite = Shapes[shapeIndex];
        }
    }
}
