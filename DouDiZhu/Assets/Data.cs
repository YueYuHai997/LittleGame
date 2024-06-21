using System;
using UnityEngine;

public enum GameEvent
{
    // 初始化阶段
    GameNone,
        
    // 初始化阶段
    GameInit,
    // 展示手牌
    GameShowHand,
    //叫地主阶段
    GameCallLandlord,
    //确定地主 展示地主牌 并加入地主手牌
    GameShowLandlord,
    //开出牌阶段
    GameBeginPlaye,
    //结束阶段
    GameOver,
    //重新开始
    GameRestart,
}

public class CardType
{
    public Pattern Type; //类型
    public Point MaxPoint; //最大点数


    public static bool operator >(CardType a, CardType b)
    {
        return a.Type == b.Type && a.MaxPoint > b.MaxPoint;
    }
    
    [Obsolete("不要使用此方法",true)]
    public static bool operator <(CardType a, CardType b)
    {
       Debug.LogError("don't Ues this Function");
       return false;
    }
}


public enum Type
{
    Spade, //黑桃
    Heart, //红桃
    Club, //梅花
    Diamond, //方块
}

public enum Point : int
{
    None = 0,
    C1 = 14,
    C2 = 15,
    C3 = 3,
    C4 = 4,
    C5 = 5,
    C6 = 6,
    C7 = 7,
    C8 = 8,
    C9 = 9,
    C10 = 10,
    C11 = 11,
    C12 = 12,
    C13 = 13,

    Joker = 99,
    JokerBig = 100,
}

public enum Pattern
{
    None, //不成型
    Single, //单
    Double, //对子
    Three, //三带    
    Three_Single, //三带一   
    Three_Double, //三代二 
    Four_Single, //四代二     单
    Four_Double, //四代二     对
    Straight_5, //顺子      不能出现 2  5~12 长度 
    Straight_6, //顺子      不能出现 2  5~12 长度 
    Straight_7, //顺子      不能出现 2  5~12 长度 
    Straight_8, //顺子      不能出现 2  5~12 长度 
    Straight_9, //顺子      不能出现 2  5~12 长度 
    Straight_10, //顺子      不能出现 2  5~12 长度 
    Straight_11, //顺子      不能出现 2  5~12 长度 
    Straight_12, //顺子      不能出现 2  5~12 长度
    DoubleStraight_3, //连对  不能出现2
    DoubleStraight_4, //连对  不能出现2
    DoubleStraight_5, //连对  不能出现2
    DoubleStraight_6, //连对  不能出现2
    DoubleStraight_7, //连对  不能出现2
    DoubleStraight_8, //连对  不能出现2
    DoubleStraight_9, //连对  不能出现2
    DoubleStraight_10, //连对  不能出现2
    TwoPlane_Single, //双底飞机   单
    TwoPlane_Double, //双底飞机   对
    ThreePlane_Single, //三底飞机   单
    ThreePlane_Double, //三底飞机   对
    FourPlane_Single, //四底飞机   单
    FourPlane_Double, //四底飞机   对0
    Boom, //炸弹      普通 王炸
}