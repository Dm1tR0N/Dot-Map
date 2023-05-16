using Microsoft.AspNetCore.Mvc;
using DotMapApi.Models;
using DotMapApi.Interface;

namespace DotMapApi.Controllers;

[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    public IActionResult GetAllUsers()
    {
        var users = _userRepository.GetAllUsers();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public IActionResult GetUserById(int id)
    {
        var user = _userRepository.GetUserById(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpPost]
    public IActionResult AddUser([FromBody] User user)
    {
        var addedUser = _userRepository.AddUser(user);
        return CreatedAtAction(nameof(GetUserById), new { id = addedUser.Id }, addedUser);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateUser(int id, [FromBody] User user)
    {
        if (id != user.Id)
        {
            return BadRequest();
        }

        var existingUser = _userRepository.GetUserById(id);
        if (existingUser == null)
        {
            return NotFound();
        }

        _userRepository.UpdateUser(user);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        var existingUser = _userRepository.GetUserById(id);
        if (existingUser == null)
        {
            return NotFound();
        }

        _userRepository.DeleteUser(id);

        return NoContent();
    }
}