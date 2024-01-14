using UnityEngine;

namespace Sand.Sandbox
{
    public class LightParticle : Particle
    {
        public override ParticleType Type => ParticleType.Light;

        private Color _color;
        public override Color Color => _color * _brightnessMultiplier;

        private float _heat = 15.0f;
        private int _heatRadius = 8;
        private int _lightRadius = 8;

        public LightParticle(ParticleGrid parentGrid) : base(parentGrid)
        {
            _color = new Color(0.886f, 0.84f, 0.087f) * _random.NextFloat(0.8f, 1.0f);
        }

        public override void UpdateState(int x, int y, float deltaTime)
        {
            ApplyToNeighboursParticles(x, y, _heatRadius, (xNeighbour, yNeighbour) =>
            {
                if (xNeighbour == x && yNeighbour == y)
                    return; // Skip the current particle

                Particle particle = GetCellAt(xNeighbour, yNeighbour);

                float distance = Mathf.Abs(xNeighbour - x) + Mathf.Abs(yNeighbour - y); // Manhattan distance
                float decayFactor = 0.1f; // Adjust this value based on your simulation requirements
                float heatTransfer = (_heat / (distance + decayFactor)) * deltaTime; // Simple linear decay

                particle.AddTemperature(heatTransfer);
            });

            ApplyToNeighboursParticles(x, y, _lightRadius, (xNeighbour, yNeighbour) =>
            {
                // Optimized distance calculation
                float deltaX = xNeighbour - x;
                float deltaY = yNeighbour - y;
                float distanceSquared = deltaX * deltaX + deltaY * deltaY;

                // Check if the neighbour is within the circular light radius
                if (distanceSquared <= _lightRadius * _lightRadius)
                {
                    Particle particle = GetCellAt(xNeighbour, yNeighbour);
                    if (particle == null) return; // Skip if no particle

                    // Adjusted brightness calculation
                    float brightness = 2.0f / (1.0f + distanceSquared);
                    brightness = Mathf.Clamp(brightness, 0.1f, 0.5f);

                    particle.AddBrightness(brightness);
                }
            });
        }

        public override void UpdateBehaviour(int x, int y, float deltaTime)
        {
        }

        protected override bool IsCellSwappable(int x, int y)
        {
            return false;
        }
    }
}