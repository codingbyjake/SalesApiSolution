using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesApi.Models;

namespace SalesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderLinesController : ControllerBase
    {
        private readonly SalesDbContext _context;

        public OrderLinesController(SalesDbContext context)
        {
            _context = context;
        }

        //*********Method to recalculate total for an order THE SHORT WAY
        //*********using linq query syntax
        //*********handmade method
        private async Task<IActionResult> RecalculateOrderTotal(int orderId) {
            var order = await _context.Orders.FindAsync(orderId);

            order.Total = (from ol in _context.OrderLines
                           join i in _context.Items
                           on ol.ItemId equals i.Id
                           where ol.OrderId == orderId
                           select new {
                               lineTotal = ol.Quantity * i.Price
                           }).Sum(x => x.lineTotal);
            await _context.SaveChangesAsync();
            return Ok();

        }

        ////*********Method to recalculate total for an order THE LONG WAY
        ////*********handmade method
        //private async Task<IActionResult> RecalculateOrderTotal(int orderId) {
        //    // read the order to be updated
        //    var order = await _context.Orders.FindAsync(orderId);
        //    // check if the order is found
        //    if (order is null) {
        //        return NotFound();
        //    }
        //    // get all the orderlines for the order including their items
        //    var orderlines = await _context.OrderLines
        //                                .Include(x => x.Item)
        //                                .Where(x => x.OrderId == orderId)
        //                                .ToListAsync();
        //    // create a collection to store the product of quantity times price
        //    // and sum the linetotals to get the grandtotal
        //    decimal grandTotal = 0m;
        //    // loop through each orderline quantity * price and put into lineTotal
        //    foreach (var ol in orderlines) {
        //        var lineTotal = ol.Quantity * ol.Item.Price;
        //        grandTotal += lineTotal;
        //    }
        //    // update the order.Total with grandTotal
        //    order.Total = grandTotal;
        //    var changed = await _context.SaveChangesAsync();
        //    // if change failed throw an exception
        //    if (changed != 1) {
        //        throw new Exception("Recalculate failed!");
        //    }
        //    return Ok();
        //}

        // GET: api/OrderLines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderLine>>> GetOrderLine()
        {
            return await _context.OrderLines.ToListAsync();
        }

        // GET: api/OrderLines/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderLine>> GetOrderLine(int id)
        {
            var orderLine = await _context.OrderLines.FindAsync(id);

            if (orderLine == null)
            {
                return NotFound();
            }

            return orderLine;
        }

        // PUT: api/OrderLines/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderLine(int id, OrderLine orderLine)
        {
            if (id != orderLine.Id)
            {
                return BadRequest();
            }

            _context.Entry(orderLine).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                await RecalculateOrderTotal(orderLine.OrderId); //*******added this call of our handmade method
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderLineExists(id))
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

        // POST: api/OrderLines
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OrderLine>> PostOrderLine(OrderLine orderLine)
        {
            _context.OrderLines.Add(orderLine);
            await _context.SaveChangesAsync();
            await RecalculateOrderTotal(orderLine.OrderId); //added

            return CreatedAtAction("GetOrderLine", new { id = orderLine.Id }, orderLine);
        }

        // DELETE: api/OrderLines/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderLine(int id)
        {
            var orderLine = await _context.OrderLines.FindAsync(id); //added
            if (orderLine == null)
            {
                return NotFound();
            }
            var orderId = orderLine.OrderId;

            _context.OrderLines.Remove(orderLine);
            await _context.SaveChangesAsync();
            await RecalculateOrderTotal(orderId);  //added

            return NoContent();
        }

        private bool OrderLineExists(int id)
        {
            return _context.OrderLines.Any(e => e.Id == id);
        }
    }
}
