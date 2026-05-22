using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services;

public class SponsorService : ISponsorService
{
    private readonly ISponsorRepository _sponsorRepository;

    private readonly ITournamentSponsorRepository
        _tournamentSponsorRepository;

    private readonly ITournamentRepository
        _tournamentRepository;

    private readonly ILogger<SponsorService> _logger;

    public SponsorService(
        ISponsorRepository sponsorRepository,
        ITournamentSponsorRepository tournamentSponsorRepository,
        ITournamentRepository tournamentRepository,
        ILogger<SponsorService> logger)
    {
        _sponsorRepository = sponsorRepository;

        _tournamentSponsorRepository =
            tournamentSponsorRepository;

        _tournamentRepository =
            tournamentRepository;

        _logger = logger;
    }

    public async Task<IEnumerable<Sponsor>>
        GetAllAsync()
    {
        _logger.LogInformation(
            "Retrieving all sponsors");

        return await _sponsorRepository
            .GetAllAsync();
    }

    public async Task<Sponsor?>
        GetByIdAsync(int id)
    {
        _logger.LogInformation(
            "Retrieving sponsor with ID: {SponsorId}",
            id);

        return await _sponsorRepository
            .GetByIdAsync(id);
    }

    public async Task<Sponsor>
        CreateAsync(Sponsor sponsor)
    {
        var exists =
            await _sponsorRepository
                .ExistsByNameAsync(sponsor.Name);

        if (exists)
        {
            throw new InvalidOperationException(
                $"Ya existe un sponsor con nombre '{sponsor.Name}'");
        }

        _logger.LogInformation(
            "Creating sponsor {SponsorName}",
            sponsor.Name);

        return await _sponsorRepository
            .CreateAsync(sponsor);
    }

    public async Task
        UpdateAsync(int id, Sponsor sponsor)
    {
        var existingSponsor =
            await _sponsorRepository
                .GetByIdAsync(id);

        if (existingSponsor == null)
        {
            throw new KeyNotFoundException(
                $"Sponsor con ID {id} no existe");
        }

        existingSponsor.Name =
            sponsor.Name;

        existingSponsor.ContactEmail =
            sponsor.ContactEmail;

        existingSponsor.Phone =
            sponsor.Phone;

        existingSponsor.WebsiteUrl =
            sponsor.WebsiteUrl;

        existingSponsor.Category =
            sponsor.Category;

        await _sponsorRepository
            .UpdateAsync(existingSponsor);
    }

    public async Task DeleteAsync(int id)
    {
        var sponsor =
            await _sponsorRepository
                .GetByIdAsync(id);

        if (sponsor == null)
        {
            throw new KeyNotFoundException(
                $"Sponsor con ID {id} no existe");
        }

        await _sponsorRepository
            .DeleteAsync(id);
    }

    public async Task<IEnumerable<TournamentSponsor>>
        GetSponsorTournamentsAsync(int sponsorId)
    {
        return await _tournamentSponsorRepository
            .GetBySponsorIdAsync(sponsorId);
    }

    public async Task<TournamentSponsor>
        LinkTournamentAsync(
            int sponsorId,
            TournamentSponsor relation)
    {
        var sponsor =
            await _sponsorRepository
                .GetByIdAsync(sponsorId);

        if (sponsor == null)
        {
            throw new KeyNotFoundException(
                "Sponsor no encontrado");
        }

        var tournament =
            await _tournamentRepository
                .GetByIdAsync(relation.TournamentId);

        if (tournament == null)
        {
            throw new KeyNotFoundException(
                "Tournament no encontrado");
        }

        var alreadyExists =
            await _tournamentSponsorRepository
                .ExistsAsync(
                    relation.TournamentId,
                    sponsorId);

        if (alreadyExists)
        {
            throw new InvalidOperationException(
                "El sponsor ya está asociado al torneo");
        }

        relation.SponsorId = sponsorId;

        await _tournamentSponsorRepository
            .CreateAsync(relation);

        return relation;
    }

    public async Task UnlinkTournamentAsync(
        int sponsorId,
        int tournamentId)
    {
        var relation =
            await _tournamentSponsorRepository
                .GetRelationAsync(
                    tournamentId,
                    sponsorId);

        if (relation == null)
        {
            throw new KeyNotFoundException(
                "Relación no encontrada");
        }

        await _tournamentSponsorRepository
            .DeleteAsync(relation.Id);
    }
}