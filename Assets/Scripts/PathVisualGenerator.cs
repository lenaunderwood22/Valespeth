using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PathCreation;

public class PathVisualGenerator : MonoBehaviour
{

    public PathCreator MyPath;
    [SerializeField] LineRenderer LineRend;

    public float GapSize = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateLine () {
        //Vector3 curPos = Curve.path.localPoints[Curve.path.localPoints.Length - 1];
        float length = MyPath.path.length;
        List<Vector3> points = new List<Vector3>();

        for (float i = 0; i < length; i += GapSize) {
            points.Add(MyPath.path.GetPointAtDistance(i));
        }

        LineRend.positionCount = points.Count;

        LineRend.SetPositions(points.ToArray());
    }
}
