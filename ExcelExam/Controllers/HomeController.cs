using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ExcelExam.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ExcelExam.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using ExcelExam.Data.DatabaseModel;
using Microsoft.EntityFrameworkCore;

namespace ExcelExam.Controllers
{

    public class HomeController : Controller
    {
        private readonly ApplicationDbContext context;

        public HomeController(ApplicationDbContext context)
        {
            this.context = context;
        }
        /// <summary>
        /// ساخت دیتا فیک مورد نظر
        /// </summary>
        /// <returns></returns>
        public IActionResult Download()
        {
            byte[] fileContents;

            using (var package = new ExcelPackage())
            {
                var worksheet1 = package.Workbook.Worksheets.Add("City");
                var worksheet2 = package.Workbook.Worksheets.Add("Sale");
                worksheet1.Cells[1, 1].Value = "Id";
                worksheet1.Cells[1, 2].Value = "City";

                worksheet2.Cells[1, 1].Value = "Id";
                worksheet2.Cells[1, 2].Value = "CityName";
                worksheet2.Cells[1, 3].Value = "ProductName";
                worksheet2.Cells[1, 4].Value = "CodeProduct";
                worksheet2.Cells[1, 5].Value = "PersonName";
                worksheet2.Cells[1, 6].Value = "Price";

                var rnd = new Random();
                for (int i = 2; i <= 1048000; i++)
                {
                    string ds = Guid.NewGuid().ToString();
                    string dsp = Guid.NewGuid().ToString();
                    var prnd = rnd.Next(100000000, 900000000);
                    worksheet1.Cells[i, 1].Value = i;
                    worksheet1.Cells[i, 2].Value = "City " + ds + ds + rnd.Next(1, 1048000);
                    worksheet2.Cells[i, 1].Value = i;
                    worksheet2.Cells[i, 2].Value = worksheet1.Cells[i, 2].Value;
                    worksheet2.Cells[i, 3].Value = "product " + dsp + ds + prnd;
                    worksheet2.Cells[i, 4].Value = "codeProduct " + dsp + ds + prnd;
                    worksheet2.Cells[i, 5].Value = "person " + Guid.NewGuid().ToString() + Guid.NewGuid().ToString();
                    worksheet2.Cells[i, 6].Value = prnd + ((prnd * rnd.Next(1, 15)) / 100);

                }
                fileContents = package.GetAsByteArray();
            }

            if (fileContents == null || fileContents.Length == 0)
            {
                return NotFound();
            }

            return File(
                 fileContents: fileContents,
                 contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                 fileDownloadName: "test.xlsx"
             );
        }

