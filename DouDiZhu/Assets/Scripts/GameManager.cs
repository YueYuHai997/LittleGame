using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEditor.VersionControl;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnityEngine.Events;
using Task = System.Threading.Tasks.Task;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Card[] PokeCards;

    /// <summary> 玩家列表 </summary>
    public Player[] Players;

    /// <summary> 当前玩家序号 </summary>
    public int PlayerNum;

    public UnityAction<int> PlayerNumChange;

    /// <summary> 地主玩家序号 </summary>
    public int Landlords;

    /// <summary> 本地玩家序号 </summary>
    public int LocalNum;

    /// <summary>  发牌间隔  </summary>
    public float SendCardTime = 0.2f;

    /// <summary>  当前选择卡牌  </summary>
    public List<Card> SelectCard = new List<Card>();

    /// <summary>  是否是多选  </summary>
    public bool MoreSelect = false;


    public Text ShowType;

    /// <summary>
    /// 倒计时
    /// </summary>
    public CountDown CountDonw;

    /// <summary>
    /// 记录每轮最大的牌
    /// </summary>
    public CardType RoundPattern = new CardType();

    /// <summary>
    /// 上一次出牌序号
    /// </summary>
    public int LastPlayerNum;

    /// <summary>
    /// 记录手牌选择的最大牌
    /// </summary>
    public CardType HandlerPattern = new CardType();

    /// <summary>  手牌区  </summary>
    public Transform Hand;

    /// <summary>  隐藏区  </summary>
    public Transform Hiden;

    /// <summary>  隐藏区  </summary>
    public Transform ChuPaiQu;

    /// <summary>  斗地主底率  </summary>
    public int GameRate = 1;

    /// <summary>  当前叫地主=倍率  </summary>
    public int CurrentRate = 0;

    /// <summary>  斗地主底数  </summary>
    public int GameCount = 1;

    public UnityAction<int, int> CallLandlord;

    //上一次叫地主的人
    public int LasterDiZhu;


    private async void Awake()
    {
        Instance = this;
        CancelBtn = this.GetComponentInChildren<CancelBtn>();
        CallLandlord += JiaoDiZhu;
    }

    private void Start()
    {
        BindEvent();

        EventManager.Execute(GameEvent.GameInit);
    }

    public void BindEvent()
    {
        EventManager.RegisterEvent(GameEvent.GameInit, async () =>
        {
            await Task.Delay(500);
            //  初始化牌堆阶段 
            Init();
            //  随机开始 座位号
            LocalNum = 0;
            //  洗牌阶段
            Shuffle(ref PokeCards);
            //  分发手牌阶段  减少三张 作为底牌
            TakeCard();
        });

        EventManager.RegisterEvent(GameEvent.GameShowHand, () =>
        {
            //  展示手牌
            Players[LocalNum].ShowHandler();
        });

        EventManager.RegisterEvent(GameEvent.GameCallLandlord, async () =>
        {
            await Task.Delay(1000);
            //  叫地主阶段
            //首先玩家0先叫地主
            //然后 是玩家1
            //然后是 玩家2
            //最后再次 玩家0 判断当前他是不是预备地主 如果是 直接下一步
            Players[0].CallLandlords(0);
        });

        EventManager.RegisterEvent(GameEvent.GameShowLandlord, async () =>
        {
            Debug.Log($"地主为：{Landlords}");

            await Task.Delay(1000);
            //展示地主牌
            ShowDiZhuCard();
        });

        EventManager.RegisterEvent(GameEvent.GameBeginPlaye, async () =>
        {
            await Task.Delay(1000);
            // 地主出牌阶段
            Players[Landlords].OnPlayerNumChanger(Landlords);
            
            //开启倒计时
            CountDonw.Play(Players[Landlords].CountDownPosition); 
        });

        EventManager.RegisterEvent(GameEvent.GameOver, (int num) =>
        {
            //结束阶段
            Debug.Log($"游戏结束 获胜者为{num}");
        });
        EventManager.RegisterEvent(GameEvent.GameRestart, () =>
        {
            //重新开始游戏
            Debug.Log("游戏结束 获胜者为");
        });
    }

    /// <summary>
    /// 清空出牌区
    /// </summary>
    public void ClearChupaiqu()
    {
        foreach (var VARIABLE in ChuPaiQu.GetComponentsInChildren<Card>())
        {
            VARIABLE.HideCard(Hiden);
        }
    }

    public void JiaoDiZhu(int playernum, int Rate)
    {
        CurrentRate = CurrentRate > Rate ? CurrentRate : Rate;
        CountDonw.Play(Players[playernum].CountDownPosition); 
        if (Rate > 0)
        {
            Landlords = playernum;
            LastPlayerNum = playernum;
            GameRate *= Rate;
        }

        if (playernum + 1 > 2)
        {
            Debug.Log(Landlords);
            if (Landlords != 0)
            {
                Players[0].Finnal(CurrentRate); //询问是否叫地主
            }
            else
            {
                //直接开始展示地主牌
                EventManager.Execute(GameEvent.GameShowLandlord);
            }
        }
        else
        {
            Players[playernum + 1].CallLandlords(CurrentRate);
        }
    }

    public Transform DiZhuPai;


    public EventManager EventManager = new EventManager();


    /// <summary>
    /// 展示地主牌
    /// </summary>
    public async void ShowDiZhuCard()
    {
        int i = 0;
        await Task.Delay(1000);

        foreach (var VARIABLE in DiZhuPai.transform.GetComponentsInChildren<Text>())
        {
            VARIABLE.text = PokeCards[i].count.ToString();
            VARIABLE.text += PokeCards[i].suit;
            i++;
        }

        Debug.Log(Landlords);

        Players[Landlords].Cards.AddRange(PokeCards[..2]);


        if (Landlords == 0)
        {
            //地主增加手牌
            PokeCards[0].ShowCard(GameManager.Instance.Hand);
            PokeCards[1].ShowCard(GameManager.Instance.Hand);
            PokeCards[2].ShowCard(GameManager.Instance.Hand);

            //手牌排序
            HandSort();

            Hand.GetComponent<HorizontalLayoutGroup>().enabled = true;
            await System.Threading.Tasks.Task.Delay(120);
            Hand.GetComponent<HorizontalLayoutGroup>().enabled = false;
        }

        GameManager.Instance.EventManager.Execute(GameEvent.GameBeginPlaye);
    }

    /// <summary> 执行下一个玩家 </summary>
    public void NextPlayer()
    {
        PlayerNum++;
        if (PlayerNum > 2)
            PlayerNum -= 3;

        PlayerNumChange?.Invoke(PlayerNum);
    }

    public void AddSelect(Card addCard)
    {
        SelectCard.Add(addCard);
        ShowType.text = JudgePattern(SelectCard).ToString();

        HandlerPattern.Type = JudgePattern(SelectCard).Item1;
        HandlerPattern.MaxPoint = JudgePattern(SelectCard).Item2;
    }


    public void RemoveSelect(Card subCard)
    {
        SelectCard.Remove(subCard);
        ShowType.text = JudgePattern(SelectCard).ToString();

        HandlerPattern.Type = JudgePattern(SelectCard).Item1;
        HandlerPattern.MaxPoint = JudgePattern(SelectCard).Item2;
    }


    public void ClearSelect()
    {
        SelectCard.Clear();
    }

    public CancelBtn CancelBtn;


    /// <summary>
    /// 出牌
    /// </summary>
    /// <param name="PlayerNum">出牌玩家序号</param>
    /// <param name="cardType">出牌类型及最大牌号</param>
    /// <returns>是否能出牌</returns>
    public bool ChuPai(int PlayerNum, CardType cardType)
    {
        //   如果出的牌没人管 则随意出牌 
        if ((LastPlayerNum == PlayerNum) &&
            cardType.Type != Pattern.None)
        {
            //可以出牌
            RoundPattern = cardType;
            NextPlayer();
            return true;
        }

        //   判断手牌选择的是否大于 本轮 出牌的牌
        if (cardType > RoundPattern)
        {
            //可以出牌
            LastPlayerNum = PlayerNum;
            RoundPattern = cardType;
            NextPlayer();
            return true;
        }

        //不能出牌，无效出牌
        Debug.Log("无法出牌");
        return false;
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
    /// 初始化
    /// </summary>
    private void Init()
    {
        for (int i = 0; i < 3; i++)
        {
            if (i == LocalNum)
                Players[i].Init(i, false);
            else
                Players[i].Init(i, true);
        }
    }

    /// <summary>
    /// 分发手牌
    /// </summary>
    void TakeCard()
    {
        int seq = 0;
        for (int i = 3; i < PokeCards.Length; i++)
        {
            Players[seq].Cards.Add(PokeCards[i]);
            seq = seq + 1 > 2 ? 0 : seq + 1;
        }

        EventManager.Execute(GameEvent.GameShowHand);
    }

    /// <summary>
    /// 洗牌算法
    /// </summary>
    /// <param name="Cards"></param>
    private void Shuffle(ref Card[] Cards)
    {
        for (int i = 0; i < Cards.Count(); i++)
        {
            var temp = Cards[i];
            int index = Random.Range(i, Cards.Count());

            Cards[i] = Cards[index];
            Cards[index] = temp;
        }
    }

    /// <summary>
    /// 牌排序
    /// </summary>
    /// <param name="Cards"></param>
    public void HandSort()
    {
        var result = CardsSort(Hand.GetComponentsInChildren<Card>().ToList());

        int num = 0;
        foreach (var item in result)
        {
            // Hand.GetComponentsInChildren<Card>().ToList().Find(
            //     X => { return X.count == item.count && X.suit == item.suit; }
            // ).transform.SetSiblingIndex(num++);

            Hand.GetComponentsInChildren<Card>().ToList().Find(
                X => { return X.Equals(item); }
            ).transform.SetSiblingIndex(num++);
        }
    }

    public List<Card> CardsSort(List<Card> Cards)
    {
        Cards.Sort((X, Y) => X.count < Y.count ? 1 : -1);
        return Cards;
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
            if (Cards[0] >= Point.Joker && Cards[1] >= Point.Joker)
                return (Pattern.Boom, Point.JokerBig);

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