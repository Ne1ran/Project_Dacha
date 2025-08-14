using UnityEngine;

namespace Game.PieMenu.Model
{
    public class PieMenuItemModel
    {
        public string Title { get; }
        public string Description { get; }
        public Sprite? Icon { get; }

        public PieMenuItemModel(string title, string description, Sprite? icon)
        {
            Title = title;
            Description = description;
            Icon = icon;
        }
    }
}