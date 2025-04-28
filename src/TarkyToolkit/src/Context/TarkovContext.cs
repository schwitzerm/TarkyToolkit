using UnityEngine;

namespace TarkyToolkit.Context;

public class TarkovContext : MonoBehaviour, ITarkovContext
{
    public EFT.GameWorld? GameWorld { get; set; }
}
