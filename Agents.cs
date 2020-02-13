﻿using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Agents
{
    CommonMethods CM = new CommonMethods();
    SpatialSubdivision Subdivision = new SpatialSubdivision(MultiAgentSystem.AreaMin, MultiAgentSystem.AreaMax, MultiAgentSystem.division);
    CellGrid _grid = new CellGrid(_3DSelfAssembly.AreaMin, _3DSelfAssembly.AreaMax);


    GameObject agent;
    Material material;
    int NumberOfAgents;
    

    public List<Vector3> randomStartPositions = new List<Vector3>();
    public List<GameObject> listAgentsObj = new List<GameObject>();
    public Dictionary<Vector2Int, List<GameObject>> dictionaryAgents = new Dictionary<Vector2Int, List<GameObject>>();




    public List<Agent> listAgents = new List<Agent>();

    // Constructor
    public Agents(GameObject prefab, Material _material, int _NumberOfAgents)
    {
        this.agent = prefab;
        this.material = _material;
        this.NumberOfAgents = _NumberOfAgents;
    }





    ////////////////////////////   CELL PLACEMENT METHODS  ////////////////////////////

    // FillCellsWithAgents: Fills the entire grid of cells with agents
    public List<Agent> FillCellsWithAgents(Cell[,,] cells)
    {
        for (int x = 0; x < _grid.AreaSize; x++)
        {
            for (int y = 0; y < _grid.AreaSize; y++)
            {
                for (int z = 0; z < _grid.AreaSize; z++)
                {
                    Cell currentCell = cells[x, y, z];
                    currentCell.Alive = true;
                    
                    Agent newAgent = new Agent(agent, currentCell.Center);
                    newAgent.Location = currentCell;
                    listAgents.Add(newAgent);
                }
            }
        }
        return listAgents;
    }


    // PlaceAgentsInRowsCells: Places the agents in a 2D regular grid
    public List<Agent> PlaceAgentsInRowsCells(Vector3 placement, Cell[,,] cells)
    {
        Vector3Int location = _grid.GetGridLocation(placement);

        int rows = (int)Mathf.Sqrt(NumberOfAgents) + 1;
        int columns = (int)Mathf.Sqrt(NumberOfAgents) + 1;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (listAgentsObj.Count < NumberOfAgents)
                {
                    Cell cell = cells[i + location.x, location.y, j + location.z];
                    cell.Alive = true;

                    Agent newAgent = new Agent(agent, cell.Center);
                    newAgent.Location = cell;
                    listAgents.Add(newAgent);
                }
            }
        }
        return listAgents;
    }


    // PlaceAgentsInGivenGeometry: Places the agents in a given list of points that forms a 3D shape (only for points inside the lattice)
    public List<Agent> PlaceAgentsInGivenGeometry(List<Vector3> listPositions, Cell[,,] cells, int AreaMin, int AreaMax)
    {
        foreach (Vector3 position in listPositions)
        {
            if (!CM.OutsideBoundaries(position, AreaMin, AreaMax))
            {
                Vector3Int location = _grid.GetGridLocation(position);

                Cell currentCell = cells[location.x, location.y, location.z];
                currentCell.Alive = true;

                Agent newAgent = new Agent(agent, currentCell.Center);
                newAgent.Location = currentCell;
                listAgents.Add(newAgent);
            }
        }
        return listAgents;
    }


    // Random placement (but connected structure)








    ////////////////////////////   AGENT PLACEMENT METHODS (DELETE?)  ////////////////////////////

    // PlaceAgentsInRows: Places the agents in a regular grid
    public List<GameObject> PlaceAgentsInRows(Vector3 placement)
    {
        int rows = (int)Mathf.Sqrt(NumberOfAgents) + 1;
        int columns = (int)Mathf.Sqrt(NumberOfAgents) + 1;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (listAgentsObj.Count < NumberOfAgents)
                {
                    Vector3 _position = new Vector3(i + i, 0, j + j) + placement;
                    GameObject placeAgent = Object.Instantiate(agent, _position, Quaternion.identity);
                    placeAgent.GetComponent<Renderer>().material = material;
                    placeAgent.tag = "Moving";
                    listAgentsObj.Add(placeAgent);
                }
            }
        }
        return listAgentsObj;
    }


    // PlaceAgentsInRowsDictionary: Places agents in a regular grid using a dictionary for spatial subdivision
    public Dictionary<Vector2Int, List<GameObject>> PlaceAgentsInRowsDictionary(Vector3 placement)
    {
        int rows = (int)Mathf.Sqrt(NumberOfAgents) + 1;
        int columns = (int)Mathf.Sqrt(NumberOfAgents) + 1;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (listAgentsObj.Count < NumberOfAgents)
                {
                    Vector3 position = new Vector3(i + i, 0, j + j) + placement;
                    Vector2Int cell = Subdivision.GridLocation(position);
                    GameObject placeAgent = Object.Instantiate(agent, position, Quaternion.identity);
                    placeAgent.GetComponent<Renderer>().material = material;
                    placeAgent.tag = "Moving";
                    listAgentsObj.Add(placeAgent);

                    if (dictionaryAgents.TryGetValue(cell, out List<GameObject> agentsInCell))
                    {
                        agentsInCell.Add(placeAgent);
                    }
                    else
                    {
                        agentsInCell = new List<GameObject>();
                        agentsInCell.Add(placeAgent);
                        dictionaryAgents.Add(cell, agentsInCell);
                    }
                }
            }
        }
        return dictionaryAgents;
    }


    // PlaceAgentsRandomly: Randomly places the agents according to the AgentStartPositions random list
    public List<GameObject> PlaceAgentsRandomly(float AreaMin, float AreaMax, float minDistance)
    {
        foreach (Vector3 position in RandomStartPositions(AreaMin, AreaMax, minDistance))
        {
            GameObject placeAgent = Object.Instantiate(agent, position, Quaternion.identity);
            placeAgent.GetComponent<Renderer>().material = material;
            placeAgent.tag = "Moving";
            listAgentsObj.Add(placeAgent);
        }
        return listAgentsObj;
    }


    // PlaceAgentsRandomlyDictionary: Randomly places the agents according to the AgentStartPositions list, using a dictionary for spatial subdivision
    public Dictionary<Vector2Int, List<GameObject>> PlaceAgentsRandomlyDictionary(float AreaMin, float AreaMax, float minDistance)
    {
        foreach (Vector3 position in RandomStartPositions(AreaMin, AreaMax, minDistance))
        {
            Vector2Int cell = Subdivision.GridLocation(position);
            GameObject placeAgent = Object.Instantiate(agent, position, Quaternion.identity);
            placeAgent.GetComponent<Renderer>().material = material;
            placeAgent.tag = "Moving";
            listAgentsObj.Add(placeAgent);

            if (dictionaryAgents.TryGetValue(cell, out List<GameObject> agentsInCell))
            {
                agentsInCell.Add(placeAgent);
            }
            else
            {
                agentsInCell = new List<GameObject>();
                agentsInCell.Add(placeAgent);
                dictionaryAgents.Add(cell, agentsInCell);
            }
        }
        return dictionaryAgents;
    }


    // RandomStartPositions: List of randomly created vectors (with no overlap/intersections) representing the starting positions of the agents
    public List<Vector3> RandomStartPositions(float AreaMin, float AreaMax, float minDistance)
    {
        int tries = 10000;            ///// tries -> loop failsafe

        while (randomStartPositions.Count < NumberOfAgents && tries-- > 0)
        {
            Vector3 position = CM.RandomPosition(AreaMin, AreaMax);
            if (!randomStartPositions.Any(p => Vector3.Distance(p, position) < minDistance))
                randomStartPositions.Add(position);
        }
        return randomStartPositions;
    }
}