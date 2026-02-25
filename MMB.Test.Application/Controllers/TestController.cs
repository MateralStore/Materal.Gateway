using Materal.MergeBlock.Web.Abstractions.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace MMB.Test.Application.Controllers;

[Route("TestAPI/[controller]/[action]")]
public class TestController : MergeBlockController
{
    [HttpGet]
    public string TestGet() => "Hello World!";
}
