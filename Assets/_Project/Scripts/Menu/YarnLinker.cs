using UnityEngine;

namespace CatRunner.Menu
{
    public class YarnLinker : MonoBehaviour
    {
        [SerializeField] private float _linkOffset = -0.3f;
        
        private HingeJoint2D _currentJoint;

        public void LinkToEndOfRope(Rigidbody2D endOfRope)
        {
            ClearLink();

            var joint = gameObject.AddComponent<HingeJoint2D>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedBody = endOfRope;
            joint.anchor = Vector2.zero;
            joint.connectedAnchor = new Vector2(0, _linkOffset);

            _currentJoint = joint;
        }

        public void ClearLink()
        {
            if (_currentJoint != null)
            {
                Destroy(_currentJoint);
                _currentJoint = null;
            }
        }
    }
}
