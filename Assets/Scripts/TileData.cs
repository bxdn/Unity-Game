public class TileData
{
    public double height;
    public readonly Biome biome;
    public Map.Detail? detail;
    public bool locked = false;

    public TileData(double height, Biome biome)
    {
        this.height = height;
        this.biome = biome;
    }
}