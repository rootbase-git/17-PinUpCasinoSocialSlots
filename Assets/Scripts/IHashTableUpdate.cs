using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHashTableUpdate<in T>
{
    void UpdateHashTableInfo(T centerSpinElement);
}
