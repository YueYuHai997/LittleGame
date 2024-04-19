using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Threading.Tasks;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using DG.Tweening;

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
    None,           //不成型
    Single,         //单
    Double,         //对子
    Three,          //三带    
    Three_Single,   //三带一   
    Three_Double,   //三代二 
    Four_Single,    //四代二     单
    Four_Double,    //四代二     对
    Straight_5,     //顺子      不能出现 2  5~12 长度 
    Straight_6,     //顺子      不能出现 2  5~12 长度 
    Straight_7,     //顺子      不能出现 2  5~12 长度 
    Straight_8,     //顺子      不能出现 2  5~12 长度 
    Straight_9,     //顺子      不能出现 2  5~12 长度 
    Straight_10,    //顺子      不能出现 2  5~12 长度 
    Straight_11,    //顺子      不能出现 2  5~12 长度 
    Straight_12,    //顺子      不能出现 2  5~12 长度
    DoubleStraight_3 ,   //连对  不能出现2
    DoubleStraight_4 ,   //连对  不能出现2
    DoubleStraight_5 ,   //连对  不能出现2
    DoubleStraight_6 ,   //连对  不能出现2
    DoubleStraight_7 ,   //连对  不能出现2
    DoubleStraight_8 ,   //连对  不能出现2
    DoubleStraight_9 ,   //连对  不能出现2
    DoubleStraight_10 ,  //连对  不能出现2
    TwoPlane_Single,     //双底飞机   单
    TwoPlane_Double,     //双底飞机   对
    ThreePlane_Single,   //三底飞机   单
    ThreePlane_Double,   //三底飞机   对
    FourPlane_Single,    //四底飞机   单
    FourPlane_Double,    //四底飞机   对0
    Boom,                //炸弹      普通 王炸
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Card[] PokeCards = new Card[54];


    public Player PlayerA;
    public Player PlayerB;
    public Player PlayerC;

    /// <summary>  发牌间隔  </summary>
    public float SendCardTime = 0.2f;
    /// <summary>  当前选择卡牌  </summary>
    public List<Card> SelectCard = new List<Card>();

    /// <summary>  是否是多选  </summary>
    public bool MoreSelect = false;

    public Text ShowType;
    
    private (Pattern, Point) SelectPattern;

    /// <summary>  手牌区  </summary>
    public Transform Hand;
    
    /// <summary>  隐藏区  </summary>
    public Transform Hiden;
    
    public void AddSelect(Card addCard)
    {
        SelectCard.Add(addCard);
        ShowType.text = JudgePattern(SelectCard).ToString();
    }

    
    
    public void RemoveSelect(Card subCard)
    {
        SelectCard.Remove(subCard);
        ShowType.text = JudgePattern(SelectCard).ToString();
    }

    
    public void ClearSelect()
    {
        SelectCard.Clear();
    }

    public CancelBtn CancelBtn;


    private void Awake()
    {
        Instance = this;
        CancelBtn = this.GetComponentInChildren<CancelBtn>();
        ShowHandler();
        // Debug.Log(IsContinuity(Point.C3, Point.C5, Point.C4));
        // Debug.Log(IsContinuity(Point.C3, Point.C2, Point.C4));
        // Debug.Log(IsContinuity(Point.C10, Point.C12, Point.C11));
        // Debug.Log(IsContinuity(Point.C12, Point.C13, Point.C1));

        // Test(new List<Card>()
        // {
        //     new Card { count = Point.C3 },
        //     new Card { count = Point.C3 },
        //     new Card { count = Point.C3 },
        //     new Card { count = Point.C4 },
        //     new Card { count = Point.C4 },
        //     new Card { count = Point.C4 },
        //     new Card { count = Point.C5 },
        //     new Card { count = Point.C6 },
        // });
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            MoreSelect = false;
        }
    }

    public void Test(List<Card> Cards)
    {
        var Plane_Base = Cards.GroupBy(X => X.count);
        foreach (var VARIABLE in Plane_Base)
        {
            Debug.Log($"{VARIABLE.Key} : {VARIABLE.Count()}");
        }

        var Plane_Base1 = Cards.GroupBy(X => X.count).Where(_ => _.Count() == 3).Select(_ => _.Key);
        foreach (var VARIABLE in Plane_Base1)
        {
            Debug.Log(VARIABLE);
        }
    }

    /// <summary>
    /// 显示手牌
    /// </summary>
    private async void ShowHandler()
    {
        await Task.Delay(TimeSpan.FromSeconds(1));
        
        foreach (var item in Hiden.GetComponentsInChildren<Card>())
        {
            item.ShowCard(Hand);
            await Task.Delay(TimeSpan.FromSeconds(SendCardTime));
        }

        var horiGroup = Hand.GetComponent<HorizontalLayoutGroup>();
        float value = horiGroup.spacing;
        float myvalue = 0;
        horiGroup.enabled = true;
        DOTween.To(() => value, x => myvalue = x, 0, SendCardTime * 2)
            .OnUpdate(() => { horiGroup.spacing = myvalue; })
            .SetEase(Ease.InFlash)
            .OnComplete(() => {   
                //排序
                HandSort();
                DOTween.To(() => 0, x => myvalue = x, value, SendCardTime * 2)
                .OnUpdate(() => { horiGroup.spacing = myvalue; })
                .SetEase(Ease.InFlash)
                .OnComplete(() =>
                {
                    horiGroup.enabled = false; 
                });});
    }


    /// <summary>
    /// 洗牌算法
    /// </summary>
    /// <param name="Cards"></param>
    private void Shuffle(ref int[] Cards)
    {
        for (int i = 0; i < Cards.Count(); i++)
        {
            int temp = Cards[i];
            int index = Random.Range(i, Cards.Count());

            Cards[i] = Cards[index];
            Cards[index] = temp;
        }
    }

    //手牌排序
    private void HandSort()
    {
        
    }

    /// <summary>
    /// 判断牌形
    /// </summary>
    /// <param name="Cards"></param>
    /// <returns></returns>
    private (Pattern, Point) JudgePattern(List<Card> Cards)
    {
        var foursame = SameAmount(Cards, 4);
        if (foursame.Item1) //存在四个相同
        {
            if (Cards.Count == 4)
                return (Pattern.Boom, Cards[0].count);
            if (Cards.Count == 6)
                return (Pattern.Four_Single, foursame.Item3);
            if (Cards.Count == 8 && Cards.GroupBy(X => X.count).Count() >= 2)
                return (Pattern.Four_Double, foursame.Item3);
        }

        var threesame = SameAmount(Cards, 3);
        if (threesame.Item1) //存在三个相同
        {
            if (threesame.Item2 == 1) //三代
            {
                if (Cards.Count == 3)
                    return (Pattern.Three, threesame.Item3);
                if (Cards.Count == 4)
                    return (Pattern.Three_Single, threesame.Item3);
                if (Cards.Count == 5 && Cards.GroupBy(X => X.count).Count() == 2)
                    return (Pattern.Three_Double, threesame.Item3);
            }
            else if (threesame.Item2 >= 2) //判断飞机
            {
                if (threesame.Item4.Contains(Point.C2))
                {
                    threesame.Item4.Remove(Point.C2);
                    if (MaxContinuity(threesame.Item4) == 3 && Cards.Count == 12)
                        return (Pattern.ThreePlane_Single, threesame.Item4.Max());
                }

                int value = MaxContinuity(threesame.Item4);
                if (value == 2)
                {
                    if (Cards.Count == 8) return (Pattern.TwoPlane_Single, threesame.Item3);
                    if (Cards.Count == 10 && (Cards.GroupBy(X => X.count).Count() == 3 ||
                                              Cards.GroupBy(X => X.count).Count() == 4))
                        return (Pattern.TwoPlane_Double, threesame.Item3);
                }

                if (value == 3)
                {
                    if (Cards.Count == 12) return (Pattern.ThreePlane_Single, threesame.Item3);
                    if (Cards.Count == 15 && (Cards.GroupBy(X => X.count).Count() == 6 ||
                                              Cards.GroupBy(X => X.count).Count() == 5))
                        return (Pattern.ThreePlane_Double, threesame.Item3);
                }

                if (value == 4) 
                {
                    if (Cards.Count == 12) return (Pattern.ThreePlane_Single, threesame.Item3);
                    if (Cards.Count == 16) return (Pattern.FourPlane_Single, threesame.Item4.Max());
                    if (Cards.Count == 20 && (Cards.GroupBy(X => X.count).Count() == 8 ||
                                              Cards.GroupBy(X => X.count).Count() == 7 ||
                                              Cards.GroupBy(X => X.count).Count() == 6))
                        return (Pattern.FourPlane_Double, threesame.Item4.Max());
                }
            }
        }

        if (Cards.Count >= 5) //是否是顺子
        {
            var res1 = Cards.GroupBy(X => X.count).Select(_ => _.Key).ToList();
            if (res1.Count() == Cards.Count && IsContinuity(res1))
            {
                switch (res1.Count())
                {
                    case 5: return (Pattern.Straight_5, res1.Max());
                    case 6: return (Pattern.Straight_6, res1.Max());
                    case 7: return (Pattern.Straight_7, res1.Max());
                    case 8: return (Pattern.Straight_8, res1.Max());
                    case 9: return (Pattern.Straight_9, res1.Max());
                    case 10: return (Pattern.Straight_10, res1.Max());
                    case 11: return (Pattern.Straight_11, res1.Max());
                    case 12: return (Pattern.Straight_12, res1.Max());
                }
            }

            var res2 = Cards.GroupBy(X => X.count).Where(_ => _.Count() == 2).Select(_ => _.Key).ToList();
            if (res2.Count() * 2 == Cards.Count && IsContinuity(res2))
            {
                switch (res2.Count())
                {
                    case 3: return (Pattern.DoubleStraight_3, res2.Max());
                    case 4: return (Pattern.DoubleStraight_4, res2.Max());
                    case 5: return (Pattern.DoubleStraight_5, res2.Max());
                    case 6: return (Pattern.DoubleStraight_6, res2.Max());
                    case 7: return (Pattern.DoubleStraight_7, res2.Max());
                    case 8: return (Pattern.DoubleStraight_8, res2.Max());
                    case 9: return (Pattern.DoubleStraight_9, res2.Max());
                    case 10: return (Pattern.DoubleStraight_10, res2.Max());
                }
            }
        }

        if (Cards.Count == 2)
        {
            if (Cards[0] >= Point.Joker && Cards[1] >= Point.JokerBig)
                return (Pattern.Boom, Cards.Max().count);

            if (Cards[0] == Cards[1])
                return (Pattern.Double, Cards[0].count);
        }

        if (Cards.Count == 1)
            return (Pattern.Single, Cards[0].count);
        return (Pattern.None, Point.None);
    }

    /// <summary>
    /// 判断是否存在 给出相同牌数
    /// </summary>
    /// <param name="Cards"></param>
    /// <param name="existCount"></param>
    /// <returns></returns>
    private (bool, int, Point, List<Point>) SameAmount(List<Card> Cards, int existCount)
    {
        var SameexistCount = //最大相同存在数量；
            Cards.GroupBy(X => X.count).Where(_ => _.Count() == existCount).Select(_ => _.Key).ToList();

        if (SameexistCount.Count > 0)
            return (true, SameexistCount.Count(), SameexistCount.Max(), SameexistCount);
        else
            return (false, SameexistCount.Count(), Point.None, SameexistCount);
    }

    /// <summary>
    /// 比较大小
    /// </summary>
    /// <param name="Type"></param>
    /// <param name="OtherMaxValue"></param>
    /// <param name="SelfMaxValue"></param>
    /// <returns></returns>
    private bool GreatMatch(Pattern Type, Point OtherMaxValue, Point SelfMaxValue)
    {
        return SelfMaxValue > OtherMaxValue;
    }

    /// <summary>
    /// 判断是否连续
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    private bool IsContinuity(params Point[] points)
    {
        if (points.Contains(Point.C2))
        {
            return false;
        }

        Array.Sort(points);
        for (int i = 1; i < points.Length; i++)
        {
            if (points[i] != points[0] + i)
                return false;
        }
        return true;
    }
    
    /// <summary>
    /// 是否连续
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    private bool IsContinuity(List<Point> points)
    {
        if (points.Contains(Point.C2))
        {
            return false;
        }

        points.Sort();
        for (int i = 1; i < points.Count(); i++)
        {
            if (points[i] != points[0] + i)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 最大连续数
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    private int MaxContinuity(List<Point> points)
    {
        points.Sort();

        int Recordresult = 0;
        int result = 1;

        for (int i = 1; i < points.Count(); i++)
        {
            if (points[i] != points[0] + i)
            {
                result = 0;
            }
            else
            {
                result++;
            }

            Recordresult = Recordresult > result ? Recordresult : result;
        }
        
        return Recordresult;
    }
}