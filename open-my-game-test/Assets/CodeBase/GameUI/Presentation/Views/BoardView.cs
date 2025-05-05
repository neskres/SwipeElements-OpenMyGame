using System;
using Game.Board.Data;
using UniRx;
using UnityEngine;

namespace GameUI.Presentation.Views
{
    public class BoardView : MonoBehaviour, IBoardView
    {
        [Header("Layout")] [SerializeField] private Transform _root;
        [SerializeField] private BlockView _prefab;
        [SerializeField] private float _cell = 1f;
        [SerializeField] private Vector2 _origin = new(-3, -3);

        private readonly Subject<(BoardCoordinates, BoardCoordinates)> _swap = new();
        public IObservable<(BoardCoordinates, BoardCoordinates)> OnSwapRequest => _swap;
        public Subject<(BoardCoordinates, BoardCoordinates)> OnSwapRequestSubject => _swap;

        public Transform Root => _root;
        public BlockView BlockPrefab => _prefab;
        public float CellSize => _cell;

        public Vector3 CoordToPos(BoardCoordinates c) =>
            new Vector3(_origin.x + c.X * _cell, _origin.y + c.Y * _cell, 0);

        public BoardCoordinates? ScreenToBoard(Vector2 scr)
        {
            var world = Camera.main.ScreenToWorldPoint(
                new Vector3(scr.x, scr.y, -Camera.main.transform.position.z));
            var local = _root.InverseTransformPoint(world);

            int x = Mathf.FloorToInt((local.x - _origin.x) / _cell + 0.5f);
            int y = Mathf.FloorToInt((local.y - _origin.y) / _cell + 0.5f);

            return new BoardCoordinates(x, y);
        }
    }
}