﻿using AmeisenBotX.Core.Common;
using AmeisenBotX.Core.Movement.Pathfinding.Enums;
using AmeisenBotX.Core.Movement.Pathfinding.Objects;
using AmeisenBotX.Logging;
using AmeisenBotX.Logging.Enums;
using System;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Movement.Pathfinding
{
    public class NavmeshServerPathfindingHandler : CustomTcpClient, IPathfindingHandler
    {
        public NavmeshServerPathfindingHandler(string ip, int port) : base(ip, port)
        {
        }

        public bool CastMovementRay(int mapId, Vector3 start, Vector3 end)
        {
            return false; // SendCastRayRequest(mapId, start, end, MovementType.CastMovementRay);
        }

        public IEnumerable<Vector3> GetPath(int mapId, Vector3 start, Vector3 end)
        {
            return SendPathRequest(mapId, start, end, MovementType.FindPath, out Vector3[] path, PathRequestFlags.CatmullRomSpline) ? path : null;
        }

        public Vector3 GetRandomPoint(int mapId)
        {
            return BuildAndSendRandomPointRequest(mapId, Vector3.Zero, 0f, out Vector3 point) ? point : Vector3.Zero;
        }

        public Vector3 GetRandomPointAround(int mapId, Vector3 start, float maxRadius)
        {
            return BuildAndSendRandomPointRequest(mapId, start, maxRadius, out Vector3 point) ? point : Vector3.Zero;
        }

        public Vector3 MoveAlongSurface(int mapId, Vector3 start, Vector3 end)
        {
            return SendPathRequest(mapId, start, end, MovementType.MoveAlongSurface, out Vector3[] path) ? path[0] : Vector3.Zero;
        }

        private unsafe bool SendPathRequest(int mapId, Vector3 start, Vector3 end, MovementType movementType, out Vector3[] path, PathRequestFlags pathRequestFlags = PathRequestFlags.None)
        {
            if (IsConnected)
            {
                try
                {
                    byte[] response = SendData(new PathRequest(mapId, start, end, pathRequestFlags, movementType), sizeof(PathRequest));

                    if (response != null && response.Length >= sizeof(Vector3))
                    {
                        int nodeCount = response.Length / sizeof(Vector3);

                        path = new Vector3[nodeCount];

                        fixed (byte* pResult = response)
                        fixed (Vector3* pPath = path)
                        {
                            Buffer.MemoryCopy(pResult, pPath, response.Length, response.Length);
                            return true;
                        }
                    }
                }
                catch (Exception e)
                {
                    AmeisenLogger.I.Log("Pathfinding", $"SendPathRequest failed:\n{e}", LogLevel.Error);
                }
            }

            path = null;
            return false;
        }

        private unsafe bool BuildAndSendRandomPointRequest(int mapId, Vector3 start, float maxRadius, out Vector3 point)
        {
            if (IsConnected)
            {
                try
                {
                    byte[] response = SendData(new RandomPointRequest(mapId, start, maxRadius), sizeof(RandomPointRequest));

                    if (response != null && response.Length >= sizeof(Vector3))
                    {
                        fixed (byte* pResult = response)
                        {
                            point = new Span<Vector3>(pResult, 1)[0];
                            return true;
                        }
                    }
                }
                catch (Exception e)
                {
                    AmeisenLogger.I.Log("Pathfinding", $"BuildAndSendRandomPointRequest failed:\n{e}", LogLevel.Error);
                }
            }

            point = Vector3.Zero;
            return false;
        }
    }
}