namespace Radiology;

public interface IRadiationModifier
{
    void Modify(ref RadiationInfo info);
}