﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using image2suggestion.Data;
using image2suggestion.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace image2suggestion.Controllers
{
    public class PhotosController : Controller
    {
        private readonly PhotoDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
       

        public PhotosController(PhotoDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment; 
        }

        // GET: Photos
        public async Task<IActionResult> Index()
        {
            var photoDbContext = _context.Photo.Include(p => p.Suggestion);
            return View(await photoDbContext.ToListAsync());
        }

        // GET: Photos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Photo == null)
            {
                return NotFound();
            }

            var photo = await _context.Photo
                .Include(p => p.Suggestion)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (photo == null)
            {
                return NotFound();
            }

            return View(photo);
        }

        // GET: Photos/Create
        public IActionResult Create()
        {
            ViewData["SuggestionID"] = new SelectList(_context.Suggestion, "Id", "Title");
            return View();
        }

        // POST: Photos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title, PhotoName,PhotoInIForm, SuggestionID")] Photo photo)
        {
            if (ModelState.IsValid)
            {
          
                //save image to wwwroot/image
                string wwwRootPath = _hostEnvironment.WebRootPath;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                string fileName = Path.GetFileNameWithoutExtension(photo.PhotoInIForm.FileName);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                string fileExtension = Path.GetExtension(photo.PhotoInIForm.FileName);
                photo.Title=fileName = photo.PhotoName+DateTime.Now.ToString("_yyyyMddHHmmss_") + fileName + fileExtension;
                ViewBag.PhotoBag = photo;
                string path = Path.Combine(wwwRootPath + "/Image/" + fileName);
               
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await photo.PhotoInIForm.CopyToAsync(fileStream);
                  
                }

                 //ViewData["Pathway"] = path;
          
                //using (var memoryStream = new MemoryStream())
                //{

                //    await photo.PhotoInIForm.CopyToAsync(memoryStream);
                //    photo.PhotoInBytes = memoryStream.ToArray();//need to save as kb?
                //    var convLong = BitConverter.ToInt64(photo.PhotoInBytes, 0);
                //    //var convKb = convLong / 1024;
                //    Decimal fileSizeInMB = Convert.ToDecimal(convLong);
                //        /// (1024.0m * 1024.0m);
                //    photo.DisplaySize = fileSizeInMB;

                //}


                //insert record
                _context.Add(photo);
                await _context.SaveChangesAsync();
             
                return RedirectToAction(nameof(Index));
            }
            
            //    var pato= Path.Combine(_hostEnvironment.WebRootPath, "image", photo.Title);
            //TempData["patti"] = pato;
            //TempData.Keep("patti");

            ViewData["SuggestionID"] = new SelectList(_context.Suggestion, "Id", "Title", photo.SuggestionID);
            return View(photo);
        }

        // GET: Photos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Photo == null)
            {
                return NotFound();
            }

            var photo = await _context.Photo.FindAsync(id);
            if (photo == null)
            {
                return NotFound();
            }
            ViewData["SuggestionID"] = new SelectList(_context.Suggestion, "Id", "Title", photo.SuggestionID);
            return View(photo);
        }

        // POST: Photos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,PhotoName, PhotoInBytes,SuggestionID")] Photo photo)
        {
            if (id != photo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(photo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhotoExists(photo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SuggestionID"] = new SelectList(_context.Suggestion, "Id", "Title", photo.SuggestionID);
            return View(photo);
        }

        // GET: Photos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Photo == null)
            {
                return NotFound();
            }

            var photo = await _context.Photo
                .Include(p => p.Suggestion)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (photo == null)
            {
                return NotFound();
            }

            return View(photo);
        }

        // POST: Photos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Photo == null)
            {
                return Problem("Entity set 'PhotoDbContext.Photo'  is null.");
            }
            var photo = await _context.Photo.FindAsync(id);
            if (photo != null)
            {
                _context.Photo.Remove(photo);
            }

            //delete image from wwwroot/image folder
            var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "image", photo.Title);
            //var imagePath = Path.Combine(wwwRootPath + "/Image/" + fileName);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);
            //delete the record
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }


        //Console.WriteLine(SizeSuffix(100005000L));

        private bool PhotoExists(int id)
        {
          return _context.Photo.Any(e => e.Id == id);
        }
    }
}
