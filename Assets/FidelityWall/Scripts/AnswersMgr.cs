using UnityEngine;
using TMPro;

public class AnswersMgr : MonoBehaviour
{
    public GlobalsMgr g;
    public IndicatorsMgr indicatorsMgr;
    public TMP_Text textAnswer;
    public GameObject marker;
    public GameObject markerOrigin;
    public PersonMgr personMgr;

    void Start()
    {
        g.toolsMgr.EnableDisableRenderer(marker, false);
        g.toolsMgr.EnableDisableRenderer(markerOrigin, false);
    }

    public int FindIndicator()
    {
        RaycastHit hitInfo;
        Vector3 pos = personMgr.transform.position + Vector3.up * .5f;
        markerOrigin.transform.position = pos;
        Vector3 dir = -personMgr.transform.right;
        if (personMgr.ynTurned) dir = personMgr.transform.right;
        int n = 9; // default for when nothing is found
        if (Physics.Raycast(pos, dir, out hitInfo, 10))
        {
            marker.transform.position = hitInfo.point;
            GameObject indicator = hitInfo.collider.gameObject; 
            n = indicator.transform.GetSiblingIndex();
        }
        indicatorsMgr.SetColor(n);
        SetAnswer(n.ToString());
        return n;
    }

    public void SetAnswer(string txt)
    {
        textAnswer.text = txt;
    }
}
