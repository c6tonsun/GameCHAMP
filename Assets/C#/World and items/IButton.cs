using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IButton
{

    bool IsSendingSignal();
    bool IsButtonLocked();
    void SetButtonLocked(bool value);

}
