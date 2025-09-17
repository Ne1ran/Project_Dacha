namespace Core.Notifications.Model
{
    public enum NotificationType
    {
        NONE = -1,
        FERTILIZER_NOT_FOUND = 0,
        TOOL_NOT_FOUND = 1,
        SEEDS_NOT_FOUND = 5,
        WATER_TOOL_NOT_FOUND = 10,
        NO_WATER = 15,
        INVENTORY_FULL = 20,
        CANNOT_HARVEST_PLANT = 25,
    }
}