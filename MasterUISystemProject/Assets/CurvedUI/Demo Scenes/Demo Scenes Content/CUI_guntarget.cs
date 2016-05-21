using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace CurvedUI
{
    public class CUI_guntarget : MonoBehaviour
    {
        [SerializeField]
        GameObject Model;
        [SerializeField]
        ParticleSystem emitter;
        [SerializeField]
        CanvasGroup cgroup;
        [SerializeField]
        CUI_PerlinNoisePosition perlinPos;
        [SerializeField]
        Text scoreLabel;

        int score = 0;

        bool dead = false;

        public void OnShot()
        {
            if (dead) return;

            dead = true;
            perlinPos.enabled = false;
            emitter.Simulate(1);
            Model.gameObject.SetActive(false);


            StartCoroutine(ShotRoutine());
            score++;
            scoreLabel.text = score.ToString();
        }

        IEnumerator ShotRoutine()
        {
            emitter.Play();
            yield return new WaitForSeconds(2.0f);
            emitter.Stop();
            emitter.time = 0.0f;
            emitter.startLifetime = emitter.startLifetime;
            Respawn();
        }

        void Respawn()
        {
            Model.gameObject.SetActive(true);

            (transform as RectTransform).anchoredPosition = Vector2.zero;
            perlinPos.samplingSpeed += perlinPos.samplingSpeed * 0.1f;

            perlinPos.enabled = true;
            dead = false;
        }


    }
}