        /// <summary>
        /// اپلود فایل مورد نظرودیتا ها
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            ViewData["cc"] = context.Cities.Count();
            ViewData["lc"] = context.Sales.Count();
            return View();
        }
        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 289715200)]
        [RequestSizeLimit(289715200)]
        public IActionResult Index(IFormFile file)
        {
            Helper.Utility.GetDataTableFromExcel(file.OpenReadStream(), context);
            ViewData["cc"] = context.Cities.Count();
            ViewData["lc"] = context.Sales.Count();
            return View();
        }
        //
        //
       /// <summary>
       /// نمایش اطلاعات دسته بندی حذف و جستجو برای هر جستجو حدود 1 تا 2 دقیقه زمان لازم است چ.ون داده ها یک میلیون هستش
       /// </summary>
       /// <param name="page"></param>
       /// <param name="sort"></param>
       /// <param name="key"></param>
       /// <param name="infoSort"></param>
       /// <param name="delete"></param>
       /// <returns></returns>
        public IActionResult Sales(int page = 0, int sort = 0, string key = "", bool infoSort = false, int delete = 0)
        {
            this.context.Database.SetCommandTimeout(180);
            if(page==0)
            {

            ViewData["pageStart"] = 1;
            }
            ViewData["s"] = infoSort;
            ViewData["lc"] = context.Sales.Count()/5;
            ViewData["p"] = page;
            if (delete != 0)
            {
                context.Sales.Remove(context.Sales.FirstOrDefault(c => c.Id == delete));
                context.SaveChanges();
                return View(context.Sales.Skip((--page) * 5).Take(5).ToList());

            }
            if (string.IsNullOrEmpty(key) == false)
            {
                int x = 0;
                float y = 0;
                try
                {
                    x = int.Parse(key);
                    y = float.Parse(key);
                    List<Sale> ds = new List<Sale>();
                    ds = context.Sales.Where(c => c.Id == x).Take(5).ToList();
                    if (ds.Count > 0)
                    {
                        ViewData["lc"] = ds.Count()/5;
                        return View(ds);
                    }
                    ds = context.Sales.Where(c => c.Price == y).Take(5).ToList();
                    if (ds.Count > 0)
                    {
                        ViewData["lc"] = ds.Count()/5;
                        return View(ds);
                    }
                    ViewData["lc"] = ds.Count()/5;
                    return View(ds);
                }
                catch
                {
                    List<Sale> ds = new List<Sale>();
                    // ds = context.Sales.Where(c => c.NameProduct.Contains(key)).Take(5).ToList();
                    ds = (from s in context.Sales
                          where EF.Functions.Like(s.CityName, "%" + key + "%")
                          select s).ToList();
                    if (ds.Count > 0)
                    {
                        ViewData["lc"] = ds.Count()/5;
                        return View(ds);
                    }
                    ds = (from s in context.Sales
                         where EF.Functions.Like(s.NameProduct, "%"+key+"%")
                         select s).ToList();
                    if (ds.Count > 0)
                    {
                        ViewData["lc"] = ds.Count()/5;
                        return View(ds);
                    }
                    ds = (from s in context.Sales
                          where EF.Functions.Like(s.PersonFullName, "%" + key + "%")
                          select s).ToList(); if (ds.Count > 0)
                    {
                        ViewData["lc"] = ds.Count()/5;
                        return View(ds);
                    }
                    ds = (from s in context.Sales
                          where EF.Functions.Like(s.ProductCode, "%" + key + "%")
                          select s).ToList();
                    if (ds.Count > 0)
                    {
                        ViewData["lc"] = ds.Count()/5;
                        return View(ds);
                    }
                    ViewData["lc"] = ds.Count()/5;
                    return View(ds);
                }

            }
            if (page != 0)
            {
                ViewData["pageStart"] = page;

                return View(context.Sales.Skip((--page) * 5).Take(5).ToList());
            }
            if (sort != 0)
            {
                if (page == 0)
                    page++;
                if (!infoSort)
                {
                    ViewData["s"] = !infoSort;

                    switch (sort)
                    {
                        case 1:
                            {
                                return View(context.Sales.Skip((--page) * 5).OrderBy(c => c.Id).Take(5).ToList());

                            }
                        case 2:
                            {
                                return View(context.Sales.Skip((--page) * 5).Take(5).OrderBy(c => c.CityName).ToList());

                            }
                        case 3:
                            {
                                return View(context.Sales.Skip((--page) * 5).Take(5).OrderBy(c => c.NameProduct).ToList());

                            }
                        case 4:
                            {
                                return View(context.Sales.Skip((--page) * 5).Take(5).OrderBy(c => c.ProductCode).ToList());

                            }
                        case 5:
                            {
                                return View(context.Sales.Skip((--page) * 5).Take(5).OrderBy(c => c.PersonFullName).ToList());

                            }
                        case 6:
                            {
                                return View(context.Sales.Skip((--page) * 5).Take(5).OrderBy(c => c.Price).ToList());

                            }
                        default:
                            {


                                return View(context.Sales.Take(5).ToList());


                            }
                    }
                }
                else
                {
                    ViewData["s"] = !infoSort;

                    switch (sort)
                    {
                        case 1:
                            {
                                return View(context.Sales.Skip((--page) * 5).Take(5).OrderByDescending(c => c.Id).ToList());

                            }
                        case 2:
                            {
                                return View(context.Sales.Skip((--page) * 5).Take(5).OrderByDescending(c => c.CityName).ToList());

                            }
                        case 3:
                            {
                                return View(context.Sales.Skip((--page) * 5).Take(5).OrderByDescending(c => c.NameProduct).ToList());

                            }
                        case 4:
                            {
                                return View(context.Sales.Skip((--page) * 5).Take(5).OrderByDescending(c => c.ProductCode).ToList());

                            }
                        case 5:
                            {
                                return View(context.Sales.Skip((--page) * 5).Take(5).OrderByDescending(c => c.PersonFullName).ToList());

                            }
                        case 6:
                            {
                                return View(context.Sales.Skip((--page) * 5).Take(5).OrderByDescending(c => c.Price).ToList());

                            }
                        default:
                            {


                                return View(context.Sales.Take(5).ToList());


                            }
                    }
                }


            }
            return View(context.Sales.Take(5).ToList());
        }
        //string cityName,string nameProduct,string productCode,string personFullName,float price

            /// <summary>
            /// اضافه کردن اطلاعات
            /// </summary>
            /// <returns></returns>
        public async Task<IActionResult> CreateSale()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateSale(Sale s)
        {
           // Sale s = new Sale();
            s.Id = context.Sales.LastOrDefault().Id + 1;
            //s.CityName = cityName;
            //s.NameProduct = nameProduct;
            //s.ProductCode = productCode;
            //s.PersonFullName = personFullName;
            //s.Price = price;
           await context.AddAsync(s);
           await context.SaveChangesAsync();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
