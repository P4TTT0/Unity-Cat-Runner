using System.Collections;
using UnityEngine;

namespace CatRunner.Core
{
    //TODO: Manejo de la camara accionados a la escucha del GameManager. Transiciones de diferentes zonas.
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform introPoint;
        [SerializeField] private Transform runnerPoint;

        [SerializeField] private float travelDuration = 1.2f;

        private Coroutine moveRoutine;

        private void OnEnable()
        {
            GameManager.Instance.OnGameStateChanged += HandleState;
        }

        private void OnDisable()
        {
            GameManager.Instance.OnGameStateChanged -= HandleState;
        }

        private void HandleState(GameState state)
        {
            if (state == GameState.FastTravel)
            {
                if (moveRoutine != null)
                    StopCoroutine(moveRoutine);

                moveRoutine = StartCoroutine(MoveCamera(introPoint.position, runnerPoint.position));
            }
        }

        private IEnumerator MoveCamera(Vector3 from, Vector3 to)
        {
            float elapsed = 0f;

            while (elapsed < travelDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / travelDuration;
                transform.position = Vector3.Lerp(from, to, t);
                yield return null;
            }

            transform.position = to;
            GameManager.Instance.SetState(GameState.ArrivalIntro);
        }
    }

}
