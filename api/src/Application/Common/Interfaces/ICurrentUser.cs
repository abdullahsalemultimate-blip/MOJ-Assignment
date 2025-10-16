namespace InventorySys.Application.Common.Interfaces;

public interface ICurrentUser
{
    string? Id { get; }
    public string? Username { get; }
    List<string>? Roles { get; }

}
