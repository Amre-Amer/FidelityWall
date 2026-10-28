using UnityEngine;
using UnityEngine.InputSystem;

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
    public GameObject body;
    public GameObject leftArm;
    public GameObject rightArm;
    public GameObject leftLeg;
    public GameObject rightLeg;
    float angAnimate;
    float rangeAnimate;
    float speedAnimate;
    bool ynRotating;
    float angRotate;
    float angRotateStart;
    int jogDirection;
    bool ynJogPause;
    public bool ynRotatePerson;
    float angRotatePersonDelta;
    public bool ynJumpPerson;
    float speedJump;
    float heightJump;
    float angJump;

    void Start()
    {
        speed = .01f;
        jogDirection = 1;
        dir = 1;
        tolerance = .02f;
        rangeAnimate = 30;
        speedAnimate = 2;
        posFrom = goFrom.transform.position;
        posTo = goTo.transform.position;
        ynRotatePerson = true;
        angRotatePersonDelta = 2;
        ynJumpPerson = true;
        speedJump = 20;
        heightJump = .25f;
    }

    void Update()
    {
        CheckKeyboard();
        if (g.ynPause) return;
        if (ynRotating)
        {
            UpdateRotate();
        } else
        {
            Move();
            switch (g.fidelityWallMgr.trainingInference)
            {
                case FidelityWallMgr.TrainingInference.train:
                    g.trainingMgr.RecordTrainingData();
                    break;
                case FidelityWallMgr.TrainingInference.infer:
                    g.inferenceMgr.Infer();
                    break;
            }
        }
        Animate();
        Rotate();
        Jump();
    }

    void Jump()
    {
        if (!ynJumpPerson || ynRotating) return;
        float y = heightJump / 2 + heightJump / 2 * Mathf.Cos(angJump * Mathf.Deg2Rad);
        Vector3 pos = body.transform.position;
        pos.y = y;
        body.transform.position = pos;
        angJump += speedJump;
    }

    void Rotate()
    {
        if(!ynRotatePerson || ynRotating) return;
        body.transform.Rotate(0, angRotatePersonDelta, 0);        
    }

    void CheckKeyboard()
    {
        if (Keyboard.current == null) return; 
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            jogDirection = -1;
        }
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            jogDirection = 1;
        }
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ynJogPause = !ynJogPause;
        }
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
        if (g.fidelityWallMgr.trainingInference == FidelityWallMgr.TrainingInference.train)
        {
            ang += Random.Range(-5, 5f);
        }
        leftArm.transform.localEulerAngles = new(-ang, 0, 0);
        rightArm.transform.localEulerAngles = new(ang, 0, 0);
        leftLeg.transform.localEulerAngles = new(ang, 0, 0);
        rightLeg.transform.localEulerAngles = new(-ang, 0, 0);
        angAnimate += speedAnimate;        
    }

    void Move()
    {
        if (ynJogPause) return;
        float distFrom = Vector3.Distance(transform.position, posFrom);
        float distTo = Vector3.Distance(transform.position, posTo);
        if (distFrom <= tolerance || distTo <= tolerance)
        {
            transform.Translate(0, 0, -speed * dir * jogDirection);
            ynTurned = !ynTurned;
            RotateStart();
        } else
        {
            transform.Translate(0, 0, speed * dir * jogDirection);
        }
    }
}
