using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using PRSApplicationCapstone.Models;

namespace PRSApplicationCapstone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorsController : ControllerBase
    {
        private readonly PrsDbContext _context;

        public VendorsController(PrsDbContext context)
        {
            _context = context;
        }
        [HttpGet("po/{vendorId}")]
        public async Task<ActionResult<Po>> CreatePo(int vendorId)
        {
            Po po = new Po();
            po.Vendor = await _context.Vendors.FindAsync(vendorId);
            var poLines =  await (from vendor in _context.Vendors
                           join product in _context.Products
                           on vendor.Id equals product.VendorId
                           join requestL in _context.RequestLines
                           on product.Id equals requestL.ProductId
                           join request in _context.Requests
                           on requestL.RequestId equals request.Id
                           where request.Status == "APPROVED" && product.VendorId == vendorId
                           select new
                           {
                               product.Id,
                               Product = product.Name,
                               requestL.Quantity,
                               product.Price,
                               LineTotal = product.Price * requestL.Quantity
                           }).ToListAsync();
            var sortedLines = new SortedList<int, Poline>();
            foreach (var line in poLines)
            {
                if (!sortedLines.ContainsKey(line.Id))
                {
                    var poline = new Poline()
                    {
                        Product = line.Product,
                        Quantity = 0,
                        Price = line.Price,
                        LineTotal = line.LineTotal
                    };
                    sortedLines.Add(line.Id, poline);
                    po.PoTotal += line.LineTotal;
                    sortedLines[line.Id].Quantity += line.Quantity;
                }
                
                
            }
            
            
            po.Polines = sortedLines.Values.ToList();
            return po;

        }

        // GET: api/Vendors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vendor>>> GetVendors()
        {
            return await _context.Vendors.ToListAsync();
        }

        // GET: api/Vendors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Vendor>> GetVendor(int id)
        {
            var vendor = await _context.Vendors.FindAsync(id);

            if (vendor == null)
            {
                return NotFound();
            }

            return vendor;
        }

        // PUT: api/Vendors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVendor(int id, Vendor vendor)
        {
            if (id != vendor.Id)
            {
                return BadRequest();
            }

            _context.Entry(vendor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VendorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Vendors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Vendor>> PostVendor(Vendor vendor)
        {
            _context.Vendors.Add(vendor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVendor", new { id = vendor.Id }, vendor);
        }

        // DELETE: api/Vendors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendor(int id)
        {
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null)
            {
                return NotFound();
            }

            _context.Vendors.Remove(vendor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VendorExists(int id)
        {
            return _context.Vendors.Any(e => e.Id == id);
        }
    }
}
