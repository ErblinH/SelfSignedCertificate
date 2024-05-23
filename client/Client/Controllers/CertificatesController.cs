using Client.ApiServices;
using Client.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Client.Controllers
{
    //[Authorize]
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

        public async Task Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }

        // GET: Certificates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //if (id == null || _context.Certificate == null)
            //{
            //    return NotFound();
            //}

            //var certificate = await _context.Certificate
            //    .FirstOrDefaultAsync(m => m.Id == id);
            //if (certificate == null)
            //{
            //    return NotFound();
            //}

            return Ok();
            // return View(certificate);
        }

        // GET: Certificates/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Certificates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Name,Subject,Issuer,KeyPairs,Data,FilePath,ValidFrom,ValidUntil,CreationDateTime,Deleted")]
            Certificate certificate)
        {
            return View();

            //if (ModelState.IsValid)
            //{
            //    _context.Add(certificate);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}

            //return View(certificate);
        }

        // GET: Certificates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            return View();
            //if (id == null || _context.Certificate == null)
            //{
            //    return NotFound();
            //}

            //var certificate = await _context.Certificate.FindAsync(id);
            //if (certificate == null)
            //{
            //    return NotFound();
            //}

            //return View(certificate);
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
            //if (id != certificate.Id)
            //{
            //    return NotFound();
            //}

            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        _context.Update(certificate);
            //        await _context.SaveChangesAsync();
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!CertificateExists(certificate.Id))
            //        {
            //            return NotFound();
            //        }
            //        else
            //        {
            //            throw;
            //        }
            //    }

            //    return RedirectToAction(nameof(Index));
            //}

            //return View(certificate);
        }

        // GET: Certificates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            return View();
            //if (id == null || _context.Certificate == null)
            //{
            //    return NotFound();
            //}

            //var certificate = await _context.Certificate
            //    .FirstOrDefaultAsync(m => m.Id == id);
            //if (certificate == null)
            //{
            //    return NotFound();
            //}

            //return View(certificate);
        }

        // POST: Certificates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            return View();
            //if (_context.Certificate == null)
            //{
            //    return Problem("Entity set 'ClientContext.Certificate'  is null.");
            //}

            //var certificate = await _context.Certificate.FindAsync(id);
            //if (certificate != null)
            //{
            //    _context.Certificate.Remove(certificate);
            //}

            //await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
        }

        private bool CertificateExists(int id)
        {
            return true;
            // return (_context.Certificate?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}