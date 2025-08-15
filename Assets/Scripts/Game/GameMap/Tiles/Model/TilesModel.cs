using System.Collections.Generic;

namespace Game.GameMap.Tiles.Model
{
    public class TilesModel
    {
        public List<SingleTileModel> Tiles { get; } = new();

        public void AddTile(SingleTileModel tile)
        {
            Tiles.Add(tile);
        }
        public void AddTileRange(List<SingleTileModel> tile)
        {
            Tiles.AddRange(tile);
        }

        public bool TryRemoveTile(string guid)
        {
            SingleTileModel? tileModel = Tiles.Find(tile => tile.Id == guid);
            if (tileModel == null) {
                return false;
            }

            Tiles.Remove(tileModel);
            return true;
        }
    }
}