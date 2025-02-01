public interface IStatusEffect
{
    void Apply();
    void Remove();
    bool IsActive { get; }
    float RemainingDuration { get; }
    void Tick(); // เรียกทุก frame หรือตามเวลาที่กำหนด
}