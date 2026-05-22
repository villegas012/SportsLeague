using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Repositories;

public interface ITournamentSponsorRepository
    : IGenericRepository<TournamentSponsor>
{
    Task<IEnumerable<TournamentSponsor>>
        GetBySponsorIdAsync(int sponsorId);

    Task<TournamentSponsor?>
        GetRelationAsync(
            int tournamentId,
            int sponsorId);

    Task<bool>
        ExistsAsync(
            int tournamentId,
            int sponsorId);
}