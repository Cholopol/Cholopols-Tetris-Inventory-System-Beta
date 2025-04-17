public enum TetrisPieceShape
{
    Frame,
    //Domino
    Domino,
    //Tromino
    Tromino_I, Tromino_L, Tromino_J,
    //Tetromino
    Tetromino_I, Tetromino_O, Tetromino_T, Tetromino_J, Tetromino_L, Tetromino_S, Tetromino_Z,
    //Pentomino
    Pentomino_I, Pentomino_L, Pentomino_J, Pentomino_U, Pentomino_T, Pentomino_P
}

public enum Dir
{
    Down,
    Left,
    Up,
    Right,
}

public enum InventorySlotType
{
    Pocket, Coat, Chesthanging, BackPack, WaistHanging, Coffer, Depository,
    LongWeapon, ShortWeapon, LargeConsume, MiddleConsume, SmallConsume
}

public enum PersistentGridType
{
    None,
    Pocket, Coffer, Depository
}

public enum PlaceState
{
    OnGridHasItem, OnGridNoItem,
    OnSlotHasItem, OnSlotNoItem,
    InvalidPos
}