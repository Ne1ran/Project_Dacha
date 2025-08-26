using UnityEngine;

namespace Game.PieMenu.Model
{
    public class PieMenuItemSelectionModel
    {
        public string ItemId { get; }
        public Sprite? Icon { get; }
        public string DescriptionSubstituteText { get; }

        public PieMenuItemSelectionModel(string itemId, Sprite? icon, string descriptionSubstituteText)
        {
            ItemId = itemId;
            Icon = icon;
            DescriptionSubstituteText = descriptionSubstituteText;
        }
    }
}