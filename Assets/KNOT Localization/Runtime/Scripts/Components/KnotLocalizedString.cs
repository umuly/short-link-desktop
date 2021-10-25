using UnityEngine;

namespace Knot.Localization.Components
{
    /// <summary>
    /// Component that allows to localize attached component's <see cref="string"/> properties and methods trough <see cref="System.Reflection"/>
    /// </summary>
    [AddComponentMenu(KnotLocalization.CoreName + "/Localized String", 1000)]
    [DisallowMultipleComponent]
    public partial class KnotLocalizedString : KnotLocalizedMethod<KnotTextKeyReference, string, KnotStringTargetMethod>
    {
        
    }
}