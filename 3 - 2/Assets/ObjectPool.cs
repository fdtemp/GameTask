
public class ObjectPool<T> where T : class {

    public delegate bool Operator(T Object);
    public delegate T Creator();

    private T[] d;
    private int Size;
    public int Count { get; private set; }
    private Operator Reset, Collect, Destroy;
    private Creator Create;

    public ObjectPool(int Size, Creator Create, Operator Reset, Operator Collect, Operator Destroy) {
        Count = 0;
        d = new T[Size];
        this.Size = Size;
        this.Create = Create;
        this.Reset = Reset;
        this.Collect = Collect;
        this.Destroy = Destroy;
    }

    public void Put(T Object) {
        if (Count == Size) {
            if (Destroy(Object)) return;
            else throw new System.Exception("ObjectPool : " + Object.GetType() + " Destroy() is sth wrong.");
        }
        if (!Collect(Object))
            throw new System.Exception("ObjectPool : " + Object.GetType() + " Collect() is sth wrong.");
        d[Count++] = (Object);
    }

    public T Get() {
        if (Count == 0) return Create();
        if (!Reset(d[--Count]))
            throw new System.Exception("ObjectPool : " + d[Count].GetType() + " Reset() is sth wrong.");
        return d[Count];
    }
}
