using UnityEngine;

namespace Sand.Sandbox
{
    public class GasParticle : Particle
    {
        public override ParticleType Type => ParticleType.Gas;

        private Color _color;
        public override Color Color => _color * _brightnessMultiplier;

        public GasParticle(ParticleGrid parentGrid) : base(parentGrid)
        {
            speed = 100.0f;
            _color = Color.magenta * _random.NextFloat(0.35f, 0.6f);
        }

        public override void UpdateState(int x, int y, float deltaTime)
        {
        }

        public override void UpdateBehaviour(int x, int y, float deltaTime)
        {
            // If the particle is at the top of the grid, skip it
            if (IsGridTop(y)) return;

            // First, try to move the particle straight down
            if (IsCellEmpty(x, y + 1) || IsCellSwappable(x, y + 1))
            {
                movementDirection = Vector2.up;

                // Tweak randomly the movement direction to make it move a bit to the sides
                movementDirection.x += _random.NextFloat(-8.0f, 8.0f);
                UpdateMovement(x, y, deltaTime, IsCellSwappable);
            }
            else
            {
                // Next, try to move the particle right or left until it finds an empty cell
                // Its random which direction
                int direction = _random.Next(2) == 0 ? -1 : 1;

                // Keep trying to move until we find an empty cell
                if (IsCellEmpty(x + direction, y) || IsCellSwappable(x + direction, y))
                {
                    movementDirection = new Vector2(direction, 0);
                    UpdateMovement(x, y, deltaTime, IsCellSwappable);
                }
            }
        }

        protected override bool IsCellSwappable(int x, int y)
        {
            return IsCellOfType(x, y, ParticleType.Water) || IsCellEmpty(x, y);
        }
    }
}