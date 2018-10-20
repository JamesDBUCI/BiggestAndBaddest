
public class Xuple<T1, T2>
{
    public T1 Value1;
    public T2 Value2;

    public Xuple(T1 value1 = default(T1), T2 value2 = default(T2))
    {
        Value1 = value1;
        Value2 = value2;
    }
}
