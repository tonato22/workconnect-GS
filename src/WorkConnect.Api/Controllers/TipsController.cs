using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkConnect.Api.Dtos;
using WorkConnect.Api.Hateoas;
using WorkConnect.Api.Helpers;
using WorkConnect.Domain.Entities;
using WorkConnect.Infrastructure.Data;

namespace WorkConnect.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TipsController : ControllerBase
    {
        private readonly WorkConnectContext _context;

        public TipsController(WorkConnectContext context)
        {
            _context = context;
        }

        // GET api/v1/tips?pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<ActionResult<PagedResult<TipResponseDto>>> GetTips([FromQuery] PaginationParams pagination)
        {
            var query = _context.Tips
                .Include(t => t.Author)
                .OrderByDescending(t => t.CreatedAt)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            var dtoItems = items.Select(t => new TipResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Content = t.Content,
                Category = t.Category,
                CreatedAt = t.CreatedAt,
                AuthorId = t.AuthorId,
                AuthorName = t.Author?.Name,
                Links = new List<LinkDto>
                {
                    new LinkDto
                    {
                        Rel = "self",
                        Href = HateoasHelper.BuildUrl(HttpContext, $"api/v1/tips/{t.Id}"),
                        Method = "GET"
                    },
                    new LinkDto
                    {
                        Rel = "update",
                        Href = HateoasHelper.BuildUrl(HttpContext, $"api/v1/tips/{t.Id}"),
                        Method = "PUT"
                    },
                    new LinkDto
                    {
                        Rel = "delete",
                        Href = HateoasHelper.BuildUrl(HttpContext, $"api/v1/tips/{t.Id}"),
                        Method = "DELETE"
                    },
                    new LinkDto
                    {
                        Rel = "author",
                        Href = HateoasHelper.BuildUrl(HttpContext, $"api/v1/users/{t.AuthorId}"),
                        Method = "GET"
                    }
                }
            }).ToList();

            var result = new PagedResult<TipResponseDto>
            {
                Items = dtoItems,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize,
                TotalCount = totalCount,
                Links = new List<LinkDto>()
            };

            // HATEOAS links de paginação
            result.Links.Add(new LinkDto
            {
                Rel = "self",
                Href = HateoasHelper.BuildUrl(HttpContext, $"api/v1/tips?pageNumber={pagination.PageNumber}&pageSize={pagination.PageSize}"),
                Method = "GET"
            });

            var totalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize);

            if (pagination.PageNumber < totalPages)
            {
                result.Links.Add(new LinkDto
                {
                    Rel = "nextPage",
                    Href = HateoasHelper.BuildUrl(HttpContext, $"api/v1/tips?pageNumber={pagination.PageNumber + 1}&pageSize={pagination.PageSize}"),
                    Method = "GET"
                });
            }

            if (pagination.PageNumber > 1)
            {
                result.Links.Add(new LinkDto
                {
                    Rel = "prevPage",
                    Href = HateoasHelper.BuildUrl(HttpContext, $"api/v1/tips?pageNumber={pagination.PageNumber - 1}&pageSize={pagination.PageSize}"),
                    Method = "GET"
                });
            }

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TipResponseDto>> GetTip(int id)
        {
            var tip = await _context.Tips
                .Include(t => t.Author)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tip == null)
                return NotFound();

            var dto = new TipResponseDto
            {
                Id = tip.Id,
                Title = tip.Title,
                Content = tip.Content,
                Category = tip.Category,
                CreatedAt = tip.CreatedAt,
                AuthorId = tip.AuthorId,
                AuthorName = tip.Author?.Name,
                Links = new List<LinkDto>
                {
                    new LinkDto
                    {
                        Rel = "self",
                        Href = HateoasHelper.BuildUrl(HttpContext, $"api/v1/tips/{tip.Id}"),
                        Method = "GET"
                    },
                    new LinkDto
                    {
                        Rel = "author",
                        Href = HateoasHelper.BuildUrl(HttpContext, $"api/v1/users/{tip.AuthorId}"),
                        Method = "GET"
                    }
                }
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<TipResponseDto>> CreateTip([FromBody] TipCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authorExists = await _context.Users.AnyAsync(u => u.Id == dto.AuthorId);
            if (!authorExists)
            {
                ModelState.AddModelError("AuthorId", "Autor não encontrado.");
                return BadRequest(ModelState);
            }

            var tip = new Tip
            {
                Title = dto.Title,
                Content = dto.Content,
                Category = dto.Category,
                AuthorId = dto.AuthorId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tips.Add(tip);
            await _context.SaveChangesAsync();

            var responseDto = new TipResponseDto
            {
                Id = tip.Id,
                Title = tip.Title,
                Content = tip.Content,
                Category = tip.Category,
                CreatedAt = tip.CreatedAt,
                AuthorId = tip.AuthorId,
                AuthorName = (await _context.Users.FindAsync(tip.AuthorId))?.Name,
                Links = new List<LinkDto>
                {
                    new LinkDto
                    {
                        Rel = "self",
                        Href = HateoasHelper.BuildUrl(HttpContext, $"api/v1/tips/{tip.Id}"),
                        Method = "GET"
                    },
                    new LinkDto
                    {
                        Rel = "author",
                        Href = HateoasHelper.BuildUrl(HttpContext, $"api/v1/users/{tip.AuthorId}"),
                        Method = "GET"
                    }
                }
            };

            return CreatedAtAction(nameof(GetTip), new { id = tip.Id, version = "1.0" }, responseDto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateTip(int id, [FromBody] TipUpdateDto dto)
        {
            var tip = await _context.Tips.FindAsync(id);
            if (tip == null)
                return NotFound();

            tip.Title = dto.Title;
            tip.Content = dto.Content;
            tip.Category = dto.Category;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTip(int id)
        {
            var tip = await _context.Tips.FindAsync(id);
            if (tip == null)
                return NotFound();

            _context.Tips.Remove(tip);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
