using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PyramidData", menuName = "ScriptableObjects/PyramidData", order = 1)]
public class PyramidData : ScriptableObject
{
    public List<Vector3> coordinates;
}
