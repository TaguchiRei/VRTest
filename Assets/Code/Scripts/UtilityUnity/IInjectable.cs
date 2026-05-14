using UnityEngine;

public interface IInjectable<in T>
{
    void Inject(T t);
}

public interface IInjectable<in T1, in T2>
{
    void Inject(T1 t1, T2 t2);
}

public interface IInjectable<in T1, in T2, in T3>
{
    void Inject(T1 t1, T2 t2, T3 t3);
}

public interface IInjectable<in T1, in T2, in T3, in T4>
{
    void Inject(T1 t1, T2 t2, T3 t3, T4 t4);
}