using iSarv.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace iSarv.Areas.AISetting.Pages;

[Authorize(Roles = "Administrator")]
public class Index : PageModel
{
    private readonly ApplicationDbContext _context;

    public Index(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<iSarv.Data.Services.AISetting> AISettings { get; set; }
    public void OnGet()
    {
        AISettings = _context.AISettings.ToList();
    }

    public async Task<IActionResult> OnPostCreateSettingAsync(iSarv.Data.Services.AISetting newSetting)
    {
        if (!ModelState.IsValid)
        {
            // Reload settings for the page if model state is invalid and you return Page()
            AISettings = await _context.AISettings.ToListAsync();
            return Page();
        }
        _context.AISettings.Add(newSetting);
        await _context.SaveChangesAsync();
        return RedirectToPage(); // Or return a success message
    }


    public async Task<IActionResult> OnPostUpdateSettingAsync(int id, string server, string apiKey, string uri, int maxTokens, string modelList)
    {
        var settingToUpdate = await _context.AISettings.FindAsync(id);
        if (settingToUpdate == null)
        {
            return new JsonResult(new { success = false, message = "Setting not found." });
        }

        settingToUpdate.Server = server;
        settingToUpdate.ApiKey = apiKey;
        settingToUpdate.Uri = uri;
        settingToUpdate.MaxTokens = maxTokens;
        settingToUpdate.ModelList = modelList;

        try
        {
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }
        catch (Exception ex)
        {
            // Log the exception ex
            return new JsonResult(new { success = false, message = "Error saving changes." });
        }
    }

    public async Task<IActionResult> OnPostDeleteSettingAsync(int id)
    {
        var settingToDelete = await _context.AISettings.FindAsync(id);
        if (settingToDelete != null)
        {
            if (settingToDelete.Server.ToLower() == "default") // Prevent deleting "Default"
            {
                // Optionally add a TempData message for the user
                TempData["ErrorMessage"] = "Cannot delete the default setting.";
                return RedirectToPage();
            }
            _context.AISettings.Remove(settingToDelete);
            await _context.SaveChangesAsync();
        }
        return RedirectToPage();
    }

}