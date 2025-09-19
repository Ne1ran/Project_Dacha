namespace Core.Notifications.Model
{
    public enum NotificationType
    {
        None = -1,
        FertilizerNotFound = 0,
        ToolNotFound = 1,
        SeedsNotFound = 5,
        WaterToolNotFound = 10,
        NoWater = 15,
        InventoryFull = 20,
        CannotHarvestPlant = 25,
    }
}