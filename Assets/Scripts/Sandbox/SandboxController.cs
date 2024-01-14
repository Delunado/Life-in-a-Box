using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
using Random = UnityEngine.Random;


namespace Sand.Sandbox
{
    [Serializable]
    public class ParticleEditorData
    {
        [SerializeField] private ParticleType _particleType;
        [SerializeField] private Color _textColor;
        [SerializeField] private KeyCode _key;

        public ParticleType ParticleType => _particleType;
        public Color TextColor => _textColor;
        public KeyCode Key => _key;
    }

    public class SandboxController : MonoBehaviour
    {
        [SerializeField] GridVisualizerSandbox gridVisualizerSandbox;
        [SerializeField] TextMeshPro _currentParticleText;
        [SerializeField] TextMeshPro _currentSpawnAreaText;
        [SerializeField] private List<ParticleEditorData> _particleEditorData;

        [SerializeField] private GameObject _menu;

        [SerializeField] private int width = 100;
        [SerializeField] private int height = 100;
        [SerializeField] private float _spawnInterval = 0.1f;
        private float _canSpawnTimer;

        private bool _isMenuOpen = false;

        private int _currentParticleTypeIndex = 0;

        private int _spawnArea = 2;
        private Dictionary<KeyCode, int> _spawnAreaMappings;

        private ParticleGrid particles;

        private void Start()
        {
            gridVisualizerSandbox.Configure(width, height);

            particles = new ParticleGrid(width, height);

            particles.Initialize();

            ConfigureChosenParticle(_currentParticleTypeIndex);
            _currentSpawnAreaText.text = "2x2";

            _menu.SetActive(_isMenuOpen);

            // Initialize Spawn Area Mappings
            _spawnAreaMappings = new Dictionary<KeyCode, int>
            {
                {KeyCode.Alpha1, 1},
                {KeyCode.Alpha2, 2},
                {KeyCode.Alpha3, 3},
                {KeyCode.Alpha4, 4},
                {KeyCode.Alpha5, 5},
                {KeyCode.Alpha6, 6},
                {KeyCode.Alpha7, 7},
                {KeyCode.Alpha8, 8},
                {KeyCode.Alpha9, 9}
            };
        }

        private void Update()
        {
            // Move Particle Type using Mouse Wheel
            if (Input.mouseScrollDelta.y > 0)
            {
                _currentParticleTypeIndex++;
                if (_currentParticleTypeIndex >= _particleEditorData.Count)
                {
                    _currentParticleTypeIndex = 0;
                }

                ConfigureChosenParticle(_currentParticleTypeIndex);
            }

            if (Input.mouseScrollDelta.y < 0)
            {
                _currentParticleTypeIndex--;
                if (_currentParticleTypeIndex < 0)
                {
                    _currentParticleTypeIndex = _particleEditorData.Count - 1;
                }

                ConfigureChosenParticle(_currentParticleTypeIndex);
            }

            // Configure Spawn Area
            foreach (KeyValuePair<KeyCode, int> mapping in _spawnAreaMappings)
            {
                if (Input.GetKeyDown(mapping.Key))
                {
                    _spawnArea = mapping.Value;
                    _currentSpawnAreaText.text = $"{_spawnArea}x{_spawnArea}";
                    break;
                }
            }

            // Configure Particle Input
            foreach (ParticleEditorData particleData in _particleEditorData)
            {
                if (Input.GetKeyDown(particleData.Key))
                {
                    ConfigureChosenParticle(_particleEditorData.FindIndex(x => x.ParticleType == particleData.ParticleType));
                }
            }

            //Menu
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _isMenuOpen = !_isMenuOpen;

                _menu.SetActive(_isMenuOpen);
            }

            // Spawn Input
            CheckInput();
        }

        private void FixedUpdate()
        {
            float deltaTime = Time.fixedDeltaTime;

            particles.Update(deltaTime);

            gridVisualizerSandbox.Visualize(particles, width, height);

            particles.Clean();
        }

        private void CheckInput()
        {
            _canSpawnTimer += Time.deltaTime;
            if (_canSpawnTimer < _spawnInterval) return;

            Vector2 mousePosition = Input.mousePosition;

            // Assuming the texture's resolution matches the screen resolution
            // If not, you would need to scale these coordinates
            int textureX = (int) mousePosition.x;
            int textureY = (int) mousePosition.y;

            // Map the texture coordinates to grid
            int gridX = textureX * width / Screen.width;
            int gridY = textureY * height / Screen.height;

            // Input Mouse
            if (Input.GetMouseButton(0))
            {
                SpawnParticle(gridX, gridY, _spawnArea, _spawnArea, _particleEditorData[_currentParticleTypeIndex].ParticleType);
            }

            if (Input.GetMouseButton(1))
            {
                RemoveParticle(gridX, gridY, _spawnArea, _spawnArea);
            }

            //Input Keyboard
            foreach (ParticleEditorData particleData in _particleEditorData)
            {
                if (Input.GetKey(particleData.Key))
                {
                    SpawnParticle(gridX, gridY, _spawnArea, _spawnArea,
                        _particleEditorData[_currentParticleTypeIndex].ParticleType);
                }
            }
        }

        private void ConfigureChosenParticle(int particleIndex)
        {
            _currentParticleTypeIndex = particleIndex;
            _currentParticleText.text = _particleEditorData[_currentParticleTypeIndex].ParticleType.ToString();
            _currentParticleText.color = _particleEditorData[_currentParticleTypeIndex].TextColor;
        }

        private void SpawnParticle(int gridX, int gridY, int areaMin, int areaMax, ParticleType particleType)
        {
            _canSpawnTimer = 0;

            int area = Random.Range(areaMin, areaMax);
            int startX = gridX - area / 2;
            int startY = gridY - area / 2;

            FillArea(startX, startY, area, particleType);
        }

        private void RemoveParticle(int gridX, int gridY, int areaMin, int areaMax)
        {
            _canSpawnTimer = 0;

            int area = Random.Range(areaMin, areaMax);
            int startX = gridX - area / 2;
            int startY = gridY - area / 2;

            // Clear the area
            RemoveArea(startX, startY, area);
        }

        private void FillArea(int startX, int startY, int area, ParticleType particleType)
        {
            for (int x = startX; x < startX + area; x++)
            {
                for (int y = startY; y < startY + area; y++)
                {
                    if (x >= 0 && x < width && y >= 0 && y < height && particles.GetParticleAt(x, y).Type == ParticleType.Empty)
                    {
                        particles.CreateAt(x, y, particleType);
                    }
                }
            }
        }

        private void RemoveArea(int startX, int startY, int area)
        {
            for (int x = startX; x < startX + area; x++)
            {
                for (int y = startY; y < startY + area; y++)
                {
                    if (x >= 0 && x < width && y >= 0 && y < height && particles.GetParticleAt(x, y).Type != ParticleType.Empty)
                    {
                        particles.CreateAt(x, y, ParticleType.Empty);
                    }
                }
            }
        }
    }
}