using UnityEngine;

namespace Game.PieMenu.Model
{
    public class PieMenuItemModel
    {
        public string InteractionName { get; }
        public string Title { get; }
        public string Description { get; }
        public Sprite? Icon { get; }

        public PieMenuItemModel(string interactionName, string title, string description, Sprite? icon)
        {
            InteractionName = interactionName;
            Title = title;
            Description = description;
            Icon = icon;
        }
    }
}