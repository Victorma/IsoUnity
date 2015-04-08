using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class CellProperties : IComparable<CellProperties>
{
    public float height;
    public CellTopType top;
    public int orientation;
    public float width;

    public float uvHash;
    public string textureHash;

    public FaceNoSC[] faces;

    public CellProperties(float height, CellTopType topType, int orientation, float width, FaceNoSC[] faces)
    {
        this.height = height;
        this.top = topType;
        this.orientation = orientation;
        this.width = width;
        this.faces = faces;
    }

    public int CompareTo(CellProperties other)
    {   
        return (fastestComparition(this, other) == 0 && completeComparition(this, other) == 0) ? 0 : 1;
    }

    private static int fastestComparition(CellProperties c1, CellProperties c2)
    {
        return (c1.height == c2.height && c1.top == c2.top && c1.orientation == c2.orientation && c1.width == c2.width && c1.uvHash == c2.uvHash && c1.textureHash == c2.textureHash) ? 0 : 1;
    }


    private static int completeComparition(CellProperties c1, CellProperties c2)
    {
        int pos = 0;
        int end = c1.faces.Length;

        bool stop = c2.faces.Length != end;

        while (!stop && pos != end)
        {
            stop = c1.faces[pos].CompareTo(c2.faces[pos]) != 0;
        }

        return end - pos;
    }

    public override string ToString()
    {
        string thing = height * 1000 + orientation * 10 + (int) top + "_" + uvHash;

        return thing;
    }

    private float old_height;
    private CellTopType old_top;
    private int old_orientation;
    private float old_width;

    private bool first = true;

    public bool Changed
    {
        get{
            if (first)
            {
                old_width = width;
                old_top = top;
                old_orientation = orientation;
                old_height = height;
                first = false;
                return false;
            }

            bool changed = (height != old_height || top != old_top || orientation != old_orientation || width != old_width);
            if (changed)
            {
                old_width = width;
                old_top = top;
                old_orientation = orientation;
                old_height = height;
            }
            return changed;
        }
    }
}