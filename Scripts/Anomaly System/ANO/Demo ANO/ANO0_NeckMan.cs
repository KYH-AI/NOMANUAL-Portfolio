
using System.Collections;
using UnityEngine;

namespace NoManual.ANO
{
    public class ANO0_NeckMan : ANO_Component
    {
        
        // NPC의 Animator 컴포넌트에 접근하기 위한 변수
        public Animator npcAnimator;

        // 플레이어가 각 Collider에게 접촉하는 걸 감지하기 위한 변수
        [Header("설정")]
        public Collider anoStart;
        public Collider anoMiddle;
        public Collider anoEnd;
        private readonly int lookStart = Animator.StringToHash("Look Start");
        private readonly int runStart = Animator.StringToHash("Run Start");
        
        
        [Header("사운드 소스")]
        [SerializeField] private AudioSource neckManBrokenSfx;
        [SerializeField] private AudioSource neckManRunSfx;
        

        public override void ANO_TriggerCheck(Collider anoTriggerZone)
        {
            if (anoTriggerZone == anoStart)
            {
                anoStart.enabled = false;
                NpcAnimator(lookStart);
                neckManBrokenSfx.Play();
            }
            else if(anoTriggerZone == anoMiddle)
            {
                anoMiddle.enabled = false;
                NpcAnimator(runStart);
                neckManRunSfx.Play();
                StartCoroutine(Fade(neckManRunSfx, 1.5f, 0f));
            }
            else if (anoTriggerZone == anoEnd)
            {
                anoEnd.enabled = false;
                ANO_Clear(true);
                DisableANO();
            }
        }

        /// <summary>
        /// NPC 애니메이션 컨트롤
        /// </summary>
        private void NpcAnimator(int animId)
        {
            npcAnimator.SetTrigger(animId);
        }
        
        
        // neckManRun 사운드를 Fade out하는 코루틴
        public IEnumerator Fade(AudioSource source, float duration, float targetVolume)
        {
            float currentTime = 0;
            float start = source.volume;
            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                source.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
                yield return null;
            }
            yield break;
        }
    }

}
