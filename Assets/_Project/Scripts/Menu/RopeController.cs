using System.Collections.Generic;
using UnityEngine;

namespace CatRunner.Menu
{
    [DisallowMultipleComponent]
    public sealed class RopeController : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private Transform ropeRoot;
        [SerializeField] private GameObject ropeSegmentPrefab;
        [SerializeField] private int segmentCount = 6;
        [SerializeField] private float segmentSpacing = 0.25f;

        [Header("Linking")]
        [SerializeField] private YarnLinker yarnLinker;

        private readonly List<Rigidbody2D> _segments = new();
        private Transform _hook;

        private void Awake()
        {
            if (ropeRoot.childCount > 0)
                _hook = ropeRoot.GetChild(0);

            BuildRope();
        }

        public void ResetRope()
        {
            ClearRope();
            BuildRope();
        }

        private void BuildRope()
        {
            _segments.Clear();

            Rigidbody2D previous = _hook?.GetComponent<Rigidbody2D>();

            for (int i = 0; i < segmentCount; i++)
            {
                Vector3 pos = ropeRoot.position + Vector3.down * (i * segmentSpacing);
                GameObject segment = Instantiate(ropeSegmentPrefab, pos, Quaternion.identity, ropeRoot);

                Rigidbody2D rb = segment.GetComponent<Rigidbody2D>();
                HingeJoint2D joint = segment.GetComponent<HingeJoint2D>();

                if (previous != null)
                    joint.connectedBody = previous;

                previous = rb;
                _segments.Add(rb);
            }

            if (yarnLinker != null && previous != null)
            {
                yarnLinker.LinkToEndOfRope(previous);
            }
        }

        private void ClearRope()
        {
            if (yarnLinker != null)
            {
                yarnLinker.ClearLink();
            }

            for (int i = ropeRoot.childCount - 1; i >= 0; i--)
            {
                Transform child = ropeRoot.GetChild(i);

                if (child == _hook)
                    continue;

                Destroy(child.gameObject);
            }

            _segments.Clear();
        }
    }
}
