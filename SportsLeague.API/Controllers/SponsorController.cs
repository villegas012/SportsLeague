using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SponsorController : ControllerBase
{
    private readonly ISponsorService _sponsorService;

    private readonly IMapper _mapper;

    public SponsorController(
        ISponsorService sponsorService,
        IMapper mapper)
    {
        _sponsorService = sponsorService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SponsorResponseDTO>>>
        GetAll()
    {
        var sponsors = await _sponsorService.GetAllAsync();

        return Ok(
            _mapper.Map<IEnumerable<SponsorResponseDTO>>(sponsors));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SponsorResponseDTO>>
        GetById(int id)
    {
        var sponsor = await _sponsorService.GetByIdAsync(id);

        if (sponsor == null)
        {
            return NotFound();
        }

        return Ok(
            _mapper.Map<SponsorResponseDTO>(sponsor));
    }

    [HttpPost]
    public async Task<ActionResult<SponsorResponseDTO>>
        Create(SponsorRequestDTO dto)
    {
        try
        {
            var sponsor = _mapper.Map<Sponsor>(dto);

            var createdSponsor =
                await _sponsorService.CreateAsync(sponsor);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdSponsor.Id },
                _mapper.Map<SponsorResponseDTO>(createdSponsor));
        }
        catch (Exception ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult>
        Update(int id, SponsorRequestDTO dto)
    {
        try
        {
            var sponsor = _mapper.Map<Sponsor>(dto);

            await _sponsorService.UpdateAsync(id, sponsor);

            return NoContent();
        }
        catch (Exception ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult>
        Delete(int id)
    {
        try
        {
            await _sponsorService.DeleteAsync(id);

            return NoContent();
        }
        catch (Exception ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpGet("{id}/tournaments")]
    public async Task<ActionResult<
        IEnumerable<TournamentSponsorResponseDTO>>>
        GetSponsorTournaments(int id)
    {
        var relations =
            await _sponsorService
                .GetSponsorTournamentsAsync(id);

        var response = relations.Select(r =>
            new TournamentSponsorResponseDTO
            {
                TournamentId = r.TournamentId,
                TournamentName = r.Tournament.Name,
                ContractAmount = r.ContractAmount,
                JoinedAt = r.JoinedAt
            });

        return Ok(response);
    }

    [HttpPost("{id}/tournaments")]
    public async Task<ActionResult>
        LinkTournament(
            int id,
            TournamentSponsorRequestDTO dto)
    {
        try
        {
            var relation =
                _mapper.Map<TournamentSponsor>(dto);

            await _sponsorService
                .LinkTournamentAsync(id, relation);

            return StatusCode(201);
        }
        catch (Exception ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpDelete("{id}/tournaments/{tournamentId}")]
    public async Task<IActionResult>
        UnlinkTournament(
            int id,
            int tournamentId)
    {
        try
        {
            await _sponsorService
                .UnlinkTournamentAsync(id, tournamentId);

            return NoContent();
        }
        catch (Exception ex)
        {
            return Conflict(ex.Message);
        }
    }
}