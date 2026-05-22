using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;

namespace SportsLeague.DataAccess.Repositories;

public class TournamentSponsorRepository
    : GenericRepository<TournamentSponsor>,
      ITournamentSponsorRepository
{
    private readonly LeagueDbContext _context;

    public TournamentSponsorRepository(
        LeagueDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TournamentSponsor>>
        GetBySponsorIdAsync(int sponsorId)
    {
        return await _context.TournamentSponsors
            .Include(ts => ts.Tournament)
            .Where(ts => ts.SponsorId == sponsorId)
            .ToListAsync();
    }

    public async Task<TournamentSponsor?>
        GetRelationAsync(
            int tournamentId,
            int sponsorId)
    {
        return await _context.TournamentSponsors
            .FirstOrDefaultAsync(ts =>
                ts.TournamentId == tournamentId &&
                ts.SponsorId == sponsorId);
    }

    public async Task<bool>
        ExistsAsync(
            int tournamentId,
            int sponsorId)
    {
        return await _context.TournamentSponsors
            .AnyAsync(ts =>
                ts.TournamentId == tournamentId &&
                ts.SponsorId == sponsorId);
    }
}