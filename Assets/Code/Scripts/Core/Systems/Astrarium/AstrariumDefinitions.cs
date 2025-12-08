using UnityEngine;

namespace Code.Scripts.Core.Systems.Astrarium
{
    public enum AstrariumCategory
    {
        Resource,
        Planet,
        Satellite,
        Species,
        Constellation,
        Artifact,
        Special
    }

    public enum DiscoveryStatus
    {
        Unknown = 0,
        Discovered = 1,
        Analyzed = 2
    }

    public interface IAstrariumEntry
    {
        string GetAstrariumID();
        string GetDisplayName();
        string GetDescription();
        Sprite GetIcon();
        AstrariumCategory GetCategory();
        GameObject Get3DModel();
    }
}