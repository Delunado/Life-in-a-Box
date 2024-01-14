using UnityEngine;

namespace Sand.Sandbox
{
    public class WandererParticle : Particle
    {
        public override ParticleType Type => ParticleType.Wanderer;

        private Color _color;
        public override Color Color => _color * _brightnessMultiplier;

        private float _newDirectionTime = 0.01f;
        private float _timeSinceLastMove = 0.0f;
        private Vector2 _currentDirection;

        public WandererParticle(ParticleGrid parentGrid) : base(parentGrid)
        {
            speed = _random.Next(50, 80);
            _color = Color.green * _random.NextFloat(0.75f, 1.0f);

            float angle = _random.NextFloat(0.0f, 360.0f);
            float xDir = Mathf.Cos(angle);
            float yDir = Mathf.Sin(angle);
            _currentDirection = new Vector2(xDir, yDir);
        }

        public override void UpdateState(int x, int y, float deltaTime)
        {
            if (_timeSinceLastMove >= _newDirectionTime)
            {
                _timeSinceLastMove = 0.0f;

                //Get a random direction in a circle, from an angle
                float angle = _random.NextFloat(0.0f, 360.0f);
                float xDir = Mathf.Cos(angle);
                float yDir = Mathf.Sin(angle);
                _currentDirection = new Vector2(xDir, yDir);
            }
            else
            {
                _timeSinceLastMove += deltaTime;
            }
        }

        public override void UpdateBehaviour(int x, int y, float deltaTime)
        {
            movementDirection = _currentDirection;
            UpdateMovement(x, y, deltaTime, IsCellSwappable);
        }

        protected override bool IsCellSwappable(int x, int y)
        {
            return IsCellEmpty(x, y) ||
                   IsCellOfType(x, y, ParticleType.Wanderer) ||
                   IsCellOfType(x, y, ParticleType.Gas) ||
                   IsCellOfType(x, y, ParticleType.Cloud);
        }
    }
}