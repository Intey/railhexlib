namespace RailHexLib
{
    public interface IDistancable<T>
    {
        int DistanceTo(T other);
    }
}