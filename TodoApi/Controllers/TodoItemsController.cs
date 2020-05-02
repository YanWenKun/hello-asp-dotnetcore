using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    /*
     * 方括号中的是 [attribute]，即属性（注意与 property 区分）。
     * 按照约定，[controller] 表示类名去掉 "Controller" 后缀得到的字符串。
     */
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDto>>> GetTodoItems()
        {
            return await _context.TodoItems
                .Select(item => ItemToDto(item))
                .ToListAsync();
        }

        // GET: api/TodoItems/5
        // 这里 {id} 作为【占位符变量】，被捕获并作为方法参数
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDto>> GetTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return ItemToDto(todoItem);
        }

        // PUT: api/TodoItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoItem(long id, TodoItemDto todoItemDto)
        {
            if (id != todoItemDto.Id)
            {
                return BadRequest();
            }

            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            todoItem.Name = todoItemDto.Name;
            todoItem.IsComplete = todoItemDto.IsComplete;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/TodoItems
        [HttpPost]
        public async Task<ActionResult<TodoItemDto>> PostTodoItem(TodoItemDto todoItemDto)
        {
            var todoItem = new TodoItem
            {
                Name = todoItemDto.Name,
                IsComplete = todoItemDto.IsComplete
            };

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            // CreatedAtAction 方法：
            //     如果成功，则返回 HTTP 201 (Created) 状态码。
            //     添加 Location 头，告知刚创建的资源地址（Restful 风格）
            return CreatedAtAction(
                nameof(GetTodoItem),
                new {id = todoItem.Id},
                ItemToDto(todoItem));
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoItemExists(long id) =>
            _context.TodoItems.Any(e => e.Id == id);

        /*
         * 将原始的数据库对象组装成传输对象（此处就是限制属性成员，不暴露敏感字段）
         */
        private static TodoItemDto ItemToDto(TodoItem todoItem) =>
            new TodoItemDto
            {
                Id = todoItem.Id,
                Name = todoItem.Name,
                IsComplete = todoItem.IsComplete
            };
    }
}