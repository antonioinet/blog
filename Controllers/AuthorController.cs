using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    
    [Authorize]
    public class AuthorController:Controller
    {

    }
}
