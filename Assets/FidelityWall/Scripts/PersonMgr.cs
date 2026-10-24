using UnityEngine;

public class PersonMgr : MonoBehaviour
{
    public GlobalsMgr g;
    public GameObject goFrom;
    public GameObject goTo;
    float speed;
    float dir;
    Vector3 posFrom;
    Vector3 posTo;

    float tolerance;
    public bool ynTurned;
    public GameObject leftArm;
    public GameObject rightArm;
    public GameObject leftLeg;
    public GameObject rightLeg;
    float angAnimate;
    float rangeAnimate;
    float speedAnimate;
    public bool ynRotating;
    float angRotate;
    float angRotateStart;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speed = .01f;
        dir = 1;
        tolerance = .02f;
        rangeAnimate = 30;
        speedAnimate = 2;
        posFrom = goFrom.transform.position;
        posTo = goTo.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (g.ynPause) return;
        if (ynRotating)
        {
            UpdateRotate();
        } else
        {
            Move();
            switch (g.fidelityWallMgr.inferenceTraining)
            {
                case FidelityWallMgr.InferenceTraining.infer:
                    g.inferenceMgr.Infer();
                    break;
                case FidelityWallMgr.InferenceTraining.train:
                    g.trainingMgr.RecordTrainingData();
                    break;
            }
        }
        Animate();
    }

    void UpdateRotate()
    {
        if (!ynRotating) return;
        float ang = 180 * Time.deltaTime;
        transform.Rotate(0, ang, 0);
        angRotate += ang;
        CheckRotateEnd();
    }

    void RotateStart()
    {
        ynRotating = true;
        angRotate = 0;
        angRotateStart = transform.localEulerAngles.y;
    }

    void CheckRotateEnd()
    {
        if (angRotate >= 180) {
            ynRotating = false;
            angRotate = 0;
            transform.localEulerAngles = new(0, angRotateStart, 0);
            transform.Rotate(0, 180, 0);
        }
    }

    void Animate()
    {
        float ang = rangeAnimate * Mathf.Sin(angAnimate * Mathf.Deg2Rad);
        ang += Random.Range(-5, 5f);
        leftArm.transform.localEulerAngles = new(-ang, 0, 0);
        rightArm.transform.localEulerAngles = new(ang, 0, 0);
        leftLeg.transform.localEulerAngles = new(ang, 0, 0);
        rightLeg.transform.localEulerAngles = new(-ang, 0, 0);
        angAnimate += speedAnimate;        
    }

    void Move()
    {
        float distFrom = Vector3.Distance(transform.position, posFrom);
        float distTo = Vector3.Distance(transform.position, posTo);
        if (distFrom <= tolerance || distTo <= tolerance)
        {
            transform.Translate(0, 0, -speed * dir);
            ynTurned = !ynTurned;
            RotateStart();
        } else
        {
            transform.Translate(0, 0, speed * dir);
        }
    }
}
