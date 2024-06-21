using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class FactoryPattern : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Factory<Shape> fac = new Factory<Shape>();

        var shape = fac.GetShape("正方形");
        shape.DoSomething();


        var shape1 = fac.GetShape("圆形");
        shape1.DoSomething();


        var shape2 = fac.GetShape("三角形");
        shape2.DoSomething();


        Factory<Pepole> Ty = new Factory<Pepole>();

        var wo = Ty.GetShape("男人");
        wo.DoSomething();


        var wo1 = Ty.GetShape("女人");
        wo1.DoSomething();
    }
}


public abstract class Shape
{
    public abstract string _Type { get; }

    public abstract void DoSomething();
}

[Factory("正方形")]
public class Square : Shape
{
    public override string _Type => "正方形";

    public override void DoSomething()
    {
        Debug.Log("我是正方形");
    }
}

[Factory("圆形")]
public class Circle : Shape
{
    public override string _Type => "圆形";

    public override void DoSomething()
    {
        Debug.Log("我是圆形");
    }
}

[Factory("三角形")]
public class Triangle : Shape
{
    public override string _Type => "三角形";

    public override void DoSomething()
    {
        Debug.Log("我是三角形");
    }
}


public abstract class Pepole
{
    public abstract string Age { get; }

    public abstract void DoSomething();
}

[Factory("男人")]
public class Man : Pepole
{
    public override string Age => "男人";

    public override void DoSomething()
    {
        Debug.Log("我是男人");
    }
}

[Factory("女人")]
public class Women : Pepole
{
    public override string Age => "圆形";

    public override void DoSomething()
    {
        Debug.Log("我是女人");
    }
}



public class Factory<T>
{
    private readonly IDictionary<string, Type> products;

    public Factory()
    {
        products = Assembly.GetExecutingAssembly().ExportedTypes
            .Where(t => t.IsSubclassOf(typeof(T)) && !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetCustomAttributes<FactoryAttribute>(), (t, a) => new { t, a.Value })
            .ToDictionary(ta => ta.Value.ToLower(), ta => ta.t);
    }

    public T GetShape(string Value)
    {
        if (products.TryGetValue(Value.ToLower(), out Type productType))
        {
            return (T)Activator.CreateInstance(productType);
        }

        throw new ArgumentException($"{Value} 未找到");
    }
}

[AttributeUsage(AttributeTargets.Class, Inherited = false)] //特性约束，约束特性作用域
public class FactoryAttribute : Attribute
{
    public string Value { get; }

    public FactoryAttribute(string value)
    {
        Value = value;
    }
}