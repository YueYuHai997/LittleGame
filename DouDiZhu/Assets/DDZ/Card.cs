using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.UI;

public class Card : MonoBehaviour,IDragHandler,IEndDragHandler,IPointerDownHandler,IBeginDragHandler,IPointerEnterHandler
{
    /// <summary> 花色 </summary>
    public Type suit;

    /// <summary> 点数 </summary>
    public Point count;
    
    /// <summary> 卡片模型  </summary>
    public Image CardMode; 

    private Vector2 DragOffset;
    private Vector2 BeginDragPosition;

    public bool IsSelect;
    
    // public override bool Equals(object other)
    // {
    //     if (other is null || other is not Card) return false;
    //     return (other as Card).count == count;
    // }
    
    public void HideCard(Transform Parents)
    {
        this.transform.parent = Parents;
        this.transform.localPosition = Vector3.zero;
    }

    public void ShowCard(Transform Parents)
    {
        this.transform.parent = Parents;
        this.transform.localPosition = Vector3.zero;
    }

    private void Start()
    {
        this.name = count.ToString();
        HideCard(GameManager.Instance.Hiden);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.Instance.MoreSelect)
        {
            if (!IsSelect)
            {
                IsSelect = true;
                GameManager.Instance.AddSelect(this);
                CardMode.transform.DOLocalMoveY(100, 0.1f).SetEase(Ease.InFlash);
                GameManager.Instance.CancelBtn.Cancle += () =>
                {
                    IsSelect = false;
                    CardMode.transform.DOLocalMoveY(0, 0.1f).SetEase(Ease.InFlash);
                };
            }
            else
            {
                IsSelect = false;
                GameManager.Instance.RemoveSelect(this);
                CardMode.transform.DOLocalMoveY(0, 0.1f).SetEase(Ease.InFlash);
                GameManager.Instance.CancelBtn.Cancle -= () =>
                {
                    CardMode.transform.DOLocalMoveY(0, 0.1f).SetEase(Ease.InFlash);
                };
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameManager.Instance.MoreSelect = true;
        if (!IsSelect)
        {
            IsSelect = true;
            GameManager.Instance.AddSelect(this);
            CardMode.transform.DOLocalMoveY(100, 0.1f).SetEase(Ease.InFlash);
            GameManager.Instance.CancelBtn.Cancle += () =>
            {
                IsSelect = false;
                CardMode.transform.DOLocalMoveY(0, 0.1f).SetEase(Ease.InFlash);
            };
        }
        else
        {
            IsSelect = false;
            GameManager.Instance.RemoveSelect(this);
            CardMode.transform.DOLocalMoveY(0, 0.1f).SetEase(Ease.InFlash);
            GameManager.Instance.CancelBtn.Cancle -= () =>
            {
                CardMode.transform.DOLocalMoveY(0, 0.1f).SetEase(Ease.InFlash);
            };
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector2 transform1 = this.transform.position;
        DragOffset = transform1 - eventData.position;
        BeginDragPosition = transform1;
    }



    public void OnEndDrag(PointerEventData eventData)
    {
        this.transform.position = BeginDragPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //this.transform.position = eventData.position + DragOffset;
    }

    public static bool operator >(Card a, Card b)
    {
        return a.count > b.count;
    }

    public static bool operator <(Card a, Card b)
    {
        return a.count < b.count;
    }

    public static bool operator >(Card a, Point b)
    {
        return a.count > b;
    }

    public static bool operator <(Card a, Point b)
    {
        return a.count < b;
    }

    public static bool operator ==(Card a, Card b)
    {
        return a.count == b.count;
    }

    public static bool operator !=(Card a, Card b)
    {
        return a.count == b.count;
    }

    public static bool operator ==(Card a, Point b)
    {
        return a.count == b;
    }

    public static bool operator !=(Card a, Point b)
    {
        return a.count == b;
    }


    public static bool operator >=(Card a, Card b)
    {
        return a.count >= b.count;
    }

    public static bool operator <=(Card a, Card b)
    {
        return a.count <= b.count;
    }

    public static bool operator >=(Card a, Point b)
    {
        return a.count >= b;
    }

    public static bool operator <=(Card a, Point b)
    {
        return a.count <= b;
    }
}