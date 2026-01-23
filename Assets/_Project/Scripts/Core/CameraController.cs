using CatRunner.Menu;
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
        [SerializeField] private RopeController ropeController;
        [SerializeField] private SwipeCutter swipeCutter;


        private Coroutine moveRoutine;

        private void Start()
        {
            GameManager.Instance.OnGameStateChanged += HandleState;
        }

        private void OnDisable()
        {
            GameManager.Instance.OnGameStateChanged -= HandleState;
        }

        private void HandleState(GameState state)
        {
            if (moveRoutine != null)
                StopCoroutine(moveRoutine);

            switch (state)
            {
                case GameState.FastTravel:
                    moveRoutine = StartCoroutine(
                        MoveCamera(introPoint.position, runnerPoint.position)
                    );
                    break;

                case GameState.ReturnToMenu:
                    moveRoutine = StartCoroutine(
                        MoveCamera(runnerPoint.position, introPoint.position, false)
                    );
                    break;
            }
        }

        private void OnReturnToMenuFinished()
        {
            ropeController.ResetRope();
            swipeCutter.ResetCutter();

            GameManager.Instance.SetState(GameState.Idle);
        }

        //https://easings.net/#easeInOutCubic
        private float EaseInOutCubic(float t)
        {
            return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
        }

        private IEnumerator MoveCamera(Vector3 from, Vector3 to, bool isStarting = true)
        {
            float elapsed = 0f;

            while (elapsed < travelDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / travelDuration;
                float easedT = EaseInOutCubic(t);
                var interpolation = Vector3.Lerp(from, to, easedT);
                transform.position = new Vector3(interpolation.x, interpolation.y, transform.position.z);
                yield return null;
            }

            transform.position = new Vector3(to.x, to.y, transform.position.z);
            
            if (isStarting)
            {
                GameManager.Instance.SetState(GameState.ArrivalIntro);
            }
            else
            {
                OnReturnToMenuFinished();
            }
        }
    }

}
