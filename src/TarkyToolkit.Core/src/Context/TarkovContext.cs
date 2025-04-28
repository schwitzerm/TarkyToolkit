using UnityEngine;

namespace TarkyToolkit.Core.Context;

public class TarkovContext : MonoBehaviour, ITarkovContext
{
    public EFT.GameWorld? GameWorld { get; set; }
}
