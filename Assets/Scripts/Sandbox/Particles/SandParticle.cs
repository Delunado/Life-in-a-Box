using UnityEngine;

namespace Sand.Sandbox
{
    public class SandParticle : Particle
    {
        public override ParticleType Type => ParticleType.Sand;

        private Color _color;
        public override Color Color => _color * _brightnessMultiplier;

        public SandParticle(ParticleGrid parentGrid) : base(parentGrid)
        {
            speed = 100.0f;
            _color = Color.yellow * _random.NextFloat(0.6f, 0.8f);
        }

        public override void UpdateState(int x, int y, float deltaTime)
        {
        }

        public override void UpdateBehaviour(int x, int y, float deltaTime)
        {
            // First, try to move the particle straight down
            if (IsCellEmpty(x, y - 1) || IsCellSwappable(x, y - 1))
            {
                movementDirection = Vector2.down;
            }
            // Next, try to move the particle down-right
            else
            {
                // Randomly choose a direction to move
                int direction = _random.Next(2) == 0 ? -1 : 1;

                if (IsCellEmpty(x + direction, y - 1) || IsCellSwappable(x + direction, y - 1))
                {
                    movementDirection = new Vector2(direction, -1);
                }
            }

            UpdateMovement(x, y, deltaTime, IsCellSwappable);
        }

        protected override bool IsCellSwappable(int x, int y)
        {
            return IsCellEmpty(x, y) || IsCellOfType(x, y, ParticleType.Water);
        }
    }
}