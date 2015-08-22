using TiledSharp;

public class MapSquare {

    public int tileSetId;
    public int x;
    public int y;

    public bool isWalkable;

    public MapSquare(TmxLayerTile sourceTile)
    {
        tileSetId = sourceTile.Gid;
        x = sourceTile.X;
        y = sourceTile.Y;
    }

}
