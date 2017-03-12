using UnityEngine;
using System.Collections;

public class World {

    public string name;
    public Grid grid;

    public World(string name) {
        this.name = name;
    }

    public void Generate()
    {
        grid = new Grid();
        grid.Generate(100, 100);
    }

    public override string ToString()
    {
        return name;
    } 
}
