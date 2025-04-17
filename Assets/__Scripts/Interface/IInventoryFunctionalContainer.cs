namespace ChosTIS
{
    public interface IInventoryFunctionalContainer : IInventoryContainer
    {
        public bool TryPlaceTetrisItem(TetrisItem tetrisItem);
        public void PlaceTetrisItem(TetrisItem tetrisItem);
        public void RemoveTetrisItem();

    }
}