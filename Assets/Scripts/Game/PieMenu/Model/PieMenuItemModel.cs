namespace Game.PieMenu.Model
{
    public class PieMenuItemModel
    {
        public string Title { get; }
        public string Description { get; }
        public string IconPath { get; }

        public PieMenuItemModel(string title, string description, string iconPath)
        {
            Title = title;
            Description = description;
            IconPath = iconPath;
        }
    }
}