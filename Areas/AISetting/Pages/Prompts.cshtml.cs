using iSarv.Data;
using iSarv.Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace iSarv.Areas.AISetting.Pages;

[Authorize(Roles = "Administrator")]
public class PromptsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public PromptsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Prompt> Prompts { get; set; }
    public void OnGet()
    {
        Prompts = _context.Prompts.ToList();
    }

    public async Task<IActionResult> OnPostCreatePromptAsync(Prompt newPrompt)
    {
        if (!ModelState.IsValid)
        {
            // Reload prompts for the page if model state is invalid and you return Page()
            Prompts = await _context.Prompts.ToListAsync();
            return Page();
        }
        _context.Prompts.Add(newPrompt);
        await _context.SaveChangesAsync();
        return RedirectToPage(); // Or return a success message
    }


    public async Task<IActionResult> OnPostUpdatePromptAsync(string name, string test, string promptText, string language, int replyLength)
    {
        var promptToUpdate = await _context.Prompts.FindAsync(name);
        if (promptToUpdate == null)
        {
            return new JsonResult(new { success = false, message = "Prompt not found." });
        }

        try
        {
            promptToUpdate.Test = (Tests)Enum.Parse(typeof(Tests), test);
            promptToUpdate.PromptText = promptText;
            promptToUpdate.Language = language;
            promptToUpdate.ReplyLength = replyLength;

            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }
        catch (Exception ex)
        {
            // Log the exception ex
            return new JsonResult(new { success = false, message = "Error saving changes." });
        }
    }

    public async Task<IActionResult> OnPostDeletePromptAsync(string id)
    {
        var promptToDelete = await _context.Prompts.FindAsync(id);
        if (promptToDelete == null)
        {
            return RedirectToPage();
        }

        try
        {
            _context.Prompts.Remove(promptToDelete);
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
        catch (Exception ex)
        {
            // Log the exception ex
            return new JsonResult(new { success = false, message = "Error deleting prompt." });
        }
    }

}
