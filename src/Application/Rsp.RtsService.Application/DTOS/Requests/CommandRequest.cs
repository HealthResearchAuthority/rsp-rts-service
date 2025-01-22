namespace Rsp.RtsService.Application.DTOS.Requests;

// TODO: Example command request, rename as appropriate
public record CommandRequest
{
    /// <summary>
    /// Id to look up the database
    /// </summary>
    public int Id { get; set; }

    // Add more properties as needed
}