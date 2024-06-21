using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    /// <summary>  玩家手牌 </summary>
    public List<Card> Cards = new List<Card>();

    [SerializeField]
    /// <summary>  当前玩家序号  </summary>
    private int PlayerNum;

    /// <summary>  是否是AI  </summary>
    private bool IsAi;

    /// <summary>  叫地主显示UI  </summary>
    public Text text;

    public Button[] callDiZhu;

    public Button[] chupaiUI;

    public bool IsFinal;

    public Transform CountDownPosition;
    
    private void Start()
    {
        text = this.GetComponentInChildren<Text>();
    }


    /// <summary>
    /// 玩家初始化
    /// </summary>
    /// <param name="num"></param>
    public void Init(int num, bool AI)
    {
        PlayerNum = num;
        IsAi = AI;
        GameManager.Instance.PlayerNumChange += OnPlayerNumChanger;
    }


    //叫地主UI 显示
    public void CallLandlords(int UperRate)
    {
        if (IsAi)
        {
            //随机返回一个结果
            CallAI(UperRate);
        }
        else
        {
            //显示 UI
            CallSelectRate(UperRate);
        }
    }

    /// <summary> 最终叫地主 </summary>
    public void Finnal(int UperRate)
    {
        Debug.Log(11111);
        if (IsAi)
        {
            //随机返回一个结果
            if (Random.Range(0, 1f) > 0.5f)
            {
                GameManager.Instance.LastPlayerNum = PlayerNum;
                GameManager.Instance.Landlords = PlayerNum;

                //展示地主牌
                GameManager.Instance.EventManager.Execute(GameEvent.GameShowLandlord);
            }
        }
        else
        {
            Debug.Log(PlayerNum);
            CallSelectRate(UperRate);
        }
    }

    /// <summary>
    /// AI 决策
    /// </summary>
    /// <param name="UperRate"></param>
    public void CallAI(int UperRate)
    {
        if (Random.Range(0f, 1f) > 0.5f)
        {
            GameManager.Instance.CallLandlord(PlayerNum, 0); //不叫
            text.text = "0";
        }
        else
        {
            if (UperRate != 3)
            {
                int rate = Random.Range(UperRate + 1, 4);
                text.text = rate.ToString();
                GameManager.Instance.CallLandlord(PlayerNum, rate);
            }
            else
            {
                GameManager.Instance.CallLandlord(PlayerNum, 3);
                text.text = "3";
            }
        }
    }

    //根据倍率开启Button选择
    public async void CallSelectRate(int Rate)
    {
        await Task.Delay(100);

        foreach (var VARIABLE in callDiZhu)
        {
            VARIABLE.gameObject.SetActive(true);
        }

        if (Rate >= 1)
            callDiZhu[1].interactable = false;
        if (Rate >= 2)
            callDiZhu[2].interactable = false;
    }


    /// <summary>
    /// 按钮选择倍率
    /// </summary>
    /// <param name="Rate"></param>
    public void CallRate(int Rate)
    {
        foreach (var VARIABLE in callDiZhu)
        {
            VARIABLE.gameObject.SetActive(false);
        }

        if (!IsFinal)
        {
            GameManager.Instance.CallLandlord(PlayerNum, Rate);

            text.text = Rate.ToString();
            IsFinal = true;
        }
        else
        {
            if (Rate != 0)
            {
                text.text = "我抢";

                GameManager.Instance.Landlords = PlayerNum;
                GameManager.Instance.LastPlayerNum = PlayerNum;
                GameManager.Instance.EventManager.Execute(GameEvent.GameShowLandlord);
            }
            else
            {
                text.text = "不抢";
                GameManager.Instance.EventManager.Execute(GameEvent.GameShowLandlord);
            }
        }
    }


    /// <summary>
    /// 出牌UI
    /// </summary>
    public void ChuPai()
    {
        if (GameManager.Instance.ChuPai(PlayerNum, GameManager.Instance.HandlerPattern))
        {
            GameManager.Instance.ClearChupaiqu();
            foreach (var VARIABLE in GameManager.Instance.SelectCard)
            {
                Cards.Remove(VARIABLE);
                VARIABLE.ShowCard(GameManager.Instance.ChuPaiQu);
                VARIABLE.CanClick(false);
            }
            GameManager.Instance.SelectCard.Clear();
            
            if (Cards.Count == 0)
            {
                GameManager.Instance.EventManager.Execute(GameEvent.GameOver, PlayerNum);
            }
        }
    }

    /// <summary>
    /// 跳过出牌
    /// </summary>
    public void Pase()
    {
        GameManager.Instance.NextPlayer(); //默认不出
    }


    /// <summary>
    /// 提示
    /// </summary>
    public void Tips()
    {
        //获取当前牌型
        //有没有炸弹？
        //有没有牌型大于当前牌型的
    }

    
    /// <summary>
    /// 出牌查询
    /// </summary>
    /// <param name="num"></param>
    public void OnPlayerNumChanger(int num)
    {
        if (num == PlayerNum)
        {
            if (IsAi)
            {
                if (GameManager.Instance.LastPlayerNum == PlayerNum)
                {
                    CardType cardType = new CardType();
                    
                    GameManager.Instance.CardsSort(Cards);
                    cardType.Type = Pattern.Single;
                    cardType.MaxPoint = Cards[^1].count;
                    
                    
                    if (GameManager.Instance.ChuPai(PlayerNum, cardType))
                    {
                        GameManager.Instance.ClearChupaiqu();

                        Cards[^1].ShowCard(GameManager.Instance.ChuPaiQu);
                        Cards[^1].CanClick(false);
                        
                        Cards.RemoveAt(Cards.Count - 1);
                        GameManager.Instance.SelectCard.Clear();
                    }
                }
                else
                {
                    text.text = "不出";
                } 
            }
            else
            {
                //显示出牌UI
                foreach (var VARIABLE in chupaiUI)
                {
                    VARIABLE?.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            //隐藏出牌ID

            foreach (var VARIABLE in chupaiUI)
            {
                VARIABLE?.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// 显示手牌
    /// </summary>
    public async void ShowHandler()
    {
        await Task.Delay(TimeSpan.FromSeconds(1));

        foreach (var item in Cards)
        {
            item.ShowCard(GameManager.Instance.Hand);
            await Task.Delay(TimeSpan.FromSeconds(GameManager.Instance.SendCardTime));
        }

        var horiGroup = GameManager.Instance.Hand.GetComponent<HorizontalLayoutGroup>();
        float value = horiGroup.spacing;
        float myvalue = 0;
        horiGroup.enabled = true;
        DOTween.To(() => value, x => myvalue = x, 0, GameManager.Instance.SendCardTime * 2)
            .OnUpdate(() => { horiGroup.spacing = myvalue; })
            .SetEase(Ease.InFlash)
            .OnComplete(() =>
            {
                //排序
                GameManager.Instance.HandSort();
                DOTween.To(() => 0, x => myvalue = x, value, GameManager.Instance.SendCardTime * 2)
                    .OnUpdate(() => { horiGroup.spacing = myvalue; })
                    .SetEase(Ease.InFlash)
                    .OnComplete(() =>
                    {
                        horiGroup.enabled = false;

                        GameManager.Instance.EventManager.Execute(GameEvent.GameCallLandlord);
                    });
            });
    }
}