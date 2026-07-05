namespace HealthCare.Claims.Modules.Membership.Domain
{
    public sealed record Member
    (
        Guid Id,
        string MemberNumber,
        string FullName,
        DateOnly DateOfBirth,
        string Gender,
        string MobileNumber,
        string City,
        string State,
        string PolicyNumber,
        MemberStatus Status
    );
}
