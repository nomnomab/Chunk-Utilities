using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChunkUtilities.Runtime.Example {
  public class RegionCenter : MonoBehaviour {
    [Header("Loader Settings")]
    [SerializeField] private Vector3Int _chunkSize = Vector3Int.one * 16;
    [SerializeField] private RegionLoader _loader;

    [Header("Visual Settings")]
    [SerializeField] private Material _testMaterial;
    [SerializeField] private float _destroySpeed = 0.5f;

    [Header("Radial Settings")]
    [SerializeField] private float _forwardSpeed = 1;
    [SerializeField] private float _radius = 8;

    private Dictionary<Vector3Int, GameObject> _chunks;
    private Transform _holderTransform;
    private List<DestroyObj> _toDestroy;
    private RegionResult _resultCache;

    private void Awake() {
      _chunks = new Dictionary<Vector3Int, GameObject>();
      _toDestroy = new List<DestroyObj>();
      _holderTransform = new GameObject("Chunk Holder").transform;
    }

    private void OnDestroy() {
      if (_holderTransform) {
        Destroy(_holderTransform.gameObject);
      }

      foreach (var item in _chunks) {
        Destroy(item.Value);
      }

      _chunks.Clear();

      foreach (var item in _toDestroy) {
        if (!item.target) {
          continue;
        }

        Destroy(item.target.gameObject);
      }

      _toDestroy.Clear();
    }

    private void Update() {
      if (!_loader) {
        return;
      }

      // scale down objects to destroy
      float deltaTime = Time.deltaTime;
      for (int i = 0; i < _toDestroy.Count; i++) {
        var obj = _toDestroy[i];
        obj.t -= deltaTime;

        float scale = obj.t / obj.duration;
        obj.target.localScale = obj.scale * scale;

        if (obj.t > 0) {
          _toDestroy[i] = obj;
          continue;
        }

        Destroy(obj.target.gameObject);
        _toDestroy.RemoveAt(i--);
      }

      // gather result from loader actions
      _loader.Refresh(transform.position, in _chunkSize, ref _resultCache);

      if (!_resultCache.isValid) {
        return;
      }

      // process chunks to load/unload
      Vector3 chunkSize = _chunkSize;
      Vector3 offset = chunkSize * 0.5f;

      foreach (var coord in _resultCache.toUnload) {
        if (!_chunks.TryGetValue(coord, out var clone)) {
          continue;
        }

        _toDestroy.Add(new DestroyObj(_destroySpeed, clone.transform));
        _chunks.Remove(coord);
      }

      foreach (var coord in _resultCache.toLoad) {
        var clone = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var t = clone.transform;

        t.SetParent(_holderTransform, false);
        t.position = coord + offset;
        t.localScale = Vector3.Scale(Vector3.one, _chunkSize);

        clone.GetComponent<MeshRenderer>().sharedMaterial = _testMaterial;

        _chunks[coord] = clone;
      }
    }

    private void LateUpdate() {
      float x = Mathf.Cos(Time.time * _forwardSpeed) * _radius;
      float y = Mathf.Sin(Time.time * _forwardSpeed) * _radius;
      transform.position += new Vector3(x, 0, y) * Time.deltaTime;
    }

    private struct DestroyObj {
      public float duration;
      public float t;
      public Transform target;
      public Vector3 scale;

      public DestroyObj(float duration, Transform target) {
        t = duration;
        scale = target.localScale;

        this.duration = duration;
        this.target = target;
      }
    }
  }
}