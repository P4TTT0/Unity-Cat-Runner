using System.Collections.Generic;
using UnityEngine;

namespace CatRunner.Menu
{
    //TODO: Agregar un LineRender entre cada uno de los links para mejorar la apariencia de la cuerda
    public sealed class RopeController : MonoBehaviour
    {
        [SerializeField] private YarnLinker _yarnLinker;
        [SerializeField] private Rigidbody2D _hook;
        [SerializeField] private GameObject _linkPrefab;
        [SerializeField] private int _linkCount = 10;

        private void Start()
        {
            CreateRope();
        }

        private void CreateRope()
        {
            var previousBody = _hook;
            for (int i = 0; i < _linkCount; i++)
            {
                var link = Instantiate(_linkPrefab, transform);
                var joint = link.GetComponent<HingeJoint2D>();
                joint.connectedBody = previousBody;
                previousBody = link.GetComponent<Rigidbody2D>();

                if (i == _linkCount - 1)
                {
                    _yarnLinker.LinkToEndOfRope(previousBody);
                }
            }
        }
    }
}
