﻿using Asp_Models;
using Asp_Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Asp_Utility;
using Asp_DataAccess.Repository.IRepository;

namespace asp_net_core_mvc.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ProductController : Controller
    {
        private readonly IProductRepository _prodRep;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IProductRepository prodRep, IWebHostEnvironment webHostEnvironment)
        {
            _prodRep = prodRep;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> objList = _prodRep.GetAll(
                includeProperties: "Category,ApplicationType");
                                                /*.Include(u => u.Category)
                                                .Include(u => u.ApplicationType);*/
            /*foreach (Product obj in objList)
            {
                obj.Category = _productRep.Category.FirstOrDefault(u=>u.Id==obj.CategoryId);
                obj.ApplicationType = _productRep.ApplicationType.FirstOrDefault(u=>u.Id==obj.ApplicationTypeId);
            }*/
            return View(objList);
        }

        //GET - UPSERT
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _prodRep.GetAllDropdownList(WC.CategoryName),
                ApplicationTypeSelectList = _prodRep.GetAllDropdownList(WC.ApplicationTypeName)
            };
            if (id == null)
            {
                //Create
                return View(productVM); //new -> insert
            } else
            {
                //Update
                productVM.Product = _prodRep.Find(id.GetValueOrDefault()); 
                if(productVM.Product == null) { return NotFound(); }
                return View(productVM);
            }
        }

        //POST - UPSERT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            
            /*if (ModelState.IsValid)
            {*/
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                if (productVM.Product.Id == 0)
                {
                    //Creating
                    string upload = webRootPath + WC.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productVM.Product.Image = fileName + extension;

                    _prodRep.Add(productVM.Product);
                }
                else
                {
                    //Updating
                    var objFromDb = _prodRep.FirstOrDefault(u=>u.Id==productVM.Product.Id, isTracking:false);
                    if (files.Count>0)
                    {
                        string upload = webRootPath + WC.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        var oldFile = Path.Combine(upload, objFromDb.Image);
                        if(System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }
                        productVM.Product.Image = fileName + extension;
                    }
                    else
                    {
                        productVM.Product.Image = objFromDb.Image;
                    }
                    _prodRep.Update(productVM.Product);
                }
                _prodRep.Save();
                return RedirectToAction("Index");
            /*}
            productVM.CategorySelectList = _prodRepo.GetAllDropdownList(WC.CategoryName);
            productVM.ApplicationTypeSelectList = _prodRepo.GetAllDropdownList(WC.ApplicationTypeName);
            return View(productVM);*/
        }
        //GET - Delete
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) { return NotFound(); }
            Product product = _prodRep
                /*.Include(u=>u.Category)
                .Include(u => u.ApplicationType)*/
                .FirstOrDefault(u=>u.Id==id, includeProperties: "Category,ApplicationType");
            if (product == null) { return NotFound(); }
            return View(product);
        }

        //POST - Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _prodRep.Find(id.GetValueOrDefault());
            if (obj == null) { return NotFound(); }

            string upload = _webHostEnvironment.WebRootPath + WC.ImagePath;
            var oldFile = Path.Combine(upload, obj.Image);

            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }
            _prodRep.Remove(obj);
            _prodRep.Save();
            return RedirectToAction("Index");
        }
    }
}
