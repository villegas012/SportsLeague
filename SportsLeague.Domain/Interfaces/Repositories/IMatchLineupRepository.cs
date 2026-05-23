using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Repositories
{
    public interface IMatchLineupRepository : IGenericRepository<MatchLineup>
    {
        Task<IEnumerable<MatchLineup>> GetByMatchAsync(int matchId);

        Task<bool> ExistsPlayerInMatchAsync(int matchId, int playerId);

        Task<int> CountStartersAsync(int matchId, int teamId);
    }
}