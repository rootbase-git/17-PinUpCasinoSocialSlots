using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWallet
{
    void OnIncreaseBalance(int value);
    void OnDecreaseBalance(int value);
}
