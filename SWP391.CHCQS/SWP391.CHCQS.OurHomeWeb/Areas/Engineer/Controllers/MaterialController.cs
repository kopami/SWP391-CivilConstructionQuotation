﻿using Microsoft.AspNetCore.Mvc;
using SWP391.CHCQS.DataAccess.Repository.IRepository;
using SWP391.CHCQS.Model;
using SWP391.CHCQS.OurHomeWeb.Areas.Engineer.ViewModels;
using SWP391.CHCQS.Utility;
using SWP391.CHCQS.Utility.Helpers;
using System.Threading.Tasks;

namespace SWP391.CHCQS.OurHomeWeb.Areas.Engineer.Controllers
{
	[Area("Engineer")]
	public class MaterialController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		//Khai bao Session cho MaterialList neu co thi lay ra khong co thi tao moi
		public List<MaterialDetailViewModel> MaterialListSession => HttpContext.Session.Get<List<MaterialDetailViewModel>>(SessionConst.MATERIAL_LIST_KEY) ?? new List<MaterialDetailViewModel>();

		//Declare Session to store CustomQuotation serve to method AddToList in TaskController and MaterialController to add Task and Material.
		public CustomQuotationListViewModel CustomQuotationSession => HttpContext.Session.Get<CustomQuotationListViewModel>(SessionConst.CUSTOM_QUOTATION_KEY) ?? new CustomQuotationListViewModel();
		public MaterialController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}



		#region API CALL LIST MATERIAL
		/// <summary>
		/// This function get all CustomeQuotation in Database and return it into JSON, this function ne lib Datatables to show data
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			//Asign MaterialListSession to materialCart 
			var materialCart = MaterialListSession;
			//Declare materialVMList
			List<MaterialViewModel> materialVMList;

			//Get a list of all material from database and projection into MaterialViewModel but not in MaterialListSession
			//When a Task has been add into MaterialListSession its will not appear in datatables
			materialVMList = _unitOfWork.Material
			.GetAll(includeProperties: "Category")
			.Where(m => m.Status == true && !MaterialListSession.Any(x => x.Material.Id == m.Id))
			.Select(x => new MaterialViewModel
			{
				Id = x.Id,
				Name = x.Name,
				InventoryQuantity = x.InventoryQuantity,
				UnitPrice = x.UnitPrice,
				Unit = x.Unit,
				Status = x.Status,
				CategoryId = x.CategoryId,
				CategoryName = x.Category.Name
			}).ToList();

			//Return Json for datatables to read
			return Json(new
			{
				data = materialVMList
			});
		}

		[HttpGet]
		public async Task<IActionResult> GetMaterialListSession()
		{
			return Json(new { data = MaterialListSession.ToList() });
		}
		#endregion

		[HttpGet]
		public async Task<IActionResult> AddToQuote(string MaterialId)
		{
			//Asign MaterialListSession to taskCart
			var materialCart = MaterialListSession;
			//Get MaterialDetailViewModel and asign to materialItem from materialCart
			var materialItem = materialCart.FirstOrDefault(x => x.Material.Id == MaterialId);

			//If materialItem equal null mean the materialItem not in Session (not in session)
			if (materialItem == null)
			{
				//Get a material from database by TaskId
				var material = _unitOfWork.Material.Get(x => x.Id == MaterialId);

				//Check if that material not in database
				if (material == null)
				{
					////Return error message to front-end show for customer. the scripts in ~/View/Shared/_Notification.cshml
					//TempData["Error"] = $"Material not found with Id = {MaterialId}";

					////Return back to the QuotationController with action Quote and pass a QuotationId get from CustomQuotationSession
					//return RedirectToAction("Quote", "Quotation", new { QuotationId = CustomQuotationSession.Id });

					//Return back to the QuotationController with action Quote and pass a QuotationId get from CustomQuotationSession
					return Json(new { success = false, message = $"Material not found with Id = {MaterialId}" });
				}
				else //if it not equal null
				{
					//Asign new MaterialDetailViewModel with projection from task for materialItem
					materialItem = new MaterialDetailViewModel
					{
						Material = material,
						QuotationId = CustomQuotationSession.Id,
						Price = material.UnitPrice
					};

					//Add materialItem into materialCart
					materialCart.Add(materialItem);
				}
			}
			else // if it already in session
			{
				////Return error message to front-end show for customer. the scripts in ~/View/Shared/_Notification.cshml
				//TempData["Error"] = $"Material already in quote with Id = {MaterialId}";

				////Return back to the QuotationController with action Quote and pass a QuotationId get from CustomQuotationSession
				//return RedirectToAction("Quote", "Quotation", new { QuotationId = CustomQuotationSession.Id });

				//Return back to the QuotationController with action Quote and pass a QuotationId get from CustomQuotationSession
				return Json(new { success = false, message = $"Material already in quote with Id = {MaterialId}" });
			}

			//Update MaterialListSession with materialCart  
			HttpContext.Session.Set(SessionConst.MATERIAL_LIST_KEY, materialCart);

			////Return success message to front-end show for customer. the scripts in ~/View/Shared/_Notification.cshml
			//TempData["Success"] = $"Add material successfully with Id = {MaterialId}";

			////Return back to the QuotationController with action Quote and pass a QuotationId get from CustomQuotationSession
			//return RedirectToAction("Quote", "Quotation", new { QuotationId = CustomQuotationSession.Id });

			//Return back to the QuotationController with action Quote and pass a QuotationId get from CustomQuotationSession
			return Json(new { success = true, message = $"Add material successfully with Id = {MaterialId}" });
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="TaskId"></param>
		/// <returns></returns>
		[HttpDelete]
		public async Task<IActionResult> DeleteFromQuote(string MaterialId)
		{
			//Asign MaterialListSession to materialCart
			var materialCart = MaterialListSession;

			//Get materialItem which exist in materialCart
			var materialItem = materialCart.Where(x => x.Material.Id == MaterialId).FirstOrDefault();

			//if materialItem not in materialCart
			if (materialItem == null)
			{
				////Return error message to front-end show for customer. the scripts in ~/View/Shared/_Notification.cshml
				//TempData["Error"] = $"Material not found with Id = {MaterialId}";

				////Return back to the QuotationController with action Quote and pass a QuotationId get from CustomQuotationSession
				//return RedirectToAction("Quote", "Quotation", new { QuotationId = CustomQuotationSession.Id });

				//Return back to the QuotationController with action Quote and pass a QuotationId get from CustomQuotationSession
				return Json(new { success = false, message = $"Material not found with Id = {MaterialId}" });
			}

			//Delete materialItem in materialCart
			materialCart.Remove(materialItem);

			//Update MaterialListSession with materialCart  
			HttpContext.Session.Set(SessionConst.MATERIAL_LIST_KEY, materialCart);

			////Return success message to front-end show for customer. the scripts in ~/View/Shared/_Notification.cshml
			//TempData["Success"] = $"Delete material successfully with Id = {MaterialId}";

			////Return back to the QuotationController with action Quote and pass a QuotationId get from CustomQuotationSession
			//return RedirectToAction("Quote", "Quotation", new { QuotationId = CustomQuotationSession.Id });

			//Return back to the QuotationController with action Quote and pass a QuotationId get from CustomQuotationSession
			return Json(new { success = false, message = $"Delete material successfully with Id = {MaterialId}" });
		}

		[HttpPost]
		public async Task<IActionResult> UpdateQuantity([FromForm] string MaterialId, [FromForm] int MaterialQuantity)
		{
			//Asign MaterialListSession to materialCart
			var materialCart = MaterialListSession;

			//Get materialItem which exist in materialCart
			var materialItem = materialCart.Where(x => x.Material.Id == MaterialId).FirstOrDefault();

			//if materialItem not in materialCart
			if (materialItem == null)
			{
				////Return error message to front-end show for customer. the scripts in ~/View/Shared/_Notification.cshml
				//TempData["Error"] = $"Material not found with Id = {MaterialId}";

				////Return back to the QuotationController with action Quote and pass a QuotationId get from CustomQuotationSession
				//return RedirectToAction("Quote", "Quotation", new { QuotationId = CustomQuotationSession.Id });

				//Return back to the QuotationController with action Quote and pass a QuotationId get from CustomQuotationSession
				return Json(new { success = false, message = $"Material not found with Id = {MaterialId}" });
			}

			//Delete exist materialItem in materialCart
			materialCart.Remove(materialItem);

			//update Quantity and Price for materialItem
			materialItem.Quantity = MaterialQuantity;
			materialItem.Price = materialItem.Material.UnitPrice * materialItem.Quantity;

			//Add new materialItem after update again into materialCart
			materialCart.Add(materialItem);

			//Update MaterialListSession with materialCart  
			HttpContext.Session.Set(SessionConst.MATERIAL_LIST_KEY, materialCart);

			////Return success message to front-end show for customer. the scripts in ~/View/Shared/_Notification.cshml
			//TempData["Success"] = $"Update material quantity successfully with Id = {MaterialId}";

			////Return back to the QuotationController with action Quote and pass a QuotationId get from CustomQuotationSession
			//return RedirectToAction("Quote", "Quotation", new { QuotationId = CustomQuotationSession.Id });

			//Return back to the QuotationController with action Quote and pass a QuotationId get from CustomQuotationSession
			return Json(new { success = true, message = $"Update material quantity successfully with Id = {MaterialId}" });
		}


	}
}
