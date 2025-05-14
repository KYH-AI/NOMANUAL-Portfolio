using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Signature : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private AudioSource signAudioSource;
    [SerializeField] private AudioClip[] signAudioClips;
    [SerializeField] private Transform signRoot;
    [SerializeField] private GameObject signLineRenderer;
    
    private Camera _mainCam;
    private LineRenderer _lineRenderer;
    private bool _init = true;
    private bool _canDrawing = false;
    private event Action _completeSignEvent = null;

    private void Awake()
    {
        _mainCam = Camera.main;
    }

    public void InitSignature(Action completeSign)
    {
        _completeSignEvent -= completeSign;
        _completeSignEvent += completeSign;
        _init = true;
    }

    private void Update()
    {
        if (_init && _canDrawing)
        {
            // 마우스 클릭 시 UI 위에서만 그림 그리기 시작
            if (Input.GetMouseButtonDown(0))
            {
                StartDrawing();
            }
            // 마우스가 클릭된 상태에서 드래그 중이면 선을 계속 그림
            else if (Input.GetMouseButton(0))
            {
                Draw();
            }
            
            // 마우스 버튼을 떼면 그림 그리기 종료
            else if (Input.GetMouseButtonUp(0))
            {
                _lineRenderer = null;
                CompleteSignature();
            }
        }
    }

    private void StartDrawing()
    {
        _lineRenderer = Instantiate(signLineRenderer).GetComponent<LineRenderer>();
        _lineRenderer.transform.SetParent(signRoot);
        Vector3 pos = _mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _mainCam.nearClipPlane));
      //  Debug.Log(pos);
        _lineRenderer.positionCount = 1;
        _lineRenderer.SetPosition(0, pos);
    }
    
    private void Draw()
    {
        if (!_lineRenderer) return;
        
        Vector3 pos = _mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _mainCam.nearClipPlane));
        

        // 현재 라인에 그려진 포인트 수를 가져옴
        var positionCount = _lineRenderer.positionCount;

        // 이미 라인에 그려진 점이 있을 때만 거리 계산
        if (positionCount > 0)
        {
            // 마지막 점과 현재 마우스 위치 사이의 거리 계산
            Vector3 lastPosition = _lineRenderer.GetPosition(positionCount - 1);
            if (Vector3.Distance(pos, lastPosition) < 0.001f)
            {
                // 만약 두 점 사이의 거리가 0.1 미만이면 새로운 점을 추가하지 않음
                return;
            }
        }
        // 새로운 점 추가
        positionCount += 1;
        _lineRenderer.positionCount = positionCount;
        _lineRenderer.SetPosition(positionCount - 1, pos);

        if (!signAudioSource.isPlaying)
        {
            signAudioSource.clip = signAudioClips[UnityEngine.Random.Range(0, signAudioClips.Length)];
            signAudioSource.Play();
        }
    }
    
    private void EndDrawing()
    {
        _canDrawing = false;
        _lineRenderer = null;
        signAudioSource.Stop();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _canDrawing = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_lineRenderer != null)
        {
            CompleteSignature();
        }
        EndDrawing();
    }

    private void CompleteSignature()
    {
        if (_completeSignEvent == null) return;
        _completeSignEvent?.Invoke();
        _completeSignEvent = null;
        signAudioSource.Stop();
    }

    private void OnDestroy()
    {
        _completeSignEvent = null;
    }
}
