using System.Collections.Generic;
using Game.PieMenu.ActionHandler;
using UnityEngine;

namespace Game.PieMenu.Model
{
    public class PieMenuItemModel
    {
        public string InteractionName { get; }
        public string Title { get; }
        public string BaseDescription { get; }
        public Sprite? Icon { get; }
        public List<PieMenuItemSelectionModel> SelectionModels { get; set; }

        public int CurrentSelectionIndex { get; set; } = -1;
        public IPieMenuActionHandler? BaseActionHandler { get; set; }


        public PieMenuItemModel(string interactionName,
                                string title,
                                string baseDescription,
                                Sprite? icon,
                                List<PieMenuItemSelectionModel> selectionModels,
                                IPieMenuActionHandler? baseActionHandler = null)
        {
            InteractionName = interactionName;
            Title = title;
            BaseDescription = baseDescription;
            Icon = icon;
            SelectionModels = selectionModels;
            BaseActionHandler = baseActionHandler;
        }
    }
}