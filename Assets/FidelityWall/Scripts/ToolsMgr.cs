using UnityEngine;

public class ToolsMgr : MonoBehaviour
{
    public void SetColor(GameObject go, Color color)
    {
        Renderer rend = go.GetComponentInChildren<Renderer>();
        rend.material.color = color;
    }    

    public void EnableDisableRenderer(GameObject go, bool ynOn)
    {
        go.GetComponentInChildren<Renderer>().enabled = ynOn;
    }
}
