using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************************************************************
//Conway's Game of Life ,Implementation by Roman Meyerson ,Aug 2020

namespace GameOfLife.Main
{
    public class GameOfLifeGrid : MonoBehaviour
    {
        // Start is called before the first frame update
        public GameObject PrototypeCell = null;
        public int Width;
        public int Height;
        public int Depth;

        Cell[,,] CellGrid;
        List<Cell> CellsToRevive;
        List<Cell> CellsToKill;

        private float _time = 0.0f;
        public float __interpolationPeriod = 1f;
        protected bool gamePaused;
        //----------------------------------------------------------------------------------------------------
        void Start()
        {
            if (PrototypeCell == null)
            {
                PrototypeCell = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                PrototypeCell.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            }
            if (Width == 0)
            {
                Width = 10; Height = 10; Depth = 10;
            }

            CellGrid = new Cell[Width, Height, Depth];//Allocating Space for Cell Array
            CreateCellGrid(); //Generating Cell Array
            gamePaused = false;
        }
        //----------------------------------------------------------------------------------------------------
        // Update is called once per frame
        void Update()
        {

            HandleClickOnCellEvent();

            //Debug.Log(_time);
            if (!gamePaused)
            {
                {
                    if (_time >= __interpolationPeriod)
                    {
                        _time = 0.0f;

                        Tick();
                    }
                    else
                    {
                        _time += Time.deltaTime;
                    }
                }
            }
        }
        //----------------------------------------------------------------------------------------------------
        //Catching Click Events from Grid's Cells
        private void HandleClickOnCellEvent()
        {
            if (Input.GetMouseButtonDown(0))
            {

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100))
                {
                    //Debug.Log(hit.transform.gameObject.name);
                    int _cellX, _cellY, _cellZ;
                    _cellX = System.Convert.ToInt32(hit.transform.transform.position.x);
                    _cellY = System.Convert.ToInt32(hit.transform.transform.position.y);
                    _cellZ = System.Convert.ToInt32(hit.transform.transform.position.z);

                    //Debug.Log("X:"+ _cellX + ",Y:" + _cellY + ",Z:" + _cellZ);
                    //Debug.Log("Status:" + CellGrid[_cellX, _cellY, _cellZ].IsAlive);
                    if (CellGrid[_cellX, _cellY, _cellZ].IsAlive)
                    {
                        CellGrid[_cellX, _cellY, _cellZ].IsAlive = false;
                        var cellRenderer = hit.transform.gameObject.GetComponent<Renderer>();
                        cellRenderer.material.color = Color.green;
                    }
                    else
                    {
                        CellGrid[_cellX, _cellY, _cellZ].IsAlive = true;
                        var cellRenderer = hit.transform.gameObject.GetComponent<Renderer>();
                        cellRenderer.material.color = Color.blue;
                    }

                }

            }
        }
        //----------------------------------------------------------------------------------------------------
        //Generating  3D Array of Cells
        public void CreateCellGrid()

        {

            for (int x = 0; x < Width; ++x)
            {
                for (int y = 0; y < Height; ++y)
                {
                    for (int z = 0; z < Depth; ++z)
                    {
                        GameObject _gObject;
                        //Get the Renderer component from the new cube
                        var cellRenderer = PrototypeCell.GetComponent<Renderer>();
                        cellRenderer.material.color = Color.green;
                        _gObject = Instantiate(PrototypeCell, new Vector3(x, y, z), Quaternion.identity);

                        CellGrid[x, y, z] = new Cell(_gObject, x, y, z);


                    }
                }
            }
            Debug.Log("Finished Initialization");

        }
        //----------------------------------------------------------------------------------------------------
        //This Code is Applying "Conway's Game of Life" Rules on our Cell Array ,preparing it for Next "Generation"
        public void Tick()
        {
            CellsToRevive = new List<Cell>();
            CellsToKill = new List<Cell>();

            //STEP 1: Iterating through Cells Array and preparing cells for Generational Leap,according to Game's  Rules
            //Selecting Cells and Categorizing them for Revival or Extermination
            for (int m = 0; m < Width; m++)
            {
                for (int n = 0; n < Height; n++)
                {
                    for (int k = 0; k < Depth; k++)
                    {

                        int numNeighbours = GetNumOfNeighbours(m, n, k);
                        if (CellGrid[m, n, k].IsAlive)
                        {
                            if (numNeighbours < 2 || numNeighbours > 9)
                            {
                                CellsToKill.Add(CellGrid[m, n, k]);
                            }
                        }
                        else
                        {
                            if (numNeighbours >= 3 && numNeighbours <= 9)
                            {
                                CellsToRevive.Add(CellGrid[m, n, k]);
                            }
                        }
                    }
                }
            }
            //STEP2: Performing appropriate Changes on Groups  of Cells, Selected in the Previous Step
            foreach (Cell cell in CellsToRevive)
            {
                cell.IsAlive = true;
                var _renderer = cell.gameobject.GetComponent<Renderer>();
                _renderer.material.color = Color.blue;
            }
            foreach (Cell cell in CellsToKill)
            {
                cell.IsAlive = false;
                var _renderer = cell.gameobject.GetComponent<Renderer>();
                _renderer.material.color = Color.green;
            }

        }
        //----------------------------------------------------------------------------------------------------
        //Counting Cell's Neighbours in 3D Grid Space
        private int GetNumOfNeighbours(int x, int y, int z)
        {
            int numNeighbours = 0;

            int minXRange = x > 0 ? -1 : 0;
            int maxXRange = x < Width - 1 ? 1 : 0;
            int minYRange = y > 0 ? -1 : 0;
            int maxYRange = y < Height - 1 ? 1 : 0;
            int minZRange = z > 0 ? -1 : 0;
            int maxZRange = z < Depth - 1 ? 1 : 0;

            for (int i = minXRange; i <= maxXRange; i++)
            {
                for (int j = minYRange; j <= maxYRange; j++)
                {
                    for (int k = minZRange; k <= maxZRange; k++)
                    {
                        if (i == 0 && j == 0 && k == 0)
                        { // Don't Count Ourselves
                            continue;
                        }
                        bool neighbourIsAlive = CellGrid[x + i, y + j, z + k].IsAlive;
                        numNeighbours += neighbourIsAlive ? 1 : 0;
                    }
                }
            }
            return numNeighbours;
        }

        //----------------------------------------------------------------------------------------------------
        public  void RestartGame()
        {
            gamePaused = true;

            for (int x = 0; x < Width; ++x)
            {
                for (int y = 0; y < Height; ++y)
                {
                    for (int z = 0; z < Depth; ++z)
                    {

                        CellGrid[x, y, z].IsAlive = false;
                        var cellRenderer=CellGrid[x, y, z].gameobject.GetComponent<Renderer>();
                        cellRenderer.material.color = Color.green;
                        
                    }
                }
            }
            gamePaused = false;
            Debug.Log("Finished ReInitialization");
        }
        //--------------------------------------------------------------------------------------------------------
        public void ChangeGridSize(int size)
        {
            if (size != Width)//if there is a change 
            {
                gamePaused = true;

                for (int x = 0; x < Width; ++x)
                {
                    for (int y = 0; y < Height; ++y)
                    {
                        for (int z = 0; z < Depth; ++z)
                        {
                            Destroy(CellGrid[x, y, z].gameobject);

                        }
                    }
                }

                Width = size; Height = size; Depth = size;
                CellGrid = new Cell[Width, Height, Depth];//Allocating Space for Cell Array
                CreateCellGrid();//Recreating Cell Array

                gamePaused = false;
            }

        }

        //--------------------------------------------------------------------------------------------------------

          public void  ChangeInterpolationPeriod(float value)
           {

            __interpolationPeriod = value;

           }
 

        public void TogglePause()
        {
            Debug.Log("Paused");
            if (gamePaused)
            {
                gamePaused = false;
            }
            else
            {
                gamePaused = true;
            }
        }


    }

    //************************************************************************************************************
    class Cell
    {
        public int x, y, z;
        public Cell(GameObject gobject, int _x, int _y, int _z)
        {
            gameobject = gobject;
            IsAlive = false;

            x = _x;
            y = _y;
            z = _z;
        }

        public bool IsAlive { get; set; }
        public GameObject gameobject { get; set; }


    }
}