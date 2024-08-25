using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Player : ProjectBehaviour
{
    public static Player Instance { get; private set; }
    private Transform thisTransform;
    private Rigidbody2D rb;
    private Camera mainCam;

    private bool mouse0Down;
    private bool mouse1Down;
    private bool mouse0Up;
    private bool mouse1Up;
    private bool mouse0Held;
    private bool mouse1Held;
    private float realtimeMouse0Held = 0f;
    private float realtimeMouse1Held = 0f;
    private Vector2 prevMousePos = Vector2.zero;
    private Vector2 mousePos;
    private Vector2 fixedTargetPos;
    [SerializeField] private Transform gunTransform;
    private float targetTimeScale = 1f;
    private float targetTimeScaleChangeSpeed = 8f;
    private float minTargetTimeScale = 0.05f;
    private float maxTargetTimeScale = 1f;
    private int numBullets = 100;
    private int maxBullets = 100;
    private int numSpecialBullets = 2;
    private int maxSpecialBullets = 2;

    private bool onGround = false;

    private float refilBulletsDelta = 0f;
    private float maxRefilBullets = 0.5f;
    private float refilSpecialBulletsDelta = 0f;
    private float maxRefilSpecialBullets = 1.3f;

    [SerializeField] private TMP_Text bulletsText;
    [SerializeField] private TMP_Text specialBulletsText;

    private DrawMeshHandler drawMeshHandler;

    [SerializeField] private ParticleSystem gunShot;
    [SerializeField] private ParticleSystem gunShotSpecial;
    [SerializeField] private ParticleSystem gunShotOverheatNormal;
    [SerializeField] private ParticleSystem gunShotOverheatNotNoraml;

    private void Awake()
    {
        Application.targetFrameRate = 120;
        Instance = this;
        thisTransform = transform;
        rb = GetComponent<Rigidbody2D>();
        mainCam = Camera.main;
        drawMeshHandler = new DrawMeshHandler();
        drawMeshHandler.OnCompleted += DrawMeshHandler_OnCompleted;
    }

    private void Start()
    {
        prevMousePos = Input.mousePosition;
        mousePos = Input.mousePosition;
    }

    private void Update()
    {
        float scale = Time.timeScale;
        scale = Mathf.Lerp(scale, targetTimeScale, targetTimeScaleChangeSpeed * Time.unscaledDeltaTime);

        if (scale < minTargetTimeScale + 0.008f)
        {
            scale = minTargetTimeScale;
        }
        else if (scale > maxTargetTimeScale - 0.008f)
        {
            scale = maxTargetTimeScale;
        }

        Time.timeScale = scale;

        Debug.Log("Time scale: " + Time.timeScale);

        GetMousePos();

        SetGunDirection();

        GetMouseInput();

        HandleMouse0Down();
        HandleMouse1Down();

        HandleMouse0Held();
        HandleMouse1Held();

        HandleMouse0Up();
        HandleMouse1Up();

        CheckForGround();

        TryRefillBullets();
        UpdateText();
        Vector3 pos = thisTransform.position;
        pos.z = -10f;
        mainCam.transform.position = pos;
    }

    private void UpdateText()
    {
        bulletsText.text = numBullets.ToString();
        specialBulletsText.text = numSpecialBullets.ToString();
    }

    private void CheckForGround()
    {
        ContactFilter2D contactFilter = new ContactFilter2D();

        RaycastHit2D[] results = new RaycastHit2D[5];

        int i = Physics2D.BoxCast(thisTransform.position, thisTransform.localScale, 0f, -thisTransform.up, contactFilter, results, 1.2f);

        onGround = false;

        for (int j  = 0; j < i; j++)
        {
            if (results[j].transform != thisTransform && results[j].transform != null)
            {
                onGround = true; 
                break;
            }
        }

        //if (results.Any(r => r.transform != thisTransform))
        //{
        //    onGround = true;
        //}
        //else
        //{
        //    onGround = false;
        //}

        Debug.Log("OnGround: " + onGround);
    }

    private void TryRefillBullets()
    {
        if (!onGround)
        {
            refilBulletsDelta = 0f;
            refilSpecialBulletsDelta = 0f;
            return;
        }

        refilBulletsDelta += Time.deltaTime;
        refilSpecialBulletsDelta += Time.deltaTime;

        if (refilBulletsDelta >= maxRefilBullets)
        {
            refilBulletsDelta = 0;
            if (numBullets < maxBullets)
            {
                numBullets++;
            }
        }

        if (refilSpecialBulletsDelta >= maxRefilSpecialBullets)
        {
            refilSpecialBulletsDelta = 0;
            if (numSpecialBullets < maxSpecialBullets)
            {
                numSpecialBullets++;
            }
        }
    }

    private void GetMousePos()
    {
        prevMousePos = mousePos;
        mousePos = Input.mousePosition;
    }

    private void SetGunDirection()
    {
        Vector2 dir;

        if (mouse1Held)
        {
            dir = gunTransform.InverseTransformPoint(mainCam.ScreenToWorldPoint(fixedTargetPos));
        }
        else
        {
            dir = gunTransform.InverseTransformPoint(mainCam.ScreenToWorldPoint(mousePos));
        }

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        gunTransform.Rotate(0f, 0f, angle);
    }

    private void GetMouseInput()
    {
        mouse0Up = Input.GetMouseButtonUp(0);
        mouse1Up = Input.GetMouseButtonUp(1);

        if (Input.GetMouseButton(0))
        {
            mouse0Held = true;
            realtimeMouse0Held += Time.unscaledDeltaTime;

            if (realtimeMouse0Held > 0.75f)
            {
                realtimeMouse0Held = 0.75f;
            }
        }

        if (Input.GetMouseButton(1))
        {
            mouse1Held = true;
            realtimeMouse1Held += Time.unscaledDeltaTime;

            if (realtimeMouse1Held > 5)
            {
                realtimeMouse1Held = 5;
            }
        }

        mouse0Down = Input.GetMouseButtonDown(0);
        mouse1Down = Input.GetMouseButtonDown(1);
    }

    private void HandleMouse0Down()
    {
        if (!mouse0Down) return;

        if (numBullets <= 0)
        {
            return;
        }

        gunShotOverheatNormal.Play();
    }

    private void HandleMouse1Down()
    {
        if (!mouse1Down) return;

        if (numSpecialBullets <= 0)
        {
            return;
        }

        gunShotOverheatNotNoraml.Play();
        Debug.Log("Down");
        drawMeshHandler.Run();

        fixedTargetPos = mousePos;

        targetTimeScale = minTargetTimeScale;
    }

    private void HandleMouse0Held()
    {
        if (!mouse0Held) return;

        if (numBullets <= 0)
        {
            return;
        }

        //ParticleSystem.MinMaxCurve minMaxCurve = gunShotOverheatNormal.emission.rateOverTime;
        //minMaxCurve.constant = realtimeMouse0Held * 10;
        //gunShotOverheatNormal.emission.rateOverTime.constant = minMaxCurve;
    }

    private void HandleMouse1Held()
    {
        if (!mouse1Held) return;

        if (numSpecialBullets <= 0)
        {
            return;
        }

        Debug.Log("Held");

        //rb.AddForce(-rb.velocity / 2f);

        Vector2 dir = mainCam.ScreenToWorldPoint(mousePos) - mainCam.ScreenToWorldPoint(prevMousePos);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        float distance = Vector2.Distance(mainCam.ScreenToWorldPoint(mousePos), mainCam.ScreenToWorldPoint(prevMousePos));

        drawMeshHandler.Update(angle, distance);
    }

    private void HandleMouse0Up()
    {
        if (!mouse0Up) return;

        if (numBullets <= 0)
        {
            return;
        }

        gunShotOverheatNormal.Stop();

        realtimeMouse0Held += 0.65f;

        Shoot();

        rb.AddForce(gunTransform.right * -14f * realtimeMouse0Held, ForceMode2D.Impulse);

        mouse0Held = false;
        realtimeMouse0Held = 0f;
    }

    private void HandleMouse1Up()
    {
        if (!mouse1Up) return;

        //if (numSpecialBullets <= 0)
        //{
        //    return;
        //}

        Debug.Log("Up");

        gunShotOverheatNotNoraml.Stop();

        mouse1Held = false;
        realtimeMouse1Held = 0f;

        drawMeshHandler.Cancel();

        targetTimeScale = maxTargetTimeScale;
    }

    private void Shoot()
    {
        numBullets--;
        Vector2 pos = gunTransform.position + gunTransform.right * 1.2f;
        gunShot.Play();
        var obj = Instantiate(GameManager.BulletPrefab, pos, gunTransform.rotation);
        obj.GetComponent<Bullet>().SetStats(35f * realtimeMouse0Held, 4 * realtimeMouse0Held, false, new Vector3(0.8f, 0.3f, 1f));
    }

    private void ShootSpecial()
    {
        numSpecialBullets--;
        Vector2 pos = gunTransform.position + gunTransform.right * 1.2f;
        gunShotSpecial.Play();
        var obj = Instantiate(GameManager.BulletPrefab, pos, gunTransform.rotation);

        obj.GetComponent<Bullet>().SetStats(40f, 10f, true, new Vector3(1.4f, 0.5f, 1f));
    }

    private void DrawMeshHandler_OnCompleted(object sender, System.EventArgs e)
    {
        drawMeshHandler.Cancel();

        gunShotOverheatNotNoraml.Stop();

        targetTimeScale = maxTargetTimeScale;

        rb.AddForce(gunTransform.right * -14f, ForceMode2D.Impulse);

        ShootSpecial();
    }
}
