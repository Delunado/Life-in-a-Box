using System;
using UnityEngine;
using Random = System.Random;

namespace Sand.Sandbox
{
    public abstract class Particle
    {
        public abstract ParticleType Type { get; }
        public abstract Color Color { get; }

        protected float _brightnessMultiplier = 1.0f;

        protected Vector2 movementDirection = Vector2.zero;
        private Vector2 movementAccumulator = Vector2.zero;
        protected float speed = 0.0f;
        
        protected float _temperature = 0.0f;
        public float Temperature => _temperature;
        
        private ParticleGrid _parentGrid;
        protected Random _random;

        protected Particle(ParticleGrid parentGrid)
        {
            _parentGrid = parentGrid;
            _random = new Random();
        }

        public abstract void UpdateState(int x, int y, float deltaTime);
        public abstract void UpdateBehaviour(int x, int y, float deltaTime);

        public void Clean()
        {
            _brightnessMultiplier = 1.0f;
        }

        public virtual void AddTemperature(float degrees)
        {
        }

        protected void UpdateMovement(int currentX, int currentY, float deltaTime, Func<int, int, bool> isSwappable)
        {
            Vector2 totalMovement = movementDirection.normalized * speed * deltaTime;

            int subSteps = Mathf.Max(1, Mathf.CeilToInt(1.0f + 0.01f * speed));

            for (int i = 0; i < subSteps; i++)
            {
                // Distribute the movement across sub-steps
                movementAccumulator += totalMovement / subSteps;

                // Calculate and perform movement for each sub-step
                int moveX = CalculateMovement(ref movementAccumulator.x);
                int moveY = CalculateMovement(ref movementAccumulator.y);

                if (moveX != 0 || moveY != 0)
                {
                    PerformMovement(currentX, currentY, moveX, moveY);
                }
            }

            // Reset the movement direction for the next update
            movementDirection = Vector2.zero;

            // AUXILIAR METHODS
            // Helper method to calculate movement in one dimension
            int CalculateMovement(ref float accumulator)
            {
                if (Mathf.Abs(accumulator) >= 1.0f)
                {
                    int movement = accumulator > 0 ? 1 : -1;
                    accumulator -= movement;
                    return movement;
                }

                return 0;
            }

            // Helper method to perform the particle swap
            void PerformMovement(int currentX, int currentY, int moveX, int moveY)
            {
                int newCellX = currentX + moveX;
                int newCellY = currentY + moveY;

                if (IsCellBounded(newCellX, newCellY) && isSwappable(newCellX, newCellY))
                {
                    SwapParticle(currentX, currentY, newCellX, newCellY);
                }
            }
        }

        protected bool IsCellBounded(int x, int y)
        {
            return _parentGrid.IsBounded(x, y);
        }

        protected bool IsCellOfType(int x, int y, ParticleType type)
        {
            return IsCellBounded(x, y) && _parentGrid.GetTypeAt(x, y) == type;
        }

        protected bool IsGridTop(int y)
        {
            return _parentGrid.IsTop(y);
        }

        protected Particle GetCellAt(int x, int y)
        {
            return !IsCellBounded(x, y) ? null : _parentGrid.GetParticleAt(x, y);
        }

        // Helper method to check if a cell is within bounds and empty
        protected bool IsCellEmpty(int x, int y)
        {
            return IsCellOfType(x, y, ParticleType.Empty);
        }

        protected int NumberOfNeighboursOfType(int x, int y, int area, ParticleType type)
        {
            int count = 0;

            for (int xNeighbour = x - area; xNeighbour <= x + area; xNeighbour++)
            {
                for (int yNeighbour = y - area; yNeighbour <= y + area; yNeighbour++)
                {
                    if (xNeighbour == x && yNeighbour == y) continue;

                    if (IsCellOfType(xNeighbour, yNeighbour, type))
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        protected abstract bool IsCellSwappable(int x, int y);


        // Particles change
        protected void SwapParticle(int x1, int y1, int x2, int y2)
        {
            _parentGrid.SwapParticles(x1, y1, x2, y2);
        }

        protected void ApplyToNeighboursParticles(int fromX, int fromY, int radius, Action<int, int> action)
        {
            for (int x = fromX - radius; x <= fromX + radius; x++)
            {
                for (int y = fromY - radius; y <= fromY + radius; y++)
                {
                    if (fromX == x && fromY == y) continue;

                    if (IsCellBounded(x, y))
                    {
                        action(x, y);
                    }
                }
            }
        }

        protected Particle CreateCellAt(int x, int y, ParticleType particleType)
        {
            return _parentGrid.CreateAt(x, y, particleType);
        }

        public void AddBrightness(float brightness)
        {
            _brightnessMultiplier += brightness;
        }
    }
}