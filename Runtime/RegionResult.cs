using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChunkUtilities.Runtime {
  public struct RegionResult {
    public bool isLoaded;
    public bool isValid;
    public readonly List<Vector3Int> toLoad;
    public readonly List<Vector3Int> toUnload;

    public RegionResult(IEnumerable<Vector3Int> toLoad, IEnumerable<Vector3Int> toUnload) {
      isLoaded = true;
      isValid = true;

      this.toLoad = toLoad?.ToList() ?? new List<Vector3Int>();
      this.toUnload = toUnload?.ToList() ?? new List<Vector3Int>();
    }
  }
}