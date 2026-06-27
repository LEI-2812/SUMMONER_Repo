using System.Collections;
using UnityEngine;

// 역할: Application 계층에서 Unity 코루틴 실행을 요청할 수 있는 최소 계약을 정의한다.
public interface ICoroutineRunner
{
    Coroutine StartCoroutine(IEnumerator routine);
}
