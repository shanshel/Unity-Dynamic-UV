using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UvMeshInfo
{

    public string name;
    public List<int> colorIdOnTexture = new List<int>();
    public List<PointGroup> pointGroups = new List<PointGroup>();
}
