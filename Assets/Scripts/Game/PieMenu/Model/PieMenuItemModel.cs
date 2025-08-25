using System.Collections.Generic;

namespace Game.PieMenu.Model
{
    public class PieMenuItemModel
    {
        public string InteractionHandlerName { get; }
        public string Title { get; }
        public string BaseDescription { get; }
        public List<PieMenuItemSelectionModel> SelectionModels { get; set; }

        public int CurrentSelectionIndex { get; set; } = -1;

        public PieMenuItemModel(string interactionHandlerName,
                                string title,
                                string baseDescription,
                                List<PieMenuItemSelectionModel> selectionModels)
        {
            InteractionHandlerName = interactionHandlerName;
            Title = title;
            BaseDescription = baseDescription;
            SelectionModels = selectionModels;
        }
    }
}