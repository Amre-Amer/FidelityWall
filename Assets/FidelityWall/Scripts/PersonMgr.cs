using UnityEngine;
using UnityEngine.Timeline;

public class PersonMgr : MonoBehaviour
{
    public GameObject goFrom;
    public GameObject goTo;
    float speed = .01f;
    float dir = 1;
    Vector3 posFrom;
    Vector3 posTo;
    float distFromTo;

    float tolerance = .01f;

    public IndicatorsMgr indicatorsMgr;
    public GameObject marker;
    public GameObject markerOrigin;
    bool ynTurned;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        posFrom = goFrom.transform.position;
        posTo = goTo.transform.position;
        distFromTo = Vector3.Distance(posFrom, posTo);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        FindIndicator();
    }

    void FindIndicator()
    {
        RaycastHit hitInfo;
        Vector3 pos = transform.position + Vector3.up * .5f;
        markerOrigin.transform.position = pos;
        Vector3 dir = -transform.right;
        if (ynTurned) dir = transform.right;
        int n = -1;
        if (Physics.Raycast(pos, dir, out hitInfo, 10))
        {
            marker.transform.position = hitInfo.point;
            GameObject indicator = hitInfo.collider.gameObject; 
            Debug.Log("hit:" + indicator.name);
            n = indicator.transform.GetSiblingIndex();
        }
        indicatorsMgr.SetColor(n);
    }

    void Move()
    {
        float distFrom = Vector3.Distance(transform.position, posFrom);
        float distTo = Vector3.Distance(transform.position, posTo);
        float diff = distFrom + distTo - distFromTo;
        if (diff > tolerance)
        {
            transform.Rotate(0, 180, 0);
            ynTurned = !ynTurned;
        }
        transform.Translate(0, 0, speed * dir);
    }
}
