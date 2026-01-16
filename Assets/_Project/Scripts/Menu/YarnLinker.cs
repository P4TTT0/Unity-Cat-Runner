using UnityEngine;

namespace CatRunner.Menu
{
    public class YarnLinker : MonoBehaviour
    {
        [SerializeField] private float _linkOffset = -0.3f;

        public void LinkToEndOfRope(Rigidbody2D endOfRope)
        {
            var joint = gameObject.AddComponent<HingeJoint2D>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedBody = endOfRope;
            joint.anchor = Vector2.zero;
            joint.connectedAnchor = new Vector2(0, _linkOffset);
        }
    }
}
