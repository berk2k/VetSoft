using Microsoft.AspNetCore.Mvc;
using TermProjectBackend.Models;

[ApiController]
[Route("[controller]")]
public class MessagesController : ControllerBase
{
    private readonly RabbitMqService _rabbitMqService;


    public MessagesController(RabbitMqService rabbitMqService)
    {
        _rabbitMqService = rabbitMqService;
    }

    [HttpGet("NotificationMessages")]
    public IActionResult GetNotificationMessages()
    {
        var messages = _rabbitMqService.GetMessages(QueueNames.NotificationQueue);
        return Ok(messages);
    }

    [HttpGet("UpdateAppointmentMessages")]
    public IActionResult GetUpdateAppointmentMessages()
    {
        var messages = _rabbitMqService.GetMessages(QueueNames.UpdateAppointmentQueue);
        return Ok(messages);
    }

    [HttpGet("DeleteAppointmentMessages")]
    public IActionResult GetDeleteAppointmentMessages()
    {
        var messages = _rabbitMqService.GetMessages(QueueNames.DeleteAppointmentQueue);
        return Ok(messages);
    }
}
