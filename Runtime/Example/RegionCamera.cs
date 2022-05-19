using UnityEngine;

namespace ChunkUtilities.Runtime.Example {
  public class RegionCamera : MonoBehaviour {
    [SerializeField] private float _distance = 10;
    [SerializeField] private Transform _target;

    private void Update() {
      if (!_target) {
        return;
      }

      transform.position = _target.position + new Vector3(0, 0.5f, -0.5f) * _distance;
      transform.LookAt(_target);
    }
  }
}