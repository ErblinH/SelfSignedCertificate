using Client.ApiServices;
using Client.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Client.Controllers
{
    public class CertificatesController : Controller
    {
        private readonly ICertificateApiService _certificateApiService;

        public CertificatesController(ICertificateApiService certificateApiService)
        {
            _certificateApiService = certificateApiService;
        }

        // GET: Certificates
        public async Task<IActionResult> Index()
        {
            await LogTokenAndClaims();
            return View(await _certificateApiService.GetAllCertificatesAsync());
        }

        public async Task LogTokenAndClaims()
        {
            var identityToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);

            Console.WriteLine($"Identity token: {identityToken}");

            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"Claim types: {claim.Type} - Claim value: {claim.Value}");
            }
        }

        // GET: Certificates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            return View();
        }

        // GET: Certificates/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Certificates/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Name,Subject,Issuer,KeyPairs,Data,FilePath,ValidFrom,ValidUntil,CreationDateTime,Deleted")]
            Certificate certificate)
        {
            return View();
        }

        // GET: Certificates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            return View();
        }

        // POST: Certificates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,Name,Subject,Issuer,KeyPairs,Data,FilePath,ValidFrom,ValidUntil,CreationDateTime,Deleted")]
            Certificate certificate)
        {
            return View();
        }

        // GET: Certificates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            return View();
        }

        // POST: Certificates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            return View();
        }
    }
}