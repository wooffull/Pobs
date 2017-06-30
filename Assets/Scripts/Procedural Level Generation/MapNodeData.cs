using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNodeData {
    public int Width { get; set; }
    public int Height { get; set; }
    public int Id { get; set; }

    public MapNodeData(int width, int height, int id)
    {
        Width = width;
        Height = height;
        Id = id;
    }
}
