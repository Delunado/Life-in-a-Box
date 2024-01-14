using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Profiling;
using Random = System.Random;

namespace Sand.Sandbox
{
    public class ParticleGrid
    {
        private Particle[,] grid;
        private Particle[,] nextGrid;

        private int _width;
        private int _height;
        private float _currentDeltaTime;

        private List<Column> _evenColumns = new();
        private List<Column> _oddColumns = new();
        private int _numberOfColumns = 12;

        private Action<Column> _processColumnState;
        private Action<Column> _processColumnBehaviour;

        Random rng = new();


        public ParticleGrid(int width, int height)
        {
            _width = width;
            _height = height;

            // Divide the grid in columns
            int baseColumnWidth = _width / _numberOfColumns;

            int startX = 0;
            int endX = baseColumnWidth - 1;

            for (int i = 0; i < _numberOfColumns; i++)
            {
                // Create the column
                Column column = new Column(startX, endX, _height, rng);

                if (i % 2 == 0)
                {
                    _evenColumns.Add(column);
                }
                else
                {
                    _oddColumns.Add(column);
                }

                // Update the coordinates
                startX = endX + 1;
                endX += baseColumnWidth;

                if (i == _numberOfColumns - 2)
                {
                    endX = _width - 1;
                }
            }

            // Initialize the grid
            grid = new Particle[width, height];
            nextGrid = new Particle[width, height];

            // Initialize the actions
            _processColumnState = ProcessColumnState;
            _processColumnBehaviour = ProcessColumnBehaviour;
        }


        public void Initialize()
        {
            // Initialize the grid with empty particles
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    grid[x, y] = new EmptyParticle(this);
                }
            }
        }

        public Color GetColorAt(int x, int y)
        {
            return grid[x, y].Color;
        }

        public void Update(float deltaTime)
        {
            _currentDeltaTime = deltaTime;

            Array.Copy(grid, nextGrid, grid.Length);

            _oddColumns.ShuffleList(rng);
            _evenColumns.ShuffleList(rng);

            UpdateState();
            UpdateBehaviour();

            grid = nextGrid;
        }

        private void UpdateState()
        {
            Parallel.ForEach(_oddColumns, _processColumnState);

            Parallel.ForEach(_evenColumns, _processColumnState);
        }

        private void UpdateBehaviour()
        {
            Parallel.ForEach(_oddColumns, _processColumnBehaviour);

            Parallel.ForEach(_evenColumns, _processColumnBehaviour);
        }

        private void ProcessColumnState(Column column)
        {
            List<(int, int)> coordinates = column.GetCoordinates();

            foreach (var (x, y) in coordinates)
            {
                UpdateCellState(x, y, _currentDeltaTime);
            }
        }

        private void ProcessColumnBehaviour(Column column)
        {
            List<(int, int)> coordinates = column.GetCoordinates();

            foreach (var (x, y) in coordinates)
            {
                UpdateCellBehaviour(x, y, _currentDeltaTime);
            }
        }

        private void UpdateCellState(int x, int y, float deltaTime)
        {
            grid[x, y].UpdateState(x, y, deltaTime);
        }

        private void UpdateCellBehaviour(int x, int y, float deltaTime)
        {
            nextGrid[x, y].UpdateBehaviour(x, y, deltaTime);
        }

        public void Clean()
        {
            foreach (Particle particle in grid)
            {
                particle.Clean();
            }
        }

        public Particle CreateAt(int x, int y, ParticleType particleType)
        {
            nextGrid[x, y] = particleType switch
            {
                ParticleType.Empty => new EmptyParticle(this),
                ParticleType.Sand => new SandParticle(this),
                ParticleType.Gas => new GasParticle(this),
                ParticleType.Rock => new RockParticle(this),
                ParticleType.Wanderer => new WandererParticle(this),
                ParticleType.Water => new WaterParticle(this),
                ParticleType.Seed => new SeedParticle(this, false),
                ParticleType.Cloud => new CloudParticle(this),
                ParticleType.Light => new LightParticle(this),
                _ => nextGrid[x, y]
            };

            return nextGrid[x, y];
        }

        public bool IsBounded(int x, int y)
        {
            return x >= 0 && x < _width && y >= 0 && y < _height;
        }

        public ParticleType GetTypeAt(int x, int y)
        {
            return grid[x, y].Type;
        }

        public void SwapParticles(int x1, int y1, int x2, int y2)
        {
            (nextGrid[x1, y1], nextGrid[x2, y2]) = (nextGrid[x2, y2], nextGrid[x1, y1]);
        }

        public bool IsTop(int y)
        {
            return y == _height - 1;
        }

        public Particle GetParticleAt(int x, int y)
        {
            return grid[x, y];
        }
    }
}