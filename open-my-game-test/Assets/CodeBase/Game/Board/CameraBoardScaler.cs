using System.Collections;
using System.Collections.Generic;
using Game.Level.Controllers;
using Game.Level.Data;
using UnityEngine;
using Zenject;

public class CameraBoardScaler : MonoBehaviour
{
    [SerializeField] private Transform _gridHolder;
    [SerializeField] private float cellSize = 1f;

    public void AdjustCameraToBoard(LevelAsset levelData)
    {
        float screenWidth = levelData.width * cellSize + cellSize * 2;
        float screenHeight = screenWidth / Camera.main.aspect;
        Vector3 position = gameObject.transform.position;

        if (levelData.width % 2 != 0)
            position.x = cellSize / 2;
        else position.x = 0;

        gameObject.transform.position = position;
        Camera.main.orthographicSize = screenHeight / 2f;
    }
}