using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHashTableUpdate<T>
{
    void UpdateHashTableInfo(T centerSpinElement);
}
