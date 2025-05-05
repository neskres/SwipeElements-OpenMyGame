using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Board.Data;
using UniRx;
using UnityEngine;

namespace GameUI.Presentation.Views
{
    public interface IBoardView
    {
        public IObservable<(BoardCoordinates from, BoardCoordinates to)> OnSwapRequest { get; }

        public Transform Root { get; }
        public BlockView BlockPrefab { get; }
        public float CellSize { get; }

        public Vector3 CoordToPos(BoardCoordinates c);
        public BoardCoordinates? ScreenToBoard(Vector2 scr);

        public Subject<(BoardCoordinates, BoardCoordinates)> OnSwapRequestSubject { get; }
    }
}