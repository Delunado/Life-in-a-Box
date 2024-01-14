using UnityEngine;

namespace Sand.Sandbox
{
    public class CloudParticle : Particle
    {
        public override ParticleType Type => ParticleType.Cloud;

        private Color _color;
        public override Color Color => _color * _brightnessMultiplier;

        private float _waterGenerationCooldown;
        private float _waterGenerationTimer = 0;

        public CloudParticle(ParticleGrid parentGrid) : base(parentGrid)
        {
            speed = 60.0f;
            _color = Color.white * _random.NextFloat(0.75f, 1.0f);
            _waterGenerationCooldown = _random.NextFloat(30.0f, 60.0f);
        }

        public override void UpdateState(int x, int y, float deltaTime)
        {
        }

        public override void UpdateBehaviour(int x, int y, float deltaTime)
        {
            _waterGenerationTimer += deltaTime;

            if (_waterGenerationTimer >= _waterGenerationCooldown)
            {
                CreateCellAt(x, y, ParticleType.Water);
                return;
            }

            if (IsGridTop(y)) return;

            // First, try to move the particle straight down
            if (IsCellEmpty(x, y + 1) || IsCellSwappable(x, y + 1))
            {
                movementDirection = Vector2.up;
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
            return IsCellEmpty(x, y) || IsCellOfType(x, y, ParticleType.Water) || IsCellOfType(x, y, ParticleType.Gas);
        }
    }
}