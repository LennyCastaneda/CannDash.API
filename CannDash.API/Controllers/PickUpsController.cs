﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using CannDash.API.Infrastructure;
using CannDash.API.Models;

namespace CannDash.API.Controllers
{
    public class PickUpsController : ApiController
    {
        private CannDashDataContext db = new CannDashDataContext();

        // GET: api/PickUps
        //Todo: authorize role for only admin
        public dynamic GetPickUps()
        {
            return db.PickUps.Select(p => new
            {
                p.DriverId,
                p.InventoryId,
                p.ProductId,
                p.Inv_Gram,
                p.Inv_TwoGrams,
                p.Inv_Eigth,
                p.Inv_Quarter,
                p.Inv_HalfOnce,
                p.Inv_Ounce,
                p.Inv_Each
            });
        }

        // GET: api/PickUps/5
        [ResponseType(typeof(PickUp))]
        [HttpGet, Route("api/pickups/{driverId}/{inventoryId}")]
        public IHttpActionResult GetPickUp(int driverId, int inventoryId)
        {
            PickUp pickUp = db.PickUps.Find(driverId,inventoryId);
            if (pickUp == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                pickUp.DriverId,
                pickUp.InventoryId,
                pickUp.ProductId,
                pickUp.Inv_Gram,
                pickUp.Inv_TwoGrams,
                pickUp.Inv_Eigth,
                pickUp.Inv_Quarter,
                pickUp.Inv_HalfOnce,
                pickUp.Inv_Ounce,
                pickUp.Inv_Each
            });
        }

        // PUT: api/PickUps/5
        [ResponseType(typeof(void))]
        [HttpPut, Route("api/pickups/{driverId}/{inventoryId}")]
        public IHttpActionResult PutPickUp(int driverId, int inventoryId, PickUp pickUp)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (driverId != pickUp.DriverId || inventoryId != pickUp.InventoryId)
            {
                return BadRequest();
            }

            db.Entry(pickUp).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PickUpExists(pickUp.DriverId, pickUp.InventoryId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/PickUps
        [ResponseType(typeof(PickUp))]
        [HttpPost, Route("api/pickups/{driverId}/{inventoryId}")]
        public IHttpActionResult PostPickUp(PickUp pickUp)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.PickUps.Add(pickUp);

            try
            {
                db.SaveChanges();

                pickUp.Driver = db.Drivers.Find(pickUp.DriverId);
                pickUp.Inventory = db.Inventories.Find(pickUp.InventoryId);
            }
            catch (DbUpdateException)
            {
                if (PickUpExists(pickUp.DriverId,pickUp.InventoryId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = pickUp.DriverId }, pickUp);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PickUpExists(int driverId, int inventoryId)
        {
            return db.PickUps.Count(e => e.DriverId == driverId && e.InventoryId == inventoryId) > 0;
        }
    }
}