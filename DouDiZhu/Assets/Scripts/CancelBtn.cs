using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CancelBtn : MaskableGraphic
{
    protected CancelBtn()
    {
        useLegacyMeshGeneration = false;
    }
    
    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        toFill.Clear();
    }

    public UnityAction Cancle;

    public void CancleBtn()
    {
        Cancle?.Invoke();
        Cancle = null;
        GameManager.Instance.ClearSelect();
    }
}
