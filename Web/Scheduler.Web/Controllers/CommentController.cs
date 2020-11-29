namespace Scheduler.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Routing;
    using Scheduler.Services.Interfaces;
    using Scheduler.Web.ViewModels.Comments;

    public class CommentController : Controller
    {
        private ICommentService commentService;
        private const string homeUrl = "/";
        private const string ActionNameDetails = "Details";
        private const string ControllerName = "Event";

        public CommentController(ICommentService commentService)
        {
            this.commentService = commentService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(InputCommentDto commentViewModel)
        {
            var modelState = this.TryValidateModel(commentViewModel);
            if (modelState == false)
            {
                return this.Redirect("/");
            }

            var result = await this.commentService.AddComment(commentViewModel);
            if (result == false)
            {
                return this.Redirect("/");
            }

            return this.RedirectToAction(actionName: ActionNameDetails, ControllerName, routeValues: new RouteValueDictionary() { { "eventId", commentViewModel.EventId } });
        }

        public async Task<IActionResult> Delete(int comentId, string eventId)
        {
            await this.commentService.DeleteComment(comentId);

            return this.RedirectToAction(actionName: ActionNameDetails, ControllerName, routeValues: new RouteValueDictionary() { { "eventId", eventId } });
        }

        public async Task<IActionResult> Edit(InputCommentDto commentViewModel, int commentId, string eventId)
        {
            var modelState = this.TryValidateModel(commentViewModel);
            if (modelState == false)
            {
                return this.Redirect(homeUrl);
            }

            await this.commentService.EditComment(commentViewModel, commentId);

            return this.RedirectToAction(actionName: ActionNameDetails, ControllerName, routeValues: new RouteValueDictionary() { { "eventId", eventId } });
        }
    }
}
