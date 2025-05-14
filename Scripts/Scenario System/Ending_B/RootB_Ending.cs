using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class RootB_Ending : MonoBehaviour
{
    /*
     * 1. scColl0 진입 시, 독백 재생 (문에 도달하기 전까지)
     *    - 독백 길이를 측정해, 독백하는 동안 문과 상호작용할 수 없도록 설정
     * 
     * 2. scColl1 진입 시  (문이 열린 직후, 진입 시 닿음)
     *    - Gamigin 1번쨰 Animation 재생
     *    - scBGM FadeIn (볼륨 0.5)
     *    - 독백1 재생
     *    - 독백 길이를 측정해, 독백하는 동안 scColl2의 isTrigger를 false
     * 
     *  3. scColl2 진입 시, Gamigin에게 카메라 고정 (scColl1의 독백이 종료된 후, 유저가 가미긴한테 다가갈 떄)
     * -> Update LookAt, 쿼터니언 Lerp 활용
     *    - scBGM FadeIn (볼륨 1)
     *    - Gamigin 2번째 Animation 재생
     *    - scSfx 재생
     *    - Camera 효과 있으면 더 좋지만, 선택사항
     * 
     *  4. Gamigin 2번째 Animation 재생 종료 시
     *    - Camera FadeOut
     *    - 독백2 재생
     *    "나는 마지막으로 남았다."
     *    "길고 외로운 전쟁이었다. 정말 많은 사람이 희생되었다."
     *    "모든 의미를 잃고, 스러져간 이들이 너무나도 많다."
     *    "어쩌면 나도 그 시체의 산에서 나뒹굴 것이다."
     *    "그것의 눈을 보고 느꼈다. 아마도, 나는 죽을 것이다."
     *    "모든 것은 나에게 가혹했다."
     *    "지금 이 선택의 순간마저도."
     *    "그리고 이건 내가 제일 피하고 싶은 선택이었다."
     *    "하지만, 유일하게 내가 선택한 일이다." 
     *    "지금, 너는 나와 함께 죽는다."
     *    "Gamigin."
     *
     *   - 대기 후, Credit Scene 넘어감
     */

     [SerializeField] private Transform gamiginTarget;
    [SerializeField] private float rotationSpeed = 1.0f;


}
