using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DrawMeshHandler
{
    private DrawMesh currentDrawMesh;
    private List<DrawMesh> drawMeshes = new List<DrawMesh>();
    private List<int> bucketList = new List<int>();

    private int currentIndex;
    private float currentDistance;

    public event EventHandler OnCompleted;

    private List<Transform> completeTransforms = new List<Transform>();
    private List<Transform> drawObjectsTransforms = new List<Transform>();

    private List<Transform> completeNodeTransforms = new List<Transform>();
    private List<Transform> drawObjectsNodeTransforms = new List<Transform>();

    public DrawMeshHandler()
    {
        List<DrawPoint> points1 = new List<DrawPoint> 
        { 
            new DrawPoint(new Vector2(-0.5f, -1)* 5),
            new DrawPoint(new Vector2(-0.5f, 1)* 5),
            new DrawPoint(new Vector2(0.5f, -1)* 5),
            new DrawPoint(new Vector2(0.5f, 1)* 5) 
        };
        List<DrawPoint> points2 = new List<DrawPoint>
        {
            new DrawPoint(new Vector2(-0.5f, -1)* 5),
            new DrawPoint(new Vector2(-0.5f, 1)* 5),
            new DrawPoint(new Vector2(0.5f, -0f)* 5)
        };
        List<DrawPoint> points3 = new List<DrawPoint>
        {
            new DrawPoint(new Vector2(-0.5f, -1) * 5),
            new DrawPoint(new Vector2(0.5f, -1)* 5),
            new DrawPoint(new Vector2(-0.5f, 1)* 5),
            new DrawPoint(new Vector2(0.5f, 1)* 5)
        };
        List<DrawPoint> points4 = new List<DrawPoint>
        {
            new DrawPoint(new Vector2(-0.5f, -1) * 5),
            new DrawPoint(new Vector2(-0.5f, 1)* 5),
            new DrawPoint(new Vector2(0.5f, 1)* 5),
            new DrawPoint(new Vector2(0.5f, -1)* 5)
        };
        List<DrawPoint> points5 = new List<DrawPoint>
        {
            new DrawPoint(new Vector2(-0.5f, -1) * 5),
            new DrawPoint(new Vector2(0.5f, 1)* 5),
            new DrawPoint(new Vector2(-0.5f, 1)* 5),
            new DrawPoint(new Vector2(0.5f, -1)* 5)
        };

        drawMeshes.Add(CreateDrawMesh(points1));
        drawMeshes.Add(CreateDrawMesh(points2));
        drawMeshes.Add(CreateDrawMesh(points3));
        drawMeshes.Add(CreateDrawMesh(points4));
        drawMeshes.Add(CreateDrawMesh(points5));

        for (int i = 0; i < drawMeshes.Count; i++)
        {
            bucketList.Add(i);
        }

        int n = bucketList.Count;
        
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            int value = bucketList[k];
            bucketList[k] = bucketList[n];
            bucketList[n] = value;
        }
    }

    public DrawMesh CreateDrawMesh(List<DrawPoint> drawPoints)
    {
        List<float> drawDirections = new List<float>();
        List<float> drawDistances = new List<float>();

        for (int i = 0; i + 1 < drawPoints.Count; i++)
        {
            DrawPoint startDrawPoint = drawPoints[i];
            DrawPoint endDrawPoint = drawPoints[i + 1];

            float distance = Vector2.Distance(startDrawPoint.Position, endDrawPoint.Position);
            drawDistances.Add(distance);

            Vector2 dir = endDrawPoint.Position - startDrawPoint.Position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            drawDirections.Add(angle);
        }

        DrawMesh drawMesh = new DrawMesh(drawPoints, drawDirections, drawDistances);
        return drawMesh;
    }

    public void Run()
    {
        if (bucketList.Count == 0)
        {
            for (int i = 0; i < drawMeshes.Count; i++)
            {
                bucketList.Add(i);
            }

            int n = bucketList.Count;

            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                int value = bucketList[k];
                bucketList[k] = bucketList[n];
                bucketList[n] = value;
            }
        }

        int rndNum = bucketList[0];
        currentDrawMesh = drawMeshes[rndNum];

        currentIndex = 0;
        currentDistance = 0f;

        InitDisplay();
    }

    public void Update(float mouseAngle, float distance)
    {
        if (currentDrawMesh == null) return;

        float angleDifference = Mathf.Abs(currentDrawMesh.DrawAngles[currentIndex] - mouseAngle);
        float ang = 90f - angleDifference;
        float magnitude = (ang / 90) * distance;

        if (magnitude < 0f)
        {
            magnitude = 0f;
        }

        magnitude *= 1.25f;

        currentDistance += magnitude;

        if (currentDistance > currentDrawMesh.DrawDistances[currentIndex])
        {
            currentIndex++;

            if (currentIndex + 1 < currentDrawMesh.DrawPoints.Count)
            {
                currentDistance = 0f;
            }
            else
            {
                currentDrawMesh = null;
                bucketList.RemoveAt(0);
                OnCompleted?.Invoke(this, EventArgs.Empty);
                return;
            }
        }

        UpdateDisplay();
    }

    private void InitDisplay()
    {
        for (int i = 0; i + 1 < currentDrawMesh.DrawPoints.Count; i++)
        {
            drawObjectsTransforms.Add(GameObject.Instantiate(ProjectBehaviour.GameManager.BarPrefab, Vector2.zero, Quaternion.identity, Camera.main.transform).GetComponent<Transform>());
            completeTransforms.Add(GameObject.Instantiate(ProjectBehaviour.GameManager.CompleteBarPrefab, Vector2.zero, Quaternion.identity, Camera.main.transform).GetComponent<Transform>());
        }

        for (int i = 0; i < currentDrawMesh.DrawPoints.Count; i++)
        {
            drawObjectsNodeTransforms.Add(GameObject.Instantiate(ProjectBehaviour.GameManager.BarNodePrefab, Vector2.zero, Quaternion.identity, Camera.main.transform).GetComponent<Transform>());
            completeNodeTransforms.Add(GameObject.Instantiate(ProjectBehaviour.GameManager.CompleteBarNodePrefab, Vector2.zero, Quaternion.identity, Camera.main.transform).GetComponent<Transform>());
        }

        for (int i = 0; i < drawObjectsTransforms.Count; i++)
        {
            Transform objTransform = drawObjectsTransforms[i];

            Vector3 pos = currentDrawMesh.DrawPoints[i].Position;
            pos.z = 10f;
            objTransform.localPosition = pos;

            float angle = currentDrawMesh.DrawAngles[i];
            objTransform.rotation = Quaternion.Euler(0f, 0f, angle);

            Vector3 scale = objTransform.localScale;
            scale.x = currentDrawMesh.DrawDistances[i];
            objTransform.localScale = scale;
        }

        for (int i = 0; i < completeTransforms.Count; i++)
        {
            Transform objTransform = completeTransforms[i];

            Vector3 pos = currentDrawMesh.DrawPoints[i].Position;
            pos.z = 10f;
            objTransform.localPosition = pos;

            float angle = currentDrawMesh.DrawAngles[i];
            objTransform.rotation = Quaternion.Euler(0f, 0f, angle);

            Vector3 scale = objTransform.localScale;
            if (currentIndex > i)
            {
                scale.x = currentDrawMesh.DrawDistances[i];
                objTransform.GetComponent<Bar>().Deactivate();
            }
            else if (currentIndex == i)
            {
                scale.x = currentDistance;
                objTransform.GetComponent<Bar>().Activate();
            }
            else
            {
                scale.x = 0f;
                objTransform.GetComponent<Bar>().Deactivate();
            }
            objTransform.localScale = scale;
        }

        for (int i = 0; i < drawObjectsNodeTransforms.Count; i++)
        {
            Transform objTransform1 = drawObjectsNodeTransforms[i]; 
            Transform objTransform2 = completeNodeTransforms[i];

            Vector3 pos = currentDrawMesh.DrawPoints[i].Position;
            pos.z = 10f;
            objTransform1.localPosition = pos;
            objTransform2.localPosition = pos;
            objTransform2.gameObject.SetActive(false);
        }
    }

    private void UpdateDisplay()
    {
        for (int i = 0; i < drawObjectsTransforms.Count; i++)
        {
            Transform objTransform = drawObjectsTransforms[i];

            //Vector2 pos = centerScreen + currentDrawMesh.DrawPoints[i].Position;
            //objTransform.position = pos;

            float angle = currentDrawMesh.DrawAngles[i];
            objTransform.rotation = Quaternion.Euler(0f, 0f, angle);

            Vector3 scale = objTransform.localScale;
            scale.x = currentDrawMesh.DrawDistances[i];
            objTransform.localScale = scale;
        }

        for (int i = 0; i < completeTransforms.Count; i++)
        {
            Transform objTransform = completeTransforms[i];

            //Vector2 pos = centerScreen + currentDrawMesh.DrawPoints[i].Position;
            //objTransform.position = pos;

            float angle = currentDrawMesh.DrawAngles[i];
            objTransform.rotation = Quaternion.Euler(0f, 0f, angle);

            Vector3 scale = objTransform.localScale;
            if (currentIndex > i)
            {
                scale.x = currentDrawMesh.DrawDistances[i];
                objTransform.GetComponent<Bar>().Deactivate();
            }
            else if (currentIndex == i)
            {
                scale.x = currentDistance;
                objTransform.GetComponent<Bar>().Activate();
            }
            else
            {
                scale.x = 0f;
                objTransform.GetComponent<Bar>().Deactivate();
            }
            objTransform.localScale = scale;
        }

        for (int i = 0; i < completeNodeTransforms.Count; i++)
        {
            if (currentIndex >= i)
            {
                completeNodeTransforms[i].gameObject.SetActive(true);
            }
        }
    }

    public void Cancel()
    {
        currentDrawMesh = null;

        currentIndex = 0;
        currentDistance = 0f;

        for (int i = 0;i < drawObjectsTransforms.Count; i++)
        {
            Transform objTransform = drawObjectsTransforms[i];
            GameObject.Destroy(objTransform.gameObject);
        }

        drawObjectsTransforms = new List<Transform>();

        for (int i = 0; i < completeTransforms.Count; i++)
        {
            Transform objTransform = completeTransforms[i];
            GameObject.Destroy(objTransform.gameObject);
        }

        completeTransforms = new List<Transform>();

        for (int i = 0; i < drawObjectsNodeTransforms.Count; i++)
        {
            Transform objTransform = drawObjectsNodeTransforms[i];
            GameObject.Destroy(objTransform.gameObject);
        }

        drawObjectsNodeTransforms = new List<Transform>();

        for (int i = 0; i < completeNodeTransforms.Count; i++)
        {
            Transform objTransform = completeNodeTransforms[i];
            GameObject.Destroy(objTransform.gameObject);
        }

        completeNodeTransforms = new List<Transform>();
    }
}

public class DrawMesh
{
    public List<DrawPoint> DrawPoints { get; private set; }
    public List<float> DrawAngles { get; private set; }
    public List<float> DrawDistances { get; private set; }

    public DrawMesh(List<DrawPoint> drawPoints, List<float> drawAngles, List<float> drawDistances)
    {
        DrawPoints = drawPoints;
        DrawAngles = drawAngles;
        DrawDistances = drawDistances;
    }
}

public struct DrawPoint
{
    public Vector2 Position;

    public DrawPoint(Vector2 position)
    {
        Position = position;
    }
}
