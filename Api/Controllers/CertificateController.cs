using Domain.ElasticSearch;
using Domain.Entities;
using Domain.Interfaces.Service;
using Domain.Request;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CertificateController : ControllerBase
    {

        private readonly ICertificateService _certificateService;

        public CertificateController(ICertificateService certificateService)
        {
            _certificateService = certificateService;
        }


        [HttpGet("{name}")]
        [ProducesResponseType(typeof(Certificate), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Certificate>> GetCertificate(Guid name)
        {
            return Ok(await _certificateService.GetCertificateAsync(name));
        }

        [HttpGet("list")]
        [ProducesResponseType(typeof(List<Certificate>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Certificate>>> ListAllCertificates()
        {
            return Ok(await _certificateService.GetAllCertificatesAsync());
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(Certificate), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Certificate>> CreateCertificates([FromBody] CreateAndSignRequest createAndSignRequest)
        {
            return Ok(await _certificateService.CreateCertificateAsync(createAndSignRequest));
        }

        [HttpGet("{name}/check")]
        [ProducesResponseType(typeof(List<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<bool>> CheckCertificates(Guid name)
        {
            return Ok(await _certificateService.CheckCertificateAsync(name));
        }

        [HttpPost("search")]
        [ProducesResponseType(typeof(SearchCertificateResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SearchCertificateResult>> SearchCertificates([FromBody] SearchCertificatesRequest searchCertificatesRequest)
        {
            return Ok(await _certificateService.SearchCertificateAsync(searchCertificatesRequest));
        }
    }
}
