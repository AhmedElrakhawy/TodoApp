using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ApiDbContext _Context;
        public TodoController(ApiDbContext Context)
        {
            _Context = Context;
        }
        public async Task<IActionResult> GetItems()
        {
            var Items = await _Context.Items.ToListAsync();
            return Ok(Items);
        }
        [HttpPost]
        public async Task<IActionResult> CreateItem(ItemData data)
        {
            if (ModelState.IsValid)
            {
                await _Context.Items.AddAsync(data);
                await _Context.SaveChangesAsync();
                return CreatedAtAction("GetItem", new { data.Id }, data);
            }
            return new JsonResult("Something went wrong") { StatusCode = 500 };
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetItem(int Id)
        {
            var Item = await _Context.Items.FirstOrDefaultAsync(x => x.Id == Id);
            if (Item == null)
                return NotFound();

            return Ok(Item);
        }
        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateItem(int Id ,ItemData data)
        {
            if (Id != data.Id)
                return BadRequest();

            var ExistItem = await _Context.Items.FirstOrDefaultAsync(x => x.Id == Id);

            if (ExistItem == null)
                return NotFound();

            ExistItem.Title = data.Title;
            ExistItem.Description = data.Description;
            ExistItem.Done = data.Done;

            await _Context.SaveChangesAsync();

            return NoContent();
        }
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteItem(int Id)
        {
            var Item =  await _Context.Items.FindAsync(Id);
            if (Item == null)
                return NotFound();

            _Context.Items.Remove(Item);
            await _Context.SaveChangesAsync();

            return Ok(Item);
        }
    }
}
