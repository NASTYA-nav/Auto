using Auto.Integration.Models;
using Auto.Integration.Repository;

namespace Auto.Integration.Controllers
{
    /// <summary>
    /// Контроллер Бренда
    /// </summary>
    public class HomeController : Controller
    {
        // GET
        public ActionResult Index()
        {
            // Репозиторий брендов
            var autoBrandRepository = new AutoBrandRepository();
            var model = autoBrandRepository.RetrieveRecords();

            return View(model);
        }
        
         // GET: HomeController/Create
         public ActionResult Create()
         {
             return View();
         }

         // POST: HomeController/Create
         [HttpPost]
         [ValidateAntiForgeryToken]
         public ActionResult Create(AutoBrand model)
         {
             try
             {
                 var repository = new AutoBrandRepository();
                 repository.Save(model);

                 return RedirectToAction(nameof(Index));
             }
             catch
             {
                 return View();
             }
         }

         // GET: HomeController/Edit/5
         public ActionResult Update(Guid id)
         {
             return View();
         }

         // POST: HomeController/Edit/
         [HttpPost]
         [ValidateAntiForgeryToken]
         public ActionResult Update(AutoBrand model)
         {
             try
             {
                 var repository = new AutoBrandRepository();
                 repository.Save(model);

                 return RedirectToAction(nameof(Index));
             }
             catch
             {
                 return View();
             }
         }

         // GET: HomeController/Delete/
         public ActionResult Delete(Guid id)
         {
             return Index();
         }

         // POST: HomeController/Delete/
         [HttpPost]
         [ValidateAntiForgeryToken]
         public ActionResult Delete(AutoBrand model)
         {
             try
             {
                 var repository = new AutoBrandRepository();
                 repository.Delete(model.Id);

                 return RedirectToAction(nameof(Index));
             }
             catch
             {
                 return Index();
             }
         }
     }
}
