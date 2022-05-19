using UnityEngine;

namespace ChunkUtilities.Runtime {
  public static class Conversion {
    /// <summary>
    /// Flattens a Vector3 into a Vector3Int
    /// </summary>
    /// <param name="vector">The input Vector3.</param>
    /// <returns></returns>
    public static Vector3Int Flatten(in Vector3 vector) {
      return new Vector3Int(
          Mathf.FloorToInt(vector.x),
          Mathf.FloorToInt(vector.y),
          Mathf.FloorToInt(vector.z)
      );
    }

    /// <summary>
    /// Converts from a world coord into a chunk grid coord.
    /// </summary>
    /// <param name="worldCoord">The world coordinates.</param>
    /// <param name="chunkSize">The size of the chunks.</param>
    /// <returns>The chunk grid coordinates.</returns>
    public static Vector3Int WorldToChunk(in Vector3 worldCoord, in Vector3Int chunkSize) {
      return new Vector3Int(
        Mathf.FloorToInt(worldCoord.x / chunkSize.x) * chunkSize.x,
        Mathf.FloorToInt(worldCoord.y / chunkSize.y) * chunkSize.y,
        Mathf.FloorToInt(worldCoord.z / chunkSize.z) * chunkSize.z
      );
    }

    /// <summary>
    /// Converts from a world coord into a local voxel coord.
    /// </summary>
    /// <param name="worldCoord">The world coordinates.</param>
    /// <param name="chunkSize">The size of the chunks.</param>
    /// <returns>The local voxel coordinates within the chunk.</returns>
    public static Vector3Int WorldToVoxel(in Vector3 worldCoord, in Vector3Int chunkSize) {
      Vector3Int chunkCoord = WorldToChunk(in worldCoord, in chunkSize);
      Vector3Int worldCoordMapped = Flatten(in worldCoord);

      return worldCoordMapped - chunkCoord;
    }
  }
}