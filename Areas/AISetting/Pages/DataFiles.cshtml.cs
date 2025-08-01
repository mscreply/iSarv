using System.Threading.Tasks;
using iSarv.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace iSarv.Areas.AISetting.Pages
{
    [Authorize(Roles = "Administrator")]
    public class DataFilesModel : PageModel
    {
        [BindProperty]
        public string UniversityFile { get; set; }
        [BindProperty]
        public string FieldOfStudyFile { get; set; }
        [BindProperty]
        public string OccupationFile { get; set; }
        public void OnGet()
        {
            UniversityFile = Utilities.ReadTextFile("App_Files/data/University.txt");
            FieldOfStudyFile = Utilities.ReadTextFile("App_Files/data/FieldOfStudy.txt");
            OccupationFile = Utilities.ReadTextFile("App_Files/data/Occupation.txt");
        }

        public void OnPost()
        {
            Utilities.SaveTextFile("App_Files/data/University.txt", UniversityFile);
            Utilities.SaveTextFile("App_Files/data/FieldOfStudy.txt", FieldOfStudyFile);
            Utilities.SaveTextFile("App_Files/data/Occupation.txt", OccupationFile);
        }
    }
}
