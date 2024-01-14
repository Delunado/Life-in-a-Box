using System;
using System.Collections.Generic;

namespace Sand.Sandbox
{
    public class Column
    {
        private int _startX;
        private int _endX;
        private int _height;
        private Random _rng;

        private List<(int, int)> _cells = new();

        public Column(int startX, int endX, int height, Random rng)
        {
            _startX = startX;
            _endX = endX;
            _height = height;

            _rng = rng;

            for (int y = 0; y < _height; y++)
            {
                for (int x = _startX; x <= _endX; x++)
                {
                    _cells.Add((x, y));
                }
            }
        }

        public List<(int, int)> GetCoordinates()
        {
            _cells.ShuffleList(_rng);
            return _cells;
        }
    }
}