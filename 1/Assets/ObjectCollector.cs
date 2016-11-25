using System.Collections;
using System.Collections.Generic;

public class ObjectCollector<T> where T : new() {

    public delegate bool Operator(T Object);

    private Stack<T> d;
    private Operator Instructor, Destructor;

    public ObjectCollector(Operator Instructor, Operator Destructor) {
        d = new Stack<T>();
        this.Instructor = Instructor;
        this.Destructor = Destructor;
    }

    public void Collect(T Object) {

        if (!Destructor(Object))
            throw new System.Exception("ObjectCollector : "+Object.GetType()+" can't be destruct by Destructor.");

        d.Push(Object);
    }

    public T Reuse() {

        if (d.Count == 0)
            return new T();

        T td = d.Pop();
        if (!Instructor(td))
            throw new System.Exception("ObjectCollector : " + td.GetType() + " can't be instruct by Intructor.");

        return td;
    }

    public bool Empty() { return d.Count == 0; }
}
