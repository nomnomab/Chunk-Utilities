using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChunkUtilities.Runtime {
  public class RegionLoader : MonoBehaviour {
    [SerializeField] private RegionShape _regionShape;
    [SerializeField] private int _sphereRadius = 1;
    [SerializeField] private Vector3Int _cubeSize = Vector3Int.one;
    [SerializeField] private bool _isSizePerAxis;
    [SerializeField] private bool _isCentered = true;
    [SerializeField] private bool _showDebug;
    [SerializeField] private Vector3Int _debugChunkSize = Vector3Int.one * 16;

    private Vector3Int _lastCoord;
    private List<Vector3Int> _cache;

    private void Awake() {
      _cache = new List<Vector3Int>();
    }

    private void OnDestroy() {
      _cache.Clear();
    }

    private void OnDrawGizmosSelected() {
      if (!_showDebug) {
        return;
      }

      IEnumerable<Vector3Int> coords = Gather(_debugChunkSize);
      Vector3 chunkSize = _debugChunkSize;
      Vector3 offset = chunkSize * 0.5f;

      Gizmos.color = Color.white * 0.5f;
      foreach (var coord in coords) {
        Gizmos.DrawWireCube(coord + offset, chunkSize);
      }
    }

    /// <summary>
    /// Attempts a refresh of the surrounding chunk positions. 
    /// Will quit early if the <paramref name="centerCoord" /> matches the last used one.
    /// </summary>
    /// <param name="centerCoord">The center of the region.</param>
    /// <param name="chunkSize">The size of each chunk.</param>
    /// <param name="result">The supplied result struct.</param>
    public void Refresh(in Vector3 centerCoord, in Vector3Int chunkSize, ref RegionResult result) {
      Vector3Int center = Conversion.WorldToChunk(in centerCoord, in chunkSize);

      if (_lastCoord == center) {
        result.isValid = false;
        return;
      }

      _lastCoord = center;

      RefreshInternal(in chunkSize, ref result);
    }

    /// <summary>
    /// Forces a refresh of the surrounding chunks regardless of if the <paramref name="centerCoord" /> is the same.
    /// </summary>
    /// <param name="centerCoord">The center of the region.</param>
    /// <param name="chunkSize">The size of each chunk.</param>
    /// <param name="result">The supplied result struct.</param>
    public void ForceRefresh(in Vector3 centerCoord, in Vector3Int chunkSize, ref RegionResult result) {
      Vector3Int center = Conversion.WorldToChunk(in centerCoord, in chunkSize);
      _lastCoord = center;

      RefreshInternal(in chunkSize, ref result);
    }

    private void RefreshInternal(in Vector3Int chunkSize, ref RegionResult result) {
      if (!result.isLoaded) {
        result = new RegionResult(null, null);
      }

      IEnumerable<Vector3Int> coords = Gather(chunkSize);

      // get unique instances
      var toLoad = coords.Except(_cache);
      var toUnload = _cache.Except(coords);

      result.toLoad.Clear();
      result.toUnload.Clear();

      result.toLoad.AddRange(toLoad);
      result.toUnload.AddRange(toUnload);

      _cache.Clear();
      _cache.AddRange(coords);

      result.isValid = true;
    }

    private IEnumerable<Vector3Int> Gather(Vector3Int chunkSize) {
      int size;
      int sizeX;
      int sizeY;
      int sizeZ;
      int halfSizeX;
      int halfSizeY;
      int halfSizeZ;

      switch (_regionShape) {
        case RegionShape.Sphere:
          sizeX = _sphereRadius + 1;
          sizeY = _sphereRadius + 1;
          sizeZ = _sphereRadius + 1;

          sizeX = sizeX * 2 + 1;
          sizeY = sizeY * 2 + 1;
          sizeZ = sizeZ * 2 + 1;

          halfSizeX = sizeX / 2;
          halfSizeY = sizeY / 2;
          halfSizeZ = sizeZ / 2;

          size = sizeX * sizeY * sizeZ;

          for (int i = 0; i < size; i++) {
            int x = (i % sizeX) - halfSizeX;
            int y = (i / sizeX % sizeY) - halfSizeY;
            int z = (i / (sizeX * sizeY)) - halfSizeZ;

            int xSqr = x * x;
            int ySqr = y * y;
            int zSqr = z * z;

            if (xSqr + ySqr + zSqr > _sphereRadius) {
              continue;
            }

            yield return new Vector3Int(x * chunkSize.x + _lastCoord.x, y * chunkSize.y + _lastCoord.y, z * chunkSize.z + _lastCoord.z);
          }
          break;
        case RegionShape.Cube:
          sizeX = _cubeSize.x;
          sizeY = _cubeSize.y;
          sizeZ = _cubeSize.z;

          if (_isSizePerAxis) {
            sizeX = sizeX * 2 + 1;
            sizeY = sizeY * 2 + 1;
            sizeZ = sizeZ * 2 + 1;
          }

          halfSizeX = sizeX / 2;
          halfSizeY = sizeY / 2;
          halfSizeZ = sizeZ / 2;

          size = sizeX * sizeY * sizeZ;

          for (int i = 0; i < size; i++) {
            int x = i % sizeX;
            int y = i / sizeX % sizeY;
            int z = i / (sizeX * sizeY);

            if (_isCentered) {
              x -= halfSizeX;
              y -= halfSizeY;
              z -= halfSizeZ;
            }

            yield return new Vector3Int(x * chunkSize.x + _lastCoord.x, y * chunkSize.y + _lastCoord.y, z * chunkSize.z + _lastCoord.z);
          }
          break;
      }
    }
  }
}