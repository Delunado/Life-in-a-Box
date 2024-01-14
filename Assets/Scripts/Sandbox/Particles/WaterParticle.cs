using System;
using UnityEngine;
using UnityEngine.Profiling;

namespace Sand.Sandbox
{
    public class WaterParticle : Particle
    {
        public override ParticleType Type => ParticleType.Water;

        private Color _color;
        public override Color Color => _color * _brightnessMultiplier;

        private float _verticalSpeed = 100.0f;
        private float _horizontalSpeed = 25.0f;

        private float _ambientTemperature = 20.0f;
        private float _temperatureLossPerSecond = 1.0f;
        private float _evaporationTemperature = 100.0f;
        private float _heatCapacity;
        private float _baseHeatTransfer = 0.5f;
        private float _attenuationFactor = 0.1f;

        private int _currentX;
        private int _currentY;
        private float _currentDeltaTime;

        private Action<int, int> _updateNeighboursTemperature;

        public WaterParticle(ParticleGrid parentGrid) : base(parentGrid)
        {
            _heatCapacity = _random.NextFloat(0.8f, 1.1f);
            _color = Color.blue * _random.NextFloat(0.8f, 1.0f);

            _updateNeighboursTemperature = UpdateNeighboursTemperature;
        }

        public override void UpdateState(int x, int y, float deltaTime)
        {
            _currentX = x;
            _currentY = y;
            _currentDeltaTime = deltaTime;

            ApplyToNeighboursParticles(x, y, 1, _updateNeighboursTemperature);

            float ambientTemperature = _ambientTemperature - _temperature;
            AddTemperature(ambientTemperature * _temperatureLossPerSecond * deltaTime);
        }

        public override void UpdateBehaviour(int x, int y, float deltaTime)
        {
            if (_temperature >= _evaporationTemperature)
            {
                CreateCellAt(x, y, ParticleType.Cloud);
                return;
            }

            // If the particle is at the bottom of the grid, skip it
            if (y == 0) return;

            // Check if thw down cell is a seed
            if (IsCellOfType(x, y - 1, ParticleType.Seed))
            {
                // Cast particle to plant
                SeedParticle plant = GetCellAt(x, y - 1) as SeedParticle;
                plant?.Watered();
                CreateCellAt(x, y, ParticleType.Empty);
                return;
            }

            // First, try to move the particle straight down
            if (IsCellSwappable(x, y - 1))
            {
                movementDirection = Vector2.down;
                speed = _verticalSpeed;
            }
            // Next, we try diagonal left-right
            else
            {
                int directionDiagonal = _random.Next(0, 2) == 0 ? -1 : 1;

                if (IsCellSwappable(x + directionDiagonal, y - 1))
                {
                    speed = _verticalSpeed;
                    movementDirection = new Vector2(directionDiagonal, -1);
                }
                else // Next, try to move the particle left or right
                {
                    int direction = _random.Next(0, 2) == 0 ? -1 : 1;

                    if (IsCellSwappable(x + direction, y))
                    {
                        speed = _horizontalSpeed;
                        movementDirection = new Vector2(direction, 0);
                    }
                }
            }

            UpdateMovement(x, y, deltaTime, IsCellSwappable);
        }

        protected override bool IsCellSwappable(int x, int y)
        {
            return IsCellEmpty(x, y) || IsCellOfType(x, y, ParticleType.Gas) || IsCellOfType(x, y, ParticleType.Cloud);
        }

        public override void AddTemperature(float degrees)
        {
            _temperature += (degrees * _heatCapacity);

            _temperature = Mathf.Clamp(_temperature, -10.0f, 105.0f);
        }

        private void UpdateNeighboursTemperature(int xNeighbour, int yNeighbour)
        {
            Particle particle = GetCellAt(xNeighbour, yNeighbour);
            float temperatureDifference = _temperature - particle.Temperature;

            if (temperatureDifference == 0.0f) return;


            float manhattanDistance = Mathf.Abs(xNeighbour - _currentX) + Mathf.Abs(yNeighbour - _currentY);
            float heatTransfer = Mathf.Max(_baseHeatTransfer - manhattanDistance * _attenuationFactor, 0) *
                                 temperatureDifference * _currentDeltaTime;

            // Ensure heat transfer is not greater than the available temperature difference
            heatTransfer = Mathf.Min(heatTransfer, Mathf.Abs(temperatureDifference));

            particle.AddTemperature(heatTransfer);
            AddTemperature(-heatTransfer);
        }
    }
}