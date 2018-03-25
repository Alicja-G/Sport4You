using Sport4You.Models.Data;
using Sport4You.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sport4You.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            List<PageVM> pagesList;

            using (DB db = new DB())
            {
                pagesList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }
            return View(pagesList);
        }

        //GET Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        //POST Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            //Check model state
            if(! ModelState.IsValid)
            {
                return View(model);
            }
           

            using (DB db = new DB())
            {
                //Declare slug
                string slug;

                //Init PageDTO
                PageDTO dto = new PageDTO();

                //DTO title
                dto.Title = model.Title;

                //Check for and set slug if needed
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }
                //Make sure title and slug are unique
                if(db.Pages.Any(x=> x.Title == model.Title) || db.Pages.Any(x => x.Slug == slug) )
                    {
                    ModelState.AddModelError("", "That title or slug already exists.");
                    return View(model);

                }

                //Dto the rest
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100; //there are not going to be more than 100 products 

                // Save DTO
                db.Pages.Add(dto);
                db.SaveChanges();
            }
            //Set TempData msg
            TempData["SM"] = "You have added the page sucessfully.";

            //Redirect
            return RedirectToAction("AddPage");
            
        }


        //GET Admin/Pages/EditPage/id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            //Declare pageVM
            PageVM model;
            

            using (DB db = new DB())
            {
                //Get the page
                PageDTO dto = db.Pages.Find(id);

                //Confirm page exists 
                if(dto == null)
                {
                    return Content("The page does not exist.");
                }

                //Init pageVM
                model = new PageVM(dto);
            }


            //return view with model
            return View(model);
        }

        //POST Admin/Pages/EditPage/id
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            //Check modelstate

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (DB db = new DB())
            {
                //Get page id
                int id = model.Id;

                //Init slug
                string slug = "home";


                //Get the page
                PageDTO dto = db.Pages.Find(id);

                //DTO the title
                dto.Title = model.Title;

                //Check for slug and set if needed
                if (model.Slug != "home")
                {
                    if(string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }

                //title and slug have to be unique
                if (db.Pages.Where(x=> x.Id != id).Any(x=> x.Title == model.Title) ||
                   db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                    {
                    ModelState.AddModelError("", "That title or slug already exists");
                    return View(model);
                }
                //DTO the rest
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;

                //Save the DTO
                db.SaveChanges();
            }


            //TempData msg
            TempData["SM"] = "You have edited the page.";

            //Redirect
            return RedirectToAction("EditPage");

           
        }


        //GET Admin/Pages/PageDetails/id
        public ActionResult PageDetails(int id)
        {
            //Declare PageVm
            PageVM model;

            using (DB db = new DB())
            {
                //Get the page
                PageDTO dto = db.Pages.Find(id);


                //Confirm page exists
                if(dto == null)
                {
                    return Content("That page does not exist.");

                }
                else
                {
                    //init pagevm
                    model = new PageVM(dto);
                }
              

               

            }


            //return view with model
            return View(model);
        }

        //DELETE Admin/Pages/DeletePage/id
        public ActionResult DeletePage(int id)
        {
            //GET the page
            using (DB db = new DB())
            {
                //Remove the page
                PageDTO dto = db.Pages.Find(id);

                // Save 
                db.Pages.Remove(dto);
                db.SaveChanges();
                

            }
            //Redirect
            return RedirectToAction("index");
        }


        //POST Admin/Pages/ReorderPages
        [HttpPost]
        public void ReorderPages(int[] id)
        {
            using (DB db = new DB())
            {
                //Set initial count
                int count = 1;


                //Declare PageDTO
                PageDTO dto;


                //Set sorting for each page
                foreach (var pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;

                    db.SaveChanges();
                    count++; 
                }


            }



        }
    }
}